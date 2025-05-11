using VideoEditorD3D.Entities;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Loggers;
using VideoEditorD3D.Application.Configs;
using VideoEditorD3D.Application.Types;
using VideoEditorD3D.Entities.ZipDatabase;

namespace VideoEditorD3D.Application;

public class ApplicationContext : IApplicationContext
{
    public ILogger Logger { get; }
    public bool KillSwitch { get; set; }
    public ApplicationConfig Config { get; }
    public ApplicationDbContext Db { get; }
    public IApplicationForm? ApplicationForm { get; private set; }
    public MainForm? MainForm { get; private set; }
    public DrawerThread? DrawerThread { get; private set; }

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
        DrawerThread = new DrawerThread(this, applicationForm);
        return DrawerThread;
    }
    public Form OnCreateStartForm(IApplicationForm applicationForm)
    {
        ApplicationForm = applicationForm;
        MainForm = new MainForm(this, applicationForm);
        return MainForm;
    }
    public void Start()
    {
        Logger.StartThread();

        //Db.Project
        var project = Db.Projects.FirstOrDefault();
        if (project == null)
        {
            project = new Project();
            Db.Projects.Add(project);
            Db.SaveChanges();
        }
        if (!project.Timelines.Any())
        {
            var timeline = new Timeline();
            project.Timelines.Add(timeline);
            project.CurrentTimelineId = timeline.Id;
            Db.SaveChanges();
        }

    }

    public void Dispose()
    {
        KillSwitch = true;
        Db.Dispose();
        Logger.Dispose();
        GC.SuppressFinalize(this);
    }
}