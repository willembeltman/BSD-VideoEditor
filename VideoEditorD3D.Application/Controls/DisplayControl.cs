using VideoEditorD3D.Direct3D.Drawing;
using VideoEditorD3D.FFMpeg.Autogen;
using VideoEditorD3D.FFMpeg.CLI;
using VideoEditorD3D.FFMpeg.Interfaces;

namespace VideoEditorD3D.Application.Controls;

public class DisplayControl : BaseControl
{
    private readonly List<GraphicsLayer> Layers;

    public DisplayControl()
    {
        Layers = new List<GraphicsLayer>();

        Draw += DisplayControl_Draw;
        Update += DisplayControl_Update;
        Load += DisplayControl_Load;
    }

    private void DisplayControl_Load(object? sender, EventArgs e)
    {
        State.Timeline.CurrentTimeUpdated += Timeline_CurrentTimeUpdated;
    }

    private void Timeline_CurrentTimeUpdated(object? sender, double e)
    {
        Invalidate();
    }

    private void DisplayControl_Update(object? sender, EventArgs e)
    {
        var maxLayer = 0;
        if (Timeline.TimelineClipVideos.Any())
        {
            maxLayer = Timeline.TimelineClipVideos.Max(a => a.TimelineLayer) + 1;
        }
        if (Layers.Count > maxLayer)
        {
            var last = Layers[Layers.Count - 1];
            Layers.Remove(last);
            GraphicsLayers.Remove(last);
        }
        if (Layers.Count < maxLayer)
        {
            Layers.Add(GraphicsLayers.CreateNewLayer());
        }

        State.UpdateCurrentTime();
    }


    private void DisplayControl_Draw(object? sender, EventArgs e)
    {
        var frames = State.GetCurrentFrames();

        foreach (var layer in Layers) layer.StartDrawing();

        for (int i = 0; i < frames.Length; i++)
        {
            var layer = Layers[i];
            var frame = frames[i];
            if (frame is VideoFrame)
            {
                layer.DrawByteArray(0, 0, Width, Height, (frame as VideoFrame).Buffer, frame.Resolution.Width, frame.Resolution.Height);
            }
            else if (frame is GpuVideoFrame)
            {
                layer.DrawTexture2D(0, 0, Width, Height, (frame as GpuVideoFrame).Texture);
            }
        }

        foreach (var layer in Layers) layer.EndDrawing();

        //Foreground.StartDrawing();
        //if (_Frame != null) Foreground.DrawByteArrayImage(0, 0, Width, Height, _Frame.Buffer, _Frame.Resolution.Width, _Frame.Resolution.Height);
        //Foreground.EndDrawing();
    }
}
