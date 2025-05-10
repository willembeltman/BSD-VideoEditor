namespace VideoEditorD3D.FFMpeg.Helpers;

public static class FFInt
{

    public static bool TryParse(string? intString, out int value)
    {
        return int.TryParse(FFConvert.ReplaceNumber(intString), out value);
    }
}
