using System.Text.RegularExpressions;

namespace Main.Utils;

public class StringUtils
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
}