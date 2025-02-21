using MyVideoEditor.DTOs;
using MyVideoEditor.Forms;
using MyVideoEditor.Services;
using MyVideoEditor.VideoObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyVideoEditor.Controls
{
    public class TimelineControl : UserControl
    {
        #region Props 

        MainForm MainForm { get; }

        ProjectService ProjectService => MainForm.ProjectService;
        StreamContainerService MediaContainerService => MainForm.MediaContainerService;
        TimelineService TimelineService => MainForm.TimelineService;
        TimeStampService TimeStampService => MainForm.TimeStampService;

        Project? Project => MainForm?.Project;
        Timeline? Timeline => MainForm?.Timeline;

        #endregion

        public TimelineControl(MainForm mainForm)
        {
            MainForm = mainForm;
            BackColor = Color.Black;

            Paint += TimelineControl_Paint;
            //InitializeComponent();
        }
        public void ProjectSet()
        {
            // Reset controls
            foreach (var item in VideoControls)
                Controls.Remove(item);
            foreach (var item in AudioControls)
                Controls.Remove(item);
        }
        private void TimelineControl_Paint(object sender, PaintEventArgs e)
        {
            if (Project == null) return;
            if (Timeline == null) return;

            // Huidige timecode bepalen / zichtbare selectie

            var timelinelength = Timeline.TotalLength;
            if (timelinelength < 60) timelinelength = 60;
            var timelinestart = Timeline.DisplayStart;
            var timelineend = timelinestart + timelinelength;

            // Video items tekenen

            var marge = 8;

            var height = (Height - marge) / 2;
            var top1 = 0;
            var top2 = height + marge;

            var videorect = new Rectangle(Left, top1, Width, height);
            var audiorect = new Rectangle(Left, top2, Width, height);

            var g = e.Graphics;
            g.Clear(Color.White);
            g.FillRectangle(Brushes.LightGray, videorect);
            g.DrawLine(Pens.Black, videorect.Left, videorect.Top, videorect.Right, videorect.Top);
            g.DrawLine(Pens.Black, videorect.Left, videorect.Bottom, videorect.Right, videorect.Bottom);

            g.FillRectangle(Brushes.LightGray, audiorect);
            g.DrawLine(Pens.Black, audiorect.Left, audiorect.Top, audiorect.Right, audiorect.Top);
            g.DrawLine(Pens.Black, audiorect.Left, audiorect.Bottom, audiorect.Right, audiorect.Bottom);

            var items = 10;
            var step = 1d;
            while (true)
            {
                if (timelinelength / step > items)
                {
                    step = step * 10;
                }
                else if (timelinelength / step < items / 5)
                {
                    step = step * 10;
                }
                else break;
            }

            for (var i = timelinestart; i <= timelineend; i += step)
            {
                var left = Convert.ToInt32(videorect.Width / timelinelength * i);

                // Draw mark line 
                g.DrawLine(Pens.Gray, left, videorect.Top + 1, left, videorect.Bottom - 1);
                g.DrawLine(Pens.Gray, left, audiorect.Top + 1, left, audiorect.Bottom - 1);

                // Draw current time (i)
                var textrect = g.MeasureString($"{i:F2}", SystemFonts.DefaultFont);
                g.DrawString($"{i:F2}", SystemFonts.DefaultFont, Brushes.Black, left, audiorect.Bottom - textrect.Height);
            }

            g.Dispose();
        }

        List<TimelineVideoControl> VideoControls { get; set; } = new List<TimelineVideoControl>();  
        List<TimelineAudioControl> AudioControls { get; set; } = new List<TimelineAudioControl>();

        public void AddTimelineVideoControl(MediaVideo mediaVideo, TimelineVideo timelineVideo, VideoStreamReader videoStreamReader)
        {
            var control = new TimelineVideoControl(timelineVideo, mediaVideo, videoStreamReader);
            VideoControls.Add(control);
            Controls.Add(control);
        }
        public void AddTimelineAudioControl(MediaAudio mediaAudio, TimelineAudio timelineAudio, AudioStreamReader audioStreamReader)
        {
            var control = new TimelineAudioControl(timelineAudio, mediaAudio, audioStreamReader);
            AudioControls.Add(control);
            Controls.Add(control);
        }


        //private void TimelineControl_Paint(object sender, PaintEventArgs e)
        //{
        //    if (Project == null) return;
        //    if (Timeline == null) return;

        //    // Huidige timecode bepalen / zichtbare selectie

        //    var timelinelength = Timeline.TotalLength;
        //    if (timelinelength < 60) timelinelength = 60;
        //    var timelinestart = Timeline.DisplayStart;
        //    var timelineend = timelinestart + timelinelength;

        //    // Video items tekenen

        //    var marge = 8;

        //    var height = (Height - marge) / 2;
        //    var top1 = 0;
        //    var top2 = height + marge;

        //    var videorect = new Rectangle(Left, top1, Width, height);
        //    var audiorect = new Rectangle(Left, top2, Width, height);

        //    var g = e.Graphics;
        //    g.Clear(Color.White);
        //    g.FillRectangle(Brushes.LightGray, videorect);
        //    g.DrawLine(Pens.Black, videorect.Left, videorect.Top, videorect.Right, videorect.Top);
        //    g.DrawLine(Pens.Black, videorect.Left, videorect.Bottom, videorect.Right, videorect.Bottom);

        //    g.FillRectangle(Brushes.LightGray, audiorect);
        //    g.DrawLine(Pens.Black, audiorect.Left, audiorect.Top, audiorect.Right, audiorect.Top);
        //    g.DrawLine(Pens.Black, audiorect.Left, audiorect.Bottom, audiorect.Right, audiorect.Bottom);


        //    var items = 10;
        //    var step = 1d;
        //    while (true)
        //    {
        //        if (timelinelength / step > items)
        //        {
        //            step = step * 10;
        //        }
        //        else if (timelinelength / step < items / 5)
        //        {
        //            step = step * 10;
        //        }
        //        else break;
        //    }

        //    for (var i = timelinestart; i <= timelineend; i += step)
        //    {
        //        var left = Convert.ToInt32(videorect.Width / timelinelength * i);

        //        // Draw mark line 
        //        g.DrawLine(Pens.Gray, left, videorect.Top + 1, left, videorect.Bottom - 1);
        //        g.DrawLine(Pens.Gray, left, audiorect.Top + 1, left, audiorect.Bottom - 1);

        //        // Draw current time (i)
        //        var textrect = g.MeasureString($"{i:F2}", SystemFonts.DefaultFont);
        //        g.DrawString($"{i:F2}", SystemFonts.DefaultFont, Brushes.Black, left, audiorect.Bottom - textrect.Height);
        //    }

        //    var list = Timeline.TimelineVideos
        //        //.Where(a =>
        //        //    (a.TimelineStartTime >= timelinestart && a.TimelineStartTime <= timelineend) ||
        //        //    (a.TimelineEndTime >= timelinestart && a.TimelineEndTime <= timelineend))
        //        .OrderBy(a => a.TimelineStartTime)
        //        .ToArray();

        //    foreach (var item in list)
        //    {
        //        var leftperc = (item.TimelineStartTime - timelinestart) / timelinelength;
        //        var rightperc = 1 - (item.TimelineEndTime - timelineend) / timelinelength;

        //        var left = Convert.ToInt32(videorect.Width * leftperc);
        //        var right = Convert.ToInt32(videorect.Width * rightperc);
        //        var width = right - left;

        //        var rect = new Rectangle(videorect.Left + left, videorect.Top + 5, width, videorect.Height - 10);

        //        g.FillRectangle(Brushes.Black, rect);
        //        var textrect = g.MeasureString($"{item.}", SystemFonts.DefaultFont);
        //        //g.DrawString()
        //    }

        //    // Audio items tekenen
        //    foreach (var item in Timeline.TimelineAudios)
        //    {

        //    }

        //    g.Dispose();
        //}
    }
}
