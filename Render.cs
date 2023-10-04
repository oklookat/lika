using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lika
{
    internal class Render
    {
        public static void Str(string str, ConsoleColor? backgroundColor = null, ConsoleColor? foregroundColor = null)
        {
            if (backgroundColor != null)
            {
                Console.BackgroundColor = (ConsoleColor)backgroundColor;
            }
            if (foregroundColor != null)
            {
                Console.ForegroundColor = (ConsoleColor)foregroundColor;
            }
            Console.WriteLine(str);
            ResetColor();
        }

        public static void StrWhiteBlack(string str)
        {
            Str(str, ConsoleColor.White, ConsoleColor.Black);
        }

        public static void Err(string err)
        {
            Str(err, ConsoleColor.DarkRed, ConsoleColor.White);
        }

        public static void Idle()
        {
            Str("Press any key to exit.");
            Console.ReadKey();
        }

        private static void ResetColor()
        {
            Console.ResetColor();
        }
        
    }
}
