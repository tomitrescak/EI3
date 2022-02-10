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
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.Write($"[{message.Source.PadLeft(22, ' ')}] ");

            Console.ForegroundColor = color;

            //Console.WriteLine("[LOG " + Thread.CurrentThread.ManagedThreadId + "] " + message.Code + " " + string.Join(";", message.Parameters));
            Console.Write($"{message.Code} {message.Message} {message.Parameters}");

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();
        }
    }
}
