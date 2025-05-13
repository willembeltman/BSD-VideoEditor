using SharpDX.Mathematics.Interop;
using System.Diagnostics;
using System.Drawing;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Forms.Generic;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Entities;
using VideoEditorD3D.Application.Types;
using VideoEditorD3D.Direct3D.Drawing;
using Rectangle = SharpDX.Rectangle;
using Point = System.Drawing.Point;
using VideoEditorD3D.Application.Timeline;

namespace VideoEditorD3D.Application.Controls;

public class TimelineControl : BackControl
{
    private readonly ApplicationContext Application;

    public GraphicsLayer BackgroundLayer { get; }

    private readonly GraphicsLayer TimeMarkersBackLayer;
    private readonly GraphicsLayer TimeMarkersForeLayer;
    private readonly GraphicsLayer VideoClipsLayer;
    private readonly GraphicsLayer PlayerPositionLayer;
    private readonly HScrollBar HScrollBarControl;

    private readonly AllBrushes Brushes = new();
    private readonly DragAndDrop DragAndDrop = new();
    private readonly Dragging SelectedClipsDragging = new();
    private readonly Dragging MiddleDragging = new();
    private readonly Scrolling Scrolling = new();
    private readonly List<System.Windows.Forms.Keys> Keys = new();

    private Timeline Timeline => Application.Timeline;
    private Rectangle TimelineRectangle => new Rectangle(0, 0, Width, Height - HScrollBarControl.Height);
    private int MiddleOffset => HScrollBarControl.Height / 2;

    public TimelineControl(ApplicationContext applicationContext, IApplicationForm applicationForm) : base(applicationForm)
    {
        Application = applicationContext;

        BackgroundLayer = CanvasLayers.CreateNewLayer();
        TimeMarkersBackLayer = CanvasLayers.CreateNewLayer();
        TimeMarkersForeLayer = CanvasLayers.CreateNewLayer();
        VideoClipsLayer = CanvasLayers.CreateNewLayer();
        PlayerPositionLayer = CanvasLayers.CreateNewLayer();

        HScrollBarControl = new HScrollBar(applicationForm);
        HScrollBarControl.Scroll += ScrollBarControl_Scroll;
        Controls.Add(HScrollBarControl);

        BackColor = new RawColor4(0.0f, 0.0f, 0.0f, 1);

        DragDrop += TimelineControl_DragDrop;
        DragEnter += TimelineControl_DragEnter;
        DragOver += TimelineControl_DragOver;
        DragLeave += TimelineControl_DragLeave;
        KeyDown += TimelineControl_KeyDown;
        KeyUp += TimelineControl_KeyUp;

        MouseDown += TimelineControl_MouseDown;
        MouseMove += TimelineControl_MouseMove;
        MouseUp += TimelineControl_MouseUp;
        MouseWheel += TimelineControl_MouseWheel;
    }

    public override void OnLoad()
    {
        base.OnLoad();
        SetupScrollbar();
    }
    public override void OnResize()
    {
        var marge = 0;
        HScrollBarControl.Height = 28;
        HScrollBarControl.Left = marge;
        HScrollBarControl.Width = Width - marge * 2;
        HScrollBarControl.Top = Height - HScrollBarControl.Height;
        base.OnResize();
    }
    public override void OnDraw()
    {
        BackgroundLayer.StartDrawing();
        TimeMarkersBackLayer.StartDrawing();
        TimeMarkersForeLayer.StartDrawing();
        VideoClipsLayer.StartDrawing();
        PlayerPositionLayer.StartDrawing();

        DrawBackground(BackgroundLayer);
        DrawTimeMarkers(TimeMarkersBackLayer, TimeMarkersForeLayer);
        DrawVideoClips(VideoClipsLayer);
        DrawPlayerPosition(PlayerPositionLayer);

        BackgroundLayer.EndDrawing();
        TimeMarkersBackLayer.EndDrawing();
        TimeMarkersForeLayer.EndDrawing();
        VideoClipsLayer.EndDrawing();
        PlayerPositionLayer.EndDrawing();

        base.OnDraw();
    }

