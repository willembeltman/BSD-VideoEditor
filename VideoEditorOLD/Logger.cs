using System.Collections.Concurrent;
using System.Diagnostics;

namespace VideoEditor
{
    public class Logger2 : IDisposable
    {
        Stopwatch Stopwatch = Stopwatch.StartNew();

        public Logger2(Engine engine)
        {
            Engine = engine;
            AutoResetEvent = new AutoResetEvent(false);
            List = new ConcurrentQueue<string>();
            Thread = new Thread(new ThreadStart(Kernel));
        }

        Engine Engine { get; }
        AutoResetEvent AutoResetEvent { get; }
        ConcurrentQueue<string> List { get; }
        public Thread Thread { get; }
        bool KillSwitch { get; set; }

        public void WriteLine(string message)
        {
            var msg = $"{Stopwatch.ElapsedMilliseconds} {message}";
            List.Enqueue(msg);
            AutoResetEvent.Set();
        }
        private void Kernel()
        {
            while (Engine.IsRunning && !KillSwitch)
            {
                AutoResetEvent.WaitOne();

                while (List.TryDequeue(out var item))
                {
                    Debug.WriteLine(item);
                }
            }
        }

        public void Dispose()
        {
            KillSwitch = true;
            if (Thread.CurrentThread != Thread)
                Thread.Join();
        }
    }
}