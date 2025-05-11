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
        }
        var timeline = new Timeline();
        project.Timelines.Add(timeline);
        var mediaFile = new MediaFile();
        project.Files.Add(mediaFile);
        var mediaStream = new MediaStream();
        mediaFile.MediaStreams.Add(mediaStream);
        var timelineVideo = new TimelineVideo()
        {
            MediaStreamId = mediaStream.Id
        };
        timeline.TimelineVideos.Add(timelineVideo);
        //mediaStream.TimelineVideos.Add(timelineVideo);

        var n = timeline.Project.Value;

        //var timeline = new Timeline()
        //{
        //    ProjectId = project.Id,
        //};
        //Db.Timelines.Add(timeline);

        //var mediaFile = new MediaFile()
        //{
        //    ProjectId = project.Id
        //};
        //Db.MediaFiles.Add(mediaFile);

        //var mediaStream = new MediaStream()
        //{
        //    MediaFileId = mediaFile.Id,
        //};
        //Db.MediaStreams.Add(mediaStream);

        //var timelineVideo = new TimelineVideo()
        //{
        //    TimelineId = timeline.Id,
        //    MediaStreamId = mediaStream.Id,
        //};
        //Db.TimelineVideos.Add(timelineVideo);
        Db.SaveChanges();

        mediaFile = Db.MediaFiles.FirstOrDefault();
        mediaStream = Db.MediaStreams.FirstOrDefault();
        timeline = Db.Timelines.FirstOrDefault();
        timelineVideo = Db.TimelineVideos.FirstOrDefault();

        var aa = mediaFile!.MediaStreams.FirstOrDefault();
        var bbb = aa!.MediaFile.Value.MediaStreams.First().TimelineVideos.First().Timeline.Value.TimelineAudios.ToArray();

    }

    public void Dispose()
    {
        KillSwitch = true;
        Db.Dispose();
        Logger.Dispose();
        GC.SuppressFinalize(this);
    }
}