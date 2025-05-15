using SharpDX.Mathematics.Interop;
using System.Diagnostics;
using System.Drawing;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Entities;
using VideoEditorD3D.Application.Types;
using VideoEditorD3D.Direct3D.Drawing;
using Rectangle = SharpDX.Rectangle;
using Point = System.Drawing.Point;
using VideoEditorD3D.Application.Controls.TimelineHelpers;
using VideoEditorD3D.Direct3D.Controls;
using VideoEditorD3D.Direct3D.Controls.Templates;

namespace VideoEditorD3D.Application.Controls;

public class TimelineControl : BackControl
{
    private readonly ApplicationContext ApplicationContext;

    private readonly GraphicsLayer TimeMarkersBackLayer;
    private readonly GraphicsLayer TimeMarkersForeLayer;
    private readonly GraphicsLayer VideoBackClipsLayer;
    private readonly GraphicsLayer VideoForeClipsLayer;
    private readonly GraphicsLayer PlayerPositionLayer;

    private readonly HScrollBar HScrollBarControl;

    private readonly DragAndDrop DragAndDrop = new();
    private readonly Dragging SelectedClipsDragging = new();
    private readonly Dragging MiddleDragging = new();
    private readonly Scrolling Scrolling = new();
    private readonly List<System.Windows.Forms.Keys> Keys = new();

    private Timeline Timeline => ApplicationContext.Timeline;
    private Rectangle TimelineRectangle => new(0, 0, Width, Height - HScrollBarControl.Height);
    private int MiddleOffset => HScrollBarControl.Height / 2;

    public double CurrentTime
    {
        get { return Timeline.CurrentTime; }
        set
        {
            Timeline.CurrentTime = value;
            Invalidate();
        }
    }
    public int VisibleAudioLayers
    {
        get { return Timeline.VisibleAudioLayers; }
        set
        {
            Timeline.VisibleAudioLayers = value;
            Invalidate();
        }
    }
    public int VisibleVideoLayers
    {
        get { return Timeline.VisibleVideoLayers; }
        set
        {
            Timeline.VisibleVideoLayers = value;
            Invalidate();
        }
    }
    public int FirstVisibleVideoLayer
    {
        get { return Timeline.FirstVisibleVideoLayer; }
        set
        {
            Timeline.FirstVisibleVideoLayer = value;
            Invalidate();
        }
    }
    public int FirstVisibleAudioLayer
    {
        get { return Timeline.FirstVisibleAudioLayer; }
        set
        {
            Timeline.FirstVisibleAudioLayer = value;
            Invalidate();
        }
    }
    public double VisibleWidth
    {
        get { return Timeline.VisibleWidth; }
        set
        {
            Timeline.VisibleWidth = value;
            Invalidate();
        }
    }
    public double VisibleStart
    {
        get { return Timeline.VisibleStart; }
        set
        {
            Timeline.VisibleStart = value;
            Invalidate();
        }
    }

