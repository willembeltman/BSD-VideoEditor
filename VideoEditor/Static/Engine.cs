using System.Diagnostics;
using VideoEditor.UI;
using DisplayControl = VideoEditor.UI.DisplayControl;

namespace VideoEditor;

public static class Engine
{
    public static Project Project { get; set; } = new Project();
    public static Timeline Timeline => Project.CurrentTimeline;

    public static TimelineControl? TimelineControl { get; set; }
    public static DisplayControl? DisplayControl { get; set; }
    public static MainForm? MainForm { get; set; }
    public static PropertiesControl? PropertiesControl { get; set; }

    static int w = 1920;
    static int h = 1080;
    static int c = 0;
    static int a = 1;
    static byte[] frameData = new byte[w * h * 4]; // 4K BGRA
    static Stopwatch Stopwatch = Stopwatch.StartNew();
    static long counter = 0;

    public static async Task Idle()
    {
        if (DisplayControl == null) return;

        var b = Convert.ToByte(c);
        Parallel.For(0, frameData.Length, i =>
        {
            frameData[i] = b;
        });
        DisplayControl.SetFrame(frameData, w, h);
        counter++;

        c += a;
        if (c == 255)
            a = -1;
        if (c == 0)
        {
            a = 1;

            Debug.WriteLine($"{Convert.ToDouble(counter) / Stopwatch.Elapsed.TotalSeconds:F2}");
            counter = 0;
            Stopwatch.Restart();
        }

    }
}
