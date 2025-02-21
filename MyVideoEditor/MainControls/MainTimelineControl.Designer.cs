namespace MyVideoEditor
{
    partial class MainTimelineControl
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
            TimelineScrollBar = new HScrollBar();
            TimelineDisplayPanel = new Panel();
            buttonBackward = new Button();
            buttonForward = new Button();
            buttonPlay = new Button();
            buttonPause = new Button();
            groupBoxProjectSettings = new GroupBox();
            ProjectFramerate = new ComboBox();
            label3 = new Label();
            ProjectHeight = new TextBox();
            label2 = new Label();
            ProjectWidth = new TextBox();
            label1 = new Label();
            groupBoxClipSettings = new GroupBox();
            groupBoxTimelineTools = new GroupBox();
            checkBoxRealtime = new CheckBox();
            checkBoxTrackTimelineToCursor = new CheckBox();
            buttonSelectionTool = new Button();
            buttonCutTool = new Button();
            textBoxZoom = new TextBox();
            buttonPlus = new Button();
            buttonMin = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            groupBoxProjectSettings.SuspendLayout();
            groupBoxTimelineTools.SuspendLayout();
            SuspendLayout();
            // 
            // TimelineScrollBar
            // 
            TimelineScrollBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TimelineScrollBar.Location = new Point(0, 689);
            TimelineScrollBar.Name = "TimelineScrollBar";
            TimelineScrollBar.Size = new Size(705, 31);
            TimelineScrollBar.TabIndex = 4;
            // 
            // TimelineDisplayPanel
            // 
            TimelineDisplayPanel.BackColor = SystemColors.ControlText;
            TimelineDisplayPanel.ForeColor = SystemColors.Control;
            TimelineDisplayPanel.Location = new Point(278, 3);
            TimelineDisplayPanel.Name = "TimelineDisplayPanel";
            TimelineDisplayPanel.Size = new Size(297, 358);
            TimelineDisplayPanel.TabIndex = 7;
            // 
            // buttonBackward
            // 
            buttonBackward.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            buttonBackward.ForeColor = SystemColors.ControlText;
            buttonBackward.Location = new Point(278, 367);
            buttonBackward.Name = "buttonBackward";
            buttonBackward.Size = new Size(63, 53);
            buttonBackward.TabIndex = 0;
            buttonBackward.Text = "⏮️";
            buttonBackward.UseVisualStyleBackColor = true;
            buttonBackward.Click += buttonBackward_Click;
            // 
            // buttonForward
            // 
            buttonForward.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            buttonForward.ForeColor = SystemColors.ControlText;
            buttonForward.Location = new Point(485, 367);
            buttonForward.Name = "buttonForward";
            buttonForward.Size = new Size(63, 53);
            buttonForward.TabIndex = 0;
            buttonForward.Text = "⏭️";
            buttonForward.UseVisualStyleBackColor = true;
            buttonForward.Click += buttonForward_Click;
            // 
            // buttonPlay
            // 
            buttonPlay.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            buttonPlay.ForeColor = SystemColors.ControlText;
            buttonPlay.Location = new Point(347, 367);
            buttonPlay.Name = "buttonPlay";
            buttonPlay.Size = new Size(63, 53);
            buttonPlay.TabIndex = 0;
            buttonPlay.Text = "▶️";
            buttonPlay.UseVisualStyleBackColor = true;
            buttonPlay.Click += buttonPlay_Click;
            // 
            // buttonPause
            // 
            buttonPause.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            buttonPause.ForeColor = SystemColors.ControlText;
            buttonPause.Location = new Point(416, 367);
            buttonPause.Name = "buttonPause";
            buttonPause.Size = new Size(63, 53);
            buttonPause.TabIndex = 0;
            buttonPause.Text = "⏸️";
            buttonPause.UseVisualStyleBackColor = true;
            buttonPause.Click += buttonPause_Click;
            // 
            // groupBoxProjectSettings
            // 
            groupBoxProjectSettings.Controls.Add(ProjectFramerate);
            groupBoxProjectSettings.Controls.Add(label3);
            groupBoxProjectSettings.Controls.Add(ProjectHeight);
            groupBoxProjectSettings.Controls.Add(label2);
            groupBoxProjectSettings.Controls.Add(ProjectWidth);
            groupBoxProjectSettings.Controls.Add(label1);
            groupBoxProjectSettings.Location = new Point(3, 6);
            groupBoxProjectSettings.Name = "groupBoxProjectSettings";
            groupBoxProjectSettings.Size = new Size(269, 284);
            groupBoxProjectSettings.TabIndex = 8;
            groupBoxProjectSettings.TabStop = false;
            groupBoxProjectSettings.Text = "Project settings";
            // 
            // ProjectFramerate
            // 
            ProjectFramerate.DropDownStyle = ComboBoxStyle.DropDownList;
            ProjectFramerate.FormattingEnabled = true;
            ProjectFramerate.Location = new Point(113, 104);
            ProjectFramerate.Name = "ProjectFramerate";
            ProjectFramerate.Size = new Size(150, 33);
            ProjectFramerate.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 107);
            label3.Name = "label3";
            label3.Size = new Size(91, 25);
            label3.TabIndex = 4;
            label3.Text = "Framerate";
            // 
            // ProjectHeight
            // 
            ProjectHeight.Location = new Point(113, 67);
            ProjectHeight.Name = "ProjectHeight";
            ProjectHeight.Size = new Size(150, 31);
            ProjectHeight.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 70);
            label2.Name = "label2";
            label2.Size = new Size(65, 25);
            label2.TabIndex = 2;
            label2.Text = "Height";
            // 
            // ProjectWidth
            // 
            ProjectWidth.Location = new Point(113, 30);
            ProjectWidth.Name = "ProjectWidth";
            ProjectWidth.Size = new Size(150, 31);
            ProjectWidth.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 33);
            label1.Name = "label1";
            label1.Size = new Size(60, 25);
            label1.TabIndex = 0;
            label1.Text = "Width";
            // 
            // groupBoxClipSettings
            // 
            groupBoxClipSettings.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            groupBoxClipSettings.Location = new Point(581, 6);
            groupBoxClipSettings.Name = "groupBoxClipSettings";
            groupBoxClipSettings.Size = new Size(269, 414);
            groupBoxClipSettings.TabIndex = 9;
            groupBoxClipSettings.TabStop = false;
            groupBoxClipSettings.Text = "Clip settings";
            // 
            // groupBoxTimelineTools
            // 
            groupBoxTimelineTools.Controls.Add(checkBoxRealtime);
            groupBoxTimelineTools.Controls.Add(checkBoxTrackTimelineToCursor);
            groupBoxTimelineTools.Controls.Add(buttonSelectionTool);
            groupBoxTimelineTools.Controls.Add(buttonCutTool);
            groupBoxTimelineTools.Location = new Point(3, 296);
            groupBoxTimelineTools.Name = "groupBoxTimelineTools";
            groupBoxTimelineTools.Size = new Size(269, 124);
            groupBoxTimelineTools.TabIndex = 10;
            groupBoxTimelineTools.TabStop = false;
            groupBoxTimelineTools.Text = "Timeline tools";
            // 
            // checkBoxRealtime
            // 
            checkBoxRealtime.AutoSize = true;
            checkBoxRealtime.Enabled = false;
            checkBoxRealtime.Location = new Point(11, 91);
            checkBoxRealtime.Name = "checkBoxRealtime";
            checkBoxRealtime.Size = new Size(105, 29);
            checkBoxRealtime.TabIndex = 3;
            checkBoxRealtime.Text = "Realtime";
            checkBoxRealtime.UseVisualStyleBackColor = true;
            // 
            // checkBoxTrackTimelineToCursor
            // 
            checkBoxTrackTimelineToCursor.AutoSize = true;
            checkBoxTrackTimelineToCursor.Location = new Point(11, 64);
            checkBoxTrackTimelineToCursor.Name = "checkBoxTrackTimelineToCursor";
            checkBoxTrackTimelineToCursor.Size = new Size(220, 29);
            checkBoxTrackTimelineToCursor.TabIndex = 0;
            checkBoxTrackTimelineToCursor.Text = "Track timeline to cursor";
            checkBoxTrackTimelineToCursor.UseVisualStyleBackColor = true;
            // 
            // buttonSelectionTool
            // 
            buttonSelectionTool.BackColor = SystemColors.ControlDark;
            buttonSelectionTool.Location = new Point(6, 28);
            buttonSelectionTool.Name = "buttonSelectionTool";
            buttonSelectionTool.Size = new Size(79, 31);
            buttonSelectionTool.TabIndex = 2;
            buttonSelectionTool.Text = "👆";
            buttonSelectionTool.UseVisualStyleBackColor = false;
            buttonSelectionTool.Click += buttonSelectionTool_Click;
            // 
            // buttonCutTool
            // 
            buttonCutTool.Location = new Point(91, 28);
            buttonCutTool.Name = "buttonCutTool";
            buttonCutTool.Size = new Size(71, 31);
            buttonCutTool.TabIndex = 1;
            buttonCutTool.Text = "✂️";
            buttonCutTool.UseVisualStyleBackColor = true;
            buttonCutTool.Click += buttonCutTool_Click;
            // 
            // textBoxZoom
            // 
            textBoxZoom.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            textBoxZoom.Location = new Point(731, 689);
            textBoxZoom.Name = "textBoxZoom";
            textBoxZoom.Size = new Size(92, 31);
            textBoxZoom.TabIndex = 6;
            textBoxZoom.Text = "1";
            textBoxZoom.TextAlign = HorizontalAlignment.Center;
            // 
            // buttonPlus
            // 
            buttonPlus.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonPlus.Font = new Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point);
            buttonPlus.Location = new Point(701, 689);
            buttonPlus.Name = "buttonPlus";
            buttonPlus.Size = new Size(30, 31);
            buttonPlus.TabIndex = 11;
            buttonPlus.Text = "+";
            buttonPlus.UseVisualStyleBackColor = true;
            // 
            // buttonMin
            // 
            buttonMin.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonMin.Font = new Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point);
            buttonMin.Location = new Point(823, 689);
            buttonMin.Name = "buttonMin";
            buttonMin.Size = new Size(30, 31);
            buttonMin.TabIndex = 12;
            buttonMin.Text = "-";
            buttonMin.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Tick += timer1_Tick;
            // 
            // MainTimelineControl
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            Controls.Add(buttonMin);
            Controls.Add(groupBoxTimelineTools);
            Controls.Add(buttonPlus);
            Controls.Add(buttonBackward);
            Controls.Add(textBoxZoom);
            Controls.Add(buttonForward);
            Controls.Add(buttonPlay);
            Controls.Add(buttonPause);
            Controls.Add(groupBoxClipSettings);
            Controls.Add(groupBoxProjectSettings);
            Controls.Add(TimelineDisplayPanel);
            Controls.Add(TimelineScrollBar);
            Margin = new Padding(2);
            Name = "MainTimelineControl";
            Size = new Size(853, 720);
            Load += TimelineControl_Load;
            groupBoxProjectSettings.ResumeLayout(false);
            groupBoxProjectSettings.PerformLayout();
            groupBoxTimelineTools.ResumeLayout(false);
            groupBoxTimelineTools.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private HScrollBar TimelineScrollBar;
        private Panel TimelineDisplayPanel;
        private Button buttonForward;
        private Button buttonPlay;
        private Button buttonPause;
        private Button buttonBackward;
        private GroupBox groupBoxProjectSettings;
        private Label label3;
        private TextBox ProjectHeight;
        private Label label2;
        private TextBox ProjectWidth;
        private Label label1;
        private GroupBox groupBoxClipSettings;
        private ComboBox ProjectFramerate;
        private GroupBox groupBoxTimelineTools;
        private CheckBox checkBoxRealtime;
        private CheckBox checkBoxTrackTimelineToCursor;
        private Button buttonSelectionTool;
        private Button buttonCutTool;
        private TextBox textBoxZoom;
        private Button buttonPlus;
        private Button buttonMin;
        private System.Windows.Forms.Timer timer1;
    }
}
