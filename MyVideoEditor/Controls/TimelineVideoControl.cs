using MyVideoEditor.DTOs;
using MyVideoEditor.VideoObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoEditor.Controls
{
    public class TimelineVideoControl : UserControl
    {
        public TimelineVideoControl(TimelineVideo timelineVideo, MediaVideo mediaVideo, VideoStreamReader videoStreamReader)
        {
            TimelineVideo = timelineVideo;
            MediaVideo = mediaVideo;
            VideoStreamReader = videoStreamReader;
            BackColor = Color.Gray;
        }

        public TimelineVideo TimelineVideo { get; }
        public MediaVideo MediaVideo { get; }
        public VideoStreamReader VideoStreamReader { get; }

    }
}
