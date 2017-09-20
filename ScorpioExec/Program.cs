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
        private static Script script;
        private class print : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                var stackInfo = script.GetCurrentStackInfo();
                var prefix = stackInfo.Breviary + ":" + stackInfo.Line + " : ";
                string str = "";
                for (int i = 0; i < args.Length; ++i) {
                    str += args[i].ToString() + " ";
                }
                Console.WriteLine(prefix + str);
                return null;
            }
        }
        public static Assembly CompilerFile(string path) {
#if !SCORPIO_NET_CORE
            CSharpCodeProvider Provider = new CSharpCodeProvider();
            CompilerParameters Parameters = new CompilerParameters();
            Parameters.ReferencedAssemblies.Add("System.dll");
            Parameters.GenerateExecutable = false;
            Parameters.GenerateInMemory = true;
            string[] fileNames = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
            CompilerResults cr = Provider.CompileAssemblyFromFile(Parameters, fileNames);
            if (cr.Errors.HasErrors) {
                string str = "cs file compiler error : \n";
                foreach (CompilerError err in cr.Errors) {
                    str += (err.ToString() + "\n");
                }
                throw new Exception(str);
            }
            return cr.CompiledAssembly;
#else
            return null;
#endif
        }
        private static void LoadLibrary(string path) {
#if !SCORPIO_NET_CORE
            if (!Directory.Exists(path)) { return; }
            string[] files = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            foreach (var file in files) {
                try {
                    script.PushAssembly(Assembly.LoadFile(file));
                    Console.WriteLine("load dll file [" + file + "] success");
                } catch (System.Exception ex) {
                    Console.WriteLine("load dll file [" + file + "] fail : " + ex.ToString());
                }
            }
#endif
        }
        private static void LoadFiles(string path) {
            if (!Directory.Exists(path)) { return; }
            try {
                script.PushAssembly(CompilerFile(path));
                Console.WriteLine("compiler path [" + path + "] success");
            } catch (System.Exception ex) {
                Console.WriteLine("compiler path [" + path + "] fail : " + ex.ToString());
            }
        }
        static void Main(string[] args) {
            script = new Script();
            script.LoadLibrary();
            script.PushAssembly(typeof(Program).GetTypeInfo().Assembly);
#if !SCORPIO_NET_CORE
            string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
#else
            string CurrentDirectory = "";
#endif
            Console.WriteLine("the current version : " + Script.Version);
            Console.WriteLine("app path is : " + CurrentDirectory);
            LoadLibrary(Path.Combine(CurrentDirectory, "dll"));
            LoadFiles(Path.Combine(CurrentDirectory, "cs"));
            if (args.Length >= 1) {
                try {
                    string file = Path.GetFullPath(args[0]);
                    string path = Path.GetDirectoryName(file);
                    LoadLibrary(Path.Combine(path, "dll"));
                    LoadFiles(Path.Combine(path, "cs"));
                    Stopwatch watch = Stopwatch.StartNew();
                    script.PushSearchPath(CurrentDirectory);
                    script.PushSearchPath(path);
                    script.SetObject("__PATH__", path);
                    script.SetObject("print", script.CreateFunction(new print()));
                    LibraryIO.Load(script);
                    Console.WriteLine("=============================");
                    ScriptObject value = script.LoadFile(file);
                    Console.WriteLine("=============================");
                    Console.WriteLine("return value : " + value);
                    Console.WriteLine("the execution time : " + watch.ElapsedMilliseconds + " ms");
                } catch (System.Exception ex) {
                    Console.WriteLine(script.GetStackInfo());
                    Console.WriteLine(ex.ToString());
                }
            } else {
                while (true) {
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
