namespace WoodenWorkshop.Common.Extensions;

public static class StringExtensions
{
    public static bool ContainsIgnoreCase(this string sourceString, string value)
    {
        return sourceString.Contains(value, StringComparison.OrdinalIgnoreCase);
    }
}