    private void DrawBackground(GraphicsLayer RenderTarget)
    {
        RenderTarget.FillRectangle(0, 0, Width, Height, BackColor);
    }
    private void DrawTimeMarkers(GraphicsLayer background, GraphicsLayer foreground)
    {
        int middle = TimelineRectangle.Height / 2;
        int videoBlockHeight = (middle - MiddleOffset) / Timeline.VisibleVideoLayers;
        int audioBlockHeight = (middle - MiddleOffset) / Timeline.VisibleAudioLayers;

        // Vertical lines + layer numbers
        for (var i = 0; i < Timeline.VisibleVideoLayers; i++)
        {
            var y = TimelineRectangle.Top + middle - i * videoBlockHeight - MiddleOffset;
            background.DrawLine(0, y, TimelineRectangle.Width, y, Brushes.HorizontalLines);

            var text = $"{i + Timeline.FirstVisibleVideoLayer}";
            var meting = background.MeasureText(text, -1, -1, "Ebrima", 8, FontStyle.Regular, -2, Brushes.Text);
            var textY = y - videoBlockHeight / 2 - meting.Height / 2;
            foreground.DrawText(text, 2, textY, -1, -1, "Ebrima", 8, FontStyle.Regular, -2, Brushes.Text);
        }
        for (var i = 0; i < Timeline.VisibleAudioLayers; i++)
        {
            var y = TimelineRectangle.Top + middle + i * audioBlockHeight + MiddleOffset;
            background.DrawLine(0, y, TimelineRectangle.Width, y, Brushes.HorizontalLines);

            var text = $"{i + Timeline.FirstVisibleAudioLayer}";
            var meting = background.MeasureText(text, -1, -1, "Ebrima", 8, FontStyle.Regular, -2, Brushes.Text);
            var textY = y + audioBlockHeight / 2 - meting.Height / 2;
            foreground.DrawText(text, 2, textY, -1, -1, "Ebrima", 8, FontStyle.Regular, -2, Brushes.Text);
        }

        // Tijd stap bepalen
        var timeIncrease = 0.00001D;
        var decimals = 5;
        while (Width / Timeline.VisibleWidth * timeIncrease < 50)
        {
            timeIncrease *= 10;
            decimals--;
        }
        if (decimals < 0) decimals = 0;

        // Horizontal lines + time 
        for (var sec = 0D; sec < double.MaxValue; sec += timeIncrease)
        {
            var x = Convert.ToInt32((sec - Timeline.VisibleStart) / Timeline.VisibleWidth * Width);
            if (x >= Width) break;
            background!.DrawLine(x, 0, x, TimelineRectangle.Height, Brushes.VerticalLines);

            var text = $"{sec.ToString("F" + decimals)}s";
            var meting = background.MeasureText(text, -1, -1, "Ebrima", 8, FontStyle.Regular, -2, Brushes.Text);
            var textY = TimelineRectangle.Top + middle - meting.Height / 2;
            foreground.DrawText(text, 2, textY, -1, -1, "Ebrima", 8, FontStyle.Regular, -2, Brushes.Text);
        }
    }
    private void DrawVideoClips(GraphicsLayer RenderTarget)
    {
        var clips = Timeline.AllClips.Concat(DragAndDrop.AllClips);
        foreach (var clip in clips)
        {
            if (clip.IsVideoClip && Timeline.FirstVisibleVideoLayer <= clip.Layer ||
                clip.IsAudioClip && Timeline.FirstVisibleAudioLayer <= clip.Layer)
            {
                var rect = CalculateRectangle(clip);

                if (rect.Left > TimelineRectangle.Width || rect.Right < 0) continue; // Clip buiten zichtbare range
                if (rect.Top > TimelineRectangle.Height || rect.Bottom < 0) continue; // Clip buiten zichtbare range

                var selected = Timeline.SelectedClips.Contains(clip);
                var fillBrush = selected ? Brushes.SelectedClip : clip.IsVideoClip ? Brushes.VideoClip : Brushes.AudioClip;
                var borderPen = Brushes.ClipBorder;

                RenderTarget.FillRectangle(
                    Convert.ToInt32(rect.Left), 
                    Convert.ToInt32(rect.Top), 
                    Convert.ToInt32(rect.Right - rect.Left), 
                    Convert.ToInt32(rect.Bottom - rect.Top), fillBrush);
                RenderTarget.DrawRectangle(
                    Convert.ToInt32(rect.Left),
                    Convert.ToInt32(rect.Top),
                    Convert.ToInt32(rect.Right - rect.Left),
                    Convert.ToInt32(rect.Bottom - rect.Top), borderPen, 1);
            }
        }
    }
    private void DrawPlayerPosition(GraphicsLayer RenderTarget)
    {
        // Bereken de x-positie van de player marker
        int x = Convert.ToInt32((Timeline.CurrentTime - Timeline.VisibleStart) / Timeline.VisibleWidth * Width);

        // Zorg ervoor dat de lijn binnen de zichtbare regio valt
        if (x >= 0 && x <= Width)
        {
            RenderTarget.DrawLine(x, 0, x, TimelineRectangle.Height, Brushes.PositionLine, 2);
        }
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
    private void TimelineControl_MouseWheel(object? sender, System.Windows.Forms.MouseEventArgs e)
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
    private void ScrollY(int delta, TimelinePosition timelinePosition)
    {
        if (timelinePosition.TimelinePart == TimelinePart.Video)
        {
            // Video
            Timeline.FirstVisibleVideoLayer += delta;
            if (Timeline.FirstVisibleVideoLayer < 0) Timeline.FirstVisibleVideoLayer = 0;
        }
        if (timelinePosition.TimelinePart == TimelinePart.Video)
        {
            // Audio
            Timeline.FirstVisibleAudioLayer += delta;
            if (Timeline.FirstVisibleAudioLayer < 0) Timeline.FirstVisibleAudioLayer = 0;
        }
    }
    private void ZoomY(int delta, TimelinePosition timelinePosition)
    {
        if (timelinePosition.TimelinePart == TimelinePart.Video)
        {
            // Video
            Timeline.VisibleVideoLayers -= delta;
            if (Timeline.VisibleVideoLayers < 1) Timeline.VisibleVideoLayers = 1;
        }
        if (timelinePosition.TimelinePart == TimelinePart.Audio)
        {
            // Audio
            Timeline.VisibleAudioLayers -= delta;
            if (Timeline.VisibleAudioLayers < 1) Timeline.VisibleAudioLayers = 1;
        }
    }
    private void ZoomX(int delta, TimelinePosition timelinePosition)
    {
        if (delta > 0)
        {
            for (int i = 0; i < delta; i++)
            {
                Timeline.VisibleWidth -= Timeline.VisibleWidth / 10;
            }
        }
        if (delta < 0)
        {
            for (int i = 0; i < delta * -1; i++)
            {
                Timeline.VisibleWidth += Timeline.VisibleWidth / 10;
            }
        }
    }
    private void SetupScrollbar()
    {
        var max = Timeline.AudioClips.Any() ? Timeline.AudioClips.Max(a => a.EndTime) : Timeline.VisibleStart + Timeline.VisibleWidth;
        max = Math.Max(max, Timeline.VisibleStart + Timeline.VisibleWidth);
        HScrollBarControl.Minimum = 0;
        HScrollBarControl.Maximum = Convert.ToInt32(Math.Ceiling(max));
        HScrollBarControl.LargeChange = Convert.ToInt32(Math.Floor(Timeline.VisibleWidth));
    }

    private void ScrollBarControl_Scroll(object? sender, System.Windows.Forms.ScrollEventArgs e)
    {
        Timeline.VisibleStart = e.NewValue;
    }

    private void TimelineControl_DragEnter(object? sender, System.Windows.Forms.DragEventArgs e)
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
    }
    private void TimelineControl_DragOver(object? sender, System.Windows.Forms.DragEventArgs e)
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
    }
    private void TimelineControl_DragDrop(object? sender, System.Windows.Forms.DragEventArgs e)
    {
        var fullNames = GetDragAndDropFilenames(e);
        if (fullNames.Length == 0) return;

        foreach (var item in DragAndDrop.VideoClips)
            Timeline.VideoClips.Add(item);
        foreach (var item in DragAndDrop.AudioClips)
            Timeline.AudioClips.Add(item);

        TimelineControl_DragLeave(sender, e);
    }
    private void TimelineControl_DragLeave(object? sender, EventArgs e)
    {
        DragAndDrop.Clear();
        SetupScrollbar();
    }
    private string[] GetDragAndDropFilenames(System.Windows.Forms.DragEventArgs e)
    {
        if (e == null || e.Data == null) return [];
        if (!e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop)) return [];
        var filesobj = e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);
        if (filesobj == null) return [];
        var files = (string[])filesobj;
        if (files == null) return [];
        return CheckFileType.Filter(files)
            .OrderBy(a => a)
            .ToArray();
    }

    private void TimelineControl_MouseDown(object? sender, System.Windows.Forms.MouseEventArgs e)
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
            Timeline.CurrentTime = startposition.Value.CurrentTime;
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
    }
    private void TimelineControl_MouseMove(object? sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (!MiddleDragging.IsDragging && !SelectedClipsDragging.IsDragging) return;

        var endPoint = new Point(e.X, e.Y);
        var endPosition = GetTimelinePosition(endPoint);
        if (endPosition == null) return;

        Debug.WriteLine($"{endPosition.Value.Layer} {endPosition.Value.CurrentTime} {endPosition.Value.TimelinePart}");

        if (MiddleDragging.IsDragging && endPosition.Value.TimelinePart == TimelinePart.Middle)
        {
            Timeline.CurrentTime = endPosition.Value.CurrentTime;
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
                Debug.WriteLine($"Dragging {diff.CurrentTime}x{diff.Layer} {clip.OldTimelineStartTime}+{diff.CurrentTime.ToString("F3")}={clip.StartTime}");
            }
            return;
        }
    }
    private void TimelineControl_MouseUp(object? sender, System.Windows.Forms.MouseEventArgs e)
    {
        MiddleDragging.IsDragging = false;
        SelectedClipsDragging.IsDragging = false;
    }

    private TimelineClip[] GetSelectedClips(System.Windows.Forms.MouseEventArgs e)
    {
        var selectedClips = new List<TimelineClip>();
        foreach (var clip in Timeline.AllClips)
        {
            var rect = CalculateRectangle(clip);
            if (rect.Left < e.X && e.X < rect.Right &&
                rect.Top < e.Y && e.Y < rect.Bottom &&
                !selectedClips.Contains(clip))
            {
                selectedClips.Add(clip);

                foreach (var clip2 in Timeline.AllClips)
                {
                    if (clip2.TimelineClipGroup.Value?.Id == clip.TimelineClipGroup.Value?.Id &&
                        !selectedClips.Contains(clip2))
                    {
                        selectedClips.Add(clip2);
                    }
                }
            }
        }
        return selectedClips.ToArray();
    }
    private TimelinePosition? GetTimelinePosition(Point ucPoint)
    {
        var currentTime = Timeline.VisibleStart + Timeline.VisibleWidth * ucPoint.X / TimelineRectangle.Width;

        var timelineHeight = TimelineRectangle.Height;
        var videoHeight = timelineHeight / 2 - MiddleOffset;
        var middleHeight = MiddleOffset * 2;
        var audioHeight = timelineHeight - videoHeight - middleHeight;
        if (ucPoint.Y < videoHeight)
        {
            var videoLayerHeight = videoHeight / Timeline.VisibleVideoLayers;
            var relativeLayerIndex = Timeline.VisibleVideoLayers - ucPoint.Y / videoLayerHeight - 1;
            var layerIndex = Timeline.FirstVisibleVideoLayer + relativeLayerIndex;
            if (layerIndex < 0) layerIndex = 0;
            return new TimelinePosition(currentTime, layerIndex, TimelinePart.Video);
        }
        else if (ucPoint.Y >= videoHeight && ucPoint.Y < videoHeight + middleHeight)
        {
            return new TimelinePosition(currentTime, 0, TimelinePart.Middle);
        }
        else if (ucPoint.Y >= videoHeight + middleHeight && ucPoint.Y < timelineHeight)
        {
            var audioLayerHeight = audioHeight / Timeline.VisibleAudioLayers;
            var relativeLayerIndex = (ucPoint.Y - videoHeight) / audioLayerHeight - 1;
            var layerIndex = Timeline.FirstVisibleAudioLayer + relativeLayerIndex;
            if (layerIndex < 0) layerIndex = 0;
            return new TimelinePosition(currentTime, layerIndex, TimelinePart.Audio);
        }
        return null;
    }
    private RawRectangleF CalculateRectangle(TimelineClip clip)
    {
        int middle = TimelineRectangle.Height / 2;
        int videoBlockHeight = (middle - HScrollBarControl.Height / 2) / Timeline.VisibleVideoLayers;
        int audioBlockHeight = (middle - MiddleOffset) / Timeline.VisibleAudioLayers;

        int x1 = Convert.ToInt32((clip.StartTime - Timeline.VisibleStart) / Timeline.VisibleWidth * TimelineRectangle.Width);
        int x2 = Convert.ToInt32((clip.EndTime - Timeline.VisibleStart) / Timeline.VisibleWidth * TimelineRectangle.Width);
        int width = x2 - x1;

        if (clip.IsVideoClip)
        {
            var layer = clip.Layer - Timeline.FirstVisibleVideoLayer;
            var y = middle - MiddleOffset - videoBlockHeight - layer * videoBlockHeight;
            var rect = new RawRectangleF(x1, y + Constants.Margin / 2, x1 + width, y + Constants.Margin / 2 + videoBlockHeight - Constants.Margin);
            return rect;
        }
        else
        {
            var layer = clip.Layer - Timeline.FirstVisibleAudioLayer;
            var y = middle + MiddleOffset + (clip.Layer - Timeline.FirstVisibleAudioLayer) * audioBlockHeight;
            var rect = new RawRectangleF(x1, y + Constants.Margin / 2, x1 + width, y + Constants.Margin / 2 + audioBlockHeight - Constants.Margin);
            return rect;
        }
    }

}
