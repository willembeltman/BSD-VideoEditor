using VideoEditorD3D.Entities;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Loggers;
using VideoEditorD3D.Application.Configs;

namespace VideoEditorD3D.Application;

public class ApplicationContext : IApplicationContext
{
    public ILogger Logger { get; }
    public bool KillSwitch { get; set; }
    public ApplicationConfig Config { get; }
    public ApplicationDbContext Db { get; }
    public IApplicationForm? ApplicationForm { get; private set; }
    public MainForm? MainForm { get; private set; }
    public VideoDrawerThread? DrawerThread { get; private set; }
    public Project CurrentProject { get; private set; } = new Project();
    public Timeline CurrentTimeline { get; private set; } = new Timeline();

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

        Logger.StartThread();

        // Load current project and timeline
        if (Db.Projects.Any())
        {
            CurrentProject = Db.Projects.First();
        }
        else
        {
            CurrentProject = new Project();
            Db.Projects.Add(CurrentProject);
            Db.SaveChanges();
        }
        if (!CurrentProject.Timelines.Any())
        {
            CurrentTimeline = new Timeline();
            CurrentProject.Timelines.Add(CurrentTimeline);
            CurrentProject.CurrentTimelineId = CurrentTimeline.Id;
            Db.SaveChanges();
            return;
        }
        if (CurrentProject.CurrentTimeline.Value == null)
        {
            CurrentTimeline = CurrentProject.Timelines.First();
            CurrentProject.CurrentTimelineId = CurrentTimeline.Id;
            Db.SaveChanges();
            return;
        }
        CurrentTimeline = CurrentProject.CurrentTimeline.Value;
    }

    public void Dispose()
    {
        KillSwitch = true;
        Db.Dispose();
        Logger.Dispose();
        GC.SuppressFinalize(this);
    }
}