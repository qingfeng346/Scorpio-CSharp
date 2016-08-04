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
        public static Assembly CompilerFile(string path) {
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
        }
        private static Script script;
        private static void LoadLibrary(string path) {
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
            Console.WriteLine("the current version : " + Script.Version);
            script.LoadLibrary();
            script.PushAssembly(typeof(Program).Assembly);
            LoadLibrary(CurrentDirectory + "/Library");
            LoadFiles(CurrentDirectory + "/Program");
            if (args.Length >= 1) {
                try {
                    string file = args[0];
                    string path = Path.GetDirectoryName(file);
                    LoadLibrary(path + "/Library");
                    LoadFiles(path + "/Program");
                    Stopwatch watch = Stopwatch.StartNew();
                    script.PushSearchPath(CurrentDirectory);
                    script.PushSearchPath(path);
                    script.SetObject("__PATH__", path);
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
