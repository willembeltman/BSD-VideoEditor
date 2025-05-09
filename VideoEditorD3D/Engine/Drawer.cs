using VideoEditorD3D.Direct3D.Canvas;
using VideoEditorD3D.Interfaces;

namespace VideoEditorD3D.Engine;

public class Drawer
{
    public Drawer(IApplication application) => Application = application;

    IApplication Application;

    public Canvas DrawCanvas()
    {
        var layers = new CanvasLayer[]
        {
            new CanvasLayer(0, Application.Device, Application.Characters, Application.Width, Application.Height)
        };
        return new Canvas(layers);
    }
}