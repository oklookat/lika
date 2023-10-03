using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lika
{
    internal class Render
    {
        public static void Str(string str)
        {
            Console.WriteLine(str);
        }

        public static void Err(string err)
        {
            Console.WriteLine($"ERROR: {err}");
        }

        public static void Idle()
        {
            Str("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
