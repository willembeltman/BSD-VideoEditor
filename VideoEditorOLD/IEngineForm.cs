using VideoEditor.UI;

namespace VideoEditor
{
    public interface IEngineForm
    {
        TimelineControlDX2D TimelineControl { get; }
        DisplayControlDX2D DisplayControl { get; }
        PropertiesControl PropertiesControl { get; }
    }
}