    public TimelineControl(ApplicationContext applicationContext, IApplicationForm applicationForm) : base(applicationForm)
    {
        ApplicationContext = applicationContext;

        TimeMarkersBackLayer = GraphicsLayers.CreateNewLayer();
        TimeMarkersForeLayer = GraphicsLayers.CreateNewLayer();
        VideoBackClipsLayer = GraphicsLayers.CreateNewLayer();
        VideoForeClipsLayer = GraphicsLayers.CreateNewLayer();
        PlayerPositionLayer = GraphicsLayers.CreateNewLayer();

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
        base.OnDraw();

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

    private void DrawTimeMarkers(GraphicsLayer background, GraphicsLayer foreground)
    {
        int middle = TimelineRectangle.Height / 2;
        int videoBlockHeight = (middle - MiddleOffset) / VisibleVideoLayers;
        int audioBlockHeight = (middle - MiddleOffset) / VisibleAudioLayers;

        // Vertical lines + layer numbers
        for (var i = 0; i < VisibleVideoLayers; i++)
        {
            var y = TimelineRectangle.Top + middle - i * videoBlockHeight - MiddleOffset;
            background.DrawLine(0, y, TimelineRectangle.Width, y, ApplicationConstants.HorizontalLines);

            var text = $"{i + FirstVisibleVideoLayer}";
            var meting = background.MeasureText(text, -1, -1, "Ebrima", 8, FontStyle.Regular, -2, ApplicationConstants.Text);
            var textY = y - videoBlockHeight / 2 - meting.Height / 2;
            foreground.DrawText(text, 2, textY, -1, -1, "Ebrima", 8, FontStyle.Regular, -2, ApplicationConstants.Text);
        }
        for (var i = 0; i < VisibleAudioLayers; i++)
        {
            var y = TimelineRectangle.Top + middle + i * audioBlockHeight + MiddleOffset;
            background.DrawLine(0, y, TimelineRectangle.Width, y, ApplicationConstants.HorizontalLines);

            var text = $"{i + FirstVisibleAudioLayer}";
            var meting = background.MeasureText(text, -1, -1, "Ebrima", 8, FontStyle.Regular, -2, ApplicationConstants.Text);
            var textY = y + audioBlockHeight / 2 - meting.Height / 2;
            foreground.DrawText(text, 2, textY, -1, -1, "Ebrima", 8, FontStyle.Regular, -2, ApplicationConstants.Text);
        }

        // Tijd stap bepalen
        var timeIncrease = 0.00001D;
        var decimals = 5;
        while (Width / VisibleWidth * timeIncrease < 50)
        {
            timeIncrease *= 10;
            decimals--;
        }
        if (decimals < 0) decimals = 0;

        // Horizontal lines + time 
        for (var sec = 0D; sec < double.MaxValue; sec += timeIncrease)
        {
            var x = Convert.ToInt32((sec - VisibleStart) / VisibleWidth * Width);
            if (x >= Width) break;
            background!.DrawLine(x, 0, x, TimelineRectangle.Height, ApplicationConstants.VerticalLines);

            var text = $"{sec.ToString("F" + decimals)}s";
            var meting = background.MeasureText(text, -1, -1, "Ebrima", 8, FontStyle.Regular, -2, ApplicationConstants.Text);
            var textY = TimelineRectangle.Top + middle - meting.Height / 2;
            foreground.DrawText(text, x, textY, -1, -1, "Ebrima", 8, FontStyle.Regular, -2, ApplicationConstants.Text);
        }
    }
    private void DrawVideoClips(GraphicsLayer RenderTarget)
    {
        var clips = Timeline.AllClips.Concat(DragAndDrop.AllClips);
        foreach (var clip in clips)
        {
            if (clip.IsVideoClip && FirstVisibleVideoLayer <= clip.Layer ||
                clip.IsAudioClip && FirstVisibleAudioLayer <= clip.Layer)
            {
                var rect = CalculateRectangle(clip);

                if (rect.Left > TimelineRectangle.Width || rect.Right < 0) continue; // Clip buiten zichtbare range
                if (rect.Top > TimelineRectangle.Height || rect.Bottom < 0) continue; // Clip buiten zichtbare range

                var selected = Timeline.SelectedClips.Contains(clip);
                var fillBrush = selected ? ApplicationConstants.SelectedClip : clip.IsVideoClip ? ApplicationConstants.VideoClip : ApplicationConstants.AudioClip;
                var borderPen = ApplicationConstants.ClipBorder;

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
        int x = Convert.ToInt32((CurrentTime - VisibleStart) / VisibleWidth * Width);

        // Zorg ervoor dat de lijn binnen de zichtbare regio valt
        if (x >= 0 && x <= Width)
        {
            RenderTarget.DrawLine(x, 0, x, TimelineRectangle.Height, ApplicationConstants.PositionLine, 2);
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
            FirstVisibleVideoLayer += delta;
            if (FirstVisibleVideoLayer < 0) FirstVisibleVideoLayer = 0;
        }
        if (timelinePosition.TimelinePart == TimelinePart.Video)
        {
            // Audio
            FirstVisibleAudioLayer += delta;
            if (FirstVisibleAudioLayer < 0) FirstVisibleAudioLayer = 0;
        }
    }
    private void ZoomY(int delta, TimelinePosition timelinePosition)
    {
        if (timelinePosition.TimelinePart == TimelinePart.Video)
        {
            // Video
            VisibleVideoLayers -= delta;
            if (VisibleVideoLayers < 1) VisibleVideoLayers = 1;
        }
        if (timelinePosition.TimelinePart == TimelinePart.Audio)
        {
            // Audio
            VisibleAudioLayers -= delta;
            if (VisibleAudioLayers < 1) VisibleAudioLayers = 1;
        }
    }
    private void ZoomX(int delta, TimelinePosition timelinePosition)
    {
        if (delta > 0)
        {
            for (int i = 0; i < delta; i++)
            {
                VisibleWidth -= VisibleWidth / 10;
            }
        }
        if (delta < 0)
        {
            for (int i = 0; i < delta * -1; i++)
            {
                VisibleWidth += VisibleWidth / 10;
            }
        }
    }
    private void SetupScrollbar()
    {
        var max = Timeline.AudioClips.Count > 0 ? Timeline.AudioClips.Max(a => a.EndTime) : Timeline.VisibleStart + VisibleWidth;
        max = Math.Max(max, Timeline.VisibleStart + VisibleWidth);
        HScrollBarControl.Minimum = 0;
        HScrollBarControl.Maximum = Convert.ToInt32(Math.Ceiling(max));
        HScrollBarControl.LargeChange = Convert.ToInt32(Math.Floor(VisibleWidth));
    }

    private void ScrollBarControl_Scroll(object? sender, System.Windows.Forms.ScrollEventArgs e)
    {
        VisibleStart = e.NewValue;
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
            CurrentTime = endPosition.Value.CurrentTime;
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
        var currentTime = VisibleStart + VisibleWidth * ucPoint.X / TimelineRectangle.Width;

        var timelineHeight = TimelineRectangle.Height;
        var videoHeight = timelineHeight / 2 - MiddleOffset;
        var middleHeight = MiddleOffset * 2;
        var audioHeight = timelineHeight - videoHeight - middleHeight;
        if (ucPoint.Y < videoHeight)
        {
            var videoLayerHeight = videoHeight / VisibleVideoLayers;
            var relativeLayerIndex = VisibleVideoLayers - ucPoint.Y / videoLayerHeight - 1;
            var layerIndex = FirstVisibleVideoLayer + relativeLayerIndex;
            if (layerIndex < 0) layerIndex = 0;
            return new TimelinePosition(currentTime, layerIndex, TimelinePart.Video);
        }
        else if (ucPoint.Y >= videoHeight && ucPoint.Y < videoHeight + middleHeight)
        {
            return new TimelinePosition(currentTime, 0, TimelinePart.Middle);
        }
        else if (ucPoint.Y >= videoHeight + middleHeight && ucPoint.Y < timelineHeight)
        {
            var audioLayerHeight = audioHeight / VisibleAudioLayers;
            var relativeLayerIndex = (ucPoint.Y - videoHeight) / audioLayerHeight - 1;
            var layerIndex = FirstVisibleAudioLayer + relativeLayerIndex;
            if (layerIndex < 0) layerIndex = 0;
            return new TimelinePosition(currentTime, layerIndex, TimelinePart.Audio);
        }
        return null;
    }
    private RawRectangleF CalculateRectangle(TimelineClip clip)
    {
        int middle = TimelineRectangle.Height / 2;
        int videoBlockHeight = (middle - HScrollBarControl.Height / 2) / VisibleVideoLayers;
        int audioBlockHeight = (middle - MiddleOffset) / VisibleAudioLayers;

        int x1 = Convert.ToInt32((clip.StartTime - VisibleStart) / VisibleWidth * TimelineRectangle.Width);
        int x2 = Convert.ToInt32((clip.EndTime - VisibleStart) / VisibleWidth * TimelineRectangle.Width);
        int width = x2 - x1;

        if (clip.IsVideoClip)
        {
            var layer = clip.Layer - FirstVisibleVideoLayer;
            var y = middle - MiddleOffset - videoBlockHeight - layer * videoBlockHeight;
            var rect = new RawRectangleF(x1, y + ApplicationConstants.Margin / 2, x1 + width, y + ApplicationConstants.Margin / 2 + videoBlockHeight - ApplicationConstants.Margin);
            return rect;
        }
        else
        {
            var layer = clip.Layer - FirstVisibleAudioLayer;
            var y = middle + MiddleOffset + (clip.Layer - FirstVisibleAudioLayer) * audioBlockHeight;
            var rect = new RawRectangleF(x1, y + ApplicationConstants.Margin / 2, x1 + width, y + ApplicationConstants.Margin / 2 + audioBlockHeight - ApplicationConstants.Margin);
            return rect;
        }
    }

}
