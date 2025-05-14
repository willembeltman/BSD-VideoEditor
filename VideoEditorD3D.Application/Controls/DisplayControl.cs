using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Drawing;
using VideoEditorD3D.FFMpeg.Types;
using VideoEditorD3D.Direct3D.Controls.Generic;

namespace VideoEditorD3D.Application.Controls;

public class DisplayControl : BackControl
{
    public DisplayControl(ApplicationContext applicationContext, IApplicationForm applicationForm) : base(applicationForm)
    {
        Background = CanvasLayers.CreateNewLayer();
        Foreground = CanvasLayers.CreateNewLayer();
    }

    private readonly GraphicsLayer Background;

    private readonly GraphicsLayer Foreground;

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

    public override void OnDraw()
    {
        Background.StartDrawing();
        Background.FillRectangle(0, 0, Width, Height, BackColor);
        Background.EndDrawing();

        Foreground.StartDrawing();
        if (_Frame != null) Foreground.DrawByteArrayImage(0, 0, Width, Height, _Frame.Buffer, _Frame.Resolution.Width, _Frame.Resolution.Height);
        Foreground.EndDrawing();

        base.OnDraw();
    }
}
