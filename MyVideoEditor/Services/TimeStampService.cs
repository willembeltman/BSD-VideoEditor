using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoEditor.Services
{
    public class TimeStampService
    {
        public TimeStampService()
        {
        }

        public string GetTimeStamp(long frameIndex, long framerateBase, long framerateDivider)
        {
            double seconds = frameIndex / (framerateBase / (double)framerateDivider);
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            return timeSpan.ToString(@"hh\:mm\:ss\.fff");
        }
    }
}
