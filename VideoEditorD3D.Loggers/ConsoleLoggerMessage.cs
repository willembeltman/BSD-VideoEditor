

namespace VideoEditorD3D.Loggers
{
    public class ConsoleLoggerMessage
    {
        public ConsoleLoggerMessage(DateTime now, ConsoleColor color, string message, bool replace)
        {
            Now = now;
            Color = color;
            Message = message;
            Replace = replace;
        }

        public DateTime Now { get; }
        public ConsoleColor Color { get; }
        public string Message { get; }
        public bool Replace { get; }
    }
}