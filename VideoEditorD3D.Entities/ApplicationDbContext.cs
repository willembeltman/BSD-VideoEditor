using VideoEditorD3D.Entities.ZipDatabase;
using VideoEditorD3D.Loggers;

namespace VideoEditorD3D.Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(string fullName, ILogger logger) : base(fullName, logger)
    {
    }

    public virtual DbSet<Project> Projects { get; set; }
    public virtual DbSet<TimelineClipAudio> TimelineAudios { get; set; }
    public virtual DbSet<MediaFile> MediaFiles { get; set; }
    public virtual DbSet<MediaStream> MediaStreams { get; set; }
    public virtual DbSet<Timeline> Timelines { get; set; }
    public virtual DbSet<TimelineClipVideo> TimelineVideos { get; set; }
}
