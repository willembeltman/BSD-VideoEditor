using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX;
using System.Diagnostics;
using System.Drawing;
using VideoEditorD3D.Direct3D;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Forms.Generic;
using VideoEditorD3D.Direct3D.Interfaces;
using Rectangle = SharpDX.Rectangle;
using Point = System.Drawing.Point;

namespace VideoEditorD3D.Application.Controls;

public class TimelineControl : BackControl
{
    private readonly GraphicsLayer Background;
    private readonly GraphicsLayer Foreground;
    private readonly HScrollBar HScrollBarControl;

    public TimelineControl(ApplicationContext application, IApplicationForm applicationForm, Form? parentForm, Control? parentControl) : base(applicationForm, parentForm, parentControl)
    {
        Background = CanvasLayers.Create();
        Foreground = CanvasLayers.Create();

        HScrollBarControl = new HScrollBar(applicationForm, parentForm, this);
        //HScrollBarControl.Scroll += ScrollBarControl_Scroll;
        Controls.Add(HScrollBarControl);

        BackColor = new RawColor4(0.0f, 0.0f, 0.0f, 1);

        //DragDrop += TimelineControl_DragDrop;
        //DragEnter += TimelineControl_DragEnter;
        //DragOver += TimelineControl_DragOver;
        //DragLeave += TimelineControl_DragLeave;
        //KeyDown += TimelineControl_KeyDown;
        //MouseDown += TimelineControl_MouseDown;
        //MouseMove += TimelineControl_MouseMove;
        //MouseUp += TimelineControl_MouseUp;
        //MouseWheel += TimelineControl_MouseWheel;
    }

    public override void OnResize()
    {
        var marge = 0;
        HScrollBarControl.Height = 35;
        HScrollBarControl.Left = marge;
        HScrollBarControl.Width = Width - marge * 2;
        HScrollBarControl.Top = Height - HScrollBarControl.Height;
        base.OnResize();
    }

    public override void OnDraw()
    {
        Background.StartDrawing();
        Background.FillRectangle(0, 0, Width, Height, BackColor);
        Background.EndDrawing();

        //Foreground.StartDrawing();
        //DrawTimeMarkers(Foreground);
        //DrawVideoClips(Foreground);
        //DrawPlayerPosition(Foreground);
        //Foreground.EndDrawing();

        base.OnDraw();
    }

    //public override void OnLoad()
    //{
    //    base.OnLoad();
    //    SetupScrollbar();
    //}

    //DragAndDrop DragAndDrop { get; } = new();
    //Dragging SelectedClipsDragging { get; } = new();
    //Dragging MiddleDragging { get; } = new();
    //Scrolling Scrolling { get; } = new();

    //Timeline Timeline => Engine.Timeline;

    //Rectangle TimelineRectangle => new Rectangle(
    //    Left,
    //    Top,
    //    Width,
    //    Height - HScrollBarControl.Height);
    //int MiddleOffset => HScrollBarControl.Height / 2;

    //private void DrawTimeMarkers(GraphicsLayer RenderTarget)
    //{
    //    int middle = TimelineRectangle.Height / 2;
    //    int videoBlockHeight = (middle - MiddleOffset) / Timeline.VisibleVideoLayers;
    //    int audioBlockHeight = (middle - MiddleOffset) / Timeline.VisibleAudioLayers;

    //    // Vertical lines + layer numbers
    //    for (var i = 0; i < Timeline.VisibleVideoLayers; i++)
    //    {
    //        var y = TimelineRectangle.Top + middle - i * videoBlockHeight - MiddleOffset;
    //        var start = new RawVector2(0, y);
    //        var end = new RawVector2(TimelineRectangle.Width, y);
    //        RenderTarget!.DrawLine(start, end, Brushes!.HorizontalLines);

    //        var text = $"{i + Timeline.FirstVisibleVideoLayer}";
    //        using var layout = new TextLayout(dwriteFactory, text, textFormat, 100, 100); // 100x100 is max grootte van tekst

