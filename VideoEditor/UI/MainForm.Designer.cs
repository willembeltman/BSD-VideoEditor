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
            getFrameToolStripMenuItem = new ToolStripMenuItem();
            test2ToolStripMenuItem = new ToolStripMenuItem();
            editorToolStripMenuItem = new ToolStripMenuItem();
            colorToolStripMenuItem = new ToolStripMenuItem();
            audioToolStripMenuItem = new ToolStripMenuItem();
            exportToolStripMenuItem1 = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            helpContentsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            timelineControl = new UI.TimelineControl(Engine);
            displayControl = new UI.DisplayControl(Engine);
            propertiesControl = new UI.PropertiesControl(Engine);
            timer = new System.Windows.Forms.Timer(components);
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.ImageScalingSize = new Size(24, 24);
            menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, mediaToolStripMenuItem, timelineToolStripMenuItem, editorToolStripMenuItem, colorToolStripMenuItem, audioToolStripMenuItem, exportToolStripMenuItem1, helpToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Padding = new Padding(4, 1, 0, 1);
            menuStrip.Size = new Size(924, 24);
            menuStrip.TabIndex = 2;
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newProjectToolStripMenuItem, openProjectToolStripMenuItem, saveProjectToolStripMenuItem, saveProjectAsToolStripMenuItem, toolStripSeparator1, exportToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 22);
            fileToolStripMenuItem.Text = "File";
            // 
            // newProjectToolStripMenuItem
            // 
            newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            newProjectToolStripMenuItem.Size = new Size(164, 22);
            newProjectToolStripMenuItem.Text = "New project";
            // 
            // openProjectToolStripMenuItem
            // 
            openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            openProjectToolStripMenuItem.Size = new Size(164, 22);
            openProjectToolStripMenuItem.Text = "Open project";
            // 
            // saveProjectToolStripMenuItem
            // 
            saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            saveProjectToolStripMenuItem.Size = new Size(164, 22);
            saveProjectToolStripMenuItem.Text = "Save project";
            // 
            // saveProjectAsToolStripMenuItem
            // 
            saveProjectAsToolStripMenuItem.Name = "saveProjectAsToolStripMenuItem";
            saveProjectAsToolStripMenuItem.Size = new Size(164, 22);
            saveProjectAsToolStripMenuItem.Text = "Save project as ...";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(161, 6);
            // 
            // exportToolStripMenuItem
            // 
            exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            exportToolStripMenuItem.Size = new Size(164, 22);
            exportToolStripMenuItem.Text = "Export video";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(161, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(164, 22);
            exitToolStripMenuItem.Text = "Exit";
            // 
            // mediaToolStripMenuItem
            // 
            mediaToolStripMenuItem.Name = "mediaToolStripMenuItem";
            mediaToolStripMenuItem.Size = new Size(42, 22);
            mediaToolStripMenuItem.Text = "Files";
            // 
            // timelineToolStripMenuItem
            // 
            timelineToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { getFrameToolStripMenuItem, test2ToolStripMenuItem });
            timelineToolStripMenuItem.Name = "timelineToolStripMenuItem";
            timelineToolStripMenuItem.Size = new Size(69, 22);
            timelineToolStripMenuItem.Text = "Timelines";
            // 
            // getFrameToolStripMenuItem
            // 
            getFrameToolStripMenuItem.Name = "getFrameToolStripMenuItem";
            getFrameToolStripMenuItem.Size = new Size(126, 22);
            getFrameToolStripMenuItem.Text = "Get frame";
            getFrameToolStripMenuItem.Click += getFrameToolStripMenuItem_Click;
            // 
            // test2ToolStripMenuItem
            // 
            test2ToolStripMenuItem.Name = "test2ToolStripMenuItem";
            test2ToolStripMenuItem.Size = new Size(126, 22);
            test2ToolStripMenuItem.Text = "Test 2";
            // 
            // editorToolStripMenuItem
            // 
            editorToolStripMenuItem.Name = "editorToolStripMenuItem";
            editorToolStripMenuItem.Size = new Size(50, 22);
            editorToolStripMenuItem.Text = "Editor";
            // 
            // colorToolStripMenuItem
            // 
            colorToolStripMenuItem.Name = "colorToolStripMenuItem";
            colorToolStripMenuItem.Size = new Size(48, 22);
            colorToolStripMenuItem.Text = "Color";
            // 
            // audioToolStripMenuItem
            // 
            audioToolStripMenuItem.Name = "audioToolStripMenuItem";
            audioToolStripMenuItem.Size = new Size(51, 22);
            audioToolStripMenuItem.Text = "Audio";
            // 
            // exportToolStripMenuItem1
            // 
            exportToolStripMenuItem1.Name = "exportToolStripMenuItem1";
            exportToolStripMenuItem1.Size = new Size(53, 22);
            exportToolStripMenuItem1.Text = "Export";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { helpContentsToolStripMenuItem, toolStripSeparator4, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 22);
            helpToolStripMenuItem.Text = "Help";
            // 
            // helpContentsToolStripMenuItem
            // 
            helpContentsToolStripMenuItem.Name = "helpContentsToolStripMenuItem";
            helpContentsToolStripMenuItem.Size = new Size(148, 22);
            helpContentsToolStripMenuItem.Text = "Help contents";
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(145, 6);
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(148, 22);
            aboutToolStripMenuItem.Text = "About";
            // 
            // timelineControl
            // 
            timelineControl.AllowDrop = true;
            timelineControl.BackColor = Color.Black;
            timelineControl.Location = new Point(9, 298);
            timelineControl.Margin = new Padding(2);
            timelineControl.Name = "timelineControl";
            timelineControl.Size = new Size(907, 220);
            timelineControl.TabIndex = 3;
            // 
            // displayControl
            // 
            displayControl.BackColor = SystemColors.ControlDarkDark;
            displayControl.Location = new Point(8, 28);
            displayControl.Margin = new Padding(1);
            displayControl.Name = "displayControl";
            displayControl.Size = new Size(650, 255);
            displayControl.TabIndex = 4;
            // 
            // propertiesControl
            // 
            propertiesControl.BackColor = SystemColors.ControlDarkDark;
            propertiesControl.Location = new Point(672, 28);
            propertiesControl.Margin = new Padding(1);
            propertiesControl.Name = "propertiesControl";
            propertiesControl.Size = new Size(244, 255);
            propertiesControl.TabIndex = 5;
            // 
            // timer
            // 
            timer.Enabled = true;
            timer.Tick += timer_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(924, 529);
            Controls.Add(propertiesControl);
            Controls.Add(displayControl);
            Controls.Add(timelineControl);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
            Margin = new Padding(2);
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
        private ToolStripMenuItem test2ToolStripMenuItem;
        private System.Windows.Forms.Timer timer;
    }
}
