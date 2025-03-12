namespace Typescript.Internal.Interfaces;

public interface IStringUtils
{
    public static abstract string CamelCaseName(string name);
    public static abstract string FirstCharToLowerCase(string str);
    public static abstract string GetFirstWord(string typeName);
    public static abstract string PascalToUppercaseWithUnderscores(string input);

}