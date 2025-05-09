using VideoEditorD3D.Direct3D;

namespace VideoEditorD3D;

public class ApplicationDrawer(Application Context)
{
    public Canvas DrawCanvas()
    {
        var layers = new CanvasLayer[]
        {
            new(0, Context.Device, Context.Characters, Context.PhysicalWidth, Context.PhysicalHeight)
        };
        return new Canvas(layers);
    }
}