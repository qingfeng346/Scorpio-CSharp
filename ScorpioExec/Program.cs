using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Scorpio.Commons;
using Scorpio;
using Scorpio.Userdata;
using Scorpio.Serialize;
using Scorpio.ScorpioReflect;

namespace ScorpioExec {
    public class LogHelper : ILogger {
        public void info(string value) {
            Console.WriteLine(value);
            Debugger.Log(0, null, value + "\n");
        }
        public void warn(string value) {
            value = "[warn]" + value;
            Console.WriteLine(value);
            Debugger.Log(0, null, value + "\n");
        }
        public void error(string value) {
            value = "[error]" + value;
            Console.WriteLine(value);
            Debugger.Log(0, null, value + "\n");
        }
    }
    public static class TestClass {
        public static void TestFunc(ref int aaa, out string bbb, int ccc, ref Vec vec) {
            aaa = 200;
            bbb = "123123123";
            vec = new Vec();
        }
        public static int TestFunc(ref int aaa, out string bbb, string ccc, ref Vec vec) {
            aaa = 200;
            bbb = "123123123";
            vec = new Vec();
            return 200;
        }
        public static void Ex(this Vec v, int a, int b) {
            Console.WriteLine(v.x + a+ b);
        }
    }
    public struct Vec {
        public int x;
    }
    public class Program {
        public static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string CurrentDirectory = Environment.CurrentDirectory;
        private static Script script;
        static void Main(string[] args) {
            try {
                var generate = new GenerateScorpioClass(typeof(TestClass));
                FileUtil.CreateFile("/Users/while/Scorpio-CSharp/ScorpioExec/" + generate.ScorpioClassName + ".cs", generate.Generate());
                Logger.SetLogger(new LogHelper());
                Scorpio.Commons.Util.PrintSystemInfo();
                Logger.info("Sco Version : " + Scorpio.Version.version);
                Logger.info("Build Date : " + Scorpio.Version.date);
                var command = CommandLine.Parse(args);
                var type = command.GetValue("-type", "-t");
                var source = command.GetValue("-source", "-s");
                var output = command.GetValue("-output", "-o");
                if (type == null || type.Length == 0) { type = command.Type; }
                if (type == null) { type = ""; }
                switch (type.ToLower()) {
                    case "register": Register(); return;            //注册sco到运行环境
                    case "pack": Pack(source, output); return;      //生成sco IL
                    case "fast": Fast(command, output); return;     //生成快速反射类
                    default: Execute(args); return;
                }
            } catch (Exception e) {
                Logger.error(e.ToString());
            }
        }
        static void LoadLibrary(string path) {
            if (!Directory.Exists(path)) { return; }
            string[] files = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            foreach (var file in files) {
                try {
                    TypeManager.PushAssembly(Assembly.LoadFile(file));
                    Logger.info("load dll file [" + file + "] success");
                } catch (System.Exception ex) {
                    Logger.error("load dll file [" + file + "] fail : " + ex.ToString());
                }
            }
        }
        static void Register() {
            Scorpio.Commons.Util.RegisterApplication(Scorpio.Commons.Util.BaseDirectory + "/sco");
        }
        static void Pack(string source, string output) {
            if (string.IsNullOrWhiteSpace(source)) { throw new Exception("找不到 -source 参数"); }
            if (string.IsNullOrWhiteSpace(output)) { throw new Exception("找不到 -output 参数"); }
            source = Path.Combine(Util.CurrentDirectory, source);
            output = Path.Combine(Util.CurrentDirectory, output);
            try {
                File.WriteAllBytes(output, SerializeUtil.Serialize(source, FileUtil.GetFileString(source)).ToArray());
            } catch (System.Exception ex) {
                Logger.error("转换出错 error : " + ex.ToString());
            }
        }
        static void Fast(CommandLine command, string output) {
            if (string.IsNullOrWhiteSpace(output)) { throw new Exception("找不到 -output 参数"); }
            output = Path.Combine(Util.CurrentDirectory, output);
            var dll = command.GetValue("-dll");
            var assembly = string.IsNullOrEmpty(dll) ? null : Assembly.LoadFile(Path.Combine(CurrentDirectory, dll));
            var className = command.GetValue("-class");
            if (string.IsNullOrWhiteSpace(className)) { throw new Exception("找不到 -class 参数"); }
            var clazz = assembly != null ? assembly.GetType(className, false, false) : null;
            if (clazz == null) { clazz = Type.GetType(className, false, false); }
            if (clazz == null) { throw new Exception("找不到class,请输入完整类型或检查类名是否正确 : " + className); }
            var generate = new GenerateScorpioClass(clazz);
            var filterName = command.GetValue("-filter");
            if (!string.IsNullOrWhiteSpace(filterName)) {
                var filterType = assembly != null ? assembly.GetType(filterName, false, false) : null;
                if (filterType == null) { filterType = Type.GetType(filterName, false, false); }
                if (filterType != null && filterType.IsSubclassOf(typeof(ClassFilter))) { generate.SetClassFilter((ClassFilter)System.Activator.CreateInstance(filterType)); }
            }
            FileUtil.CreateFile(Path.Combine(output, generate.ScorpioClassName + ".cs"), generate.Generate());
        }
        static void Execute(string[] args) {
            TypeManager.PushAssembly(typeof(Program).Assembly);
            script = new Script();
            script.LoadLibraryV1();
            LoadLibrary(Path.Combine(CurrentDirectory, "dll"));
            if (args.Length >= 1) {
                try {
                    string file = Path.Combine(CurrentDirectory, args[0]);
                    string path = Path.GetDirectoryName(file);
                    Stopwatch watch = Stopwatch.StartNew();
                    script.PushSearchPath(path);
                    script.PushSearchPath(CurrentDirectory);
                    Logger.info("=============================");
                    ScriptValue value = script.LoadFile(file);
                    Logger.info("=============================");
                    Logger.info("return value : " + value);
                    Logger.info("the execution time : " + watch.ElapsedMilliseconds + " ms");
                } catch (System.Exception ex) {
                    Logger.error(ex.ToString());
                }
            } else {
                while (true) {
                    try {
                        string str = Console.ReadLine();
                        if (str == "exit") {
                            break;
                        } else if (str == "clear") {
                            Console.Clear();
                        } else if (str == "version") {
                            Logger.info($@"version : {Scorpio.Version.version}
        build date : {Scorpio.Version.date}");
                        } else {
                            script.LoadString(str);
                        }
                    } catch (System.Exception ex) {
                        Logger.error(ex.ToString());
                    }
                }
            }
        }

    }
}
