namespace VideoEditor.Types;

public class Frame
{
    public Frame(Resolution resolution, long index)
    {
        Resolution = resolution;
        Index = index;
        Buffer = new byte[resolution.Width * resolution.Height * 4];
    }

    public Resolution Resolution { get; }
    public long Index { get; set; }
    public byte[] Buffer { get; }
}
