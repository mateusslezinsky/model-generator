using Typescript;

namespace ExampleUsage;

class Program
{
    private static DirectoryInfo? TryGetSolutionDirectoryInfo(string? currentPath = null)
    {
        var directory = new DirectoryInfo(
            currentPath ?? Directory.GetCurrentDirectory()
        );
        while (directory != null && !directory.GetFiles("*.sln").Any())
        {
            directory = directory.Parent;
        }

        return directory;
    }

    public static void Main()
    {
        var directory = TryGetSolutionDirectoryInfo();
        if (directory != null)
        {
            var typescriptOutPath = $"{directory}/ExampleUsage/Out";
            var csharpModelsPaths = new List<string>
            {
                $"{directory}/ExampleUsage/Models"
            };
            var outSubFolders = new List<string>
            {
                "addresses",
                "products",
                "orders",
                "payments",
                "customers",
                "categories",
            };

            var convertModels = new ConvertModels(
                typescriptOutPath: typescriptOutPath,
                csharpModelsPaths: csharpModelsPaths,
                outSubFolders: outSubFolders
            );
            convertModels.GenerateTypeScriptInterfaces();
        }
    }
}