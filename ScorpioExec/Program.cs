using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Scorpio.Commons;
using Scorpio.ScorpioReflect;
using Scorpio.Serialize;
using Scorpio.Userdata;
using ScorpioLibrary;

namespace ScorpioExec {
    public class Program {
        private static readonly string CurrentDirectory = Util.CurrentDirectory;

        private const string HelpRegister = @"
注册运行程序到环境变量";
        private const string HelpVersion = @"
查询sco版本，并检查最新版本
    --check|-c       (选填)是否检查最新版本
    --preview|-p     (选填)是否检查preview版本";
        private const string HelpPack = @"
编译生成sco的IL文件
    --source|-s      (必填)脚本文本文件
    --output|-o      (必填)IL输出文件
    --ignore|-g      (选填)忽略的全局函数列表,多函数使用分号;隔开";
        private const string HelpFast = @"
生成快速反射文件
    --class|-c       (必填)class完整名称,多class使用分号;隔开
    --output|-o      (必填)快速反射文件输出目录
    --dll|-d         (选填)dll文件路径,不填则找当前程序集
    --filter|-f      (选填)过滤器完整名称,多过滤器使用分号;隔开
    --extension|-e   (选填)扩展函数类完整名称,多扩展函数类使用分号;隔开";
        private const string HelpDelegate = @"
生成Delegate仓库
    --class|-c       (必填)class完整名称,多class使用分号;隔开
    --output|-o      (必填)Delegate仓库输出文件
    --dll|-d         (选填)dll文件路径,不填则找当前程序集";
        private const string HelpInterface = @"
生成Interface类
    --class|-c       (必填)class完整名称,多class使用分号;隔开
    --output|-o      (必填)Interface输出目录
    --dll|-d         (选填)dll文件路径,不填则找当前程序集";
        private const string HelpExecute = @"
命令列表
    register         注册运行程序到环境变量
    pack             编译生成sco的IL文件
    fast             生成快速反射文件
    version          当前sco版本,检查最新版本
    [文件路径]        运行sco文本文件或IL文件";
        private readonly static string[] ParameterSource = new [] { "-s", "--source", "-source" };
        private readonly static string[] ParameterOutput = new [] { "-o", "--output", "-output" };
        private readonly static string[] ParameterClass = new [] { "-c", "--class", "-class" };
        private readonly static string[] ParameterDll = new [] { "-d", "--dll", "-dll" };
        private readonly static string[] ParameterIgnore = new [] { "-i", "--ignore", "-ignore" };
        private readonly static string[] ParameterDefine = new[] { "--define", "-define" };
        private readonly static string[] ParameterFilter = new [] { "-f", "--filter", "-filter" };
        private readonly static string[] ParameterExtension = new [] { "-e", "--extension", "-extension" };
        private readonly static string[] ParameterCheck = new [] { "-c", "--check", "-check" };
        private readonly static string[] ParameterPreview = new [] { "-p", "--preview", "-preview" };
        static Perform perform;
        static void Main (string[] args) {
            perform = new Perform ();
            perform.Help = HelpExecute;
            perform.AddExecute ("register", HelpRegister, Register);
            perform.AddExecute ("version", HelpVersion, VersionExec);
            perform.AddExecute ("pack", HelpPack, Pack);
            perform.AddExecute ("fast", HelpFast, Fast);
            perform.AddExecute ("delegate", HelpDelegate, DelegateFactory);
            perform.AddExecute ("interface", HelpInterface, Interface);
            perform.AddExecute ("", HelpExecute, Execute);
            perform.Start (args, null, null);
        }
        static void Register (CommandLine command, string[] args) {
            Util.RegisterApplication ($"{Util.BaseDirectory}/{AppDomain.CurrentDomain.FriendlyName}");
        }
        static void VersionExec (CommandLine command, string[] args) {
            Logger.info (Scorpio.Version.version);
            if (!command.HadValue (ParameterCheck)) { return; }
            Logger.info ("正在检查最新版本...");
            var result = Util.RequestString ("http://api.github.com/repos/qingfeng346/Scorpio-CSharp/releases", (request) => {
                request.Headers.Add ("Authorization", "token e5ff670eb105f044273f4c81276a67cd1341e649");
            });
            var isPreview = command.HadValue (ParameterPreview);
            var datas = Json.Deserialize (result, true) as List<object>;
            foreach (Dictionary<string, object> data in datas) {
                var name = data["name"] as string;
                if (name.Contains (Scorpio.Version.version)) {
                    Logger.info ($"当前已经是最新版本 : {name}");
                    return;
                }
                bool newVersion = false;
                if ((bool) data["prerelease"]) {
                    if (isPreview) { newVersion = true; }
                } else {
                    newVersion = true;
                }
                if (newVersion) {
                    var url = data["html_url"].ToString ();
                    var published_at = data["published_at"].ToString ();
                    Logger.info ($"发现新版本:{name}  发布时间:{published_at}");
                    Logger.info ("==========更新内容==========");
                    Logger.info (data["body"].ToString ());
                    Logger.info ($"新版本下载地址 : {url}");
                    Logger.info ("安装新版本命令 : pwsh -Command \"Invoke - Expression((New - Object System.Net.WebClient).DownloadString('https://qingfeng346.gitee.io/installsco.ps1'))\"");
                    return;
                }
            }
        }
        static void Pack (CommandLine command, string[] args) {
            var source = perform.GetPath (ParameterSource);
            var output = perform.GetPath (ParameterOutput);
            var ignore = command.GetValueDefault (ParameterIgnore, "");
            var define = command.GetValueDefault (ParameterDefine, "");
            File.WriteAllBytes (output, Serializer.Serialize (
                source, 
                FileUtil.GetFileString (source), 
                ignore.Split (";"), 
                define.Split(";")).ToArray ());
            Logger.info ($"生成IL文件  {source} -> {output}");
        }
        static void Fast (CommandLine command, string[] args) {
            var output = perform.GetPath (ParameterOutput);
            var strClass = command.GetValue (ParameterClass);
            if (strClass.isNullOrWhiteSpace ()) { throw new Exception ("找不到 -class 参数"); }
            var dll = command.GetValue (ParameterDll);
            var assembly = dll.isNullOrWhiteSpace () ? null : Assembly.LoadFile (Path.Combine (CurrentDirectory, dll));
            var strExtension = command.GetValue (ParameterExtension);

            ClassFilter filter = null;
            var strFilter = command.GetValue (ParameterFilter);
            if (!strFilter.isNullOrWhiteSpace ()) {
                var filterType = GetType (assembly, strFilter, typeof (ClassFilter));
                if (filterType != null) {
                    filter = (ClassFilter) Activator.CreateInstance (filterType);
                }
            }
            var extensions = new List<Type> ();
            if (!strExtension.isNullOrWhiteSpace ()) {
                foreach (var cl in strExtension.Split (';')) {
                    extensions.Add (GetType (assembly, cl, null));
                }
            }
            var classNames = strClass.Split (";");
            foreach (var className in classNames) {
                var clazz = GetType (assembly, className, null);
                if (clazz == null) { throw new Exception ($"找不到 class, 请输入完整类型或检查类名是否正确 : {className}"); }
                var generate = new GenerateScorpioClass (clazz);
                generate.SetClassFilter (filter);
                foreach (var ex in extensions) {
                    generate.AddExtensionType (ex);
                }
                var outputFile = Path.Combine (output, generate.ScorpioClassName + ".cs");
                FileUtil.CreateFile (outputFile, generate.Generate ());
                Logger.info ($"生成快速反射类 {className} -> {outputFile}");
            }
        }
        static void DelegateFactory (CommandLine command, string[] args) {
            var output = perform.GetPath (ParameterOutput);
            var strClass = command.GetValue (ParameterClass);
            if (strClass.isNullOrWhiteSpace ()) { throw new Exception ("找不到 -class 参数"); }

            var dll = command.GetValue (ParameterDll);
            var assembly = dll.isNullOrWhiteSpace () ? null : Assembly.LoadFile (Path.Combine (CurrentDirectory, dll));
            var generate = new GenerateScorpioDelegate ();
            var classNames = strClass.Split (";");
            foreach (var className in classNames) {
                var clazz = GetType (assembly, className, null);
                if (clazz == null) { throw new Exception ($"找不到 class, 请输入完整类型或检查类名是否正确 : {className}"); }
                generate.AddType (clazz);
            }
            FileUtil.CreateFile (output, generate.Generate (0));
            Logger.info ($"生成Delegate仓库 {output}");
        }
        static void Interface (CommandLine command, string[] args) {
            var output = perform.GetPath (ParameterOutput);
            var strClass = command.GetValue (ParameterClass);
            if (strClass.isNullOrWhiteSpace ()) { throw new Exception ("找不到 -class 参数"); }

            var dll = command.GetValue (ParameterDll);
            var assembly = dll.isNullOrWhiteSpace () ? null : Assembly.LoadFile (Path.Combine (CurrentDirectory, dll));
            var classNames = strClass.Split (";");
            foreach (var className in classNames) {
                var clazz = GetType (assembly, className, null);
                if (clazz == null) { throw new Exception ($"找不到 class, 请输入完整类型或检查类名是否正确 : {className}"); }
                var generate = new GenerateScorpioInterface (clazz);
                var outputFile = Path.Combine (output, generate.ScorpioClassName + ".cs");
                FileUtil.CreateFile (outputFile, generate.Generate ());
                Logger.info ($"生成Interface类 {className} -> {outputFile}");
            }
        }

