using System.Diagnostics;
using System.Windows.Forms;
using VideoEditor.Types;
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

    static Stopwatch Stopwatch { get; set; } = Stopwatch.StartNew();
    static AutoResetEvent Invoke { get; set; } = new AutoResetEvent(false);
    static double StartTime { get; set; }

    public static FpsCounter FpsCounter { get; set; } = new FpsCounter();
    public static Timeline Timeline => Project.CurrentTimeline;


    public static void StartEngine()
    {
        TheThread = new Thread(new ThreadStart(TheThreadJob));
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
            StartTime = Timeline.CurrentTime;
            IsPlaying = true;
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
            //if (!Invoke.WaitOne(1000)) continue;
            while (IsRunning) // (IsPlaying && IsRunning)
            {
                // Calculate wait till next frame
                var wait = Convert.ToInt32(Timeline.NextTime * 1000 - Stopwatch.ElapsedMilliseconds);

                // Sleep the thread if needed
                if (wait > 0) Thread.Sleep(wait);
                else Thread.Sleep(Convert.ToInt32(1000 / Timeline.Fps.Value));

                if (IsPlaying)
                {
                    // Set the current time (after sleeping)
                    Timeline.CurrentTime = StartTime + Stopwatch.Elapsed.TotalSeconds;
                }

                // Get the frames for the current time
                var clipframes = Timeline.CurrentVideoClips
                    .Select(clip => new ClipFrame(clip, clip.GetCurrentFrame()))
                    .Where(a => a.Frame != null)
                    .OrderBy(a => a.Clip.Layer)
                    .ToArray();

                // Merge frames to one frame
                var frame = CreateFrame(clipframes);
                if (frame == null) continue;

                // The display the frame
                DisplayControl?.SetFrame(frame);
                TimelineControl?.Invalidate();
                FpsCounter.Tick();
            }
        }
    }

    private static Frame? CreateFrame(ClipFrame[] clipframes)
    {
        // TODO make a frame of all frames
        var clipframe = clipframes.FirstOrDefault();
        return clipframe?.Frame;
    }
}
