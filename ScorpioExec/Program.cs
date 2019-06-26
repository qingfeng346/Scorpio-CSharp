using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Scorpio.Commons;
using Scorpio.Userdata;
using Scorpio.Serialize;
using Scorpio.ScorpioReflect;
using System.Collections.Generic;

namespace ScorpioExec {
    public class Program {
        private static readonly string CurrentDirectory = Util.CurrentDirectory;
        private static readonly string BaseDirectory = Util.BaseDirectory;
        
        private const string HelpRegister = @"
注册运行程序到环境变量";
        private const string HelpPack = @"
编译生成sco的IL文件
    -source|-s      脚本文本文件
    -output|-o      IL输出文件";
        private const string HelpFast = @"
生成快速反射文件
    -dll            dll文件路径
    -class          class完整名称
    -output|-o      快速反射文件输出目录";
        private const string HelpVersion = @"
查询sco版本，并检查最新版本
    -preview|-p     是否检查preview版本";
        private const string HelpExecute = @"
命令列表
    register        注册运行程序到环境变量
    pack            编译生成sco的IL文件
    fast            生成快速反射文件
    version         查询sco版本，并检查最新版本
    [文件路径]       运行sco文本文件或IL文件";
        static void Main(string[] args) {
            Launch.AddExecute("register", HelpRegister, Register);
            Launch.AddExecute("pack", HelpPack, Pack);
            Launch.AddExecute("fast", HelpFast, Fast);
            Launch.AddExecute("version", HelpVersion, VersionExec);
            Launch.AddExecute("", HelpExecute, Execute);
            Launch.Start(args, null, null);
        }
        static void Register(CommandLine command, string[] args) { 
            Util.RegisterApplication($"{Util.BaseDirectory}/{AppDomain.CurrentDomain.FriendlyName}");
        }
        static void Pack(CommandLine command, string[] args) {
            var source = Launch.GetPath("-source", "-s");
            var output = Launch.GetPath("-output", "-o");
            File.WriteAllBytes(output, SerializeUtil.Serialize(source, FileUtil.GetFileString(source)).ToArray());
            Logger.info($"生成IL文件  {source} -> {output}");
        }
        static void Fast(CommandLine command, string[] args) {
            var output = Launch.GetPath("-output", "-o");
            var dll = command.GetValue("-dll");
            var assembly = dll.isNullOrWhiteSpace() ? null : Assembly.LoadFile(Path.Combine(CurrentDirectory, dll));
            var className = command.GetValue("-class");
            if (className.isNullOrWhiteSpace()) { throw new Exception("找不到 -class 参数"); }
            var clazz = assembly?.GetType(className, false, false);
            if (clazz == null) { clazz = Type.GetType(className, false, false); }
            if (clazz == null) { throw new Exception($"找不到 class, 请输入完整类型或检查类名是否正确 : {className}"); }
            var generate = new GenerateScorpioClass(clazz);
            var filterName = command.GetValue("-filter");
            if (!string.IsNullOrWhiteSpace(filterName)) {
                var filterType = assembly?.GetType(filterName, false, false);
                if (filterType == null) { filterType = Type.GetType(filterName, false, false); }
                if (filterType != null && filterType.IsSubclassOf(typeof(ClassFilter))) { generate.SetClassFilter((ClassFilter)System.Activator.CreateInstance(filterType)); }
            }
            var outputFile = Path.Combine(output, generate.ScorpioClassName + ".cs");
            FileUtil.CreateFile(outputFile, generate.Generate());
            Logger.info($"生成快速反射类 {className} -> {outputFile}");
        }
        static void VersionExec(CommandLine command, string[] args) {
            Logger.info($@"Sco Version : {Scorpio.Version.version}
Build Date : {Scorpio.Version.date}");
            var result = Util.RequestString("http://api.github.com/repos/qingfeng346/Scorpio-CSharp/releases", (request) => {
                request.Headers.Add("Authorization", "token c9fab45cde8bb710244d791018fefd6f4c6a80b5");
            });
            var isPreview = command.HadValue("-preview", "-p");
            var datas = Json.Deserialize(result, true) as List<object>;
            foreach (Dictionary<string, object> data in datas) {
                var name = data["name"] as string;
                if (name.Contains(Scorpio.Version.version)) {
                    Logger.info($"当前已经是最新版本 : {name}");
                    return;
                }
                bool newVersion = false;
                if ((bool)data["prerelease"]) {
                    if (isPreview) { newVersion = true; }
                } else {
                    newVersion = true;
                }
                if (newVersion) {
                    var url = data["html_url"].ToString();
                    Logger.info($"发现新版本 : {name}");
                    Logger.info($"下载地址 : {url}");
                    //Logger.info($"是否立刻去下载  是(Y)√  否(N)");
                    //var str = Console.ReadLine();
                    //if (string.IsNullOrWhiteSpace(str) || str.ToUpper() == "Y") {
                    //    var url = data["html_url"].ToString().Replace("https://", "http://");
                    //    Logger.info("打开网页 : " + url);
                    //}
                    return;
                }
            }
        }
        static void Execute(CommandLine command, string[] args) {
            Util.PrintSystemInfo();
            Logger.info("Sco Version : " + Scorpio.Version.version);
            Logger.info("Build Date : " + Scorpio.Version.date);
            Logger.info("Application Name : " + AppDomain.CurrentDomain.FriendlyName);
            TypeManager.PushAssembly(typeof(Program).Assembly);
            var script = new Scorpio.Script();
            script.LoadLibraryV1();
            LoadLibrary(Path.Combine(CurrentDirectory, "dll"));
            if (args.Length >= 1) {
                var file = Path.Combine(CurrentDirectory, args[0]);
                var path = Path.GetDirectoryName(file);
                script.PushSearchPath(path);
                script.PushSearchPath(CurrentDirectory);
                Logger.info("=============================");
                var watch = Stopwatch.StartNew();
                var value = script.LoadFile(file);
                Logger.info("=============================");
                Logger.info("return value : " + value);
                Logger.info("the execution time : " + watch.ElapsedMilliseconds + " ms");
            } else {
                while (true) {
                    try {
                        string str = Console.ReadLine();
                        if (str == "exit") {
                            break;
                        } else if (str == "clear") {
                            Console.Clear();
                        } else {
                            script.LoadString(str);
                        }
                    } catch (System.Exception ex) {
                        Logger.error(ex.ToString());
                    }
                }
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
    }
}
