using System.Collections.Concurrent;
using System.Diagnostics;

namespace VideoEditorD3D.Loggers;

public class ConsoleLogger : ILogger
{
    private AutoResetEvent NewMessageReceived;
    private Thread LoggerThread;
    private ConcurrentQueue<ConsoleLoggerMessage> Lines;
    private int CurrentIndex = 0;
    private string LastMessage = "";
    private bool NeedNewLine = false;
    private bool KillSwitch = false;
    public Stopwatch Stopwatch;

    public ConsoleLogger()
    {
        Lines = new ConcurrentQueue<ConsoleLoggerMessage>();
        NewMessageReceived = new AutoResetEvent(false);
        LoggerThread = new Thread(Kernel);
        LoggerThread.Name = "TradingBot: ConsoleLogger Kernel";
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
            if (Console.ForegroundColor != message.Color)
                Console.ForegroundColor = message.Color;
            if (message.Replace)
            {
                Console.CursorLeft = 0;
                Console.Write(new string(' ', LastMessage.Length));
                Console.CursorLeft = 0;
                Console.Write(message.Message);
                NeedNewLine = true;
            }
            else
            {
                if (NeedNewLine)
                {
                    NeedNewLine = false;
                    Console.CursorLeft = 0;
                    Console.Write(new string(' ', LastMessage.Length));
                    Console.CursorLeft = 0;
                }
                Console.WriteLine(message.Message);
            }

            LastMessage = message.Message;
        }
    }

    public void WriteLine(string message) => WriteLine(message, ConsoleColor.White);
    public void WriteLine(string message, ConsoleColor color)
    {
        Lines.Enqueue(new ConsoleLoggerMessage(DateTime.Now, color, message, false));
        NewMessageReceived.Set();
    }
    public void RewriteLine(string message) => RewriteLine(message, ConsoleColor.White);
    public void RewriteLine(string message, ConsoleColor color)
    {
        var interval = 1d / 60;
        var currentTime = Stopwatch.Elapsed.TotalSeconds;
        var huidigeIndex = Convert.ToInt32(Math.Floor(currentTime / interval));

        if (CurrentIndex < huidigeIndex)
        {
            CurrentIndex = huidigeIndex;
            Lines.Enqueue(new ConsoleLoggerMessage(DateTime.Now, color, message, true));
            NewMessageReceived.Set();
        }
    }
    public void WriteException(string message) => WriteLine(message, ConsoleColor.Red);
    public void WriteException(Exception ex) => WriteException("Exception is thrown:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

    public void Dispose()
    {
        KillSwitch = true;
        if (LoggerThread.ThreadState == System.Threading.ThreadState.Running && Thread.CurrentThread != LoggerThread)
            LoggerThread.Join();
    }
}
