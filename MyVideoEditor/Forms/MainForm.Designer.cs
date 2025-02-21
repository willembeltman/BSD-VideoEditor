namespace MyVideoEditor.Forms
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
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newProjectToolStripMenuItem = new ToolStripMenuItem();
            openProjectToolStripMenuItem = new ToolStripMenuItem();
            saveProjectToolStripMenuItem = new ToolStripMenuItem();
            saveProjectAsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            distinctFramesToolToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            mediaToolStripMenuItem = new ToolStripMenuItem();
            timelinesToolStripMenuItem = new ToolStripMenuItem();
            timelineToolStripMenuItem = new ToolStripMenuItem();
            insertAFileToolStripMenuItem = new ToolStripMenuItem();
            findVisualStudioToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(28, 28);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem, toolStripMenuItem1, mediaToolStripMenuItem, timelinesToolStripMenuItem, timelineToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(5, 2, 0, 2);
            menuStrip1.Size = new Size(1002, 33);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newProjectToolStripMenuItem, openProjectToolStripMenuItem, saveProjectToolStripMenuItem, saveProjectAsToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(54, 29);
            fileToolStripMenuItem.Text = "File";
            // 
            // newProjectToolStripMenuItem
            // 
            newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            newProjectToolStripMenuItem.Size = new Size(247, 34);
            newProjectToolStripMenuItem.Text = "New Project";
            newProjectToolStripMenuItem.Click += newProjectToolStripMenuItem_Click;
            // 
            // openProjectToolStripMenuItem
            // 
            openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            openProjectToolStripMenuItem.Size = new Size(247, 34);
            openProjectToolStripMenuItem.Text = "Open Project";
            openProjectToolStripMenuItem.Click += openProjectToolStripMenuItem_Click;
            // 
            // saveProjectToolStripMenuItem
            // 
            saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            saveProjectToolStripMenuItem.Size = new Size(247, 34);
            saveProjectToolStripMenuItem.Text = "Save Project";
            saveProjectToolStripMenuItem.Click += saveProjectToolStripMenuItem_Click;
            // 
            // saveProjectAsToolStripMenuItem
            // 
            saveProjectAsToolStripMenuItem.Name = "saveProjectAsToolStripMenuItem";
            saveProjectAsToolStripMenuItem.Size = new Size(247, 34);
            saveProjectAsToolStripMenuItem.Text = "Save Project As...";
            saveProjectAsToolStripMenuItem.Click += saveProjectAsToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(244, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(247, 34);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(58, 29);
            editToolStripMenuItem.Text = "Edit";
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { distinctFramesToolToolStripMenuItem, findVisualStudioToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(69, 29);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // distinctFramesToolToolStripMenuItem
            // 
            distinctFramesToolToolStripMenuItem.Name = "distinctFramesToolToolStripMenuItem";
            distinctFramesToolToolStripMenuItem.Size = new Size(273, 34);
            distinctFramesToolToolStripMenuItem.Text = "Distinct Frames Tool";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(65, 29);
            helpToolStripMenuItem.Text = "Help";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(32, 29);
            toolStripMenuItem1.Text = "|";
            // 
            // mediaToolStripMenuItem
            // 
            mediaToolStripMenuItem.Name = "mediaToolStripMenuItem";
            mediaToolStripMenuItem.Size = new Size(77, 29);
            mediaToolStripMenuItem.Text = "Media";
            mediaToolStripMenuItem.Click += mediaToolStripMenuItem_Click;
            // 
            // timelinesToolStripMenuItem
            // 
            timelinesToolStripMenuItem.Name = "timelinesToolStripMenuItem";
            timelinesToolStripMenuItem.Size = new Size(101, 29);
            timelinesToolStripMenuItem.Text = "Timelines";
            timelinesToolStripMenuItem.Click += timelinesToolStripMenuItem_Click;
            // 
            // timelineToolStripMenuItem
            // 
            timelineToolStripMenuItem.BackColor = SystemColors.ControlDark;
            timelineToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { insertAFileToolStripMenuItem });
            timelineToolStripMenuItem.ForeColor = SystemColors.ControlText;
            timelineToolStripMenuItem.Name = "timelineToolStripMenuItem";
            timelineToolStripMenuItem.Size = new Size(93, 29);
            timelineToolStripMenuItem.Text = "Timeline";
            timelineToolStripMenuItem.Click += timelineToolStripMenuItem_Click;
            // 
            // insertAFileToolStripMenuItem
            // 
            insertAFileToolStripMenuItem.Name = "insertAFileToolStripMenuItem";
            insertAFileToolStripMenuItem.Size = new Size(293, 34);
            insertAFileToolStripMenuItem.Text = "Insert video in timeline";
            insertAFileToolStripMenuItem.Click += insertFileToolStripMenuItem_Click;
            // 
            // findVisualStudioToolStripMenuItem
            // 
            findVisualStudioToolStripMenuItem.Name = "findVisualStudioToolStripMenuItem";
            findVisualStudioToolStripMenuItem.Size = new Size(273, 34);
            findVisualStudioToolStripMenuItem.Text = "FindVisualStudio";
            findVisualStudioToolStripMenuItem.Click += findVisualStudioToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1002, 664);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(2);
            Name = "MainForm";
            Text = "Form1";
            Load += MainForm_Load;
            Resize += MainForm_Resize;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openProjectToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem newProjectToolStripMenuItem;
        private ToolStripMenuItem saveProjectToolStripMenuItem;
        private ToolStripMenuItem saveProjectAsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem distinctFramesToolToolStripMenuItem;
        private ToolStripMenuItem timelineToolStripMenuItem;
        private ToolStripMenuItem insertAFileToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem mediaToolStripMenuItem;
        private ToolStripMenuItem timelinesToolStripMenuItem;
        private ToolStripMenuItem findVisualStudioToolStripMenuItem;
    }
}