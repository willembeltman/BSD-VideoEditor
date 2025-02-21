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

namespace MyVideoEditor.Controls
{
    public partial class MainTimelinesControl : UserControl
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

        public MainTimelinesControl(MainForm mainForm)
        {
            MainForm = mainForm;
            InitializeComponent();
        }

        public void ProjectSet()
        {
            if (Project == null)
                return;

        }
    }
}
