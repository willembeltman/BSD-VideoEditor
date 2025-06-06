using System.Diagnostics;
using System.Formats.Tar;
using VideoEditorD3D.Application.Buffers;
using VideoEditorD3D.Direct3D.Events;
using VideoEditorD3D.Entities;
using VideoEditorD3D.FFMpeg.CLI;
using Point = System.Drawing.Point;

namespace VideoEditorD3D.Application.Controls.TimelineControl;

public partial class TimelineControl
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

        var timelinePosition = GetTimelinePosition(new Point(e.X, e.Y));
        if (timelinePosition == null)
        {
            DragAndDrop.Clear();
            return;
        }

        e.Effect = System.Windows.Forms.DragDropEffects.Copy;

        var currentTime = timelinePosition.Value.CurrentTime;
        var layerStartIndex = timelinePosition.Value.Layer;
        foreach (var fullName in fullNames)
        {
            var mediaContainer = MediaContainerInfo.Open(fullName);
            if (mediaContainer == null) continue;
            if (mediaContainer.Duration == null) continue;
            var duration = mediaContainer.Duration.Value;

            var mediaFile = DragAndDrop.MediaFiles.FirstOrDefault(a => a.FullName == fullName);
            if (mediaFile == null)
            {
                mediaFile = new MediaFile
                {
                    Duration = duration,
                    FullName = fullName
                };
                DragAndDrop.MediaFiles.Add(mediaFile);
            }

            var start = currentTime;
            var layerIndex = layerStartIndex;
            foreach (var videoStream in mediaContainer.VideoStreams.OrderBy(a => a.Index))
            {
                var mediaStream = new MediaStream()
                {
                    Index = videoStream.Index,
                    IsVideo = true,
                    Resolution = videoStream.Resolution!.Value,
                    Fps = videoStream.Fps!.Value,
                };
                mediaStream.MediaFile.Value = mediaFile;
                //mediaFile.MediaStreams.Add(mediaStream);

                DragAndDrop.MediaStreams.Add(mediaStream);

                var clip = new TimelineClipVideo()
                {
                    TimelineLayer = layerIndex,
                    TimelineStartTime = start,
                    TimelineLengthTime = duration,
                    ClipStartTime = 0,
                    ClipLengthTime = duration,
                };
                clip.MediaStream.Value = mediaStream;
                clip.Timeline.Value = Timeline;
                //mediaStream.TimelineClipVideos.Add(clip);

                DragAndDrop.VideoClips.Add(clip);
                layerIndex++;
            }

            layerIndex = 0;
            foreach (var audioStream in mediaContainer.AudioStreams.OrderBy(a => a.Index))
            {
                var mediaStream = new MediaStream()
                {
                    Index = audioStream.Index,
                    IsAudio = true,
                    SampleRate = audioStream.SampleRate,
                    Channels = audioStream.Channels
                };
                mediaStream.MediaFile.Value = mediaFile;
                //mediaFile.MediaStreams.Add(mediaStream);

                DragAndDrop.MediaStreams.Add(mediaStream);

                var clip = new TimelineClipAudio()
                {
                    TimelineLayer = layerIndex,
                    TimelineStartTime = start,
                    TimelineLengthTime = duration,
                    ClipStartTime = 0,
                    ClipEndTime = duration,
                };
                clip.MediaStream.Value = mediaStream;
                clip.Timeline.Value = Timeline;
                //mediaStream.TimelineClipAudios.Add(clip);

                DragAndDrop.AudioClips.Add(clip);
                layerIndex++;
            }

            currentTime += duration;
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
            var file = DragAndDrop.MediaFiles.FirstOrDefault(a => a.FullName == fullName);
            if (file == null) continue;

            var layer = layerIndex;
            var videoClips = DragAndDrop.VideoClips
                .Where(a => a.MediaStream.Value.MediaFile.Value.FullName == fullName)
                .OrderBy(a => a.MediaStream.Value.Index);
            foreach (var videoClip in videoClips)
            {
                videoClip.TimelineLayer = layer;
                videoClip.TimelineStartTime = currentTime;
                layer++;
            }

            layer = layerIndex;
            var audioClips = DragAndDrop.AudioClips
                .Where(a => a.MediaStream.Value.MediaFile.Value.FullName == fullName)
                .OrderBy(a => a.MediaStream.Value.Index);
            foreach (var audioClip in audioClips)
            {
                audioClip.TimelineLayer = layer;
                audioClip.TimelineStartTime = currentTime;
                layer++;
            }

            currentTime += file.Duration;
        }

        SetupScrollbar();
    }
    private void TimelineControl_DragDrop(object? sender, DragEvent e)
    {
        var fullNames = GetDragAndDropFilenames(e);
        if (fullNames.Length == 0) return;

        foreach (var item in DragAndDrop.VideoClips)
        {
            var timelineGroup = GetTimelineGroup(item.MediaStream.Value.MediaFile.Value);

            item.TimelineClipGroupId = timelineGroup.Id;
            Timeline.TimelineClipVideos.Add(item);

            State.VideoBuffers.Add(new SoftwareVideoBuffer(Timeline, item));
        }
        foreach (var item in DragAndDrop.AudioClips)
        {
            var timelineGroup = GetTimelineGroup(item.MediaStream.Value.MediaFile.Value);

            item.TimelineClipGroupId = timelineGroup.Id;
            Timeline.TimelineClipAudios.Add(item);

            State.AudioBuffers.Add(new AudioBuffer(Timeline, item));
        }

        TimelineControl_DragLeave(sender, e);
    }

    private TimelineClipGroup GetTimelineGroup(MediaFile mediaFile)
    {
        var group = Timeline.TimelineClipGroups.FirstOrDefault(a => a.MediaFileId == mediaFile.Id);
        if (group == null)
        {
            group = new TimelineClipGroup()
            {
                MediaFileId = mediaFile.Id
            };
            Timeline.TimelineClipGroups.Add(group);
        }
        return group;
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
        /// 3. Je klikt op het midden = currentTime veranderen


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
                clip.OldLayer = clip.TimelineLayer;
                clip.OldTimelineStartTime = clip.TimelineStartTime;
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
                clip.TimelineLayer = clip.OldLayer + diff.Layer;
                if (clip.TimelineLayer < 0) clip.TimelineLayer = 0;
                clip.TimelineStartTime = clip.OldTimelineStartTime + diff.CurrentTime;
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
