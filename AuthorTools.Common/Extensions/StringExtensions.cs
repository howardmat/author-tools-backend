namespace AuthorTools.Common.Extensions;

public static class StringExtensions
{
    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
            return str;

        char[] chars = str.ToCharArray();
        chars[0] = char.ToLower(chars[0]);

        return new string(chars);
    }
}
