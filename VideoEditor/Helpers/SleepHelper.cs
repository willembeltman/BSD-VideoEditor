using System.Diagnostics;
using VideoEditor.Types;

namespace VideoEditor.Helpers;

public class SleepHelper
{
    public SleepHelper(Engine engine)
    {
        Engine = engine;
        Stopwatch = Stopwatch.StartNew();
    }

    public Engine Engine { get; }
    public Stopwatch Stopwatch { get; }

    private Fps Fps => Engine.Timeline.Fps;

    public int GetSleepTimeTillNextFrame()
    {
        double offset = 0;

        // Calculate wait till next frame
        var elapsed = Stopwatch.Elapsed.TotalSeconds;
        var currentFrameIndex = elapsed * Fps;
        var nextFrameIndex = Math.Ceiling(currentFrameIndex);
        var nextElapsed = nextFrameIndex / Fps + offset;
        var wait = nextElapsed - elapsed;
        var waitMs = Convert.ToInt32(wait * 1000);
        var fpswaitMs = Convert.ToInt32(1000 / Fps);

        // Sleep the thread if needed
        if (0 < waitMs && waitMs <= fpswaitMs)
            return waitMs;
        else
            return fpswaitMs;
    }
}
