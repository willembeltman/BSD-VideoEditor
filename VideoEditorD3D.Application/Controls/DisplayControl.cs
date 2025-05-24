using VideoEditorD3D.Direct3D.Controls.Templates;
using VideoEditorD3D.Direct3D.Drawing;
using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditorD3D.Application.Controls;

public class DisplayControl : BackControl
{
    private readonly GraphicsLayer Foreground;

    public DisplayControl() 
    {
        Foreground = GraphicsLayers.CreateNewLayer();
        Draw += DisplayControl_Draw;
    }

    private Frame? _Frame;
    public Frame? Frame
    {
        get => _Frame;
        set
        {
            if (_Frame != value)
            {
                _Frame = value;
                Invalidate();
            }
        }
    }

    private void DisplayControl_Draw(object? sender, EventArgs e)
    {
        Foreground.StartDrawing();
        if (_Frame != null) Foreground.DrawByteArrayImage(0, 0, Width, Height, _Frame.Buffer, _Frame.Resolution.Width, _Frame.Resolution.Height);
        Foreground.EndDrawing();
    }
}