    //        var textY = y - videoBlockHeight / 2 - layout.Metrics.Height / 2;
    //        var textPosition = new RawVector2(2, (float)textY);
    //        RenderTarget!.DrawTextLayout(textPosition, layout, Brushes.Text);
    //    }
    //    for (var i = 0; i < Timeline.VisibleAudioLayers; i++)
    //    {
    //        var y = TimelineRectangle.Top + middle + i * audioBlockHeight + MiddleOffset;
    //        var start = new RawVector2(0, y);
    //        var end = new RawVector2(TimelineRectangle.Width, y);
    //        RenderTarget!.DrawLine(start, end, Brushes!.HorizontalLines);

    //        var text = $"{i + Timeline.FirstVisibleAudioLayer}";
    //        using var layout = new TextLayout(dwriteFactory, text, textFormat, 100, 100);

    //        var textY = y + audioBlockHeight / 2 - layout.Metrics.Height / 2;
    //        var textPosition = new RawVector2(2, (float)textY);
    //        RenderTarget.DrawTextLayout(textPosition, layout, Brushes.Text);
    //    }

    //    // Tijd stap bepalen
    //    var timeIncrease = 0.00001D;
    //    var decimals = 5;
    //    while (Width / Timeline.VisibleWidth * timeIncrease < 50)
    //    {
    //        timeIncrease *= 10;
    //        decimals--;
    //    }
    //    if (decimals < 0) decimals = 0;

    //    // Horizontal lines + time 
    //    for (var sec = 0D; sec < double.MaxValue; sec += timeIncrease)
    //    {
    //        var x = Convert.ToInt32((sec - Timeline.VisibleStart) / Timeline.VisibleWidth * Width);
    //        if (x >= Width) break;
    //        var start = new RawVector2(x, 0);
    //        var end = new RawVector2(x, TimelineRectangle.Height);
    //        RenderTarget!.DrawLine(start, end, Brushes!.VerticalLines);

    //        var text = $"{sec.ToString("F" + decimals)}s";
    //        using var layout = new TextLayout(dwriteFactory, text, textFormat, 100, 100);
    //        var textY = TimelineRectangle.Top + middle - layout.Metrics.Height / 2;
    //        var textPosition = new RawVector2(x + 2, (float)textY);
    //        RenderTarget!.DrawTextLayout(textPosition, layout, Brushes!.Text);
    //    }
    //}
    //private void DrawVideoClips(GraphicsLayer RenderTarget)
    //{
    //    var clips = Timeline.AllClips.Concat(DragAndDrop.AllClips);
    //    foreach (var clip in clips)
    //    {
    //        if (clip.IsVideoClip && Timeline.FirstVisibleVideoLayer <= clip.Layer ||
    //            clip.IsAudioClip && Timeline.FirstVisibleAudioLayer <= clip.Layer)
    //        {
    //            var rect = CalculateRectangle(clip);

    //            if (rect.Left > TimelineRectangle.Width || rect.Right < 0) continue; // Clip buiten zichtbare range
    //            if (rect.Top > TimelineRectangle.Height || rect.Bottom < 0) continue; // Clip buiten zichtbare range

    //            var selected = Timeline.SelectedClips.Contains(clip);
    //            var fillBrush = selected ? Brushes!.SelectedClip : clip.IsVideoClip ? Brushes!.VideoClip : Brushes!.AudioClip;
    //            var borderPen = Brushes.ClipBorder;

    //            RenderTarget!.FillRectangle(rect, fillBrush);
    //            RenderTarget!.DrawRectangle(rect, borderPen);
    //        }
    //    }
    //}
    //private void DrawPlayerPosition(GraphicsLayer RenderTarget)
    //{
    //    using var pen = new Pen(Color.Red, 2);

    //    // Bereken de x-positie van de player marker
    //    int x = Convert.ToInt32((Timeline.CurrentTime - Timeline.VisibleStart) / Timeline.VisibleWidth * Width);

    //    // Zorg ervoor dat de lijn binnen de zichtbare regio valt
    //    if (x >= 0 && x <= Width)
    //    {
    //        RenderTarget!.DrawLine(new RawVector2(x, 0), new RawVector2(x, TimelineRectangle.Height), Brushes!.PositionLine);
    //    }
    //}

    //private void TimelineControl_MouseWheel(object? sender, System.Windows.Forms.MouseEventArgs e)
    //{
    //    var scrollDelta = Scrolling.GetScrollDelta(e);

    //    var applicationPoint = new Point(e.X, e.Y);
    //    var timelinePosition = GetTimelinePositionControl(applicationPoint);
    //    if (timelinePosition == null) return;

