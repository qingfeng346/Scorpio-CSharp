using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using Scorpio;
using ScorpioLibrary;

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
            if (Environment.OSVersion.ToString().ToLower().Contains("windows")) {
                var p = new List<string>(Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User).Split(';'));
                if (!p.Contains(BaseDirectory)) {
                    p.Add(BaseDirectory);
                    Environment.SetEnvironmentVariable("Path", string.Join(";", p.ToArray()), EnvironmentVariableTarget.User);
                } else {
                    Console.WriteLine("path is already existed");
                }
            } else {
                ProcessStartInfo info = new ProcessStartInfo("ln");
                info.Arguments = "-s " + BaseDirectory + "sco /usr/bin/";
                info.CreateNoWindow = false;
                info.ErrorDialog = true;
                info.UseShellExecute = true;
                info.RedirectStandardOutput = false;
                info.RedirectStandardError = false;
                info.RedirectStandardInput = false;
                Process process = Process.Start(info);
                process.WaitForExit();
                process.Close();
            }
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
                //File.WriteAllBytes(output, ScorpioMaker.Serialize(source, Encoding.UTF8.GetString(buffer, 0, buffer.Length)));
            } catch (System.Exception ex) {
                Console.WriteLine("转换出错 error : " + ex.ToString());	
            }
        }
        static void Unpack(string source, string output) {
            source = Path.Combine(CurrentDirectory, source);
            output = Path.Combine(CurrentDirectory, output);
            try {
                byte[] buffer = GetFileBuffer(source);
                //File.WriteAllBytes(output, Encoding.UTF8.GetBytes(ScorpioMaker.DeserializeToString(buffer)));
            } catch (System.Exception ex) {
                Console.WriteLine("转换出错 error : " + ex.ToString());	
            }
        }
        static void Execute(string[] args) {
            script = new Script();
            script.LoadLibrary();
            LibraryIO.Load(script);
            script.PushAssembly(typeof(Program).Assembly);
            Console.WriteLine("os version : " + Environment.OSVersion.ToString());
            Console.WriteLine("sco version : " + Scorpio.Version.version);
            Console.WriteLine("build date : " + Scorpio.Version.date);
            Console.WriteLine("app path is : " + BaseDirectory);
            Console.WriteLine("environment path is : " + CurrentDirectory);
            LoadLibrary(Path.Combine(CurrentDirectory, "dll"));
            if (args.Length >= 1) {
                try {
                    string file = Path.Combine(CurrentDirectory, args[0]);
                    string path = Path.GetDirectoryName(file);
                    Stopwatch watch = Stopwatch.StartNew();
                    script.PushSearchPath(path);
                    script.PushSearchPath(CurrentDirectory);
                    //script.SetObject("__PATH__", path);
                    Console.WriteLine("=============================");
                    var value = script.LoadFile(file);
                    Console.WriteLine("=============================");
                    Console.WriteLine("return value : " + value);
                    Console.WriteLine("the execution time : " + watch.ElapsedMilliseconds + " ms");
                } catch (System.Exception ex) {
                    //Console.WriteLine(script.GetStackInfo());
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
                            Console.WriteLine($@"version : {Scorpio.Version.version}
build date : {Scorpio.Version.date}");
                        } else {
                            script.LoadString(str);
                        }
                    } catch (System.Exception ex) {
                        //Console.WriteLine(script.GetStackInfo());
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }
        static void Main(string[] args) {
            try {
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
                //var types = new Type[] { typeof(TestClass) };
                //var generate = new Scorpio.ScorpioReflect.GenerateScorpioClass(typeof(TestClass));
                ////generate.SetClassFilter(new Filter());
                //File.WriteAllBytes(@"E:\Scorpio-CSharp\ScorpioExec\ScorpioTestClass.cs", Encoding.UTF8.GetBytes(generate.Generate()));
                //FileUtil.CreateFile(EditorUtil.ScriptBuildPath + generate.ScorpioClassName + ".cs", generate.Generate());

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
