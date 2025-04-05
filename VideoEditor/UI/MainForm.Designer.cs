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
            components = new System.ComponentModel.Container();
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
            editorToolStripMenuItem = new ToolStripMenuItem();
            colorToolStripMenuItem = new ToolStripMenuItem();
            audioToolStripMenuItem = new ToolStripMenuItem();
            exportToolStripMenuItem1 = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            helpContentsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            timer = new System.Windows.Forms.Timer(components);
            playPauseToolStripMenuItem = new ToolStripMenuItem();
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
            timelineToolStripMenuItem.Name = "timelineToolStripMenuItem";
            timelineToolStripMenuItem.Size = new Size(101, 29);
            timelineToolStripMenuItem.Text = "Timelines";
            // 
            // editorToolStripMenuItem
            // 
            editorToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { playPauseToolStripMenuItem });
            editorToolStripMenuItem.Name = "editorToolStripMenuItem";
            editorToolStripMenuItem.Size = new Size(93, 29);
            editorToolStripMenuItem.Text = "Timeline";
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
            // timer
            // 
            timer.Enabled = true;
            // 
            // playPauseToolStripMenuItem
            // 
            playPauseToolStripMenuItem.Name = "playPauseToolStripMenuItem";
            playPauseToolStripMenuItem.Size = new Size(270, 34);
            playPauseToolStripMenuItem.Text = "Play / Pause";
            playPauseToolStripMenuItem.Click += playPauseToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1320, 882);
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
        private ToolStripMenuItem helpContentsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem editorToolStripMenuItem;
        private ToolStripMenuItem colorToolStripMenuItem;
        private ToolStripMenuItem audioToolStripMenuItem;
        private ToolStripMenuItem exportToolStripMenuItem1;
        private System.Windows.Forms.Timer timer;
        private ToolStripMenuItem playPauseToolStripMenuItem;
    }
}
