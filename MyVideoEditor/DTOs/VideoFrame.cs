namespace MyVideoEditor.DTOs
{
    public class VideoFrame
    {
        public VideoFrame(byte[] frameData, int frameWidth, int frameHeight, long index)
        {
            Index = index;
            FrameData = frameData;
            Width = frameWidth;
            Height = frameHeight;
        }

        public long Index { get; set; }
        public byte[]? FrameData { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Color GetPixel(int x, int y)
        {
            var frameoffset = Width * y * 3 + x * 3;
            return Color.FromArgb(
                FrameData[frameoffset + 0],
                FrameData[frameoffset + 1],
                FrameData[frameoffset + 2]);

        }
        public void SetPixel(int x, int y, Color difpixel)
        {
            var pos = (y * Width + x) * 3;
            FrameData[pos + 0] = difpixel.R;
            FrameData[pos + 1] = difpixel.G;
            FrameData[pos + 2] = difpixel.B;
        }
    }
}