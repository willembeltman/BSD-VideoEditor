namespace VideoEditor
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newProjectToolStripMenuItem = new ToolStripMenuItem();
            openProjectToolStripMenuItem = new ToolStripMenuItem();
            saveProjectToolStripMenuItem = new ToolStripMenuItem();
            saveProjectAsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exportToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            mediaToolStripMenuItem = new ToolStripMenuItem();
            timelineToolStripMenuItem = new ToolStripMenuItem();
            getFrameToolStripMenuItem = new ToolStripMenuItem();
            editorToolStripMenuItem = new ToolStripMenuItem();
            colorToolStripMenuItem = new ToolStripMenuItem();
            audioToolStripMenuItem = new ToolStripMenuItem();
            exportToolStripMenuItem1 = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            helpContentsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            timelineControl = new UI.TimelineControl();
            displayControl = new UI.DisplayControl();
            propertiesControl = new UI.PropertiesControl();
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.ImageScalingSize = new Size(24, 24);
            menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, mediaToolStripMenuItem, timelineToolStripMenuItem, editorToolStripMenuItem, colorToolStripMenuItem, audioToolStripMenuItem, exportToolStripMenuItem1, helpToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(1320, 33);
            menuStrip.TabIndex = 2;
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newProjectToolStripMenuItem, openProjectToolStripMenuItem, saveProjectToolStripMenuItem, saveProjectAsToolStripMenuItem, toolStripSeparator1, exportToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(54, 29);
            fileToolStripMenuItem.Text = "File";
            // 
            // newProjectToolStripMenuItem
            // 
            newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            newProjectToolStripMenuItem.Size = new Size(250, 34);
            newProjectToolStripMenuItem.Text = "New project";
            // 
            // openProjectToolStripMenuItem
            // 
            openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            openProjectToolStripMenuItem.Size = new Size(250, 34);
            openProjectToolStripMenuItem.Text = "Open project";
            // 
            // saveProjectToolStripMenuItem
            // 
            saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            saveProjectToolStripMenuItem.Size = new Size(250, 34);
            saveProjectToolStripMenuItem.Text = "Save project";
            // 
            // saveProjectAsToolStripMenuItem
            // 
            saveProjectAsToolStripMenuItem.Name = "saveProjectAsToolStripMenuItem";
            saveProjectAsToolStripMenuItem.Size = new Size(250, 34);
            saveProjectAsToolStripMenuItem.Text = "Save project as ...";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(247, 6);
            // 
            // exportToolStripMenuItem
            // 
            exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            exportToolStripMenuItem.Size = new Size(250, 34);
            exportToolStripMenuItem.Text = "Export video";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(247, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(250, 34);
            exitToolStripMenuItem.Text = "Exit";
            // 
            // mediaToolStripMenuItem
            // 
            mediaToolStripMenuItem.Name = "mediaToolStripMenuItem";
            mediaToolStripMenuItem.Size = new Size(62, 29);
            mediaToolStripMenuItem.Text = "Files";
            // 
            // timelineToolStripMenuItem
            // 
            timelineToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { getFrameToolStripMenuItem });
            timelineToolStripMenuItem.Name = "timelineToolStripMenuItem";
            timelineToolStripMenuItem.Size = new Size(101, 29);
            timelineToolStripMenuItem.Text = "Timelines";
            // 
            // getFrameToolStripMenuItem
            // 
            getFrameToolStripMenuItem.Name = "getFrameToolStripMenuItem";
            getFrameToolStripMenuItem.Size = new Size(192, 34);
            getFrameToolStripMenuItem.Text = "Get frame";
            getFrameToolStripMenuItem.Click += getFrameToolStripMenuItem_Click;
            // 
            // editorToolStripMenuItem
            // 
            editorToolStripMenuItem.Name = "editorToolStripMenuItem";
            editorToolStripMenuItem.Size = new Size(75, 29);
            editorToolStripMenuItem.Text = "Editor";
            // 
            // colorToolStripMenuItem
            // 
            colorToolStripMenuItem.Name = "colorToolStripMenuItem";
            colorToolStripMenuItem.Size = new Size(71, 29);
            colorToolStripMenuItem.Text = "Color";
            // 
            // audioToolStripMenuItem
            // 
            audioToolStripMenuItem.Name = "audioToolStripMenuItem";
            audioToolStripMenuItem.Size = new Size(76, 29);
            audioToolStripMenuItem.Text = "Audio";
            // 
            // exportToolStripMenuItem1
            // 
            exportToolStripMenuItem1.Name = "exportToolStripMenuItem1";
            exportToolStripMenuItem1.Size = new Size(79, 29);
            exportToolStripMenuItem1.Text = "Export";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { helpContentsToolStripMenuItem, toolStripSeparator4, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(65, 29);
            helpToolStripMenuItem.Text = "Help";
            // 
            // helpContentsToolStripMenuItem
            // 
            helpContentsToolStripMenuItem.Name = "helpContentsToolStripMenuItem";
            helpContentsToolStripMenuItem.Size = new Size(224, 34);
            helpContentsToolStripMenuItem.Text = "Help contents";
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(221, 6);
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(224, 34);
            aboutToolStripMenuItem.Text = "About";
            // 
            // timelineControl
            // 
            timelineControl.AllowDrop = true;
            timelineControl.BackColor = Color.Black;
            timelineControl.Location = new Point(13, 497);
            timelineControl.Name = "timelineControl";
            timelineControl.Size = new Size(1296, 442);
            timelineControl.TabIndex = 3;
            // 
            // displayControl
            // 
            displayControl.BackColor = SystemColors.ControlDarkDark;
            displayControl.Location = new Point(11, 47);
            displayControl.Margin = new Padding(1, 2, 1, 2);
            displayControl.Name = "displayControl";
            displayControl.Size = new Size(929, 425);
            displayControl.TabIndex = 4;
            // 
            // propertiesControl
            // 
            propertiesControl.BackColor = SystemColors.ControlDarkDark;
            propertiesControl.Location = new Point(960, 47);
            propertiesControl.Margin = new Padding(1, 2, 1, 2);
            propertiesControl.Name = "propertiesControl";
            propertiesControl.Size = new Size(349, 425);
            propertiesControl.TabIndex = 5;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1320, 957);
            Controls.Add(propertiesControl);
            Controls.Add(displayControl);
            Controls.Add(timelineControl);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
            Name = "MainForm";
            ShowIcon = false;
            Text = "My video editor";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            MouseDown += MainForm_MouseDown;
            MouseLeave += MainForm_MouseLeave;
            MouseMove += MainForm_MouseMove;
            MouseUp += MainForm_MouseUp;
            Resize += MainForm_Resize;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newProjectToolStripMenuItem;
        private ToolStripMenuItem openProjectToolStripMenuItem;
        private ToolStripMenuItem saveProjectToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exportToolStripMenuItem;
        private ToolStripMenuItem saveProjectAsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem mediaToolStripMenuItem;
        private ToolStripMenuItem timelineToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private UI.TimelineControl timelineControl;
        private UI.DisplayControl displayControl;
        private ToolStripMenuItem helpContentsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem editorToolStripMenuItem;
        private ToolStripMenuItem colorToolStripMenuItem;
        private ToolStripMenuItem audioToolStripMenuItem;
        private ToolStripMenuItem exportToolStripMenuItem1;
        private UI.PropertiesControl propertiesControl;
        private ToolStripMenuItem getFrameToolStripMenuItem;
    }
}
