using EntityFrameworkZip;

namespace VideoEditorD3D.Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(string zipFullName) : base(zipFullName) { }

    public virtual DbSet<Project> Projects { get; set; }
    public virtual DbSet<MediaFile> MediaFiles { get; set; }
    public virtual DbSet<MediaStream> MediaStreams { get; set; }
    public virtual DbSet<Timeline> Timelines { get; set; }
    public virtual DbSet<TimelineClipAudio> TimelineClipAudios { get; set; }
    public virtual DbSet<TimelineClipGroup> TimelineClipGroups { get; set; }
    public virtual DbSet<TimelineClipVideo> TimelineClipVideos { get; set; }
}
