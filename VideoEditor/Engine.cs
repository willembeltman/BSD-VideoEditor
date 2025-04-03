using System.Diagnostics;
using VideoEditor.Static;
using VideoEditor.Types;
using VideoEditor.UI;
using DisplayControl = VideoEditor.UI.DisplayControl;

namespace VideoEditor;

public class Engine
{
    public Project Project { get; set; } = new Project();
    public bool IsRunning { get; set; } = true;
    public bool IsPlaying { get; private set; } = false;

    public TimelineControl? TimelineControl { get; set; }
    public DisplayControl? DisplayControl { get; set; }
    public MainForm? MainForm { get; set; }
    public PropertiesControl? PropertiesControl { get; set; }
    Thread? TheThread { get; set; }

    Stopwatch Stopwatch { get; set; } = Stopwatch.StartNew();
    double StartTime { get; set; }

    public FpsCounter FpsCounter { get; set; } = new FpsCounter();
    public Timeline Timeline => Project.CurrentTimeline;


    public void StartEngine()
    {
        TheThread = new Thread(new ThreadStart(TheThreadJob));
        TheThread.Start();
    }
    public void StopEngine()
    {
        if (TheThread == null) return;

        IsRunning = false;
        if (TheThread != Thread.CurrentThread)
            TheThread.Join();
    }

    public void Play()
    {
        if (!IsPlaying)
        {
            Stopwatch.Restart();
            StartTime = Timeline.CurrentTime;
            IsPlaying = true;
        }
    }
    public void Stop()
    {
        IsPlaying = false;
    }

    private void TheThreadJob()
    {
        while (IsRunning)
        {
            // Calculate wait till next frame
            var wait = Convert.ToInt32(Timeline.NextTime * 1000 - StartTime * 1000 - Stopwatch.ElapsedMilliseconds);
            var fpswait = Convert.ToInt32(1000 / Timeline.Fps.Value);

            // Sleep the thread if needed
            if (wait > 0 && wait < fpswait)
                Thread.Sleep(wait);
            
            else if (!IsPlaying) // Prevent 
                Thread.Sleep(fpswait);

            if (IsPlaying)
            {
                // Set the current time (after sleeping)
                Timeline.CurrentTime = StartTime + Stopwatch.Elapsed.TotalSeconds;
            }

            // Get the frames (for the current time)
            var clipframes = Timeline.CurrentVideoClips
                .Select(clip => new ClipFrame(clip, clip.GetCurrentFrame()))
                .Where(a => a.Frame != null)
                .OrderBy(a => a.Clip.Layer)
                .ToArray();

            // Update fps counter
            FpsCounter.Tick();

            // Merge frames to one frame
            var frame = FlattenFrames(clipframes);
            if (frame == null) continue;

            // Then display the frame
            DisplayControl?.SetFrame(frame);
            TimelineControl?.Invalidate();
        }
    }

    private Frame? FlattenFrames(ClipFrame[] clipframes)
    {
        // TODO make a frame of all frames
        var clipframe = clipframes.FirstOrDefault();
        return clipframe?.Frame;
    }
}
