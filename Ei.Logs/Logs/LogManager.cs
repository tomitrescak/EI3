using System;
using System.Collections.Generic;

namespace Ei.Logs
{
    public static class Log
    {
        public enum Level
        {
            Debug,
            Info,
            Error
        }

        static Log()
        {
            logs = new List<ILog>();
            LogLevel = Level.Debug;
        }

        public static Level LogLevel { get; set; }

        static List<ILog> logs;

        public static bool IsDebug { get { return LogLevel == 0; } }

        public static void Register(ILog log)
        {
            logs.Add(log);
        }

        public static void Debug(string message)
        {
            LogMessage(0, message);
        }

        public static void Debug(string source, string message)
        {
            LogMessage(0, message, source);
        }

        public static void Info(string message)
        {
            LogMessage(1, message);
        }

        public static void Info(string source, Enum code, params string[] parameters)
        {
            LogMessage(1, source, code.ToString(), parameters);
        }

        public static void Info(string source, string message)
        {
            LogMessage(1, message, source);
        }

        //        public static void Info(Enum code, params string[] parameters)
        //        {
        //            LogMessage(1, code.ToString(), parameters);
        //        }

        public static void Warning(string message)
        {
            LogMessage(2, message);
        }

        public static void Warning(string source, string message)
        {
            LogMessage(2, message, source);
        }

        public static void Error(string message)
        {
            LogMessage(3, message);
        }

        public static void Error(string source, string message)
        {
            LogMessage(3, message, source);
        }

        private static void LogMessage(int level, string message, string source = null)
        {
            if (level < (int) LogLevel) return; // ignore messages below specified log level

            foreach (var log in logs)
            {
                log.Log(new LogMessage
                {
                    Message = message,
                    Source = source
                });
            }
        }

        private static void LogMessage(int level, string source, string code, params string[] parameters)
        {
            if (level < (int)LogLevel) return; // ignore messages below specified log level

            foreach (var log in logs)
            {
                log.Log(new LogMessage
                {
                    Code = code,
                    Parameters = parameters,
                    Source = source
                });
            }
        }

    }
}
