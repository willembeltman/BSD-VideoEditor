using MyVideoEditor.DTOs;

namespace MyVideoEditor.Services
{
    public class StreamInfoService
    {
        public StreamInfo CreateStreamInfo(int streamindex, string[] parameters)
        {
            var res = new StreamInfo();
            res.Index = streamindex;
            res.Codec = parameters[0];
            res.CodecTypeString = parameters[1];

            if (parameters.Length == 5)
            {
                if (double.TryParse(parameters[3]
                    .Replace(".", System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                    .Replace(",", System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator), out double duration))
                    res.Duration = duration;

                if (int.TryParse(parameters[4], out int bitrate))
                    res.Bitrate = bitrate;
            }

            if (parameters.Length == 7)
            {
                if (int.TryParse(parameters[2], out int width) &&
                    int.TryParse(parameters[3], out int height))
                    res.Resolution = new Size(width, height);

                var fpsparameters = parameters[4].Split(new string[] { "/" }, StringSplitOptions.None);
                if (fpsparameters.Length == 2)
                {
                    if (long.TryParse(fpsparameters[0], out long fpsbase))
                        res.FramerateBase = fpsbase;
                    if (long.TryParse(fpsparameters[1], out long fpsdivider))
                        res.FramerateDivider = fpsdivider;
                }

                if (double.TryParse(parameters[5]
                    .Replace(".", System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                    .Replace(",", System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator), out double duration))
                    res.Duration = duration;

                if (int.TryParse(parameters[6], out int bitrate))
                    res.Bitrate = bitrate;
            }

            return res;
        }
    }
}
