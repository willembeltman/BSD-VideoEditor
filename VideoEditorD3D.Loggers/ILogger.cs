

namespace VideoEditorD3D.Loggers
{
    public interface ILogger
    {
        void WriteLine(string line);
        void WriteLine(string message, ConsoleColor color);
        void RewriteLine(string message);
        void RewriteLine(string message, ConsoleColor color);
        void WriteException(Exception ex);
        void WriteException(string message);
    }
}