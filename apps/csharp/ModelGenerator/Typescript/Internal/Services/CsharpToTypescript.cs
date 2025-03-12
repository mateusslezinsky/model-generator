using Microsoft.CodeAnalysis;
using Typescript.Internal.Utils;
using Typescript.Internal.Domain;
using Typescript.Internal.Interfaces;

namespace Typescript.Internal.Services;

public class CsharpToTypescript : ICsharpToTypescript
{
    public HashSet<string>? globalConvertedTypes { get; set; }

    public TsType GenerateInterfaces(INamedTypeSymbol symbol, string currentRelativeFolder,
        string basePath, IEnumerable<string> folders)
    {
        var dependencies = new HashSet<INamedTypeSymbol>();
        foreach (var member in symbol.GetMembers().OfType<IPropertySymbol>())
        {
            if (member.DeclaredAccessibility != Accessibility.Public)
                continue;
            if (member.Name.Contains("Navigation"))
                continue;

            ITypeSymbol propType = member.Type;
            var arrayType = GetArrayOrEnumerableType(propType);
            var nullableType = GetNullableType(propType);
            ITypeSymbol effectiveType = nullableType ?? arrayType ?? propType;

            if (effectiveType != symbol && !IsPrimitive(effectiveType) &&
                globalConvertedTypes.Contains(effectiveType.Name))
            {
                if (effectiveType is INamedTypeSymbol depSymbol)
                {
                    dependencies.Add(depSymbol);
                }
            }

            if (propType is INamedTypeSymbol namedPropType && namedPropType.IsGenericType)
            {
                if (namedPropType.AllInterfaces.Any(i =>
                        i.OriginalDefinition.ToString() == "System.Collections.Generic.IEnumerable<T>") ||
                    namedPropType.Name == "ICollection" || namedPropType.Name == "List" ||
                    namedPropType.Name == "IEnumerable")
                {
                    foreach (var arg in namedPropType.TypeArguments)
                    {
                        if (arg != symbol && !IsPrimitive(arg) && globalConvertedTypes.Contains(arg.Name))
                        {
                            if (arg is INamedTypeSymbol depSymbol)
                            {
                                dependencies.Add(depSymbol);
                            }
                        }
                    }
                }
            }
        }

        var importLines = new List<string>();
        foreach (var dep in dependencies)
        {
            var depFileName = StringUtils.FirstCharToLowerCase(dep.Name);
            var depRelFolder = Paths.GetPredefinedOutputFolder(dep.Name, folders);
            if (string.IsNullOrEmpty(depRelFolder))
            {
                var key = StringUtils.GetFirstWord(dep.Name);
                depRelFolder = key.ToLower();
            }

            var importPath = Paths.GetRelativeImportPath(currentRelativeFolder, depRelFolder, basePath, depFileName);
            importLines.Add($"import {{ {dep.Name} }} from '{importPath}';");
        }

        var lines = new List<string>();
        if (importLines.Count > 0)
        {
            lines.AddRange(importLines);
            lines.Add("");
        }

        if (symbol.TypeKind == TypeKind.Enum)
        {
            ConvertEnum(lines, symbol);
        }
        else
        {
            ConvertClassOrInterface(lines, symbol);
        }

        var filename = $"{symbol.Name}.ts";
        return new TsType { Name = filename, Lines = lines.ToArray() };
    }

    private void ConvertClassOrInterface(List<string> lines, INamedTypeSymbol symbol)
    {
        lines.Add($"export interface {symbol.Name} {{");
        foreach (var member in symbol.GetMembers().OfType<IPropertySymbol>())
        {
            if (member.DeclaredAccessibility != Accessibility.Public)
                continue;
            if (member.Name.Contains("Navigation"))
                continue;

            if (!member.IsIndexer)
            {
                ITypeSymbol propType = member.Type;
                var arrayType = GetArrayOrEnumerableType(propType);
                var nullableType = GetNullableType(propType);

                ITypeSymbol typeToUse = nullableType ?? arrayType ?? propType;
                var convertedType = ConvertType(typeToUse);
                var suffix = arrayType is not null ? "[]" : "";
                suffix = nullableType is not null ? " | null" : suffix;

                lines.Add($"  {StringUtils.CamelCaseName(member.Name)}: {convertedType}{suffix};");
            }
        }

        lines.Add("}");
    }

    private string ConvertType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.SpecialType == SpecialType.System_Object)
        {
            return globalConvertedTypes is not null && globalConvertedTypes.Contains(typeSymbol.Name)
                ? typeSymbol.Name
                : "any";
        }

        if (Constants.ConvertedTypes.TryGetValue(typeSymbol.ToString(), out string tsType))
        {
            return tsType;
        }

        if (Constants.ConvertedTypes.TryGetValue(typeSymbol.Name, out tsType))
        {
            return tsType;
        }

        if (typeSymbol is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            var stringifiedConstructedFrom = namedType.ConstructedFrom.ToString();
            if (stringifiedConstructedFrom is null) return typeSymbol.Name;

            if (Constants.ListLikeInterfaces.Any(x => stringifiedConstructedFrom.StartsWith(x)))
            {
                var elementType = namedType.TypeArguments[0];
                return $"{ConvertType(elementType)}[]";
            }

            if (Constants.DictionaryLikeInterfaces.Any(x => stringifiedConstructedFrom.StartsWith(x)))
            {
                var keyType = namedType.TypeArguments[0];
                var valueType = namedType.TypeArguments[1];
                return $"{{ [key: {ConvertType(keyType)}]: {ConvertType(valueType)} }}";
            }
        }

        return typeSymbol.Name;
    }

    private void ConvertEnum(List<string> lines, INamedTypeSymbol symbol)
    {
        var members = symbol.GetMembers().OfType<IFieldSymbol>().Where(f => f.ConstantValue is not null).ToList();
        lines.Add($"export enum {symbol.Name} {{");
        foreach (var member in members)
        {
            lines.Add($"  {StringUtils.PascalToUppercaseWithUnderscores(member.Name)} = {member.ConstantValue},");
        }

        lines.Add("}");
    }

    private INamedTypeSymbol? GetArrayOrEnumerableType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is IArrayTypeSymbol arrayType)
        {
            return arrayType.ElementType as INamedTypeSymbol;
        }

        if (typeSymbol is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            var stringifiedConstructedFrom = namedType.ConstructedFrom.ToString();
            if (stringifiedConstructedFrom is null) return null;
            if (Constants.ListLikeInterfaces.Any(x => stringifiedConstructedFrom.StartsWith(x)))
            {
                return namedType.TypeArguments[0] as INamedTypeSymbol;
            }

            foreach (var iface in namedType.AllInterfaces)
            {
                if (iface.OriginalDefinition.ToString() == "System.Collections.Generic.IEnumerable<T>")
                {
                    return iface.TypeArguments[0] as INamedTypeSymbol;
                }
            }
        }

        return null;
    }

    private ITypeSymbol? GetNullableType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol namedType &&
            namedType.OriginalDefinition.ToString() == "System.Nullable<T>")
        {
            return namedType.TypeArguments[0];
        }

        return null;
    }

    private bool IsPrimitive(ITypeSymbol typeSymbol)
    {
        return Constants.ConvertedTypes.ContainsKey(typeSymbol.ToString()) ||
               Constants.ConvertedTypes.ContainsKey(typeSymbol.Name);
    }
}