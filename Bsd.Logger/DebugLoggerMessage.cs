

namespace Bsd.Logger
{
    public class DebugLoggerMessage
    {
        public DebugLoggerMessage(DateTime now, string message)
        {
            Now = now;
            Message = message;
        }

        public DateTime Now { get; }
        public string Message { get; }
    }
}