    //    if ((ModifierKeys & Keys.Control) == Keys.Control)
    //    {
    //        ZoomX(scrollDelta, timelinePosition.Value);
    //    }
    //    else if ((ModifierKeys & Keys.Shift) == Keys.Shift)
    //    {
    //        ZoomY(scrollDelta, timelinePosition.Value);
    //    }
    //    else
    //    {
    //        ScrollY(scrollDelta, timelinePosition.Value);
    //    }
    //    SetupScrollbar();
    //}
    //private void ScrollY(int delta, TimelinePosition timelinePosition)
    //{
    //    if (timelinePosition.TimelinePart == TimelinePart.Video)
    //    {
    //        // Video
    //        Timeline.FirstVisibleVideoLayer += delta;
    //        if (Timeline.FirstVisibleVideoLayer < 0) Timeline.FirstVisibleVideoLayer = 0;
    //    }
    //    if (timelinePosition.TimelinePart == TimelinePart.Video)
    //    {
    //        // Audio
    //        Timeline.FirstVisibleAudioLayer += delta;
    //        if (Timeline.FirstVisibleAudioLayer < 0) Timeline.FirstVisibleAudioLayer = 0;
    //    }
    //}
    //private void ZoomY(int delta, TimelinePosition timelinePosition)
    //{
    //    if (timelinePosition.TimelinePart == TimelinePart.Video)
    //    {
    //        // Video
    //        Timeline.VisibleVideoLayers -= delta;
    //        if (Timeline.VisibleVideoLayers < 1) Timeline.VisibleVideoLayers = 1;
    //    }
    //    if (timelinePosition.TimelinePart == TimelinePart.Audio)
    //    {
    //        // Audio
    //        Timeline.VisibleAudioLayers -= delta;
    //        if (Timeline.VisibleAudioLayers < 1) Timeline.VisibleAudioLayers = 1;
    //    }
    //}
    //private void ZoomX(int delta, TimelinePosition timelinePosition)
    //{
    //    if (delta > 0)
    //    {
    //        for (int i = 0; i < delta; i++)
    //        {
    //            Timeline.VisibleWidth -= Timeline.VisibleWidth / 10;
    //        }
    //    }
    //    if (delta < 0)
    //    {
    //        for (int i = 0; i < delta * -1; i++)
    //        {
    //            Timeline.VisibleWidth += Timeline.VisibleWidth / 10;
    //        }
    //    }
    //}
    //private void SetupScrollbar()
    //{
    //    var max = Timeline.AudioClips.Any() ? Timeline.AudioClips.Max(a => a.TimelineEndTime) : Timeline.VisibleStart + Timeline.VisibleWidth;
    //    max = Math.Max(max, Timeline.VisibleStart + Timeline.VisibleWidth);
    //    HScrollBarControl.Minimum = 0;
    //    HScrollBarControl.Maximum = Convert.ToInt32(Math.Ceiling(max));
    //    HScrollBarControl.SmallChange = 1;
    //    HScrollBarControl.LargeChange = Convert.ToInt32(Math.Floor(Timeline.VisibleWidth));
    //}

    //private void ScrollBarControl_Scroll(object? sender, System.Windows.Forms.ScrollEventArgs e)
    //{
    //    Timeline.VisibleStart = e.NewValue;
    //}

    //private void TimelineControl_DragEnter(object? sender, System.Windows.Forms.DragEventArgs e)
    //{
    //    var fullNames = GetDragAndDropFilenames(e);
    //    if (fullNames.Length == 0)
    //    {
    //        DragAndDrop.Clear();
    //        return;
    //    }

    //    var formPoint = new Point(e.X, e.Y);
    //    var timelinePosition = GetTimelinePositionForm(formPoint);
    //    if (timelinePosition == null)
    //    {
    //        DragAndDrop.Clear();
    //        return;
    //    }

    //    e.Effect = DragDropEffects.Copy;

    //    var currentTime = timelinePosition.Value.CurrentTime;
    //    var layerIndex = timelinePosition.Value.Layer;
    //    foreach (var fullName in fullNames)
    //    {
    //        var file = File.Open(fullName);
    //        if (file == null) continue;
    //        if (file.Duration == null) continue;

