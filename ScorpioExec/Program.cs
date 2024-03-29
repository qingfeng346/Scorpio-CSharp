using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Scorpio;
using Scorpio.Commons;
using Scorpio.Compile.Compiler;
using Scorpio.FastReflect;
using Scorpio.Runtime;
using Scorpio.Serialize;
using ScorpioLibrary;

namespace ScorpioExec {
    public class Program {
        private static readonly string CurrentDirectory = ScorpioUtil.CurrentDirectory;

        private const string OptionDescribe = @"
编译选项,json格式
    defines          字符串数据,全局宏定义
    ignoreFunctions  字符串数据,忽略全局函数
    staticTypes      字符串数据,静态类
    staticVariables  字符串数据,静态变量
    const            文件路径,定义常量文件
";
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
    --search|-search (选填)#import 搜索路径列表,分号;隔开
    --option         (选填)编译选项
" + OptionDescribe;
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
        private readonly static string[] ParameterSearch = new [] { "--search", "-search" };
        private readonly static string[] ParameterOption = new [] { "--option" };
        private readonly static string[] ParameterFilter = new [] { "-f", "--filter", "-filter" };
        private readonly static string[] ParameterExtension = new [] { "-e", "--extension", "-extension" };
        private readonly static string[] ParameterCheck = new [] { "-c", "--check", "-check" };
        private readonly static string[] ParameterPreview = new [] { "-p", "--preview", "-preview" };
        static Perform perform;
        static void Main (string[] args) {
            perform = new Perform ();
            perform.Help = HelpExecute;
            perform.AddExecute ("version", HelpVersion, VersionExec);
            perform.AddExecute ("pack", HelpPack, Pack);
            perform.AddExecute ("fast", HelpFast, Fast);
            perform.AddExecute ("delegate", HelpDelegate, DelegateFactory);
            perform.AddExecute ("interface", HelpInterface, Interface);
            perform.AddExecute ("", HelpExecute, Execute);
            perform.Start (args, null, null);
        }
        static void VersionExec (Perform perform, CommandLine command, string[] args) {
            Logger.info (Scorpio.Version.version);
            if (!command.HadValue (ParameterCheck)) { return; }
            Logger.info ("正在检查最新版本...");
            var result = ScorpioUtil.RequestString ("http://api.github.com/repos/qingfeng346/Scorpio-CSharp/releases", (request) => {
                request.Headers.Add ("Authorization", "token e5ff670eb105f044273f4c81276a67cd1341e649");
            });
            var isPreview = command.HadValue (ParameterPreview);
            var datas = ScorpioJson.Deserialize (result, true) as List<object>;
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
        static void Pack (Perform perform, CommandLine command, string[] args) {
            var source = perform.GetPath (ParameterSource);
            var output = perform.GetPath (ParameterOutput);
            var search = command.GetValueDefault (ParameterSearch, "");
            var searchPaths = new List<string>(search.Split(";")) {
                CurrentDirectory,
                Path.GetDirectoryName(source)
            };
            //GlobalCacheCompiler globalCache = new GlobalCacheCompiler(0);
            GlobalCacheCompiler globalCache = null;
            var compileOption = ParseOption (command.GetValueDefault (ParameterOption, ""), searchPaths);
            File.WriteAllBytes (output, Serializer.SerializeBytes (
                source,
                FileUtil.GetFileString (source),
                searchPaths.ToArray (),
                compileOption, null, globalCache));
            Logger.info ($"生成IL文件  {source} -> {output}");
            //File.WriteAllBytes($"{Path.GetDirectoryName(output)}/cache", Serializer.SerializeGlobalCache(globalCache.GlobalCache));
        }
        static void Fast (Perform perform, CommandLine command, string[] args) {
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
        static void DelegateFactory (Perform perform, CommandLine command, string[] args) {
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
            FileUtil.CreateFile (output, generate.Generate ());
            Logger.info ($"生成Delegate仓库 {output}");
        }
        static void Interface (Perform perform, CommandLine command, string[] args) {
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

        static void Execute (Perform perform, CommandLine command, string[] args) {
            ScorpioUtil.PrintSystemInfo ();
            Logger.info ($"Version : {Scorpio.Version.version}[{Scorpio.Version.date}]");
            var script = new Scorpio.Script ();
            script.LoadLibraryV1 ();
            script.LoadLibraryExtend ();
            script.PushAssembly(typeof(Program));
            script.PushAssembly(typeof(TestClass));
            script.PushReferencedAssemblies(typeof(Program).Assembly);
            LoadLibrary (Path.Combine (CurrentDirectory, "dll"));
            if (args.Length >= 1) {
                try {
                    var file = Path.Combine (CurrentDirectory, args[0]);
                    if (!File.Exists (file)) {
                        Logger.info ($"文件 : {file} 不存在");
                        return;
                    }
                    script.PushSearchPath (Path.GetDirectoryName (file));
                    script.PushSearchPath (CurrentDirectory);
                    //var globalCaches = new GlobalCache[1];
                    //globalCaches[0] = Deserializer.DeserializeGlobalCache(File.ReadAllBytes("E:\\Scorpio-CSharp\\ScorpioExec\\cache"));
                    //script.GlobalCaches = globalCaches;
                    var sArgs = new string[args.Length - 1];
                    Array.Copy (args, 1, sArgs, 0, sArgs.Length);
                    script.SetArgs (sArgs);
                    Logger.info ("=============================");
                    var watch = Stopwatch.StartNew ();
                    var value = script.LoadFile (file, ParseOption (command.GetValueDefault (ParameterOption, ""), script.SearchPaths));
                    while (script.UpdateCoroutine ()) { }
                    script.ClearStack();
#if SCORPIO_DEBUG
                    script.CollectLeak(out var set, out var globalObjects, out var count, out var total);
#endif
                    Logger.info ("=============================");
                    Logger.info ("return value : " + value);
                    Logger.info ("the execution time : " + watch.ElapsedMilliseconds + " ms");
                } catch (Exception e) {
                    var stackInfo = script.GetStackInfo ();
                    Logger.info ($"{stackInfo.Breviary}:{stackInfo.Line} {e}");
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
                    ScorpioTypeManager.PushAssembly (Assembly.LoadFile (file));
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
        static CompileOption ParseOption (string str, IEnumerable<string> scriptSearchPaths) {
            try {
                if (string.IsNullOrWhiteSpace (str)) { return null; }
                var option = ScorpioJson.Deserialize (File.Exists (str) ? System.Text.Encoding.UTF8.GetString (File.ReadAllBytes (str)) : str) as Dictionary<string, object>;
                var compileOption = new CompileOption ();
                if (option.TryGetValue ("defines", out var defines)) {
                    if (!(defines is IList)) { throw new Exception ("defines 参数必须是字符串数组"); }
                    var values = new List<string> ();
                    foreach (object value in defines as IList) {
                        values.Add (value.ToString ());
                    }
                    compileOption.defines = values;
                }
                if (option.TryGetValue ("ignoreFunctions", out var ignoreFunctions)) {
                    if (!(ignoreFunctions is IList)) { throw new Exception ("ignoreFunctions 参数必须是字符串数组"); }
                    var values = new List<string> ();
                    foreach (object value in ignoreFunctions as IList) {
                        values.Add (value.ToString ());
                    }
                    compileOption.ignoreFunctions = values;
                }
                if (option.TryGetValue ("staticTypes", out var staticTypes)) {
                    if (!(staticTypes is IList)) { throw new Exception ("staticTypes 参数必须是字符串数组"); }
                    var values = new List<string> ();
                    foreach (object value in staticTypes as IList) {
                        values.Add (value.ToString ());
                    }
                    compileOption.staticTypes = values;
                }
                if (option.TryGetValue ("staticVariables", out var staticVariables)) {
                    if (!(staticTypes is IList)) { throw new Exception ("staticVariables 参数必须是字符串数组"); }
                    var values = new List<string> ();
                    foreach (object value in staticVariables as IList) {
                        values.Add (value.ToString ());
                    }
                    compileOption.staticVariables = values;
                }
                if (option.TryGetValue ("searchPaths", out var searchPaths)) {
                    if (!(searchPaths is IList)) { throw new Exception ("searchPaths 参数必须是字符串数组"); }
                    var values = new List<string> ();
                    foreach (object value in searchPaths as IList) {
                        values.Add (value.ToString ());
                    }
                    compileOption.searchPaths = values;
                }
                if (option.TryGetValue ("const", out var constFile)) {
                    if (!(constFile is string)) { throw new Exception ("const 参数必须是文件路径"); }
                    string fileName = constFile as string;
                    foreach (var searchPath in scriptSearchPaths) {
                        var fullFileName = Path.Combine (searchPath, fileName);
                        if (File.Exists (fullFileName)) {
                            var script = new Scorpio.Script ();
                            script.LoadLibraryV1 ();
                            foreach (var path in scriptSearchPaths) {
                                script.PushSearchPath (path);
                            }
                            compileOption.scriptConst = script.LoadConst (fullFileName);
                            break;
                        }
                    }
                }
                return compileOption;
            } catch (Exception e) {
                throw new Exception ("编译选项解析失败 : " + e.ToString ());
            }
        }
    }
}