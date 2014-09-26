using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using Scorpio;
using System.Reflection;
namespace ScorpioExec
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Script script = new Script();
            try {
                if (args.Length >= 1) {
                    double start = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds;
                    script.LoadLibrary();
                    script.SetObject("Environment", typeof(Environment));
                    Console.WriteLine("返回值为:" + script.LoadFile(args[0]));
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
