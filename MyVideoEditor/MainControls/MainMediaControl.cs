using MyVideoEditor.Controls;
using MyVideoEditor.DTOs;
using MyVideoEditor.Forms;
using MyVideoEditor.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyVideoEditor
{
    public partial class MainMediaControl : UserControl
    {
        #region Props 

        MainForm MainForm { get; }

        ProjectService ProjectService => MainForm.ProjectService;
        StreamContainerService MediaContainerService => MainForm.MediaContainerService;
        TimelineService TimelineService => MainForm.TimelineService;
        TimeStampService TimeStampService => MainForm.TimeStampService;

        Project? Project => MainForm.Project;
        Timeline? Timeline => MainForm.Timeline;

        #endregion

        public MainMediaControl(MainForm mainForm)
        {
            MainForm = mainForm;
            InitializeComponent();
        }


        public void ProjectSet()
        {
            if (Project == null)
                return;

            //Medias = MediaContainerService.Open(project.MediaContainersFullnames)
            //    .Select(a => new MediaItem(a))
            //    .ToArray();

            //dataGridView1.DataSource = Medias;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {

        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {

        }
    }

    internal class MediaControlItem
    {
        private StreamContainer a;

        public MediaControlItem(StreamContainer a)
        {
            this.a = a;

        }
        public string Name => a.FullName;
    }
}
