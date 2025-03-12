using Microsoft.CodeAnalysis;
using Typescript.Internal.Domain;

namespace Typescript.Internal.Interfaces;

public interface ICsharpToTypescript
{
    public HashSet<string>? globalConvertedTypes { get; set; }

    public TsType GenerateInterfaces(INamedTypeSymbol symbol, string currentRelativeFolder,
        string basePath, IEnumerable<string> folders);
}