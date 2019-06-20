using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Scorpio.Commons;
using Scorpio;
using Scorpio.Userdata;
using Scorpio.Serialize;
using Scorpio.ScorpioReflect;
using System.Net;
using System.Collections.Generic;

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
    public class Program {
        //private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        public static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string CurrentDirectory = Environment.CurrentDirectory;
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
    [文件路径]      运行sco文本文件或IL文件";
        private static Script script;
        static void Main(string[] args) {
            try {
                Logger.SetLogger(new LogHelper());
                var command = CommandLine.Parse(args);
                var type = command.GetValue("-type", "-t");
                var source = command.GetValue("-source", "-s");
                var output = command.GetValue("-output", "-o");
                var hasHelp = command.HadValue("-help", "--help", "-h");
                if (type == null || type.Length == 0) { type = command.Type ?? ""; }
                if (hasHelp) {
                    switch (type.ToLower()) {
                        case "register": Logger.info(HelpRegister); return;
                        case "pack": Logger.info(HelpPack); return;
                        case "fast": Logger.info(HelpFast); return;
                        //case "version": Logger.info(HelpVersion); return;
                        default: Logger.info(HelpExecute); return;
                    }
                } else {
                    switch (type.ToLower()) {
                        case "register": Register(); return;            //注册sco到运行环境
                        case "pack": Pack(source, output); return;      //生成sco IL
                        case "fast": Fast(command, output); return;     //生成快速反射类
                        //case "version": Version(command); return;       //
                        default: Execute(args); return;
                    }
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
            source = Path.GetFullPath(Path.Combine(Util.CurrentDirectory, source));
            output = Path.GetFullPath(Path.Combine(Util.CurrentDirectory, output));
            try {
                File.WriteAllBytes(output, SerializeUtil.Serialize(source, FileUtil.GetFileString(source)).ToArray());
                Logger.info($"生成IL文件  {source} -> {output}");
            } catch (System.Exception ex) {
                Logger.error("转换出错 error : " + ex.ToString());
            }
        }
        static void Fast(CommandLine command, string output) {
            if (string.IsNullOrWhiteSpace(output)) { throw new Exception("找不到 -output 参数"); }
            output = Path.GetFullPath(Path.Combine(Util.CurrentDirectory, output));
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
            var outputFile = Path.Combine(output, generate.ScorpioClassName + ".cs");
            FileUtil.CreateFile(outputFile, generate.Generate());
            Logger.info($"生成快速反射类 {className} -> {outputFile}");
        }
//        static void Version(CommandLine command) {
//            Logger.info($@"Sco Version : {Scorpio.Version.version}
//Build Date : {Scorpio.Version.date}");
//            var result = Request("http://api.github.com/repos/qingfeng346/Scorpio-CSharp/releases", (request) => {
//                request.Headers.Add("Authorization", "token c9fab45cde8bb710244d791018fefd6f4c6a80b5");
//            });
//            if (string.IsNullOrEmpty(result)) { return; }
//            var isPreview = command.HadValue("-preview", "-p");
//            var datas = new JsonParser(result, true).Parse() as List<object>;
//            foreach (Dictionary<string, object> data in datas) {
//                var name = data["name"] as string;
//                if (name != Version)
//                var prerelease = (bool)data["prerelease"];
//                if (isPreview) {
//                    Logger.info("发现新版本 : " +);
//                    Process.Start(data["html_url"])
//                }
//                if ()
//            }
//        }
        //private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) {
        //    return true; //总是接受
        //}
        //public static string Request(string url, Action<HttpWebRequest> postRequest) {
        //    try {
        //        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
        //        var request = (HttpWebRequest)HttpWebRequest.Create(url);
        //        request.Method = "GET";
        //        request.ProtocolVersion = HttpVersion.Version10;
        //        request.UserAgent = DefaultUserAgent;
        //        request.Credentials = CredentialCache.DefaultCredentials;
        //        if (postRequest != null) postRequest(request);
        //        using (var response = request.GetResponse()) {
        //            using (var stream = response.GetResponseStream()) {
        //                var bytes = new byte[8192];
        //                using (var memoryStream = new MemoryStream()) {
        //                    while (true) {
        //                        var readSize = stream.Read(bytes, 0, 8192);
        //                        if (readSize <= 0) { break; }
        //                        memoryStream.Write(bytes, 0, readSize);
        //                    }
        //                    return System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
        //                }
        //            }
        //        }
        //    } catch (Exception e) {
        //        Logger.error("Request is Error : {0}", e.Message);
        //    }
        //    return null;
        //}
        static void Execute(string[] args) {
            Scorpio.Commons.Util.PrintSystemInfo();
            Logger.info("Sco Version : " + Scorpio.Version.version);
            Logger.info("Build Date : " + Scorpio.Version.date);
            Logger.info("Application Name : " + AppDomain.CurrentDomain.FriendlyName);
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
