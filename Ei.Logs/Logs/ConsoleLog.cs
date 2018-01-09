using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ei.Logs
{
    public class ConsoleLog : ILog
    {
        public void Log(ILogMessage message)
        {
            //Console.WriteLine("[LOG " + Thread.CurrentThread.ManagedThreadId + "] " + message.Code + " " + string.Join(";", message.Parameters));
            Console.WriteLine((string.IsNullOrEmpty(message.Code) ? message.Message : (message.Code + message.Parameters)));
        }
    }
}
