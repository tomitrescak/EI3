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
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            

            Console.Write($"[{message.Source.PadLeft(22, ' ')}] ");

            Console.ForegroundColor = color;
            if (message.Level == 0)
            {
                // Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }
            else if (message.Level == 2)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Yellow;
            }
            else if (message.Level == 3)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkRed;
            }
            else if (message.Level == 4)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkGreen;
            }



            //Console.WriteLine("[LOG " + Thread.CurrentThread.ManagedThreadId + "] " + message.Code + " " + string.Join(";", message.Parameters));
            Console.Write($"{message.Code} {message.Message} {message.Parameters}");

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();
        }
    }
}
