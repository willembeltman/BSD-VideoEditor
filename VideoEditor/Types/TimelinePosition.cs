using VideoEditor.Enums;

namespace VideoEditor.Types;

public readonly struct TimelinePosition
{
    public TimelinePosition(double currentTime, int layerIndex, TimelinePart mediaFormat)
    {
        CurrentTime = currentTime;
        Layer = layerIndex;
        TimelinePart = mediaFormat;
    }

    public double CurrentTime { get; }
    public int Layer { get; }
    public TimelinePart TimelinePart { get; }
}
