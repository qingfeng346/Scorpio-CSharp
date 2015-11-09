using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Scorpio.Extensions
{
    public class DefaultScriptExtensions : IScriptExtensions
    {
        public void print(string str)
        {
#if SCORPIO_UWP
            System.Diagnostics.Debug.WriteLine(str);
#else
            System.Console.WriteLine(str);
#endif
        }
        public Assembly GetAssembly(Type type)
        {
#if SCORPIO_UWP
            return System.Reflection.IntrospectionExtensions.GetTypeInfo(type).Assembly;
#else
            return type.Assembly;
#endif
        }
        public byte[] GetFileBuffer(string file)
        {
            FileStream stream = File.OpenRead(file);
            long length = stream.Length;
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Dispose();
            return buffer;
        }
        public MethodInfo GetMethodInfo(Delegate del)
        {
#if SCORPIO_UWP
            return System.Reflection.RuntimeReflectionExtensions.GetMethodInfo(del);
#else
            return del.Method;
#endif
        }
    }
}
