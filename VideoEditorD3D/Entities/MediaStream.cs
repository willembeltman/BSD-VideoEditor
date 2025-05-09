using VideoEditorD3D.Database;

namespace VideoEditorD3D.Entities
{
    public class MediaStream : IEntity
    {
        public long Id { get; set; }
        public int Index { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
