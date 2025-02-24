using VideoEditor.Enums;

namespace VideoEditor.Types;

public readonly struct TimelinePosition
{
    public TimelinePosition(TimelineCurrentTime currentTime, int layerIndex, TimelinePart mediaFormat)
    {
        CurrentTime = currentTime;
        Layer = layerIndex;
        TimelinePart = mediaFormat;
    }
    public TimelineCurrentTime CurrentTime { get; }
    public int Layer { get; }
    public TimelinePart TimelinePart { get; }
}
