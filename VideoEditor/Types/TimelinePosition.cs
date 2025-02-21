using VideoEditor.Enums;

namespace VideoEditor.Types;

public readonly struct TimelinePosition
{
    public TimelinePosition(double currentTime, int layerIndex, MediaFormat mediaFormat)
    {
        CurrentTime = currentTime;
        Layer = layerIndex;
        MediaFormat = mediaFormat;
    }

    public double CurrentTime { get; }
    public int Layer { get; }
    public MediaFormat MediaFormat { get; }
}
