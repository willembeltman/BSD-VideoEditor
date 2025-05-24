using System.Diagnostics;
using VideoEditorD3D.Direct3D.Controls.Templates;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Entities;
using Point = System.Drawing.Point;

namespace VideoEditorD3D.Application.Controls.Timeline;

public partial class TimelineControl : BackControl
{

    private void TimelineControl_Load(object? sender, EventArgs e)
    {
        SetupScrollbar();
    }
    private void TimelineControl_Resize(object? sender, EventArgs e)
    {
        var marge = 0;
        HScrollBarControl.Height = 28;
        HScrollBarControl.Left = marge;
        HScrollBarControl.Width = Width - marge * 2;
        HScrollBarControl.Top = Height - HScrollBarControl.Height;
    }

    private void TimelineControl_Draw(object? sender, EventArgs e)
    {

        TimeMarkersBackLayer.StartDrawing();
        TimeMarkersForeLayer.StartDrawing();
        VideoForeClipsLayer.StartDrawing();
        PlayerPositionLayer.StartDrawing();

        DrawTimeMarkers(TimeMarkersBackLayer, TimeMarkersForeLayer);
        DrawVideoClips(VideoForeClipsLayer);
        DrawPlayerPosition(PlayerPositionLayer);

        TimeMarkersBackLayer.EndDrawing();
        TimeMarkersForeLayer.EndDrawing();
        VideoForeClipsLayer.EndDrawing();
        PlayerPositionLayer.EndDrawing();
    }

    private void TimelineControl_KeyDown(object? sender, System.Windows.Forms.KeyEventArgs e)
    {
        if (e.KeyCode == System.Windows.Forms.Keys.Delete || e.KeyCode == System.Windows.Forms.Keys.Back)
        {
            // Delete
        }
        Keys.Add(e.KeyCode);
    }
    private void TimelineControl_KeyUp(object? sender, System.Windows.Forms.KeyEventArgs e)
    {
        Keys.Clear();
    }
    private void TimelineControl_MouseWheel(object? sender, MouseEvent e)
    {
        var scrollDelta = Scrolling.GetScrollDelta(e);

        var applicationPoint = new Point(e.X, e.Y);
        var timelinePosition = GetTimelinePosition(applicationPoint);
        if (timelinePosition == null) return;

        if (Keys.Any(a => a == System.Windows.Forms.Keys.Control) ||
            Keys.Any(a => a == System.Windows.Forms.Keys.ControlKey))
        {
            ZoomX(scrollDelta, timelinePosition.Value);
        }
        else if (Keys.Any(a => a == System.Windows.Forms.Keys.Shift) ||
                 Keys.Any(a => a == System.Windows.Forms.Keys.ShiftKey))
        {
            ZoomY(scrollDelta, timelinePosition.Value);
        }
        else
        {
            ScrollY(scrollDelta, timelinePosition.Value);
        }
        SetupScrollbar();
        Invalidate();
    }

    private void ScrollBarControl_Scroll(object? sender, System.Windows.Forms.ScrollEventArgs e)
    {
        VisibleStart = e.NewValue;
    }

