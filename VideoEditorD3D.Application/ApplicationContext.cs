using Bsd.Logger;
using VideoEditorD3D.Application.Forms;
using VideoEditorD3D.Application.Helpers;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Entities;

namespace VideoEditorD3D.Application;

public class ApplicationContext : IApplicationContext
{
    private VideoDrawerThread? _DrawerThread;

    public ILogger? Logger { get; }
    public ApplicationSettings Config { get; }
    public ApplicationDbContext Db { get; }
    public Project Project { get; }
    public Timeline Timeline { get; }

    public ApplicationContext()
    {
        Logger = new DebugLogger();
        Config = ApplicationSettings.Load();

        if (Config.LastDatabaseFullName == null)
        {
            Config.LastDatabaseFullName = $"NewProject_{DateTime.Now:yyyy-MM-dd HH-mm}.zip";
            Db = new ApplicationDbContext(Config.LastDatabaseFullName);
            Config.Save();
        }
        else
        {
            Db = new ApplicationDbContext(Config.LastDatabaseFullName);
        }

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

    public IDrawerThread? OnCreateDrawerThread(IApplicationForm applicationForm)
    {
        return new VideoDrawerThread(this, applicationForm);
    }
    public Form OnCreateMainForm(IApplicationForm applicationForm)
    {
        return new MainForm(this);
    }

    public void Dispose()
    {
        Logger?.Dispose();
        _DrawerThread?.Dispose();
        GC.SuppressFinalize(this);
    }
}