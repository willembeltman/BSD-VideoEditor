using SharpDX.Direct3D11;
using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Vertices;
using Buffer = SharpDX.Direct3D11.Buffer;
using Control = VideoEditorD3D.Direct3D.Controls.Control;

namespace VideoEditorD3D.Direct3D.Drawing;

public class GraphicsLayer(Control control) : IDisposable
{
    private IApplicationForm ApplicationForm => control.ApplicationForm;

    public int AbsoluteLeft => control.AbsoluteLeft;
    public int AbsoluteTop => control.AbsoluteTop;
    public int Width => ApplicationForm.Width;
    public int Height => ApplicationForm.Height;
    public Device Device => ApplicationForm.Device;
    public CharacterCollection Characters => ApplicationForm.Characters;

    public List<Vertex> LineVertices { get; } = [];
    public Buffer? LineVerticesBuffer { get; set; }
    public List<Vertex> TriangleVertices { get; } = [];
    public Buffer? TriangleVerticesBuffer { get; set; }
    public List<ITextureImage> TextureImages { get; } = [];

    public void StartDrawing()
    {
        LineVerticesBuffer?.Dispose();
        LineVerticesBuffer = null;
        TriangleVerticesBuffer?.Dispose();
        TriangleVerticesBuffer = null;

        foreach (var image in TextureImages)
            image.Dispose();
        foreach (var image in TextureImages)
            image.Dispose();

        LineVertices.Clear();
        TriangleVertices.Clear();
        TextureImages.Clear();
        TextureImages.Clear();
    }

    public void EndDrawing()
    {
        if (LineVertices.Count > 0)
            LineVerticesBuffer = Buffer.Create(ApplicationForm.Device, BindFlags.VertexBuffer, LineVertices.ToArray());
        if (TriangleVertices.Count > 0)
            TriangleVerticesBuffer = Buffer.Create(ApplicationForm.Device, BindFlags.VertexBuffer, TriangleVertices.ToArray());
    }

    public void Dispose()
    {
        foreach (var image in TextureImages)
            image.Dispose();

        GC.SuppressFinalize(this);
    }
}
