namespace VideoEditorD3D.Database.Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(string fullName) : base(fullName)
    {
        AudioClips = new DbSet<TimelineAudio>(this);
        MediaFiles = new DbSet<MediaFile>(this);
        MediaStreams = new DbSet<MediaStream>(this);
        Timelines = new DbSet<Timeline>(this);
        VideoClips = new DbSet<TimelineVideo>(this);
    }

    public DbSet<TimelineAudio> AudioClips { get; }
    public DbSet<MediaFile> MediaFiles { get; }
    public DbSet<MediaStream> MediaStreams { get; }
    public DbSet<Timeline> Timelines { get; }
    public DbSet<TimelineVideo> VideoClips { get; }
}
