using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using Scorpio;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
namespace ScorpioExec
{
    public class Program
    {
        public static string CurrentDirectory { get { return AppDomain.CurrentDomain.BaseDirectory; } }
        public static Assembly CompilerFile(string path)
        {
            CSharpCodeProvider Provider = new CSharpCodeProvider();
            CompilerParameters Parameters = new CompilerParameters();
            Parameters.ReferencedAssemblies.Add("System.dll");
            Parameters.GenerateExecutable = false;
            Parameters.GenerateInMemory = true;
            string[] fileNames = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
            CompilerResults cr = Provider.CompileAssemblyFromFile(Parameters, fileNames);
            if (cr.Errors.HasErrors) {
                string str = "cs文件编译错误: \n";
                foreach (CompilerError err in cr.Errors) {
                    str += (err.ToString() + "\n");
                }
                throw new Exception(str);
            }
            return cr.CompiledAssembly;
        }
        static void Main(string[] args)
        {
            Script script = new Script();
            Console.WriteLine("开始执行，当前版本:" + Script.Version);
            script.LoadLibrary();
            script.PushAssembly(typeof(Program).Assembly);
            if (Directory.Exists(CurrentDirectory + "/Library"))
            {
                string[] files = Directory.GetFiles(CurrentDirectory + "/Library", "*.dll", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    try {
                        script.PushAssembly(Assembly.LoadFile(file));
                        Console.WriteLine("导入文件[" + file + "]成功");
                    } catch (System.Exception ex) {
                        Console.WriteLine("导入文件[" + file + "]失败 " + ex.ToString());
                    }
                }
            }
            if (Directory.Exists(CurrentDirectory + "/Program")) {
                try {
                    script.PushAssembly(CompilerFile(CurrentDirectory + "/Program"));
                } catch (System.Exception ex) {
                    Console.WriteLine("编译文件失败 " + ex.ToString());
                }
            }
            if (args.Length >= 1) {
                try {
                    Stopwatch watch = Stopwatch.StartNew();
                    script.PushSearchPath(CurrentDirectory);
                    script.PushSearchPath(Path.GetDirectoryName(args[0]));
                    script.SetObject("__PATH__", Path.GetDirectoryName(args[0]));
                    LibraryIO.Load(script);
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
