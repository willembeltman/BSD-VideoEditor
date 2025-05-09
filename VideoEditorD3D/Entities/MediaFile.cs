using VideoEditorD3D.Database;

namespace VideoEditorD3D.Entities
{
    public class MediaFile : IEntity
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}
