using System.Collections.Concurrent;
using System.Diagnostics;

namespace VideoEditor.Static
{
    public class FpsCounter
    {
        Stopwatch Stopwatch = Stopwatch.StartNew();
        ConcurrentQueue<double> fpslist = new ConcurrentQueue<double>();
        public void Tick()
        {
            var currenttime = Stopwatch.Elapsed.TotalSeconds;
            fpslist.Enqueue(currenttime);

            var dequeueLenght = fpslist.Count(time => time < currenttime - 10);
            for (int i = 0; i < dequeueLenght; i++)
            {
                fpslist.TryDequeue(out var result);
            }
        }

        public int Fps
        {
            get
            {
                return fpslist
                    .Count(time => time >= Stopwatch.Elapsed.TotalSeconds - 1);
            }
        }
    }
}
