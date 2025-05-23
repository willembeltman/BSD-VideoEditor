﻿using EntityFrameworkZip;
using VideoEditorD3D.FFMpeg;

namespace VideoEditorD3D.Entities;

public abstract class TimelineClip : IEntity
{
    public long Id { get; set; }
    public long TimelineId { get; set; }
    public long MediaStreamId { get; set; }
    public long TimelineClipGroupId { get; set; }
    public double StartTime { get; set; }
    public double LengthTime { get; set; }
    public double EndTime
    {
        get => StartTime + LengthTime; 
        set
        {
            LengthTime = value - StartTime;
            if (LengthTime < 0)
            {
                LengthTime = 0;
                StartTime = value;
            }
        }
    }
    public double ClipStartTime { get; set; }
    public double ClipLengthTime { get; set; }
    public double ClipEndTime
    {
        get => ClipStartTime + ClipLengthTime;
        set
        {
            ClipLengthTime = value - ClipStartTime;
            if (ClipLengthTime < 0)
            {
                ClipLengthTime = 0;
                ClipStartTime = value;
            }
        }
    }
    public int Layer { get; set; }

    public virtual ILazy<Timeline> Timeline { get; set; }
    public virtual ILazy<MediaStream> MediaStream { get; set; }
    public virtual ILazy<TimelineClipGroup> TimelineClipGroup { get; set; }

    [NotMapped]
    public abstract bool IsVideoClip { get; }
    [NotMapped]
    public abstract bool IsAudioClip { get; }
    [NotMapped]
    public StreamInfo StreamInfo { get; set; }
    [NotMapped]
    public int OldLayer { get; set; }
    [NotMapped]
    public double OldTimelineStartTime { get; set; }
}