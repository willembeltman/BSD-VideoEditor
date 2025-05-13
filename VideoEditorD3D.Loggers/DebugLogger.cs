using System.Collections.Concurrent;
using System.Diagnostics;

namespace VideoEditorD3D.Loggers;

public class DebugLogger : ILogger
{
    private readonly AutoResetEvent NewMessageReceived;
    private readonly Thread LoggerThread;
    private readonly ConcurrentQueue<DebugLoggerMessage> Lines;
    private int CurrentIndex;
    private bool KillSwitch = false;
    public Stopwatch Stopwatch;

    public DebugLogger()
    {
        Lines = new ConcurrentQueue<DebugLoggerMessage>();
        NewMessageReceived = new AutoResetEvent(false);
        LoggerThread = new Thread(Kernel)
        {
            Name = "DebugLogger Kernel"
        };
        Stopwatch = Stopwatch.StartNew();
    }

    public void StartThread()
    {
        LoggerThread.Start();
    }
    private void Kernel()
    {
        while (!KillSwitch)
            Loop();
    }
    private void Loop()
    {
        if (!NewMessageReceived.WaitOne(100)) return;

        while (Lines.TryDequeue(out var message))
        {
            Debug.WriteLine(message.Message);
        }
    }

    public void WriteLine(string message, ConsoleColor color) => WriteLine(message);
    public void WriteLine(string message)
    {
        Lines.Enqueue(new DebugLoggerMessage(DateTime.Now, message));
        NewMessageReceived.Set();
    }
    public void ReWriteLine(string message, ConsoleColor color) => ReWriteLine(message);
    public void ReWriteLine(string message)
    {
        var interval = 1d / 60;
        var currentTime = Stopwatch.Elapsed.TotalSeconds;
        var huidigeIndex = Convert.ToInt32(Math.Floor(currentTime / interval));

        if (CurrentIndex < huidigeIndex)
        {
            CurrentIndex = huidigeIndex;
            Lines.Enqueue(new DebugLoggerMessage(DateTime.Now, message));
            NewMessageReceived.Set();
        }
    }
    public void WriteException(string message) => WriteLine(message);
    public void WriteException(Exception ex) => WriteException("Exception is thrown:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

    public void Dispose()
    {
        KillSwitch = true;
        if (LoggerThread.ThreadState == System.Threading.ThreadState.Running && Thread.CurrentThread != LoggerThread)
            LoggerThread.Join();

        GC.SuppressFinalize(this);
    }
}
