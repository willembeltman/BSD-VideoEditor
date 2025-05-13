using System.Collections.Concurrent;
using System.Diagnostics;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Loggers;

public class ConsoleLogger : ILogger
{
    private readonly AutoResetEvent NewMessageReceived;
    private readonly Thread LoggerThread;
    private readonly ConcurrentQueue<ConsoleLoggerMessage> Lines;
    private int CurrentIndex = 0;
    private string LastMessage = "";
    private bool NeedNewLine = false;
    private bool KillSwitch = false;
    public Stopwatch Stopwatch;

    public ConsoleLogger()
    {
        Lines = new ConcurrentQueue<ConsoleLoggerMessage>();
        NewMessageReceived = new AutoResetEvent(false);
        LoggerThread = new Thread(Kernel)
        {
            Name = "ConsoleLogger Kernel"
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
            if (Console.ForegroundColor != message.Color)
                Console.ForegroundColor = message.Color;
            if (message.Replace)
            {
                if (NeedNewLine)
                {
                    Console.CursorLeft = 0;
                    Console.Write(new string(' ', LastMessage.Length));
                    Console.CursorLeft = 0;
                    Console.Write(message.Message);
                }
                else
                {
                    Console.WriteLine(message.Message);
                }
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
    public void ReWriteLine(string message) => ReWriteLine(message, ConsoleColor.White);
    public void ReWriteLine(string message, ConsoleColor color)
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

        GC.SuppressFinalize(this);
    }
}
