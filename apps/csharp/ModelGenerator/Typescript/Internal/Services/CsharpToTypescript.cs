using Microsoft.CodeAnalysis;
using Typescript.Internal.Utils;
using Typescript.Internal.Domain;

namespace Typescript.Internal.Services;

public class CsharpToTypescript
{
    private static HashSet<string> globalConvertedTypes;

    public static void SetGlobalConvertedTypes(HashSet<string> value)
    {
        globalConvertedTypes = value;
    }

    public static TsType GenerateInterfaces(INamedTypeSymbol symbol, string currentRelativeFolder,
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

            // Skip the current type (symbol) to prevent self-import
            if (effectiveType != symbol && !IsPrimitive(effectiveType) && globalConvertedTypes.Contains(effectiveType.Name))
            {
                if (effectiveType is INamedTypeSymbol depSymbol)
                {
                    dependencies.Add(depSymbol);
                }
            }

            if (propType is INamedTypeSymbol namedPropType && namedPropType.IsGenericType)
            {
                if (namedPropType.AllInterfaces.Any(i => i.OriginalDefinition.ToString() == "System.Collections.Generic.IEnumerable<T>") ||
                    namedPropType.Name == "ICollection" || namedPropType.Name == "List" || namedPropType.Name == "IEnumerable")
                {
                    foreach (var arg in namedPropType.TypeArguments)
                    {
                        // Skip the current type (symbol) for type arguments
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
            string depFileName = StringUtils.FirstCharToLowerCase(dep.Name);
            string depRelFolder = Paths.GetPredefinedOutputFolder(dep.Name, folders);
            if (string.IsNullOrEmpty(depRelFolder))
            {
                string key = StringUtils.GetFirstWord(dep.Name);
                depRelFolder = key.ToLower();
            }

            string importPath = Paths.GetRelativeImportPath(currentRelativeFolder, depRelFolder, basePath, depFileName);
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

        string filename = $"{symbol.Name}.ts";
        return new TsType { Name = filename, Lines = lines.ToArray() };
    }

    private static void ConvertClassOrInterface(List<string> lines, INamedTypeSymbol symbol)
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
                // Regular property handling
                ITypeSymbol propType = member.Type;
                var arrayType = GetArrayOrEnumerableType(propType);
                var nullableType = GetNullableType(propType);
                ITypeSymbol typeToUse = nullableType ?? arrayType ?? propType;
                string convertedType = ConvertType(typeToUse);
                string suffix = arrayType != null ? "[]" : "";
                suffix = nullableType != null ? " | null" : suffix;
                lines.Add($"  {StringUtils.CamelCaseName(member.Name)}: {convertedType}{suffix};");
            }
        }

        lines.Add("}");
    }

    private static string ConvertType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.SpecialType == SpecialType.System_Object)
        {
            return (globalConvertedTypes != null && globalConvertedTypes.Contains(typeSymbol.Name))
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
            if (namedType.ConstructedFrom.ToString().StartsWith("List") ||
                namedType.ConstructedFrom.ToString().StartsWith("IEnumerable") ||
                namedType.ConstructedFrom.ToString().StartsWith("ICollection"))
            {
                var elementType = namedType.TypeArguments[0];
                return $"{ConvertType(elementType)}[]";
            }
            if (namedType.ConstructedFrom.ToString().StartsWith("IDictionary"))
            {
                var keyType = namedType.TypeArguments[0];
                var valueType = namedType.TypeArguments[1];
                return $"{{ [key: {ConvertType(keyType)}]: {ConvertType(valueType)} }}";
            }
        }

        return typeSymbol.Name;
    }

    private static void ConvertEnum(List<string> lines, INamedTypeSymbol symbol)
    {
        var members = symbol.GetMembers().OfType<IFieldSymbol>().Where(f => f.ConstantValue != null).ToList();
        lines.Add($"export enum {symbol.Name} {{");
        foreach (var member in members)
        {
            lines.Add($"  {member.Name} = {member.ConstantValue},");
        }

        lines.Add("}");
    }

    private static INamedTypeSymbol GetArrayOrEnumerableType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is IArrayTypeSymbol arrayType)
        {
            return arrayType.ElementType as INamedTypeSymbol;
        }

        if (typeSymbol is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            if (namedType.ConstructedFrom.ToString().StartsWith("System.Collections.Generic.List") ||
                namedType.ConstructedFrom.ToString().StartsWith("System.Collections.Generic.IEnumerable") ||
                namedType.ConstructedFrom.ToString().StartsWith("System.Collections.Generic.ICollection"))
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

    private static ITypeSymbol? GetNullableType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol namedType &&
            namedType.OriginalDefinition.ToString() == "System.Nullable<T>")
        {
            return namedType.TypeArguments[0];
        }

        return null;
    }

    private static bool IsPrimitive(ITypeSymbol typeSymbol)
    {
        return Constants.ConvertedTypes.ContainsKey(typeSymbol.ToString()) ||
               Constants.ConvertedTypes.ContainsKey(typeSymbol.Name);
    }
}