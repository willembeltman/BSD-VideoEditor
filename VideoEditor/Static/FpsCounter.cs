using System.Collections.Concurrent;
using System.Diagnostics;

namespace VideoEditor.Static
{
    public class FpsCounter
    {
        const int secterug = 5;
        Stopwatch Stopwatch = Stopwatch.StartNew();
        ConcurrentQueue<double> fpslist = new ConcurrentQueue<double>();
        public void Tick()
        {
            var currenttime = Stopwatch.Elapsed.TotalSeconds;
            fpslist.Enqueue(currenttime);

            var dequeueLenght = fpslist.Count(time => time < currenttime - secterug);
            for (int i = 0; i < dequeueLenght; i++)
            {
                fpslist.TryDequeue(out var result);
            }
        }

        public double GetFps()
        {
            var now = Stopwatch.Elapsed.TotalSeconds;
            var list = fpslist
                .Where(time => time >= now - secterug)
                .ToArray();
            return Convert.ToDouble(list.Length) / Math.Min(secterug, now);
        }
    }
}