        static void Execute (CommandLine command, string[] args) {
            Util.PrintSystemInfo ();
            Logger.info ($"Version : {Scorpio.Version.version}[{Scorpio.Version.date}]");
            TypeManager.PushAssembly(typeof(Program).Assembly);
            foreach (var assemblyName in typeof(Program).Assembly.GetReferencedAssemblies()) {
                TypeManager.PushAssembly(Assembly.Load(assemblyName));
            }
            var script = new Scorpio.Script ();
            script.LoadLibraryV1 ();
            script.LoadLibraryExtend();
            LoadLibrary (Path.Combine (CurrentDirectory, "dll"));
            if (args.Length >= 1) {
                try {
                    var file = Path.Combine (CurrentDirectory, args[0]);
                    if (!File.Exists (file)) {
                        Logger.info ($"文件 : {file} 不存在");
                        return;
                    }
                    var path = Path.GetDirectoryName (file);
                    script.PushSearchPath (path);
                    script.PushSearchPath (CurrentDirectory);
                    var sArgs = new string[args.Length - 1];
                    Array.Copy(args, 1, sArgs, 0, sArgs.Length);
                    script.SetArgs(sArgs);
                    Logger.info ("=============================");
                    var watch = Stopwatch.StartNew ();
                    var value = script.LoadFile (file);
                    while (script.UpdateCoroutine()) { }
                    Logger.info ("=============================");
                    Logger.info ("return value : " + value);
                    Logger.info ("the execution time : " + watch.ElapsedMilliseconds + " ms");
                } catch (Exception e) {
                    var stackInfo = script.GetStackInfo ();
                    Logger.info($"{stackInfo.Breviary}:{stackInfo.Line} {e}");
                }
            } else {
                while (true) {
                    try {
                        string str = Console.ReadLine ();
                        if (str == "exit") {
                            break;
                        } else if (str == "clear") {
                            Console.Clear ();
                        } else {
                            script.LoadString (str);
                        }
                    } catch (System.Exception ex) {
                        Logger.error (ex.ToString ());
                    }
                }
            }
        }
        static void LoadLibrary (string path) {
            if (!Directory.Exists (path)) { return; }
            string[] files = Directory.GetFiles (path, "*.dll", SearchOption.AllDirectories);
            foreach (var file in files) {
                try {
                    TypeManager.PushAssembly (Assembly.LoadFile (file));
                    Logger.info ("load dll file [" + file + "] success");
                } catch (System.Exception ex) {
                    Logger.error ("load dll file [" + file + "] fail : " + ex.ToString ());
                }
            }
        }
        static Type GetType (Assembly assembly, string typeName, Type parent) {
            var type = assembly?.GetType (typeName, false, false);
            if (type == null) { type = Type.GetType (typeName, false, false); }
            if (parent != null) {
                if (type != null && type.IsSubclassOf (parent)) {
                    return type;
                }
                return null;
            }
            return type;
        }
    }
}