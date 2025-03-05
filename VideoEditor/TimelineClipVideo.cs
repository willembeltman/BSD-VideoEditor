using VideoEditor.Static;
using VideoEditor.Types;

namespace VideoEditor;

public class TimelineClipVideo : TimelineClip, ITimelineClip, IDisposable
{
    public TimelineClipVideo(Timeline timeline, StreamInfo streamInfo, TimelineClipGroup group) : base(timeline, streamInfo, group)
    {
    }

    public double TimelineStartTime
    {
        get => Convert.ToDouble(TimelineStartIndex) * Timeline.Fps.Divider / Timeline.Fps.Base;
        set => TimelineStartIndex = Convert.ToInt64(value * Timeline.Fps.Base / Timeline.Fps.Divider);
    }
    public double TimelineEndTime
    {
        get => Convert.ToDouble(TimelineEndIndex) * Timeline.Fps.Divider / Timeline.Fps.Base;
        set => TimelineEndIndex = Convert.ToInt64(value * Timeline.Fps.Base / Timeline.Fps.Divider);
    }
    public double ClipStartTime
    {
        get => Convert.ToDouble(ClipStartIndex) * StreamInfo.Fps!.Value.Divider / StreamInfo.Fps!.Value.Base;
        set => ClipStartIndex = Convert.ToInt64(value * StreamInfo.Fps!.Value.Base / StreamInfo.Fps!.Value.Divider);
    }
    public double ClipEndTime
    {
        get => Convert.ToDouble(ClipEndIndex) * StreamInfo.Fps!.Value.Divider / StreamInfo.Fps!.Value.Base;
        set => ClipEndIndex = Convert.ToInt64(value * StreamInfo.Fps!.Value.Base / StreamInfo.Fps!.Value.Divider);
    }

    public bool IsVideoClip => true;
    public bool IsAudioClip => false;

    double NextTime => Convert.ToDouble(CurrentIndex + 1) * StreamInfo.Fps!.Value.Divider / StreamInfo.Fps!.Value.Base;

    double CurrentTime
    {
        get => Convert.ToDouble(CurrentIndex) * StreamInfo.Fps!.Value.Divider / StreamInfo.Fps!.Value.Base;
        set => CurrentIndex = Convert.ToInt64(value * StreamInfo.Fps!.Value.Base / StreamInfo.Fps!.Value.Divider);
    }
    public TimeStamp CurrentTimeStamp
    {
        get => new TimeStamp(CurrentTime);
        set => CurrentTime = value.TotalSeconds;
    }

    Resolution CurrentResolution { get; set; }
    IEnumerator<byte[]>? Source { get; set; }

    public byte[]? GetFrame()
    {
        var reload = false;

        if (Source == null) reload = true;
        else if (Timeline.Resolution != CurrentResolution) reload = true;
        else if (Timeline.CurrentTime < CurrentTime) reload = true;
        else if (Timeline.CurrentTime > CurrentTime + 5) reload = true;

        if (reload)
        {
            if (Source != null)
            {
                Source.Dispose();
            }
            CurrentResolution = Timeline.Resolution;
            CurrentTime = Timeline.CurrentTime;
            Source = FFMpeg
                .ReadFrames(StreamInfo.File.FullName, Timeline.Resolution, Timeline.CurrentTimeStamp)
                .GetEnumerator();
            if (!Source.MoveNext()) return null;
        }

        if (Source == null) return null;

        while (!(CurrentTime <= Timeline.CurrentTime && Timeline.CurrentTime < NextTime))
        {
            Source.MoveNext();
            CurrentIndex++;
        }
        if (Source.Current == null) return null;
        return Source.Current;
    }
    public void Dispose()
    {
        Source?.Dispose();
    }

    //bool IsRunning { get; set; }
    //Thread? Thread { get; set; }
    //public void Start()
    //{
    //    IsRunning = true;
    //    Thread = new Thread(new ThreadStart(TheThread));
    //    Thread.Start();
    //}
    //public void Dispose()
    //{
    //    IsRunning = false;
    //    if (Thread != null && Thread.CurrentThread != Thread) Thread.Join();
    //    Thread = null;
    //}
    //public void TheThread()
    //{
    //    while (Engine.IsRunning && IsRunning)
    //    {
    //        var currentTime = Timeline.CurrentTime;

    //    }
    //}

}

