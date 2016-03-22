using System;
using System.Reflection;

namespace Scorpio
{
    public interface IScriptExtensions {
        void print(string str);
        Assembly GetAssembly(Type type);
        MethodInfo GetMethodInfo(Delegate del);
        bool IsEnum(Type type);
        bool FileExist(string file);
        byte[] GetFileBuffer(string file);
    }
}
