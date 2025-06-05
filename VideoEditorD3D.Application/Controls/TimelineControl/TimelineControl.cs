using SharpDX.Mathematics.Interop;
using System.Drawing;
using VideoEditorD3D.Application.Types;
using VideoEditorD3D.Direct3D.Controls;
using VideoEditorD3D.Direct3D.Drawing;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Entities;
using Point = System.Drawing.Point;
using Rectangle = SharpDX.Rectangle;

namespace VideoEditorD3D.Application.Controls.TimelineControl;

public partial class TimelineControl : BaseControl
{
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

    public TimelineControl() 
    {
        TimeMarkersBackLayer = GraphicsLayers.CreateNewLayer();
        TimeMarkersForeLayer = GraphicsLayers.CreateNewLayer();
        VideoBackClipsLayer = GraphicsLayers.CreateNewLayer();
        VideoForeClipsLayer = GraphicsLayers.CreateNewLayer();
        PlayerPositionLayer = GraphicsLayers.CreateNewLayer();

        HScrollBarControl = new HScrollBar();
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

        Load += TimelineControl_Load;
        Resize += TimelineControl_Resize;
        Draw += TimelineControl_Draw;
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
        var max = Timeline.TimelineClipAudios.Count > 0 ? Timeline.TimelineClipAudios.Max(a => a.TimelineEndTime) : Timeline.VisibleStart + VisibleWidth;
        max = Math.Max(max, Timeline.VisibleStart + VisibleWidth);
        HScrollBarControl.Minimum = 0;
        HScrollBarControl.Maximum = Convert.ToInt32(Math.Ceiling(max));
        HScrollBarControl.LargeChange = Convert.ToInt32(Math.Floor(VisibleWidth));
    }

    private string[] GetDragAndDropFilenames(DragEvent e)
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

    private TimelineClip[] GetSelectedClips(MouseEvent e)
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
                    if (clip2.TimelineClipGroup?.Value.Id == clip.TimelineClipGroup?.Value.Id &&
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

        int x1 = Convert.ToInt32((clip.TimelineStartTime - VisibleStart) / VisibleWidth * TimelineRectangle.Width);
        int x2 = Convert.ToInt32((clip.TimelineEndTime - VisibleStart) / VisibleWidth * TimelineRectangle.Width);
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
