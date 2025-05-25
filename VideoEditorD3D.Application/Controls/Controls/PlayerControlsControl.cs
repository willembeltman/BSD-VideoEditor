using VideoEditorD3D.Direct3D.Controls;
using VideoEditorD3D.Direct3D.Controls.Templates;

namespace VideoEditorD3D.Application.Controls.Controls
{
    public class PlayerControlsControl : BackControl
    {
        private readonly Button btnStop;
        private readonly Button btnForwardPlayback;
        private readonly Button btnBackwardPlayback;

        public PlayerControlsControl()
        {
            btnStop = new Button();
            btnStop.Click += BtnStop_Click;
            btnStop.Text = "||";
            Controls.Add(btnStop);

            btnForwardPlayback = new Button();
            btnForwardPlayback.Click += BtnForwardPlayback_Click;
            btnForwardPlayback.Text = "|>";
            Controls.Add(btnForwardPlayback);

            btnBackwardPlayback = new Button();
            btnBackwardPlayback.Click += BtnBackwardPlayback_Click;
            btnBackwardPlayback.Text = "<|";
            Controls.Add(btnBackwardPlayback);

            Load += PlayerControlsControl_Load;
            Resize += PlayerControlsControl_Resize;
        }

        private void PlayerControlsControl_Load(object? sender, EventArgs e)
        {
            btnBackwardPlayback.Left = 1;
            btnBackwardPlayback.Top = 1;
            btnBackwardPlayback.Width = Height - 2;
            btnBackwardPlayback.Height = Height - 2;

            btnStop.Left = btnBackwardPlayback.Right + 5;
            btnStop.Top = 1;
            btnStop.Width = Height - 2;
            btnStop.Height = Height - 2;

            btnForwardPlayback.Left = btnStop.Right + 5;
            btnForwardPlayback.Top = 1;
            btnForwardPlayback.Width = Height - 2;
            btnForwardPlayback.Height = Height - 2;
        }

        private void BtnBackwardPlayback_Click(object? sender, Direct3D.Forms.MouseEvent e)
        {
        }

        private void BtnForwardPlayback_Click(object? sender, Direct3D.Forms.MouseEvent e)
        {
        }

        private void BtnStop_Click(object? sender, Direct3D.Forms.MouseEvent e)
        {
        }

        private void PlayerControlsControl_Resize(object? sender, EventArgs e)
        {
        }
    }
}
