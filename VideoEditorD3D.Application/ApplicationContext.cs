using VideoEditorD3D.Entities;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Application.Types;
using Bsd.Logger;

namespace VideoEditorD3D.Application;

public class ApplicationContext : IApplicationContext
{
    public ILogger? Logger { get; }
    public bool KillSwitch { get; set; }
    public ApplicationConfig Config { get; }
    public ApplicationDbContext Db { get; }

    public Project Project { get; private set; } = new Project(); // Initialisatie om nullability te vermijden, wordt netjes ingeladen bij de load
    public Timeline Timeline { get; private set; } = new Timeline(); // Initialisatie om nullability te vermijden, wordt netjes ingeladen bij de load

    public IApplicationForm? ApplicationForm { get; private set; }
    public MainForm? MainForm { get; private set; }
    public VideoDrawerThread? DrawerThread { get; private set; }

    public ApplicationContext()
    {
        Logger = new DebugLogger();
        Config = ApplicationConfig.Load();
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