    //        var group = new TimelineClipGroup();
    //        var start = currentTime;
    //        currentTime += file.Duration.Value;
    //        var layer = layerIndex;
    //        foreach (var videoStream in file.VideoStreams.OrderBy(a => a.Index))
    //        {
    //            var clip = new TimelineClipVideo(Timeline, videoStream, group)
    //            {
    //                Layer = layer,
    //                TimelineStartTime = start,
    //                TimelineLengthTime = file.Duration.Value,
    //                ClipStartTime = 0,
    //                ClipLengthTime = file.Duration.Value
    //            };
    //            DragAndDrop.VideoClips.Add(clip);
    //            layer++;
    //        }

    //        layer = 0;
    //        foreach (var audioStream in file.AudioStreams.OrderBy(a => a.Index))
    //        {
    //            var clip = new TimelineClipAudio(Timeline, audioStream, group)
    //            {
    //                Layer = layer,
    //                TimelineStartTime = start,
    //                TimelineEndTime = currentTime,
    //                ClipStartTime = 0,
    //                ClipEndTime = file.Duration.Value
    //            };
    //            DragAndDrop.AudioClips.Add(clip);
    //            layer++;
    //        }

    //        DragAndDrop.Files.Add(file);
    //    }
    //}
    //private void TimelineControl_DragOver(object? sender, System.Windows.Forms.DragEventArgs e)
    //{
    //    var fullNames = GetDragAndDropFilenames(e);
    //    if (fullNames.Length == 0)
    //    {
    //        DragAndDrop.Clear();
    //        return;
    //    }

    //    var formPoint = new Point(e.X, e.Y);
    //    var timelinePosition = GetTimelinePositionForm(formPoint);
    //    if (timelinePosition == null)
    //    {
    //        DragAndDrop.Clear();
    //        return;
    //    }

    //    var currentTime = timelinePosition.Value.CurrentTime;
    //    var layerIndex = timelinePosition.Value.Layer;
    //    foreach (var fullName in fullNames)
    //    {
    //        var file = DragAndDrop.Files.FirstOrDefault(a => a.FullName == fullName);
    //        if (file == null) continue;
    //        if (file.Duration == null) continue;

    //        var start = currentTime;
    //        currentTime += file.Duration.Value;
    //        var layer = layerIndex;
    //        foreach (var videoStream in file.VideoStreams.OrderBy(a => a.Index))
    //        {
    //            var cachedVideoStream = DragAndDrop.VideoClips
    //                .FirstOrDefault(a => a.StreamInfo.EqualTo(videoStream));

    //            if (cachedVideoStream != null)
    //            {
    //                cachedVideoStream.Layer = layer;
    //                cachedVideoStream.TimelineStartTime = start;
    //                cachedVideoStream.TimelineEndTime = currentTime;
    //                layer++;
    //            }
    //        }

    //        layer = layerIndex;
    //        foreach (var audioStream in file.AudioStreams.OrderBy(a => a.Index))
    //        {
    //            var cachedAudioStream = DragAndDrop.AudioClips
    //                .FirstOrDefault(a => a.StreamInfo.EqualTo(audioStream));

    //            if (cachedAudioStream != null)
    //            {
    //                cachedAudioStream.Layer = layer;
    //                cachedAudioStream.TimelineStartTime = start;
    //                cachedAudioStream.TimelineEndTime = currentTime;
    //                layer++;
    //            }
    //        }
    //    }

    //    SetupScrollbar();
    //}
    //private void TimelineControl_DragDrop(object? sender, System.Windows.Forms.DragEventArgs e)
    //{
    //    var fullNames = GetDragAndDropFilenames(e);
    //    if (fullNames.Length == 0) return;

    //    Timeline.VideoClips.AddRange(DragAndDrop.VideoClips);
    //    Timeline.AudioClips.AddRange(DragAndDrop.AudioClips);

    //    TimelineControl_DragLeave(sender, e);
    //}
    //private void TimelineControl_DragLeave(object? sender, EventArgs e)
    //{
    //    DragAndDrop.Clear();
    //    SetupScrollbar();
    //}
    //private string[] GetDragAndDropFilenames(DragEventArgs e)
    //{
    //    if (e == null || e.Data == null) return [];
    //    if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return [];
    //    var filesobj = e.Data.GetData(DataFormats.FileDrop);
    //    if (filesobj == null) return [];
    //    var files = (string[])filesobj;
    //    if (files == null) return [];
    //    return CheckFileType.Filter(files)
    //        .OrderBy(a => a)
    //        .ToArray();
    //}

