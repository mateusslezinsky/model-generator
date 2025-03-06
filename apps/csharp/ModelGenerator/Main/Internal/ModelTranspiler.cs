using Main.Internal.Domain;
using Main.Internal.Services;
using Main.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Main.Internal
{
    public class ModelTranspiler
    {
        private static List<INamedTypeSymbol> GetFullNamespaceModelSymbols(List<SyntaxTree> syntaxTrees, CSharpCompilation compilation)
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
                            var isModelNamespace = containingNamespace.Contains("DTOs") || containingNamespace.Contains("Models");
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
        
        public static void GenerateTypeScriptInterfaces(string basePath, string modelsPath, List<Category> categories)
        {
            if (Directory.Exists(basePath))
            {
                Directory.Delete(basePath, true);
            }

            var csFiles = Directory.GetFiles(modelsPath, "*.cs", SearchOption.AllDirectories);
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
            ConversorService.SetGlobalConvertedTypes(modelNames);

            var dynamicGroupCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var symbol in modelSymbols)
            {
                string typeName = symbol.Name;
                var modelOutputFolder = Paths.GetPredefinedOutputFolder(typeName, categories);
                if (!string.IsNullOrEmpty(modelOutputFolder))
                {
                    continue;
                }
                string key = StringUtils.GetFirstWord(typeName);
                if (string.IsNullOrEmpty(key))
                {
                    key = typeName.ToLower();
                }

                if (dynamicGroupCounts.ContainsKey(key))
                {
                    dynamicGroupCounts[key]++;
                }
                else
                {
                    dynamicGroupCounts[key] = 1;
                }
            }

            ConversorService.SetDynamicGroupCountsGlobal(dynamicGroupCounts);

            foreach (var symbol in modelSymbols)
            {
                string folder = Paths.GetPredefinedOutputFolder(symbol.Name, categories);
                if (string.IsNullOrEmpty(folder))
                {
                    string key = StringUtils.GetFirstWord(symbol.Name);
                    if (string.IsNullOrEmpty(key))
                        key = symbol.Name.ToLower();
                    folder = (ConversorService.GetDynamicGroupCountsGlobal().TryGetValue(key, out int count) &&
                              count >= 2)
                        ? key.ToLower()
                        : "other";
                }

                string currentRelativeFolder = folder;

                var tsType = ConversorService.ConvertCsharpToTypescript(symbol, currentRelativeFolder, basePath, categories);
                string fileName = StringUtils.FirstCharToLowerCase(tsType.Name.Replace(".ts", ""));
                string fullFolderPath = string.IsNullOrEmpty(currentRelativeFolder)
                    ? basePath
                    : Path.Combine(basePath, currentRelativeFolder);

                if (!Directory.Exists(fullFolderPath))
                {
                    Directory.CreateDirectory(fullFolderPath);
                }

                string fullPath = Path.Combine(fullFolderPath, fileName + ".ts");
                File.WriteAllLines(fullPath, tsType.Lines);
            }
        }
    }
}