using VideoEditorD3D.FF.Types;

namespace VideoEditor.Static
{
    public class ClipFrame
    {
        public ClipFrame(TimelineClipVideo clip, Frame? frame)
        {
            Clip = clip;
            Frame = frame;
        }

        public TimelineClipVideo Clip { get; }
        public Frame? Frame { get; }
    }
}