    //private void TimelineControl_MouseDown(object? sender, System.Windows.Forms.MouseEventArgs e)
    //{
    //    /// Er zijn 3 verschillende mogelijkheden:
    //    /// 1. Niets is geselecteerd en je selecteerd een clip/groep = Selecteren
    //    /// 2. Clip/groep is geselecteerd en daar klik je op = Dragging
    //    /// 3. Je klikt op het midden


    //    var startpoint = new Point(e.X, e.Y);
    //    var startposition = GetTimelinePositionControl(startpoint);
    //    if (startposition == null) return;

    //    Debug.WriteLine($"{startposition.Value.Layer} {startposition.Value.CurrentTime} {startposition.Value.TimelinePart}");

    //    if (startposition.Value.TimelinePart == TimelinePart.Middle)
    //    {
    //        MiddleDragging.Set(startpoint, startposition);
    //        Timeline.CurrentTime = startposition.Value.CurrentTime;
    //        return;
    //    }

    //    var selectedClips = GetSelectedClips(e);
    //    if (selectedClips.Any(a => Timeline.SelectedClips.Any(b => b.Equals(a))))
    //    {
    //        SelectedClipsDragging.Set(startpoint, startposition);
    //        foreach (var clip in selectedClips)
    //        {
    //            clip.OldLayer = clip.Layer;
    //            clip.OldTimelineStartTime = clip.TimelineStartTime;
    //        }
    //        return;
    //    }

    //    Timeline.SelectedClips.Clear();
    //    Timeline.SelectedClips.AddRange(selectedClips);
    //}
    //private void TimelineControl_MouseMove(object? sender, System.Windows.Forms.MouseEventArgs e)
    //{
    //    if (!MiddleDragging.IsDragging && !SelectedClipsDragging.IsDragging) return;

    //    var endPoint = new Point(e.X, e.Y);
    //    var endPosition = GetTimelinePositionControl(endPoint);
    //    if (endPosition == null) return;

    //    Debug.WriteLine($"{endPosition.Value.Layer} {endPosition.Value.CurrentTime} {endPosition.Value.TimelinePart}");

    //    if (MiddleDragging.IsDragging && endPosition.Value.TimelinePart == TimelinePart.Middle)
    //    {
    //        Timeline.CurrentTime = endPosition.Value.CurrentTime;
    //        return;
    //    }

    //    if (SelectedClipsDragging.IsDragging && endPosition.Value != SelectedClipsDragging.StartPosition)
    //    {
    //        var diff = endPosition.Value - SelectedClipsDragging.StartPosition;
    //        foreach (var clip in Timeline.SelectedClips)
    //        {
    //            clip.Layer = clip.OldLayer + diff.Layer;
    //            if (clip.Layer < 0) clip.Layer = 0;
    //            clip.TimelineStartTime = clip.OldTimelineStartTime + diff.CurrentTime;
    //            Debug.WriteLine($"Dragging {diff.CurrentTime}x{diff.Layer} {clip.OldTimelineStartTime}+{diff.CurrentTime.ToString("F3")}={clip.TimelineStartTime}");
    //        }
    //        return;
    //    }
    //}
    //private void TimelineControl_MouseUp(object? sender, System.Windows.Forms.MouseEventArgs e)
    //{
    //    MiddleDragging.IsDragging = false;
    //    SelectedClipsDragging.IsDragging = false;
    //}

    //private ITimelineClip[] GetSelectedClips(MouseEventArgs e)
    //{
    //    var selectedClips = new List<ITimelineClip>();
    //    foreach (var clip in Timeline.AllClips)
    //    {
    //        var rect = CalculateRectangle(clip);
    //        if (rect.Left < e.X && e.X < rect.Right &&
    //            rect.Top < e.Y && e.Y < rect.Bottom &&
    //            !selectedClips.Contains(clip))
    //        {
    //            selectedClips.Add(clip);

