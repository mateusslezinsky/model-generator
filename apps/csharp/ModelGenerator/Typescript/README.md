# Model Generator

This library aims to generate Typescript interfaces from many programming languages.

In the backend development it is very common to create DTOs (Data Transfer Objects) to represent the data that is being sent or received by the API. It is also very common to have models that generate database migrations. This library generates Typescript interfaces for frontend (whether it is to be used in React, Angular, or any other web frameworks) from the backend.

## Usage
```csharp
 // Import statements
 // ...
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
```

To get started, you must provide the paths to the C# models and the path where the Typescript interfaces will be generated. The `outSubFolders` parameter is optional and it is used to aggregate the interfaces in subfolders.
We have a base class called `ConvertModels` and at this class we have exposed methods to convert to either interfaces, types or classes in typescript. You can also find this example at `ExampleUsage` C# project.

## Contributing

If you want to contribute to this project, please read the [CONTRIBUTING.md](CONTRIBUTING.md) file.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.