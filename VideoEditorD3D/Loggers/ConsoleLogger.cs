using VideoEditorD3D.Interfaces;

namespace VideoEditorD3D.Loggers
{
    public class ConsoleLogger
    {
        public ConsoleLogger(IApplication application)
        {
            Application = application;
        }

        public IApplication Application { get; }

        public void WriteException(Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}