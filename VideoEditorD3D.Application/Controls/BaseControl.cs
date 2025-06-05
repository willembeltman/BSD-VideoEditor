using VideoEditorD3D.Direct3D.Controls.Templates;
using VideoEditorD3D.Entities;

namespace VideoEditorD3D.Application.Controls
{
    public class BaseControl : BackControl
    {
        public MainForm MainForm => (MainForm)ParentForm;
        public ApplicationState State => MainForm.ApplicationContext;
        public Project Project => State.Project;
        public Timeline Timeline => State.Timeline;
    }
}
