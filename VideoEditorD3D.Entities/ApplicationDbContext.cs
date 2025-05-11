using VideoEditorD3D.Entities.ZipDatabase;

namespace VideoEditorD3D.Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(string fullName) : base(fullName)
    {
        Project = new DbObject<Project>(this);
        TimelineAudios = new DbSet<TimelineAudio>(this);
        MediaFiles = new DbSet<MediaFile>(this);
        MediaStreams = new DbSet<MediaStream>(this);
        Timelines = new DbSet<Timeline>(this);
        TimelineVideos = new DbSet<TimelineVideo>(this);
        Load();
    }

    public DbObject<Project> Project { get; }
    public DbSet<TimelineAudio> TimelineAudios { get; }
    public DbSet<MediaFile> MediaFiles { get; }
    public DbSet<MediaStream> MediaStreams { get; }
    public DbSet<Timeline> Timelines { get; }
    public DbSet<TimelineVideo> TimelineVideos { get; }
}
