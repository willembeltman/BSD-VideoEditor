﻿using EntityFrameworkZip;

namespace VideoEditorD3D.Entities;

public class MediaStream : IEntity
{
    public long Id { get; set; }
    public long MediaFileId { get; set; }
    public int Index { get; set; }
    public string Type { get; set; }

    public virtual ILazy<MediaFile> MediaFile { get; set; }
    public virtual ICollection<TimelineClipVideo> TimelineVideos { get; set; }
}