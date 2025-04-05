namespace VideoEditor.UI
{
    partial class PropertiesControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            vScrollBar1 = new VScrollBar();
            lblFps = new Label();
            UpdateTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // vScrollBar1
            // 
            vScrollBar1.Location = new Point(361, 0);
            vScrollBar1.Name = "vScrollBar1";
            vScrollBar1.Size = new Size(32, 455);
            vScrollBar1.TabIndex = 0;
            // 
            // lblFps
            // 
            lblFps.AutoSize = true;
            lblFps.BackColor = Color.Beige;
            lblFps.Location = new Point(8, 8);
            lblFps.Margin = new Padding(4, 0, 4, 0);
            lblFps.Name = "lblFps";
            lblFps.Size = new Size(59, 25);
            lblFps.TabIndex = 1;
            lblFps.Text = "label1";
            // 
            // UpdateTimer
            // 
            UpdateTimer.Enabled = true;
            UpdateTimer.Tick += UpdateTimer_Tick;
            // 
            // PropertiesControl
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            Controls.Add(lblFps);
            Controls.Add(vScrollBar1);
            Name = "PropertiesControl";
            Size = new Size(401, 455);
            Resize += PropertiesControl_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private VScrollBar vScrollBar1;
        private Label lblFps;
        private System.Windows.Forms.Timer UpdateTimer;
    }
}
