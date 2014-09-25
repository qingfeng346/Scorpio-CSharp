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
            int start = Environment.TickCount;
            Script script = new Script();
            try {
                if (args.Length >= 1) {
                    string str = File.ReadAllText(args[0]);
                    script.LoadLibrary();
                    script.SetObject("Environment", typeof(Environment));
                    Console.WriteLine("返回值为:" + script.LoadString("", str));
                    Console.WriteLine("运行时间:" + (Environment.TickCount - start) + " ms");
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
