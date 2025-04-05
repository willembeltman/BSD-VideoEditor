using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.WIC;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using Factory = SharpDX.Direct2D1.Factory;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Format = SharpDX.DXGI.Format;
using Rectangle = System.Drawing.Rectangle;
using Point = System.Drawing.Point;
using Color = System.Drawing.Color;
using SharpDX.Mathematics.Interop;
using VideoEditor.Types;
using System.Diagnostics;
using VideoEditor.Enums;
using VideoEditor.Static;

namespace VideoEditor.UI;

public class TimelineControlDX2D : Control
{
    private Engine Engine;
    private HScrollBar HScrollBarControl;

    private WindowsScaling? Scaling;
    private Factory? Factory;
    private ImagingFactory? ImagingFactory;
    private WindowRenderTarget? RenderTarget;
    private SolidColorBrushes? Brushes;

    public TimelineControlDX2D(Engine engine)
    {
        Engine = engine;
        Thread = new Thread(new ThreadStart(Kernel));
        FpsCounter = new FpsCounter();

        SuspendLayout();

        HScrollBarControl = new HScrollBar();
        HScrollBarControl.Scroll += ScrollBarControl_Scroll;
        Controls.Add(HScrollBarControl);

        AllowDrop = true;
        BackColor = Color.Black;
        Size = new Size(665, 430);

        DragDrop += TimelineControl_DragDrop;
        DragEnter += TimelineControl_DragEnter;
        DragOver += TimelineControl_DragOver;
        DragLeave += TimelineControl_DragLeave;
        KeyDown += TimelineControl_KeyDown;
        MouseDown += TimelineControl_MouseDown;
        MouseMove += TimelineControl_MouseMove;
        MouseUp += TimelineControl_MouseUp;
        MouseWheel += TimelineControl_MouseWheel;

        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

        ResumeLayout(false);
    }

    public FpsCounter FpsCounter { get; }
    public Thread Thread { get; }

    DragAndDrop DragAndDrop { get; } = new();
    Dragging SelectedClipsDragging { get; } = new();
    Dragging MiddleDragging { get; } = new();
    Scrolling Scrolling { get; } = new();

    Timeline Timeline => Engine.Timeline;
    SleepHelper SleepHelper => Engine.SleepHelper;

    Rectangle TimelineRectangle => new Rectangle(
        ClientRectangle.Left,
        ClientRectangle.Top,
        ClientRectangle.Width,
        ClientRectangle.Height - HScrollBarControl.Height);
    int MiddleOffset => HScrollBarControl.Height / 2;
    int PhysicalWidth => (int)(Width * Scaling);
    int PhysicalHeight => (int)(Height * Scaling);

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        Scaling = new WindowsScaling(Handle);
        Factory = new Factory();
        ImagingFactory = new ImagingFactory();

