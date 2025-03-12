namespace Typescript.Internal.Interfaces;

public interface IPaths
{
    public static abstract string GetPredefinedOutputFolder(string typeName, IEnumerable<string> folders);

    public static abstract string GetRelativeImportPath(string currentRelativeFolder, string dependencyRelativeFolder,
        string basePath, string depFileName);
}