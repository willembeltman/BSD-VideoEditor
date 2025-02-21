using VideoEditor.Forms;

namespace VideoEditor;

public static class Engine
{
    public static Project Project { get; set; } = new Project();
    public static Timeline Timeline => Project.CurrentTimeline;

    public static TimelineControl? TimelineControl { get; set; }
    public static DisplayControl? DisplayControl { get; set; }
    public static MainForm? MainForm { get; set; }
    public static PropertiesControl? PropertiesControl { get; set; }

    public static void Start()
    {
        //throw new NotImplementedException();
    }
}
