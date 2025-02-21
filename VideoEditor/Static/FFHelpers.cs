namespace VideoEditor.Static;

public static class FFHelpers
{
    public static string? ReplaceNumber(string? text)
    {
        if (text == null) return null;
        return text
            .Replace(".", Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)
            .Replace(",", Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
    }

    public static bool TryParseToDouble(string? doubleString, out double value)
    {
        return double.TryParse(ReplaceNumber(doubleString), out value);
    }

    public static bool TryParseToInt(string? intString, out int value)
    {
        return int.TryParse(intString, out value);
    }
}
