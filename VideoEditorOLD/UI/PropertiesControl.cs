﻿namespace VideoEditor.UI;

public partial class PropertiesControl : UserControl
{
    private Engine Engine;

    public PropertiesControl(Engine engine)
    {
        Engine = engine;
        InitializeComponent();
    }

    private void PropertiesControl_Resize(object sender, EventArgs e)
    {
        vScrollBar1.Top = 0;
        vScrollBar1.Left = ClientRectangle.Width - vScrollBar1.Width;
        vScrollBar1.Height = ClientRectangle.Height;
    }

    private void UpdateTimer_Tick(object sender, EventArgs e)
    {
        lblFps.Text = $"Timeline: {Engine.TimelineControl.FpsCounter.GetFps():F1} fps\r\n" +
            $"Display: {Engine.DisplayControl.FpsCounter.GetFps():F1} fps\r\n" +
            $"Frame: {Engine.FrameTime:F1} ms";
    }
}
