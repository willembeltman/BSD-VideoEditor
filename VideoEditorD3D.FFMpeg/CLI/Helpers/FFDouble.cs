namespace VideoEditorD3D.FFMpeg.CLI.Helpers;

public static class FFDouble
{
    public static bool TryParse(string? doubleString, out double value)
    {
        return double.TryParse(FFConvert.ReplaceNumber(doubleString), out value);
    }
}
