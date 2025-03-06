using Main.Internal.Domain;

namespace Main.Utils;

public class Paths
{
    
    public static DirectoryInfo? TryGetSolutionDirectoryInfo(string? currentPath = null)
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
    public static string GetPredefinedOutputFolder(string typeName, List<Category> categories)
    {
        foreach (var category in categories)
        {
            if (typeName.IndexOf(category.Name, StringComparison.OrdinalIgnoreCase) >= 0)
                return category.Folder;
        }
        return "";
    }
    
    public static string GetRelativeImportPath(string currentRelativeFolder, string dependencyRelativeFolder,
        string basePath, string depFileName)
    {
        string currentAbs = string.IsNullOrEmpty(currentRelativeFolder)
            ? basePath
            : Path.Combine(basePath, currentRelativeFolder);
        string depAbs = string.IsNullOrEmpty(dependencyRelativeFolder)
            ? basePath
            : Path.Combine(basePath, dependencyRelativeFolder);
        currentAbs = Path.GetFullPath(currentAbs).Replace("\\", "/");
        depAbs = Path.GetFullPath(depAbs).Replace("\\", "/");
        string relPath = Path.GetRelativePath(currentAbs, depAbs).Replace("\\", "/");
        if (string.IsNullOrEmpty(relPath) || relPath == ".")
            return $"./{depFileName}";
        if (!relPath.StartsWith("."))
            relPath = "./" + relPath;
        return $"{relPath}/{depFileName}";
    }
}