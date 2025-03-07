using PluralizeService.Core;

namespace Typescript.Internal.Utils;

public class Paths
{
    public static string GetPredefinedOutputFolder(string typeName, IEnumerable<string> folders)
    {
        var foundFolder = folders.FirstOrDefault(
            folder =>
            {
                var singularizedFolder = PluralizationProvider.Singularize(folder);
                return StringUtils.FirstCharToLowerCase(typeName).Contains(singularizedFolder);
            });
        if (foundFolder != null)
            return foundFolder.ToLower();
        return "other";
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