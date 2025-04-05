using System.Collections.Concurrent;
using System.Diagnostics;

namespace VideoEditor.Static
{
    public class FpsCounter
    {
        const int AantalSecondenTerug = 2;
        Stopwatch Stopwatch = Stopwatch.StartNew();
        ConcurrentQueue<double> fpslist = new ConcurrentQueue<double>();

        public void Tick()
        {
            fpslist.Enqueue(Stopwatch.Elapsed.TotalSeconds);
        }

        public double GetFps()
        {
            if (fpslist.Count == 0) return 0;
            var currentTime = Stopwatch.Elapsed.TotalSeconds;
            var selectedTime = fpslist.Max();
            //var selectedTime = currentTime - maxTime > 1 ? currentTime : maxTime;

            var dequeueLenght = fpslist.Count(time => time < selectedTime - AantalSecondenTerug);
            for (int i = 0; i < dequeueLenght; i++)
            {
                fpslist.TryDequeue(out var result);
            }

            return Convert.ToDouble(fpslist.Count) / Math.Min(AantalSecondenTerug, selectedTime);
        }
    }
}
