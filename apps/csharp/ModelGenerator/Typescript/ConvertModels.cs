using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Typescript.Internal.Services;
using Typescript.Internal.Utils;

namespace Typescript;

public class ConvertModels
{
    private readonly string _typescriptOutPath;
    private readonly IEnumerable<string> _csharpModelsPath;
    private readonly IEnumerable<string> _outSubFolders;

    public ConvertModels(string typescriptOutPath, IEnumerable<string> csharpModelsPaths,
        IEnumerable<string> outSubFolders)
    {
        _typescriptOutPath = typescriptOutPath;
        _csharpModelsPath = csharpModelsPaths;
        _outSubFolders = outSubFolders;
    }

    private List<INamedTypeSymbol> GetFullNamespaceModelSymbols(List<SyntaxTree> syntaxTrees,
        CSharpCompilation compilation)
    {
        var modelSymbols = new List<INamedTypeSymbol>();
        foreach (var tree in syntaxTrees)
        {
            var semanticModel = compilation.GetSemanticModel(tree);
            var root = tree.GetRoot();
            var typeDeclarations = root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>();
            foreach (var declaration in typeDeclarations)
            {
                var symbol = semanticModel.GetDeclaredSymbol(declaration);
                if (symbol != null && symbol.ContainingNamespace != null)
                {
                    string containingNamespace = symbol.ContainingNamespace.ToString();
                    if (containingNamespace != null)
                    {
                        var isModelNamespace = containingNamespace.Contains("DTOs") ||
                                               containingNamespace.Contains("Models");
                        if (isModelNamespace)
                        {
                            modelSymbols.Add(symbol);
                        }
                    }
                }
            }
        }

        return modelSymbols;
    }

    public void GenerateTypeScriptInterfaces()
    {
        if (Directory.Exists(_typescriptOutPath))
        {
            Directory.Delete(_typescriptOutPath, true);
        }

        var csFiles = new List<string>();
        foreach (var csharpModelPath in _csharpModelsPath)
        {
            var pathExists = Directory.Exists(csharpModelPath);
            if (!pathExists)
            {
                throw new Exception(
                    "Provided path for models does not exist. Please provide a valid path to convert models.");
            }

            var isDirectoryEmpty = !Directory.EnumerateFileSystemEntries(csharpModelPath).Any();
            if (isDirectoryEmpty)
            {
                throw new Exception("Provided path is empty. Please provide a path that contains valid .cs files.");
            }
            
            csFiles.AddRange(Directory.GetFiles(csharpModelPath, "*.cs", SearchOption.AllDirectories));
        }

        var syntaxTrees = csFiles.Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file))).ToList();

        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
        };

        var compilation = CSharpCompilation.Create(
            "ModelCompilation",
            syntaxTrees: syntaxTrees,
            references: references
        );

        var modelSymbols = GetFullNamespaceModelSymbols(syntaxTrees, compilation);

        var modelNames = new HashSet<string>(modelSymbols.Select(s => s.Name));
        CsharpToTypescript.SetGlobalConvertedTypes(modelNames);

        foreach (var symbol in modelSymbols)
        {
            string folder = Paths.GetPredefinedOutputFolder(symbol.Name, _outSubFolders);

            var tsType = CsharpToTypescript.GenerateInterfaces(symbol, folder, _typescriptOutPath, _outSubFolders);
            string fileName = StringUtils.FirstCharToLowerCase(tsType.Name.Replace(".ts", ""));
            string fullFolderPath = string.IsNullOrEmpty(folder)
                ? _typescriptOutPath
                : Path.Combine(_typescriptOutPath, folder);

            if (!Directory.Exists(fullFolderPath))
            {
                Directory.CreateDirectory(fullFolderPath);
            }

            string fullPath = Path.Combine(fullFolderPath, fileName + ".ts");
            File.WriteAllLines(fullPath, tsType.Lines);
        }
        
        Console.WriteLine("TypeScript interfaces generated successfully. You can find the converted files in the out folder provided.");
    }
}