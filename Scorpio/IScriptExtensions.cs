using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace Scorpio
{
    public interface IScriptExtensions {
        void print(string str);
        Assembly GetAssembly(Type type);
        MethodInfo GetMethodInfo(Delegate del);
        byte[] GetFileBuffer(string file);
        bool IsEnum(Type type);
    }
}
