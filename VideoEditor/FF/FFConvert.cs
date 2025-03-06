namespace VideoEditor.FF;

public static class FFConvert
{
    public static string? ReplaceNumber(string? text)
    {
        if (text == null) return null;
        return text
            .Replace(",", "")
            .Replace(".", Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
    }
}
