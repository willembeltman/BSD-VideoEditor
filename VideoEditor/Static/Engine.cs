using System.Diagnostics;
using VideoEditor.UI;
using DisplayControl = VideoEditor.UI.DisplayControl;

namespace VideoEditor.Static;

public static class Engine
{
    public static Project Project { get; set; } = new Project();
    public static bool IsRunning { get; set; } = true;
    public static bool IsPlaying { get; private set; } = false;

    public static TimelineControl? TimelineControl { get; set; }
    public static DisplayControl? DisplayControl { get; set; }
    public static MainForm? MainForm { get; set; }
    public static PropertiesControl? PropertiesControl { get; set; }
    static Thread? TheThread { get; set; }

    static Stopwatch Stopwatch { get; set; } = new Stopwatch();
    static AutoResetEvent Invoke { get; set; } = new AutoResetEvent(false);
    static double StartTime { get; set; }

    public static Timeline Timeline => Project.CurrentTimeline;


    public static void StartEngine()
    {
        TheThread = new Thread(() => TheThreadJob());
        TheThread.Start();
    }
    public static void StopEngine()
    {
        if (TheThread == null) return;

        IsRunning = false;
        Invoke.Set();
        if (TheThread != Thread.CurrentThread)
            TheThread.Join();
    }

    public static void Play()
    {
        if (!IsPlaying)
        {
            Stopwatch.Restart();
            IsPlaying = true;
            StartTime = Timeline.CurrentTime;
            Invoke.Set();
        }
    }
    public static void Stop()
    {
        IsPlaying = false;
    }

    private static void TheThreadJob()
    {
        while (IsRunning)
        {
            if (!Invoke.WaitOne(1000)) continue;
            while (IsPlaying && IsRunning)
            {
                var wait = Convert.ToInt32(Timeline.NextTime * 1000 - Stopwatch.ElapsedMilliseconds);
                if (wait > 0)
                {
                    Thread.Sleep(wait);
                }

                Timeline.CurrentTime = Stopwatch.Elapsed.TotalSeconds;
                foreach (var video in Timeline.CurrentVideoClips)
                {
                    var framedata = video.GetFrame();
                    DisplayControl.SetFrame(framedata, Timeline.Resolution.Width, Timeline.Resolution.Height);
                    TimelineControl.Invalidate();
                }
            }
        }
    }
}
