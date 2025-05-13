namespace VideoEditorD3D.Application.TimelineUtils;

public class Scrolling
{
    public int OldSmallScrollDelta { get; set; } = 0;
    public int TotalBigScrollDelta { get; set; } = 0;
    public int GetScrollDelta(System.Windows.Forms.MouseEventArgs e)
    {
        TotalBigScrollDelta += e.Delta;

        if (TotalBigScrollDelta / 120 == OldSmallScrollDelta)
            return 0;

        var delta = TotalBigScrollDelta / 120 - OldSmallScrollDelta;
        OldSmallScrollDelta = TotalBigScrollDelta / 120;
        return delta;
    }
}