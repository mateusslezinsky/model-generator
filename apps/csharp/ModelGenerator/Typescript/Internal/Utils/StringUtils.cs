using System.Text.RegularExpressions;
using Typescript.Internal.Interfaces;

namespace Typescript.Internal.Utils;

public class StringUtils : IStringUtils
{
    public static string CamelCaseName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;
        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }

    public static string FirstCharToLowerCase(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }
    
    public static string GetFirstWord(string typeName)
    {
        var match = Regex.Match(typeName, @"^([A-Z]+[a-z]*)");
        return match.Success ? match.Groups[1].Value : typeName;
    }
    
    public static string PascalToUppercaseWithUnderscores(string input)
    {
        return Regex.Replace(input, "(?<!^)([A-Z])", "_$1")
            .ToUpperInvariant();
    }
}