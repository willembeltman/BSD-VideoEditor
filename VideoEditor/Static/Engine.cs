using System.Diagnostics;
using VideoEditor.UI;
using DisplayControl = VideoEditor.UI.DisplayControl;

namespace VideoEditor.Static;

public static class Engine
{
    public static Project Project { get; set; } = new Project();
    public static bool IsRunning { get; set; } = true;

    public static TimelineControl? TimelineControl { get; set; }
    public static DisplayControl? DisplayControl { get; set; }
    public static MainForm? MainForm { get; set; }
    public static PropertiesControl? PropertiesControl { get; set; }

    static Stopwatch Stopwatch { get; set; } = new Stopwatch();
    static bool IsPlaying { get; set; } = false;
    public static double StartTime { get; private set; }
    static AutoResetEvent Invoke { get; set; } = new AutoResetEvent(false);

    public static Timeline Timeline => Project.CurrentTimeline;

    static Thread? TheThread { get; set; }

    public static void StartEngine()
    {
        TheThread = new Thread(() => InnerStartEngine());
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

    private static void InnerStartEngine()
    {
        while (IsRunning)
        {
            if (!Invoke.WaitOne(1000)) continue;
            while (IsPlaying && IsRunning)
            { 
                while(Timeline.NextTime > Stopwatch.Elapsed.TotalSeconds)
                {
                    Thread.Sleep(1);
                }
                Timeline.CurrentTime = Stopwatch.Elapsed.TotalSeconds;
                var videos = Timeline.GetCurrentVideoClips();
                foreach (var video in videos)
                {
                    var framedata = video.GetFrame();
                    DisplayControl.SetFrame(framedata, Timeline.Resolution.Width, Timeline.Resolution.Height);
                }
            }
        }
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

    //static int w = 1920;
    //static int h = 1080;
    //static int c = 0;
    //static int a = 1;
    //static byte[] frameData = new byte[w * h * 4]; // 4K BGRA
    //static Stopwatch Stopwatch = Stopwatch.StartNew();
    //static long counter = 0;


    //public static void Idle()
    //{
    //    if (DisplayControl == null) return;

    //    //var b = Convert.ToByte(c);
    //    //Parallel.For(0, frameData.Length, i =>
    //    //{
    //    //    frameData[i] = b;
    //    //});
    //    //DisplayControl.SetFrame(frameData, w, h);
    //    //counter++;

    //    //c += a;
    //    //if (c == 255)
    //    //    a = -1;
    //    //if (c == 0)
    //    //{
    //    //    a = 1;

    //    //    Debug.WriteLine($"{Convert.ToDouble(counter) / Stopwatch.Elapsed.TotalSeconds:F2}");
    //    //    counter = 0;
    //    //    Stopwatch.Restart();
    //    //}
    //}
}
