
namespace VideoEditorD3D.Loggers
{
    public interface ILogger
    {
        void WriteException(Exception ex);
        void WriteLine(string line);
    }
}