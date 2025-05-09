using VideoEditorD3D.Database;
using VideoEditorD3D.Entities;

namespace VideoEditorD3D.Engine
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(string fullName) : base(fullName)
        {
            ProjectSettings = new DbObject<ProjectSettings>(this);

            MediaFiles = new DbSet<MediaFile>(this);
            MediaStreams = new DbSet<MediaStream>(this);
            VideoClips = new DbSet<VideoClip>(this);
            AudioClips = new DbSet<AudioClip>(this);
        }

        public DbObject<ProjectSettings> ProjectSettings { get; }

        public DbSet<MediaFile> MediaFiles { get; }
        public DbSet<MediaStream> MediaStreams { get; }
        public DbSet<VideoClip> VideoClips { get; }
        public DbSet<AudioClip> AudioClips { get; }
    }
}
