using System;
using Scorpio.Commons;
using System.Collections.Generic;
using System.IO;
using System.Text;
public class LogHelper : ILogger {
    public void info(string value) {
        Console.WriteLine(value);
    }
    public void warn(string value) {
        value = "[warn]" + value;
        Console.WriteLine(value);
    }
    public void error(string value) {
        value = "[error]" + value;
        Console.WriteLine(value);
    }
}
public class Launch {
    public static readonly string CurrentDirectory = Environment.CurrentDirectory;
    private static CommandLine command;
    public class ExecuteData {
        public string help;
        public Action<CommandLine, string[]> execute;
    }
    private static Dictionary<string, ExecuteData> executes = new Dictionary<string, ExecuteData>();
    public static void Start(string[] args, Action<CommandLine, string[]> pre, Action<CommandLine, string[]> post) {
        var type = "";
        try {
            Logger.SetLogger(new LogHelper());
            command = CommandLine.Parse(args);
            var hasHelp = command.HadValue("-help", "--help", "-h");
            type = command.GetValue("-type", "-t");
            if (type.isNullOrWhiteSpace()) { type = command.Type; }
            type = type.ToLower();
            if (!executes.ContainsKey(type)) {
                type = "";
            }
            if (pre != null) pre(command, args);
            var data = executes[type];
            if (hasHelp) {
                Logger.info(data.help);
            } else {
                data.execute(command, args);
            }
            if (post != null) post(command, args);
        } catch (Exception e) {
            Logger.error("执行命令 [{0}] 出错 : {1}", type, e.ToString());
        }
    }
    public static void AddExecute(string type, string help, Action<CommandLine, string[]> execute) {
        executes[type.ToLower()] = new ExecuteData() { help = help, execute = execute };
    }
    public static string GetPath(params string[] keys) {
        return GetPath(keys, false);
    }
    public static string GetPath(string[] keys, bool throwException = false) {
        var path = command.GetValue(keys);
        if (path.isNullOrWhiteSpace()) {
            if (throwException) {
                var keyStr = new StringBuilder();
                foreach (var key in keys) {
                    keyStr.Append($"{key}|");
                }
                throw new Exception($"找不到 {keyStr} 参数");
            }
            path = "";
        }
        return Path.GetFullPath(Path.Combine(CurrentDirectory, path));
    }
}