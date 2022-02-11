﻿namespace Ei.Logs
{
    public interface ILog
    {
        void Log(ILogMessage message);
    }

    public interface ILogMessage
    {
        int Level { get; set; }
        string Source { get; set; }
        string Code { get; set; }
        string Message { get; set; }
        string[] Parameters { get; set; }
    }
}
