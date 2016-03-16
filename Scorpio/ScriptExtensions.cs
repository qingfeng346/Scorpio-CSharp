using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Scorpio.Extensions;
namespace Scorpio
{
    public class ScriptExtensions
    {
        private static IScriptExtensions m_Extensions = null;
        static ScriptExtensions() {
            SetExtensions(new DefaultScriptExtensions());
        }
        public static void SetExtensions(IScriptExtensions extensions) {
            m_Extensions = extensions;
        }
        public static void print(string str) {
            m_Extensions.print(str);
        }
        public static Assembly GetAssembly(Type type) {
            return m_Extensions.GetAssembly(type);
        }
        public static MethodInfo GetMethodInfo(Delegate del) {
            return m_Extensions.GetMethodInfo(del);
        }
        public static bool IsEnum(Type type) {
            return m_Extensions.IsEnum(type);
        }
        public static bool FileExist(string file) {
            return m_Extensions.FileExist(file);
        }
        public static byte[] GetFileBuffer(string file) {
            return m_Extensions.GetFileBuffer(file);
        }
    }
}
