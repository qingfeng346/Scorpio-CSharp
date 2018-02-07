using System;
using System.Collections.Generic;

public class CommandLineArgument {
    private List<string> args = new List<string>();
    public CommandLineArgument(string name) {
    }
    public void Add(string arg) {
        args.Add(arg);
    }
    public string Get(int index) {
        return args[index];
    }
    public override string ToString() {
        return args.Count > 0 ? args[0] : "";
    }
}
public class CommandLine {
    private Dictionary<string, CommandLineArgument> arguments = new Dictionary<string, CommandLineArgument>();
    public CommandLine(string[] args) {
        CommandLineArgument argument = null;
        for (int i = 0; i < args.Length; ++i) {
            string arg = args[i];
            if (arg.StartsWith("-")) {
                argument = new CommandLineArgument(arg);
                arguments[arg] = argument;
            } else if (argument != null) {
                argument.Add(arg);
            }
        }
    }
    public CommandLineArgument Get(string key) {
        if (arguments.ContainsKey(key))
            return arguments[key];
        else
            return new CommandLineArgument(key);
    }
}