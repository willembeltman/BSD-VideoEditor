namespace VideoEditorD3D.Direct3D.Interfaces;

public interface IFrame
{
    byte[] Data { get; }
    int Width { get; }
    int Height { get; }
}