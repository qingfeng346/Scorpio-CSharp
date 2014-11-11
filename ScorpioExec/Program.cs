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
            Console.WriteLine("开始执行，当前版本:" + Script.Version);
            script.LoadLibrary();
            script.PushAssembly(typeof(Program).Assembly);
            if (args.Length >= 1) {
                try {
                    Stopwatch watch = Stopwatch.StartNew();
                    Console.WriteLine("返回值为:" + script.LoadFile(args[0]));
                    Console.WriteLine("运行时间:" + watch.ElapsedMilliseconds + " ms");
                } catch (System.Exception ex) {
                    Console.WriteLine(script.GetStackInfo());
                    Console.WriteLine(ex.ToString());
                }
                Console.ReadKey();
            } else {
                while (true)
                {
                    try {
                        string str = Console.ReadLine();
                        if (str == "exit")  { 
                            break;
                        } else if (str == "clear") {
                            Console.Clear();
                        } else if (str == "version") {
                            Console.WriteLine(Script.Version);
                        } else {
                            script.LoadString(str);
                        }
                    } catch (System.Exception ex) {
                        Console.WriteLine(script.GetStackInfo());
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }
    }
}
