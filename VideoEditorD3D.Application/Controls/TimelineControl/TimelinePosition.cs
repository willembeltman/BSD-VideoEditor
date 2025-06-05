namespace VideoEditorD3D.Application.Controls.TimelineControl;

public readonly struct TimelinePosition
{
    public TimelinePosition(
        double currentTime,
        int layerIndex,
        TimelinePart mediaFormat//, 
                                //TimelineClipVideo[] videoClips, 
                                //TimelineClipAudio[] audioClips
        )
    {
        CurrentTime = currentTime;
        Layer = layerIndex;
        TimelinePart = mediaFormat;
        //VideoClips = videoClips;
        //AudioClips = audioClips;
    }
    public double CurrentTime { get; }
    public int Layer { get; }
    public TimelinePart TimelinePart { get; }
    //public TimelineClipVideo[] VideoClips { get; }
    //public TimelineClipAudio[] AudioClips { get; }

    public static implicit operator double(TimelinePosition ms)
    {
        return ms.CurrentTime;
    }
    public static implicit operator int(TimelinePosition ms)
    {
        return ms.Layer;
    }
    public static bool operator ==(TimelinePosition p1, TimelinePosition p2)
    {
        return p1.Equals(p2);
    }
    public static bool operator !=(TimelinePosition p1, TimelinePosition p2)
    {
        return !p1.Equals(p2);
    }
    public static TimelinePosition operator +(TimelinePosition p1, TimelinePosition p2)
    {
        return new TimelinePosition(
            p1.CurrentTime + p2.CurrentTime,
            p1.Layer + p2.Layer,
            p1.TimelinePart
            );
    }
    public static TimelinePosition operator -(TimelinePosition p1, TimelinePosition p2)
    {
        return new TimelinePosition(
            p1.CurrentTime - p2.CurrentTime,
            p1.Layer - p2.Layer,
            p1.TimelinePart
            );
    }
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (!(obj is TimelinePosition?)) return false;

        var other = obj as TimelinePosition?;
        if (other == null) return false;
        if (CurrentTime != other.Value.CurrentTime) return false;
        if (Layer != other.Value.Layer) return false;
        if (TimelinePart != other.Value.TimelinePart) return false;
        return true;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(CurrentTime, Layer, TimelinePart);
    }
}
