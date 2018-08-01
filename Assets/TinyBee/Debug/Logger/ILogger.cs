namespace TinyBee.Logger
{
    public interface ILogger
    {
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogException(System.Exception exception);
    }
}