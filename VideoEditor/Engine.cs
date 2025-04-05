using System.Diagnostics;
using VideoEditor.Static;
using VideoEditor.Types;
using VideoEditor.UI;

namespace VideoEditor;

public class Engine : IDisposable
{
    public Engine(IEngineForm engineForm)
    {
        EngineForm = engineForm;
        Thread = new Thread(new ThreadStart(Kernel));
        SleepHelper = new SleepHelper(this);
    }

    public IEngineForm EngineForm { get; }
    public Thread Thread { get; }
    public SleepHelper SleepHelper { get; }

    public TimelineControlDX2D TimelineControl => EngineForm.TimelineControl;
    public DisplayControlDX2D DisplayControl => EngineForm.DisplayControl;
    public PropertiesControl PropertiesControl => EngineForm.PropertiesControl;

    public Project Project { get; set; } = new Project();
    public bool IsRunning { get; set; } = true;
    public bool IsPlaying { get; private set; } = false;

    Stopwatch Stopwatch { get; set; } = Stopwatch.StartNew();
    double StartTime { get; set; }

    public FpsCounter FpsCounter { get; set; } = new FpsCounter();
    public Timeline Timeline => Project.CurrentTimeline;

    public void StartAll()
    {
        Thread.Start();
        TimelineControl.Thread.Start();
        DisplayControl.Thread.Start();
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

    private void Kernel()
    {
        while (IsRunning)
        {
            //var sleep = SleepHelper.SleepTillNextFrame();

            //Debug.WriteLine(sleep.ToString());

            //Thread.Sleep(sleep / 4);

            Thread.Sleep(5);

            if (IsPlaying)
            {
                // Set the current time so everybody will update
                Timeline.CurrentTime = StartTime + Stopwatch.Elapsed.TotalSeconds;
            }

            //Debug.WriteLine(Timeline.CurrentFrameIndex + " Engine");

            FpsCounter.Tick();
        }
    }

    public void Dispose()
    {
        Project.Dispose();

        IsRunning = false;
        if (Thread != Thread.CurrentThread)
            Thread.Join();
    }

}
