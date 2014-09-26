using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using Scorpio;
namespace ScorpioExec
{
    class Program
    {
        static void Main(string[] args)
        {
            double start = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds;
            Script script = new Script();
            try {
                if (args.Length >= 1) {
                    string str = File.ReadAllText(args[0]);
                    script.LoadLibrary();
                    script.SetObject("Environment", typeof(Environment));
                    Console.WriteLine("返回值为:" + script.LoadString("", str));
                    Console.WriteLine("运行时间:" + (Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds - start) + " ms");
                }
            } catch (System.Exception ex) {
                Console.WriteLine(script.GetStackInfo());
                Console.WriteLine(ex.ToString());
            }
            Console.ReadKey();
        }
    }
}
