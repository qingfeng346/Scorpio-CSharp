using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using Scorpio;
using Scorpio.Serialize;

namespace ScorpioExec
{
    public class Program
    {
        public static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string CurrentDirectory = Environment.CurrentDirectory;
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
        static void Register() {
            var p = new List<string>(Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User).Split(';'));
            if (!p.Contains(BaseDirectory)) {
                p.Add(BaseDirectory);
                Environment.SetEnvironmentVariable("Path", string.Join(";", p.ToArray()), EnvironmentVariableTarget.User);
            }
            Console.WriteLine("path is already existed");
        }
        static byte[] GetFileBuffer(String fileName) {
            FileStream stream = File.OpenRead(fileName);
            long length = stream.Length;
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();
            stream.Dispose();
            return buffer;
        }
        static void Pack(string source, string output) {
            source = Path.Combine(CurrentDirectory, source);
            output = Path.Combine(CurrentDirectory, output);
            try {
                byte[] buffer = GetFileBuffer(source);
                File.WriteAllBytes(output, ScorpioMaker.Serialize(source, Encoding.UTF8.GetString(buffer, 0, buffer.Length)));
            } catch (System.Exception ex) {
                Console.WriteLine("转换出错 error : " + ex.ToString());	
            }
        }
        static void Unpack(string source, string output) {
            source = Path.Combine(CurrentDirectory, source);
            output = Path.Combine(CurrentDirectory, output);
            try {
                byte[] buffer = GetFileBuffer(source);
                File.WriteAllBytes(output, Encoding.UTF8.GetBytes(ScorpioMaker.DeserializeToString(buffer)));
            } catch (System.Exception ex) {
                Console.WriteLine("转换出错 error : " + ex.ToString());	
            }
        }
        static void Execute(string[] args) {
            script = new Script();
            script.LoadLibrary();
            script.PushAssembly(typeof(Program).Assembly);
            Console.WriteLine("os version : " + Environment.OSVersion.ToString());
            Console.WriteLine("sco version : " + Script.Version);
            Console.WriteLine("app path is : " + BaseDirectory);
            Console.WriteLine("environment path is : " + CurrentDirectory);
            LoadLibrary(Path.Combine(CurrentDirectory, "dll"));
            if (args.Length >= 1) {
                try {
                    string file = Path.Combine(CurrentDirectory, args[0]);
                    Stopwatch watch = Stopwatch.StartNew();
                    script.PushSearchPath(CurrentDirectory);
                    script.SetObject("__PATH__", CurrentDirectory);
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
        static void Main(string[] args) {
            CommandLine command = new CommandLine(args);
            string type = command.Get("-t").ToString();
            if (type == "register") {
                Register();
            } else if (type == "pack" || type == "unpack") {
                string source = command.Get("-s").ToString();
                string output = command.Get("-o").ToString();
                if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(output)) {
                    Console.WriteLine("参数出错 -s [源文件] -o [输出文件] 是必须参数");
                    return;
                }
                if (type == "pack")
                    Pack(source, output);
                else
                    Unpack(source, output);
            } else {
                Execute(args);
            }
        }
    }
}
