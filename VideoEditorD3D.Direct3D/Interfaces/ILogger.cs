

namespace VideoEditorD3D.Direct3D.Interfaces
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