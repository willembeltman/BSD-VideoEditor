

namespace VideoEditorD3D.Loggers
{
    public interface ILogger : IDisposable
    {
        void WriteLine(string message, ConsoleColor color);
        void ReWriteLine(string message);
        void ReWriteLine(string message, ConsoleColor color);
        void WriteException(Exception ex);
        void WriteException(string message);
        void StartThread();
    }
}