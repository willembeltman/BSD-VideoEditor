using VideoEditorD3D.Direct3D;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Forms.Generic;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditorD3D.Application.Controls;

public class DisplayControl : BackControl
{
    public DisplayControl(ApplicationContext application, IApplicationForm applicationForm, Form? parentForm, Control? parentControl) : base(applicationForm, parentForm, parentControl)
    {
        Background = CanvasLayers.Create();
        Foreground = CanvasLayers.Create();
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
        if (_Frame != null) Foreground.DrawFrame(0, 0, Width, Height, _Frame);
        Foreground.EndDrawing();

        base.OnDraw();
    }
}
