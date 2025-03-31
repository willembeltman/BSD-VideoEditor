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
            vScrollBar1 = new VScrollBar();
            lblFps = new Label();
            SuspendLayout();
            // 
            // vScrollBar1
            // 
            vScrollBar1.Location = new Point(253, 0);
            vScrollBar1.Name = "vScrollBar1";
            vScrollBar1.Size = new Size(32, 273);
            vScrollBar1.TabIndex = 0;
            // 
            // lblFps
            // 
            lblFps.AutoSize = true;
            lblFps.Location = new Point(212, 0);
            lblFps.Name = "lblFps";
            lblFps.Size = new Size(38, 15);
            lblFps.TabIndex = 1;
            lblFps.Text = "label1";
            // 
            // PropertiesControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            Controls.Add(lblFps);
            Controls.Add(vScrollBar1);
            Margin = new Padding(2);
            Name = "PropertiesControl";
            Size = new Size(281, 273);
            Resize += PropertiesControl_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private VScrollBar vScrollBar1;
        private Label lblFps;
    }
}
