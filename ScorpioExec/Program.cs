using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scorpio;
namespace ScorpioExec
{
    class Program
    {
        static void Main(string[] args)
        {
            Script script = new Script();
            try {
                if (args.Length >= 1) {
                    string str = File.ReadAllText(args[0]);
                    script.LoadLibrary();
                    Console.WriteLine("返回值为:" + script.LoadString("", str));
                }
            } catch (System.Exception ex) {
                Console.WriteLine(script.GetStackInfo());
                Console.WriteLine(ex.ToString());
            }
            Console.ReadKey();
        }
        private static object time(object[] Parameters)
        {
            return Environment.TickCount;
        }
    }
}
