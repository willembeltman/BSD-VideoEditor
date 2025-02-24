namespace VideoEditor.Types;

public struct TimelineCurrentTime
{
    public TimelineCurrentTime(double currentTime, TimelineClipVideo[] videoClips, TimelineClipAudio[] audioClips)
    {
        CurrentTime = currentTime;
        VideoClips = videoClips;
        AudioClips = audioClips;
    }
    public double CurrentTime { get; }
    public TimelineClipVideo[] VideoClips { get; }
    public TimelineClipAudio[] AudioClips { get; }
}
