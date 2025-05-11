using System.ComponentModel.DataAnnotations.Schema;
using VideoEditorD3D.Entities.ZipDatabase;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;

namespace VideoEditorD3D.Entities;

public class MediaStream : IEntity
{
    public long Id { get; set; }
    public long MediaFileId { get; set; }
    public int Index { get; set; }
    public string? Type { get; set; }

    [ForeignKey("MediaFileId")]
    public virtual MediaFile? MediaFile { get; set; }

    [ForeignKey("MediaStreamId")]
    public virtual ICollection<TimelineVideo> TimelineVideos { get; set; } = [];
}

//public class MediaStreamProxy : MediaStream
//{
//    private readonly MediaStream Item;
//    private readonly ApplicationDbContext Db;

//    public MediaStreamProxy(MediaStream item, ApplicationDbContext db)
//    {
//        Item = item;
//        Db = db;
//    }

//    public new long Id { get => Item.Id; set => Item.Id = value; }
//    public new long MediaFileId { get => Item.MediaFileId; set => Item.MediaFileId = value; }
//    public new int Index { get => Item.Index; set => Item.Index = value; }
//    public new string? Type { get => Item.Type; set => Item.Type = value; }

//    private MediaFile? _MediaFile { get; set; }
//    public override MediaFile? MediaFile
//    {
//        get
//        {
//            if (_MediaFile == null)
//            {
//                _MediaFile = Db.MediaFiles.FirstOrDefault(a => a.Id == MediaFileId);
//            }
//            return _MediaFile;
//        }
//        set
//        {
//            _MediaFile = value;
//        }
//    }

//    private ICollection<TimelineVideo>? _TimelineVideos;
//    public override ICollection<TimelineVideo> TimelineVideos
//    {
//        get
//        {
//            if (_TimelineVideos == null)
//            {
//                _TimelineVideos = new EntityProxyForeignCollection<TimelineVideo, MediaStream>(Db.TimelineVideos, Item, (a, b) => a.MediaStreamId == b.Id, (a, b) => a.MediaStreamId = b.Id);
//            }
//            return _TimelineVideos;
//        }
//        set
//        {
//            _TimelineVideos = value;
//        }
//    }

//    public static MediaStream CreateProxy(MediaStream mediaStream, ApplicationDbContext db)
//    {
//        return new MediaStreamProxy(mediaStream, db);
//    }
//}