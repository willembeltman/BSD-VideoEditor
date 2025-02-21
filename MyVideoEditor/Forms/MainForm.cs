using MyVideoEditor.Controls;
using MyVideoEditor.DTOs;
using MyVideoEditor.Services;
using MyVideoEditor.VideoObjects;

namespace MyVideoEditor.Forms
{
    public partial class MainForm : Form
    {
        public MainMediaControl MediaControl { get; }
        public MainTimelinesControl MainTimelinesControl { get; }
        public MainTimelineControl MainTimelineControl { get; }

        public FfmpegExecuteblesPaths FfmpegExecuteblesPaths { get; }
        public StreamInfoService StreamInfoService { get; }
        public ProjectService ProjectService { get; }
        public StreamContainerService MediaContainerService { get; }
        public TimelineService TimelineService { get; }
        public TimeStampService TimeStampService { get; }

        Project? _Project { get; set; }
        public Project? Project
        {
            get
            {
                return _Project;
            }
            set
            {
                _Project = value;
                MediaControl.ProjectSet();
                MainTimelinesControl.ProjectSet();
                MainTimelineControl.ProjectSet();
            }
        }

        Timeline? _Timeline { get; set; }
        public Timeline? Timeline
        {
            get
            {
                if (Project == null) return null;
                if (Project.CurrentTimelineId == Guid.Empty) return null;
                if (_Timeline == null || _Timeline.Id != Project.CurrentTimelineId)
                {
                    _Timeline = Project.Timelines.First(a => a.Id == Project.CurrentTimelineId);
                }
                return _Timeline;
            }
            set
            {
                if (Project == null) return;
                Project.CurrentTimelineId = value?.Id ?? Guid.Empty;
                _Timeline = value;
            }
        }


        public MainForm()
        {
            if (Environment.ProcessPath == null) throw new Exception("Cannot get execution path");

            FfmpegExecuteblesPaths =
                new FfmpegExecuteblesPaths(Environment.ProcessPath, "Executebles");
            StreamInfoService = new StreamInfoService();
            TimeStampService = new TimeStampService();
            TimelineService = new TimelineService(this);
            MediaContainerService =
                new StreamContainerService(FfmpegExecuteblesPaths, StreamInfoService);
            ProjectService =
                new ProjectService(this, FfmpegExecuteblesPaths, MediaContainerService, TimelineService, TimeStampService);

            MediaControl = new MainMediaControl(this);
            Controls.Add(MediaControl);

            MainTimelinesControl = new MainTimelinesControl(this);
            Controls.Add(MainTimelinesControl);

            MainTimelineControl = new MainTimelineControl(this);
            Controls.Add(MainTimelineControl);

            InitializeComponent();
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            MainForm_Resize(sender, e);

            Project = ProjectService.NewProjectClicked_AfterConfirm();
            timelineToolStripMenuItem_Click(sender, e);
        }
        private void MainForm_Resize(object sender, EventArgs e)
        {
            MediaControl.Left = 0;
            MediaControl.Top = menuStrip1.Height;
            MediaControl.Width = ClientRectangle.Width;
            MediaControl.Height = ClientRectangle.Height - menuStrip1.Height;

            MainTimelinesControl.Left = 0;
            MainTimelinesControl.Top = menuStrip1.Height;
            MainTimelinesControl.Width = ClientRectangle.Width;
            MainTimelinesControl.Height = ClientRectangle.Height - menuStrip1.Height;

            MainTimelineControl.Left = 0;
            MainTimelineControl.Top = menuStrip1.Height;
            MainTimelineControl.Width = ClientRectangle.Width;
            MainTimelineControl.Height = ClientRectangle.Height - menuStrip1.Height;

            MainTimelineControl.TimelinePanel_Resize(sender, e);
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Project != null)
                if (ProjectService.Close() == false)
                    return;

            Project = ProjectService.NewProjectClicked_AfterConfirm();
        }
        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Project != null)
                if (ProjectService.Close() == false)
                    return;

            Project = ProjectService.OpenProjectClicked_AfterConfirm();
        }
        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Project == null)
            {
                MessageBox.Show("No project open?");
                return;
            }
            ProjectService.SaveProjectClicked();
        }
        private void saveProjectAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Project == null)
            {
                MessageBox.Show("No project open?");
                return;
            }
            ProjectService.SaveAsProjectClicked();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Project == null) return;
            ProjectService.Close();
            Close();
        }

        private void mediaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mediaToolStripMenuItem.BackColor = SystemColors.ControlDark;
            timelinesToolStripMenuItem.BackColor = SystemColors.Control;
            timelineToolStripMenuItem.BackColor = SystemColors.Control;

            MediaControl.Visible = true;
            MainTimelinesControl.Visible = false;
            MainTimelineControl.Visible = false;

            Invalidate();
        }
        private void timelinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mediaToolStripMenuItem.BackColor = SystemColors.Control;
            timelinesToolStripMenuItem.BackColor = SystemColors.ControlDark;
            timelineToolStripMenuItem.BackColor = SystemColors.Control;

            MediaControl.Visible = false;
            MainTimelinesControl.Visible = true;
            MainTimelineControl.Visible = false;

            Invalidate();
        }
        private void timelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mediaToolStripMenuItem.BackColor = SystemColors.Control;
            timelinesToolStripMenuItem.BackColor = SystemColors.Control;
            timelineToolStripMenuItem.BackColor = SystemColors.ControlDark;

            MediaControl.Visible = false;
            MainTimelinesControl.Visible = false;
            MainTimelineControl.Visible = true;

            Invalidate();
        }

        private void insertFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Project == null || Timeline == null)
            {
                MessageBox.Show("No project open?");
                return;
            }
            ProjectService.InsertVideosButtonClicked(Project);
            Invalidate();
        }

        private void findVisualStudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }
}