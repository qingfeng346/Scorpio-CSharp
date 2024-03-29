﻿using Scorpio;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ScorpioLibrary {
    public class LibraryOS {
        public static void Load(Script script) {
            var functions = new (string, ScorpioHandle)[] {
                ("platform", new platform()),
                ("isWindows", new isWindows()),
                ("isLinux", new isLinux()),
                ("isOSX", new isOSX()),
                ("machineName", new machineName()),
                ("userName", new userName()),
                ("dotnetVersion", new dotnetVersion()),
                ("version", new version()),
                ("getEnvironmentVariable", new getEnvironmentVariable()),
                ("setEnvironmentVariable", new setEnvironmentVariable()),
                ("getFolderPath", new getFolderPath()),
                ("process", new process(script)),
            };
            script.AddLibrary("os", functions);
        }
        private class platform : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    return new ScriptValue("windows");
                } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                    return new ScriptValue("linux");
                } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                    return new ScriptValue("osx");
                } else {
                    return new ScriptValue("unknown");
                }
            }
        }
        private class isWindows : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isLinux : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isOSX : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                return RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class machineName : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                return new ScriptValue(Environment.MachineName);
            }
        }
        private class userName : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                return new ScriptValue(Environment.UserName);
            }
        }
        private class dotnetVersion : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                return new ScriptValue(RuntimeInformation.FrameworkDescription);
            }
        }
        private class version : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                return new ScriptValue(RuntimeInformation.OSDescription);
            }
        }
        private class getEnvironmentVariable : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                return new ScriptValue(Environment.GetEnvironmentVariable(Parameters[0].ToString(), (EnvironmentVariableTarget)Parameters[1].ToInt32()));
            }
        }
        private class setEnvironmentVariable : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                Environment.SetEnvironmentVariable(Parameters[0].ToString(), Parameters[1].ToString(), (EnvironmentVariableTarget)Parameters[2].ToInt32());
                return ScriptValue.Null;
            }
        }
        private class getFolderPath : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                return new ScriptValue(Environment.GetFolderPath((Environment.SpecialFolder)Parameters[0].ToInt32()));
            }
        }
        private class process : ScorpioHandle {
            private Script script;
            public process(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                using (var process = new Process()) {
                    process.StartInfo.FileName = Parameters[0].ToString();
                    if (length > 1) process.StartInfo.Arguments = Parameters[1].ToString();
                    if (length > 2) process.StartInfo.WorkingDirectory = Parameters[2].ToString();
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.EnableRaisingEvents = true;
                    if (length > 3) Parameters[3].call(ScriptValue.Null, process);
                    process.Start();
                    var result = new ScriptMapStringPolling(script, 2);
                    result.SetValue("output", new ScriptValue(process.StandardOutput.ReadToEnd()));
                    result.SetValue("exitCode", new ScriptValue((double)process.ExitCode));
                    return new ScriptValue(result);
                }
            }
        }
    }
}
