using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Ei.Logs
{
    public static class Log
    {
        public enum Level
        {
            Debug,
            Info,
            Warning,
            Error,
            Off
        }

        static Log()
        {
            logs = new List<ILog>();
            LogLevel = Level.Debug;
        }

        public static Level LogLevel { get; set; }

        static List<ILog> logs;

        public static bool IsDebug { get { return LogLevel == Level.Debug; } }

        public static bool IsInfo { get { return LogLevel <= Level.Info; } }

        public static void Register(ILog log)
        {
            logs.Add(log);
        }

        //public static void Debug(string message)
        //{
        //    LogMessage(0, message);
        //}

        public static void Debug(string agent, string component, string message)
        {
            LogMessage(0, message, agent, component);
        }

        //public static void Info(string message)
        //{
        //    LogMessage(1, message);
        //}

//        public static void Info(string source, Enum code, params string[] parameters)
//        {
//            LogMessage(1, source, code.ToString(), parameters);
//        }

        public static void Info(string agent, string component, string message)
        {
            LogMessage(1, message, agent, component);
        }

        //        public static void Info(Enum code, params string[] parameters)
        //        {
        //            LogMessage(1, code.ToString(), parameters);
        //        }

        //public static void Warning(string message)
        //{
        //    LogMessage(2, message);
        //}

        public static void Warning(string agent, string component, string message)
        {
            LogMessage(2, message, agent, component);
        }

        //public static void Error(string message)
        //{
        //    LogMessage(3, message);
        //}

        public static void Error(string agent, string component, string message)
        {
            LogMessage(3, message, agent, component);
        }

        public static void Success(string agent, string component, string message)
        {
            LogMessage(4, message, agent, component);
        }

        private static void LogMessage(int level, string message, string agent = null, string component = null)
        {
            // return;
            if (level < (int) LogLevel) return; // ignore messages below specified log level

            foreach (var log in logs)
            {
                log.Log(new LogMessage
                {
                    Message = message,
                    Agent = agent,
                    Component = component,
                    Level = level
                });
            }

#if DEBUG
            System.Diagnostics.Debug.WriteLine(agent + "/" + component + " :" + message);
#endif

        }

        private static void LogMessage(int level, string agent, string component, string code, params string[] parameters)
        {
            // return;
            if (level < (int)LogLevel) return; // ignore messages below specified log level

            foreach (var log in logs)
            {
                log.Log(new LogMessage
                {
                    Code = code,
                    Parameters = parameters,
                    Agent = agent,
                    Component = component
                });
            }
        }

    }
}
