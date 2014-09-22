using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Collections;
using Scorpio.Variable;
namespace Scorpio
{
    public static class Util
    {
        public static bool SetObject(VariableDictionary variables, string str, ScriptObject obj)
        {
            if (!variables.ContainsKey(str)) return false;
            Set_impl(variables, str, obj);
            return true;
        }
        public static void AssignObject(VariableDictionary variables, string str, ScriptObject obj)
        {
            if (!variables.ContainsKey(str))
            {
                variables.Add(str, obj);
                return;
            }
            Set_impl(variables, str, obj);
        }
        private static void Set_impl(VariableDictionary variables, string str, ScriptObject obj)
        {
            ScriptObject el = variables[str];
            if (el.Type != obj.Type) {
                variables[str] = obj;
            } else {
                if (el.IsNull || el.IsNumber || el.IsString) {
                    el.Assign(obj);
                } else {
                    variables[str] = obj;
                }
            }
        }
        private static bool IsNumber(Type type)
        {
            return (type == typeof(sbyte) || type == typeof(byte) ||
                    type == typeof(short) || type == typeof(ushort) ||
                    type == typeof(int) || type == typeof(uint) ||
                    type == typeof(float) || type == typeof(double) ||
                    type == typeof(decimal) || type == typeof(long) ||
                    type == typeof(ulong));
        }
        public static object ChangeType(ScriptObject par, Type type)
        {
            if (par.GetType() == type) {
                return par;
            } else if (par is IScriptPrimitiveObject) {
                return Convert.ChangeType(((IScriptPrimitiveObject)par).ObjectValue, type);
            } else if (par is ScriptUserdata) {
                return ((ScriptUserdata)par).Value;
            }
            return par;
        }
        public static bool CanChangeType(ScriptObject[] pars, Type[] types)
        {
            for (int i = 0; i < pars.Length;++i )
            {
                if (!CanChangeType(pars[i], types[i]))
                    return false;
            }
            return true;
        }
        public static bool CanChangeType(ScriptObject par, Type type)
        {
            if (par.GetType() == type) {
                return true;
            } else if (par.IsString && type == typeof(string)) {
                return true;
            } else if (par.IsNumber && IsNumber(type)) {
                return true;
            } else if (par.IsUserData && ((ScriptUserdata)par).ValueType == type) {
                return true;
            }
            return false;
        }
    }
}