        var renderTargetProperties = new RenderTargetProperties
        {
            PixelFormat = new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Ignore)
        };

        var hwndProperties = new HwndRenderTargetProperties
        {
            Hwnd = Handle,
            PixelSize = new Size2(PhysicalWidth, PhysicalHeight),
            PresentOptions = PresentOptions.Immediately
        };

        RenderTarget = new WindowRenderTarget(Factory, renderTargetProperties, hwndProperties);
        Brushes = new SolidColorBrushes(RenderTarget);

        SetupScrollbar();
    }
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        lock (this)
        {
            RenderTarget?.Resize(new Size2(PhysicalWidth, PhysicalHeight));
        }

        HScrollBarControl.Left = 0;
        HScrollBarControl.Top = ClientRectangle.Height - HScrollBarControl.Height;
        HScrollBarControl.Width = ClientRectangle.Width;
    }

    public void Kernel()
    {
        while (Engine.IsRunning)
        {
            Thread.Sleep(SleepHelper.SleepTillNextFrame() + 4);
            //Debug.WriteLine(Timeline.CurrentFrameIndex + " Timeline");
            Draw();
            FpsCounter.Tick();
        }
    }

    public void Draw()
    {
        if (RenderTarget == null)
            return;

        lock (this)
        {
            RenderTarget.BeginDraw();
            RenderTarget.Clear(new Color4(0, 0, 0, 1));

            DrawTimeMarkers();
            DrawVideoClips();
            DrawPlayerPosition();

            RenderTarget.EndDraw();
        }
    }

    private void DrawTimeMarkers()
    {
        using var font = new Font("Arial", Constants.TextSize);
        using var brush = new SolidBrush(Color.White);

        int middle = TimelineRectangle.Height / 2;
        int videoBlockHeight = (middle - MiddleOffset) / Timeline.VisibleVideoLayers;
        int audioBlockHeight = (middle - MiddleOffset) / Timeline.VisibleAudioLayers;

        for (var i = 0; i < Timeline.VisibleVideoLayers; i++)
        {
            var y = TimelineRectangle.Top + middle - i * videoBlockHeight - MiddleOffset;
            RenderTarget.DrawLine(new RawVector2(0, y), new RawVector2(Width, y), Brushes.HorizontalLines);

            //var text = $"{i + Timeline.FirstVisibleVideoLayer}";
            //var textSize = RenderTarget.MeasureString(text, font);
            //var textY = y - videoBlockHeight / 2 - (int)(textSize.Height / 2);
            //RenderTarget.DrawString(text, font, brush, 2, textY);
        }
        for (var i = 0; i < Timeline.VisibleAudioLayers; i++)
        {
            var y = TimelineRectangle.Top + middle + i * audioBlockHeight + MiddleOffset;
            RenderTarget.DrawLine(new RawVector2(0, y), new RawVector2(Width, y), Brushes.HorizontalLines);

            //var text = $"{i + Timeline.FirstVisibleAudioLayer}";
            //var textSize = RenderTarget.MeasureString(text, font);
            //var textY = y + audioBlockHeight / 2 - (int)(textSize.Height / 2);
            //RenderTarget.DrawString(text, font, brush, 2, textY);
        }

        var timeIncrease = 0.01D;
        var decimals = 2;
        while (Width / Timeline.VisibleWidth * timeIncrease < 50)
        {
            timeIncrease *= 10;
            decimals--;
        }
        if (decimals < 0) decimals = 0;

        for (var sec = 0D; sec < double.MaxValue; sec += timeIncrease)
        {
            var x = Convert.ToInt32((sec - Timeline.VisibleStart) / Timeline.VisibleWidth * Width);
            if (x >= Width) break;
            RenderTarget.DrawLine(new RawVector2(x, 0), new RawVector2(x, Height), Brushes.VerticalLines);

            //var text = $"{sec.ToString("F" + decimals)}s";
            //var textSize = RenderTarget.MeasureString(text, font);
            //var textY = TimelineRectangle.Top + middle - (int)(textSize.Height / 2);
            //RenderTarget.DrawString(text, font, brush, x + 2, textY);
        }

    }
    private void DrawVideoClips()
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

                RenderTarget.FillRectangle(rect, fillBrush);
                RenderTarget.DrawRectangle(rect, borderPen);
            }
        }
    }
    private void DrawPlayerPosition()
    {
        using var pen = new Pen(Color.Red, 2);

        // Bereken de x-positie van de player marker
        int x = Convert.ToInt32((Timeline.CurrentTime - Timeline.VisibleStart) / Timeline.VisibleWidth * Width);

        // Zorg ervoor dat de lijn binnen de zichtbare regio valt
        if (x >= 0 && x <= Width)
        {
            RenderTarget.DrawLine(new RawVector2(x, 0), new RawVector2(x, Height), Brushes.PositionLine);
        }
    }

    private void TimelineControl_MouseWheel(object? sender, MouseEventArgs e)
    {
        var scrollDelta = Scrolling.GetScrollDelta(e);

        var applicationPoint = new Point(e.X, e.Y);
        var timelinePosition = GetTimelinePositionControl(applicationPoint);
        if (timelinePosition == null) return;

        if ((ModifierKeys & Keys.Control) == Keys.Control)
        {
            ZoomX(scrollDelta, timelinePosition.Value);
        }
        else if ((ModifierKeys & Keys.Shift) == Keys.Shift)
        {
            ZoomY(scrollDelta, timelinePosition.Value);
        }
        else
        {
            ScrollY(scrollDelta, timelinePosition.Value);
        }
        SetupScrollbar();
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
        var max = Timeline.AudioClips.Any() ? Timeline.AudioClips.Max(a => a.TimelineEndTime) : Timeline.VisibleStart + Timeline.VisibleWidth;
        max = Math.Max(max, Timeline.VisibleStart + Timeline.VisibleWidth);
        HScrollBarControl.Minimum = 0;
        HScrollBarControl.Maximum = Convert.ToInt32(Math.Ceiling(max));
        HScrollBarControl.SmallChange = 1;
        HScrollBarControl.LargeChange = Convert.ToInt32(Math.Floor(Timeline.VisibleWidth));
    }

    private void ScrollBarControl_Scroll(object? sender, ScrollEventArgs e)
    {
        Timeline.VisibleStart = e.NewValue;
    }


    private void TimelineControl_DragEnter(object? sender, DragEventArgs e)
    {
        var fullNames = GetDragAndDropFilenames(e);
        if (fullNames.Length == 0)
        {
            DragAndDrop.Clear();
            return;
        }

        var formPoint = new Point(e.X, e.Y);
        var timelinePosition = GetTimelinePositionForm(formPoint);
        if (timelinePosition == null)
        {
            DragAndDrop.Clear();
            return;
        }

        e.Effect = DragDropEffects.Copy;

        var currentTime = timelinePosition.Value.CurrentTime;
        var layerIndex = timelinePosition.Value.Layer;
        foreach (var fullName in fullNames)
        {
            var file = File.Open(fullName);
            if (file == null) continue;
            if (file.Duration == null) continue;

            var group = new TimelineClipGroup();
            var start = currentTime;
            currentTime += file.Duration.Value;
            var layer = layerIndex;
            foreach (var videoStream in file.VideoStreams.OrderBy(a => a.Index))
            {
                var clip = new TimelineClipVideo(Timeline, videoStream, group)
                {
                    Layer = layer,
                    TimelineStartTime = start,
                    TimelineLengthTime = file.Duration.Value,
                    ClipStartTime = 0,
                    ClipLengthTime = file.Duration.Value
                };
                DragAndDrop.VideoClips.Add(clip);
                layer++;
            }

            layer = 0;
            foreach (var audioStream in file.AudioStreams.OrderBy(a => a.Index))
            {
                var clip = new TimelineClipAudio(Timeline, audioStream, group)
                {
                    Layer = layer,
                    TimelineStartTime = start,
                    TimelineEndTime = currentTime,
                    ClipStartTime = 0,
                    ClipEndTime = file.Duration.Value
                };
                DragAndDrop.AudioClips.Add(clip);
                layer++;
            }

            DragAndDrop.Files.Add(file);
        }
    }
    private void TimelineControl_DragOver(object? sender, DragEventArgs e)
    {
        var fullNames = GetDragAndDropFilenames(e);
        if (fullNames.Length == 0)
        {
            DragAndDrop.Clear();
            return;
        }

        var formPoint = new Point(e.X, e.Y);
        var timelinePosition = GetTimelinePositionForm(formPoint);
        if (timelinePosition == null)
        {
            DragAndDrop.Clear();
            return;
        }

        var currentTime = timelinePosition.Value.CurrentTime;
        var layerIndex = timelinePosition.Value.Layer;
        foreach (var fullName in fullNames)
        {
            var file = DragAndDrop.Files.FirstOrDefault(a => a.FullName == fullName);
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
                    cachedVideoStream.TimelineStartTime = start;
                    cachedVideoStream.TimelineEndTime = currentTime;
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
                    cachedAudioStream.TimelineStartTime = start;
                    cachedAudioStream.TimelineEndTime = currentTime;
                    layer++;
                }
            }
        }

        SetupScrollbar();
    }
    private void TimelineControl_DragDrop(object? sender, DragEventArgs e)
    {
        var fullNames = GetDragAndDropFilenames(e);
        if (fullNames.Length == 0) return;

        Timeline.VideoClips.AddRange(DragAndDrop.VideoClips);
        Timeline.AudioClips.AddRange(DragAndDrop.AudioClips);

        TimelineControl_DragLeave(sender, e);
    }
    private void TimelineControl_DragLeave(object? sender, EventArgs e)
    {
        DragAndDrop.Clear();
        SetupScrollbar();
    }
    private string[] GetDragAndDropFilenames(DragEventArgs e)
    {
        if (e == null || e.Data == null) return [];
        if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return [];
        var filesobj = e.Data.GetData(DataFormats.FileDrop);
        if (filesobj == null) return [];
        var files = (string[])filesobj;
        if (files == null) return [];
        return CheckFileType.Filter(files)
            .OrderBy(a => a)
            .ToArray();
    }

    private void TimelineControl_MouseDown(object? sender, MouseEventArgs e)
    {
        /// Er zijn 3 verschillende mogelijkheden:
        /// 1. Niets is geselecteerd en je selecteerd een clip/groep = Selecteren
        /// 2. Clip/groep is geselecteerd en daar klik je op = Dragging
        /// 3. Je klikt op het midden


        var startpoint = new Point(e.X, e.Y);
        var startposition = GetTimelinePositionControl(startpoint);
        if (startposition == null) return;

        Debug.WriteLine($"{startposition.Value.Layer} {startposition.Value.CurrentTime} {startposition.Value.TimelinePart}");

        if (startposition.Value.TimelinePart == TimelinePart.Middle)
        {
            MiddleDragging.Set(startpoint, startposition);
            Timeline.CurrentTime = startposition.Value.CurrentTime;
            return;
        }

        var selectedClips = GetSelectedClips(e);
        if (selectedClips.Any(a => Timeline.SelectedClips.Any(b => b.Equals(a))))
        {
            SelectedClipsDragging.Set(startpoint, startposition);
            foreach (var clip in selectedClips)
            {
                clip.OldLayer = clip.Layer;
                clip.OldTimelineStartTime = clip.TimelineStartTime;
            }
            return;
        }

        Timeline.SelectedClips.Clear();
        Timeline.SelectedClips.AddRange(selectedClips);
    }
    private void TimelineControl_MouseMove(object? sender, MouseEventArgs e)
    {
        if (!MiddleDragging.IsDragging && !SelectedClipsDragging.IsDragging) return;

        var endPoint = new Point(e.X, e.Y);
        var endPosition = GetTimelinePositionControl(endPoint);
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
                clip.TimelineStartTime = clip.OldTimelineStartTime + diff.CurrentTime;
                Debug.WriteLine($"Dragging {diff.CurrentTime}x{diff.Layer} {clip.OldTimelineStartTime}+{diff.CurrentTime.ToString("F3")}={clip.TimelineStartTime}");
            }
            return;
        }
    }
    private void TimelineControl_MouseUp(object? sender, MouseEventArgs e)
    {
        MiddleDragging.IsDragging = false;
        SelectedClipsDragging.IsDragging = false;
    }

    private ITimelineClip[] GetSelectedClips(MouseEventArgs e)
    {
        var selectedClips = new List<ITimelineClip>();
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
                    if (clip2.Group.IsEqualTo(clip.Group) &&
                        !selectedClips.Contains(clip2))
                    {
                        selectedClips.Add(clip2);
                    }
                }
            }
        }
        return selectedClips.ToArray();
    }
    private TimelinePosition? GetTimelinePositionForm(Point formPoint)
    {
        var ucPoint = PointToClient(formPoint);
        return GetTimelinePositionControl(ucPoint);
    }
    private TimelinePosition? GetTimelinePositionControl(Point ucPoint)
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
    private RawRectangleF CalculateRectangle(ITimelineClip clip)
    {
        int middle = TimelineRectangle.Height / 2;
        int videoBlockHeight = (middle - HScrollBarControl.Height / 2) / Timeline.VisibleVideoLayers;
        int audioBlockHeight = (middle - MiddleOffset) / Timeline.VisibleAudioLayers;

        int x1 = Convert.ToInt32((clip.TimelineStartTime - Timeline.VisibleStart) / Timeline.VisibleWidth * TimelineRectangle.Width);
        int x2 = Convert.ToInt32((clip.TimelineEndTime - Timeline.VisibleStart) / Timeline.VisibleWidth * TimelineRectangle.Width);
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

    private void TimelineControl_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
        {
            // Delete

        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Brushes?.Dispose();
        RenderTarget?.Dispose();
        Factory?.Dispose();
        ImagingFactory?.Dispose();
    }
}