using VideoEditorD3D.Direct3D.Interfaces;
namespace VideoEditorD3D.Types;

public class Frame : IFrame
{
    public Frame(byte[] data, int width, int height)
    {
        Data = data;
        Width = width;
        Height = height;
    }

    public byte[] Data { get; }
    public int Width { get; }
    public int Height { get; }
}
