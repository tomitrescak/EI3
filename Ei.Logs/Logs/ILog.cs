namespace Ei.Logs
{
    public interface ILog
    {
        void Log(ILogMessage message);
    }

    public interface ILogMessage
    {
        int Level { get; set; }
        string Agent { get; set; }
        string Component { get; set; }
        string Code { get; set; }
        string Message { get; set; }
        string[] Parameters { get; set; }
    }
}
