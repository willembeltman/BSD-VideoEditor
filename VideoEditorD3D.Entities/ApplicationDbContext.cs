using VideoEditorD3D.Entities.ZipDatabase;
using VideoEditorD3D.Loggers;

namespace VideoEditorD3D.Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(string fullName, ILogger logger) : base(fullName)
    {
        Projects = new DbSet<Project>(this, logger);
        TimelineAudios = new DbSet<TimelineClipAudio>(this, logger);
        MediaFiles = new DbSet<MediaFile>(this, logger);
        MediaStreams = new DbSet<MediaStream>(this, logger);
        Timelines = new DbSet<Timeline>(this, logger);
        TimelineVideos = new DbSet<TimelineClipVideo>(this, logger);
    }

    public DbSet<Project> Projects { get; }
    public DbSet<TimelineClipAudio> TimelineAudios { get; }
    public DbSet<MediaFile> MediaFiles { get; }
    public DbSet<MediaStream> MediaStreams { get; }
    public DbSet<Timeline> Timelines { get; }
    public DbSet<TimelineClipVideo> TimelineVideos { get; }
}
