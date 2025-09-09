using Bsd.Logger;
using System.Diagnostics;
using VideoEditorD3D.Application.Buffers;
using VideoEditorD3D.Application.Helpers;
using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Controls;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Entities;
using VideoEditorD3D.FFMpeg.Interfaces;

namespace VideoEditorD3D.Application;

public class ApplicationState : IApplicationState
{
    private VideoDrawerThread? _DrawerThread;

    public ILogger? Logger { get; }
    public ApplicationSettings Config { get; }
    public ApplicationDbContext Db { get; }
    public Project Project { get; }
    public Timeline Timeline { get; }
    public ObservableArrayCollection<SoftwareVideoBuffer> VideoBuffers { get; }
    public ObservableArrayCollection<AudioBuffer> AudioBuffers { get; }
    public Stopwatch PlaybackStopwatch { get; }
    public bool PlaybackBackward { get; set; }
    public double PlaybackStart { get; set; }

    public ApplicationState()
    {
        Logger = new DebugLogger();
        Config = ApplicationSettings.Load();
        PlaybackStopwatch = new Stopwatch();

        VideoBuffers = [];
        VideoBuffers.Added += VideoAdded;
        VideoBuffers.Removed += VideoRemoved;
        AudioBuffers = [];
        AudioBuffers.Added += AudioAdded;
        AudioBuffers.Removed += AudioRemoved;

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

    private void AudioRemoved(object? sender, AudioBuffer e)
    {
        e.Dispose();
    }

    private void VideoRemoved(object? sender, SoftwareVideoBuffer e)
    {
        e.Dispose();
    }

    private void AudioAdded(object? sender, AudioBuffer e)
    {
        e.StartThread();
    }

    private void VideoAdded(object? sender, SoftwareVideoBuffer e)
    {
        e.StartThread();
    }

    public IDrawerThread? OnCreateDrawerThread(IApplicationForm applicationForm)
    {
        return new VideoDrawerThread(this, applicationForm);
    }
    public Form OnCreateMainForm(IApplicationForm applicationForm)
    {
        return new MainForm(this);
    }

    public bool UpdateCurrentTime()
    {
        if (PlaybackStopwatch.IsRunning)
        {
            Timeline.CurrentTime = PlaybackStart + PlaybackStopwatch.Elapsed.TotalSeconds * (PlaybackBackward ? -1 : 1);
            return true;
        }
        return false;
    }
    public IVideoFrame[] GetCurrentFrames()
    {
        return VideoBuffers
            .Where(a => 
                a.TimelineStartTime <= Timeline.CurrentTime && Timeline.CurrentTime <= a.TimelineEndTime)
            .OrderBy(a => a.TimelineLayer)
            .Select(a => a.GetCurrentFrame(Timeline.CurrentTime))
            .ToArray();
    }

    public void Dispose()
    {
        Logger?.Dispose();
        _DrawerThread?.Dispose();
        GC.SuppressFinalize(this);
    }

}