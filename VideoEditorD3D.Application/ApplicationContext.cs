using VideoEditorD3D.Entities;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;
using Bsd.Logger;
using VideoEditorD3D.Application.Helpers;
using VideoEditorD3D.Application.Forms;

namespace VideoEditorD3D.Application;

public class ApplicationContext : IApplicationContext
{
    public ILogger? Logger { get; }
    public bool KillSwitch { get; set; }
    public ApplicationSettings Config { get; }
    public ApplicationDbContext Db { get; }

    public Project Project { get; private set; } = new Project(); // Initialise with empty project, not needed, but Timeline needs to be set to something, so let's be consistent.
    public Timeline Timeline { get; private set; } = new Timeline(); // Initialise with empty timeline because the form wants to use the fps value of the project, which doesn't work when Timeline is null.

    public IApplicationForm? ApplicationForm { get; private set; }
    public MainForm? MainForm { get; private set; }
    public VideoDrawerThread? DrawerThread { get; private set; }

    public ApplicationContext()
    {
        Logger = new DebugLogger();
        Config = ApplicationSettings.Load();
        if (Config.LastDatabaseFullName == null)
        {
            Db = new ApplicationDbContext($"NewProject_{DateTime.Now:yyyy-MM-dd HH-mm}.zip");
            Config.LastDatabaseFullName = Db.FullName;
            Config.Save();
        }
        else
        {
            Db = new ApplicationDbContext(Config.LastDatabaseFullName);
        }
    }

    public IDrawerThread? OnCreateDrawerThread(IApplicationForm applicationForm)
    {
        ApplicationForm = applicationForm;
        DrawerThread = new VideoDrawerThread(this, applicationForm);
        return DrawerThread;
    }
    public Form OnCreateStartForm(IApplicationForm applicationForm)
    {
        ApplicationForm = applicationForm;
        MainForm = new MainForm(this, applicationForm);
        return MainForm;
    }
    public void Start(IApplicationForm applicationForm)
    {
        ApplicationForm = applicationForm;
        Logger?.StartThread();
        LoadCurrentProjectAndTimeline();
    }

    private void LoadCurrentProjectAndTimeline()
    {
        if (Db.Projects.Any())
        {
            Project = Db.Projects.First();
        }
        else
        {
            Project = new Project();
            Db.Projects.Add(Project);
            Db.SaveChanges();
        }
        if (!Project.Timelines.Any())
        {
            Timeline = new Timeline();
            Project.Timelines.Add(Timeline);
            Project.CurrentTimelineId = Timeline.Id;
            Db.SaveChanges();
            return;
        }
        if (Project.CurrentTimeline.Value == null)
        {
            Timeline = Project.Timelines.First();
            Project.CurrentTimelineId = Timeline.Id;
            Db.SaveChanges();
            return;
        }
        Timeline = Project.CurrentTimeline.Value;
    }

    public void Dispose()
    {
        KillSwitch = true;
        Db.Dispose();
        Logger?.Dispose();
        GC.SuppressFinalize(this);
    }
}