    private void TimelineControl_DragEnter(object? sender, DragEvent e)
    {
        var fullNames = GetDragAndDropFilenames(e);
        if (fullNames.Length == 0)
        {
            DragAndDrop.Clear();
            return;
        }

        var formPoint = new Point(e.X, e.Y);
        var timelinePosition = GetTimelinePosition(formPoint);
        if (timelinePosition == null)
        {
            DragAndDrop.Clear();
            return;
        }

        e.Effect = System.Windows.Forms.DragDropEffects.Copy;

        var currentTime = timelinePosition.Value.CurrentTime;
        var layerIndex = timelinePosition.Value.Layer;
        foreach (var fullName in fullNames)
        {
            var file = VideoEditorD3D.FFMpeg.MediaContainer.Open(fullName);
            if (file == null) continue;
            if (file.Duration == null) continue;

            var group = new TimelineClipGroup();
            var start = currentTime;
            currentTime += file.Duration.Value;
            var layer = layerIndex;
            foreach (var videoStream in file.VideoStreams.OrderBy(a => a.Index))
            {
                var clip = new TimelineClipVideo()
                {
                    TimelineId = Timeline.Id,
                    TimelineClipGroupId = group.Id,
                    StreamInfo = videoStream,
                    Layer = layer,
                    StartTime = start,
                    LengthTime = file.Duration.Value,
                    ClipStartTime = 0,
                    ClipLengthTime = file.Duration.Value
                };
                DragAndDrop.VideoClips.Add(clip);
                layer++;
            }

            layer = 0;
            foreach (var audioStream in file.AudioStreams.OrderBy(a => a.Index))
            {
                var clip = new TimelineClipAudio()
                {
                    TimelineId = Timeline.Id,
                    TimelineClipGroupId = group.Id,
                    StreamInfo = audioStream,
                    Layer = layer,
                    StartTime = start,
                    EndTime = currentTime,
                    ClipStartTime = 0,
                    ClipEndTime = file.Duration.Value
                };
                DragAndDrop.AudioClips.Add(clip);
                layer++;
            }

            DragAndDrop.MediaContainers.Add(file);
        }

        Invalidate();
    }
    private void TimelineControl_DragOver(object? sender, DragEvent e)
    {
        var fullNames = GetDragAndDropFilenames(e);
        if (fullNames.Length == 0)
        {
            DragAndDrop.Clear();
            return;
        }

        var formPoint = new Point(e.X, e.Y);
        var timelinePosition = GetTimelinePosition(formPoint);
        if (timelinePosition == null)
        {
            DragAndDrop.Clear();
            return;
        }

        var currentTime = timelinePosition.Value.CurrentTime;
        var layerIndex = timelinePosition.Value.Layer;
        foreach (var fullName in fullNames)
        {
            var file = DragAndDrop.MediaContainers.FirstOrDefault(a => a.FullName == fullName);
            if (file == null) continue;
            if (file.Duration == null) continue;

            var start = currentTime;
            currentTime += file.Duration.Value;
            var layer = layerIndex;
            foreach (var videoStream in file.VideoStreams.OrderBy(a => a.Index))
            {
                var cachedVideoStream = DragAndDrop.VideoClips
                    .FirstOrDefault(a => a.StreamInfo.EqualTo(videoStream));

                if (cachedVideoStream != null)
                {
                    cachedVideoStream.Layer = layer;
                    cachedVideoStream.StartTime = start;
                    cachedVideoStream.EndTime = currentTime;
                    layer++;
                }
            }

            layer = layerIndex;
            foreach (var audioStream in file.AudioStreams.OrderBy(a => a.Index))
            {
                var cachedAudioStream = DragAndDrop.AudioClips
                    .FirstOrDefault(a => a.StreamInfo.EqualTo(audioStream));

                if (cachedAudioStream != null)
                {
                    cachedAudioStream.Layer = layer;
                    cachedAudioStream.StartTime = start;
                    cachedAudioStream.EndTime = currentTime;
                    layer++;
                }
            }
        }

        SetupScrollbar();
        Invalidate();
    }
    private void TimelineControl_DragDrop(object? sender, DragEvent e)
    {
        var fullNames = GetDragAndDropFilenames(e);
        if (fullNames.Length == 0) return;

        foreach (var item in DragAndDrop.VideoClips)
            Timeline.VideoClips.Add(item);
        foreach (var item in DragAndDrop.AudioClips)
            Timeline.AudioClips.Add(item);

        TimelineControl_DragLeave(sender, e);
        Invalidate();
    }
    private void TimelineControl_DragLeave(object? sender, EventArgs e)
    {
        DragAndDrop.Clear();
        SetupScrollbar();
        Invalidate();
    }

    private void TimelineControl_MouseDown(object? sender, MouseEvent e)
    {
        /// Er zijn 3 verschillende mogelijkheden:
        /// 1. Niets is geselecteerd en je selecteerd een clip/groep = Selecteren
        /// 2. Clip/groep is geselecteerd en daar klik je op = Dragging
        /// 3. Je klikt op het midden


        var startpoint = new Point(e.X, e.Y);
        var startposition = GetTimelinePosition(startpoint);
        if (startposition == null) return;

        Debug.WriteLine($"{startposition.Value.Layer} {startposition.Value.CurrentTime} {startposition.Value.TimelinePart}");

        if (startposition.Value.TimelinePart == TimelinePart.Middle)
        {
            MiddleDragging.Set(startpoint, startposition);
            CurrentTime = startposition.Value.CurrentTime;
            return;
        }

        var selectedClips = GetSelectedClips(e);
        if (Enumerable.Any<TimelineClip>(selectedClips, a => Timeline.SelectedClips.Any((object b) => b.Equals(a))))
        {
            SelectedClipsDragging.Set(startpoint, startposition);
            foreach (var clip in selectedClips)
            {
                clip.OldLayer = clip.Layer;
                clip.OldTimelineStartTime = clip.StartTime;
            }
            return;
        }

        Timeline.SelectedClips.Clear();
        Timeline.SelectedClips.AddRange(selectedClips);
        Invalidate();
    }
    private void TimelineControl_MouseMove(object? sender, MouseEvent e)
    {
        if (!MiddleDragging.IsDragging && !SelectedClipsDragging.IsDragging) return;

        var endPoint = new Point(e.X, e.Y);
        var endPosition = GetTimelinePosition(endPoint);
        if (endPosition == null) return;

        //Debug.WriteLine($"{endPosition.Value.Layer} {endPosition.Value.CurrentTime} {endPosition.Value.TimelinePart}");

        if (MiddleDragging.IsDragging && endPosition.Value.TimelinePart == TimelinePart.Middle)
        {
            CurrentTime = endPosition.Value.CurrentTime;
            Invalidate();
            return;
        }

        if (SelectedClipsDragging.IsDragging && endPosition.Value != SelectedClipsDragging.StartPosition)
        {
            var diff = endPosition.Value - SelectedClipsDragging.StartPosition;
            foreach (var clip in Timeline.SelectedClips)
            {
                clip.Layer = clip.OldLayer + diff.Layer;
                if (clip.Layer < 0) clip.Layer = 0;
                clip.StartTime = clip.OldTimelineStartTime + diff.CurrentTime;
                //Debug.WriteLine($"Dragging {diff.CurrentTime}x{diff.Layer} {clip.OldTimelineStartTime}+{diff.CurrentTime.ToString("F3")}={clip.StartTime}");
            }
            Invalidate();
            return;
        }
    }
    private void TimelineControl_MouseUp(object? sender, MouseEvent e)
    {
        MiddleDragging.IsDragging = false;
        SelectedClipsDragging.IsDragging = false;
        Invalidate();
    }

}