    //            foreach (var clip2 in Timeline.AllClips)
    //            {
    //                if (clip2.Group.IsEqualTo(clip.Group) &&
    //                    !selectedClips.Contains(clip2))
    //                {
    //                    selectedClips.Add(clip2);
    //                }
    //            }
    //        }
    //    }
    //    return selectedClips.ToArray();
    //}
    //private TimelinePosition? GetTimelinePositionForm(Point formPoint)
    //{
    //    var ucPoint = PointToClient(formPoint);
    //    return GetTimelinePositionControl(ucPoint);
    //}
    //private TimelinePosition? GetTimelinePositionControl(Point ucPoint)
    //{
    //    var currentTime = Timeline.VisibleStart + Timeline.VisibleWidth * ucPoint.X / TimelineRectangle.Width;

    //    var timelineHeight = TimelineRectangle.Height;
    //    var videoHeight = timelineHeight / 2 - MiddleOffset;
    //    var middleHeight = MiddleOffset * 2;
    //    var audioHeight = timelineHeight - videoHeight - middleHeight;
    //    if (ucPoint.Y < videoHeight)
    //    {
    //        var videoLayerHeight = videoHeight / Timeline.VisibleVideoLayers;
    //        var relativeLayerIndex = Timeline.VisibleVideoLayers - ucPoint.Y / videoLayerHeight - 1;
    //        var layerIndex = Timeline.FirstVisibleVideoLayer + relativeLayerIndex;
    //        if (layerIndex < 0) layerIndex = 0;
    //        return new TimelinePosition(currentTime, layerIndex, TimelinePart.Video);
    //    }
    //    else if (ucPoint.Y >= videoHeight && ucPoint.Y < videoHeight + middleHeight)
    //    {
    //        return new TimelinePosition(currentTime, 0, TimelinePart.Middle);
    //    }
    //    else if (ucPoint.Y >= videoHeight + middleHeight && ucPoint.Y < timelineHeight)
    //    {
    //        var audioLayerHeight = audioHeight / Timeline.VisibleAudioLayers;
    //        var relativeLayerIndex = (ucPoint.Y - videoHeight) / audioLayerHeight - 1;
    //        var layerIndex = Timeline.FirstVisibleAudioLayer + relativeLayerIndex;
    //        if (layerIndex < 0) layerIndex = 0;
    //        return new TimelinePosition(currentTime, layerIndex, TimelinePart.Audio);
    //    }
    //    return null;
    //}
    //private RawRectangleF CalculateRectangle(ITimelineClip clip)
    //{
    //    int middle = TimelineRectangle.Height / 2;
    //    int videoBlockHeight = (middle - HScrollBarControl.Height / 2) / Timeline.VisibleVideoLayers;
    //    int audioBlockHeight = (middle - MiddleOffset) / Timeline.VisibleAudioLayers;

    //    int x1 = Convert.ToInt32((clip.TimelineStartTime - Timeline.VisibleStart) / Timeline.VisibleWidth * TimelineRectangle.Width);
    //    int x2 = Convert.ToInt32((clip.TimelineEndTime - Timeline.VisibleStart) / Timeline.VisibleWidth * TimelineRectangle.Width);
    //    int width = x2 - x1;

    //    if (clip.IsVideoClip)
    //    {
    //        var layer = clip.Layer - Timeline.FirstVisibleVideoLayer;
    //        var y = middle - MiddleOffset - videoBlockHeight - layer * videoBlockHeight;
    //        var rect = new RawRectangleF(x1, y + Constants.Margin / 2, x1 + width, y + Constants.Margin / 2 + videoBlockHeight - Constants.Margin);
    //        return rect;
    //    }
    //    else
    //    {
    //        var layer = clip.Layer - Timeline.FirstVisibleAudioLayer;
    //        var y = middle + MiddleOffset + (clip.Layer - Timeline.FirstVisibleAudioLayer) * audioBlockHeight;
    //        var rect = new RawRectangleF(x1, y + Constants.Margin / 2, x1 + width, y + Constants.Margin / 2 + audioBlockHeight - Constants.Margin);
    //        return rect;
    //    }
    //}

    //private void TimelineControl_KeyDown(object? sender, System.Windows.Forms.KeyEventArgs e)
    //{
    //    if (e.KeyCode == System.Windows.Forms.Keys.Delete || e.KeyCode == System.Windows.Forms.Keys.Back)
    //    {
    //        // Delete

    //    }
    //}
}
