#if SCORPIO_UWP && !UNITY_EDITOR
#define UWP
#endif

using System;
using System.IO;
using System.Reflection;
using Scorpio;

namespace Scorpio.Extensions
{
    public class DefaultScriptExtensions : IScriptExtensions {
        public void print(string str) {
#if UWP
            System.Diagnostics.Debug.WriteLine(str);
#else
            System.Console.WriteLine(str);
#endif
        }
        public Assembly GetAssembly(Type type) {
#if UWP
            return type.GetTypeInfo().Assembly;
#else
            return type.Assembly;
#endif
        }
        public MethodInfo GetMethodInfo(Delegate del) {
#if UWP
            return del.GetMethodInfo();
#else
            return del.Method;
#endif
        }
        public bool IsEnum(Type type) {
#if UWP
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }
        public bool FileExist(string file) {
            return !Util.IsNullOrEmpty(file) && File.Exists(file);
        }
        public byte[] GetFileBuffer(string file) {
            FileStream stream = File.OpenRead(file);
            long length = stream.Length;
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Dispose();
            return buffer;
        }
    }
}
