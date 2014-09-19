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
            try {
                if (args.Length >= 1) {
                    string str = File.ReadAllText(args[0]);
                    Script script = new Script();
                    script.LoadLibrary();
                    script.RegisterFunction("time", time);
                    Console.WriteLine("返回值为:" + script.LoadString("", str));
                }
            } catch (System.Exception ex) {
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
