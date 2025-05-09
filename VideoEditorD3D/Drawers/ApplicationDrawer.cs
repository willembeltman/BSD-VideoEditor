using VideoEditorD3D.Direct3D.Canvas;

namespace VideoEditorD3D.Drawers;

public class ApplicationDrawer(ApplicationContext Context)
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