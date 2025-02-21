using MyVideoEditor.DTOs;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoEditor.Services
{
    public class VideoFrameService
    {

        public VideoFrame Clone(VideoFrame frame)
        {
            byte[]? bytes = null;
            if (frame.FrameData != null)
            {
                bytes = new byte[frame.FrameData.Length];
                Array.Copy(frame.FrameData, bytes, bytes.Length);
            }
            return new VideoFrame()
            {
                Index = frame.Index,
                FrameData = bytes,
                Width = frame.Width,
                Height = frame.Height
            };
        }

        public static VideoFrame FromImage(Bitmap image)
        {
            // Lock the bitmap data for direct access
            var bitmap = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // Get a pointer to the pixel data
            var ptr = bitmap.Scan0;
            var size = image.Width * image.Height * 3;
            var framedata = new byte[size];

            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    var imgoffset = bitmap.Stride * y + x * 4;
                    var red = Marshal.ReadByte(ptr, imgoffset + 0);
                    var green = Marshal.ReadByte(ptr, imgoffset + 1);
                    var blue = Marshal.ReadByte(ptr, imgoffset + 2);
                    var alpha = Marshal.ReadByte(ptr, imgoffset + 3);

                    var frameoffset = image.Width * y * 3 + x * 3;
                    framedata[frameoffset + 0] = blue;
                    framedata[frameoffset + 1] = green;
                    framedata[frameoffset + 2] = red;
                }

            return new VideoFrame()
            {
                FrameData = framedata,
                Width = image.Width,
                Height = image.Height
            };
        }
    }
}
