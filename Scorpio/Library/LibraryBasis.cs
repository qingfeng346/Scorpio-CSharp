using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Scorpio;
using Scorpio.Exception;
using Scorpio.Variable;
namespace Scorpio.Library
{
    public class LibraryBasis
    {
        private class ArrayPairs : ScorpioHandle
        {
            Script m_Script;
            List<ScriptObject>.Enumerator m_Enumerator;
            int m_Index = 0;
            public ArrayPairs(Script script, ScriptArray obj)
            {
                m_Script = script;
                m_Index = 0;
                m_Enumerator = obj.GetIterator();
            }
            public object Call(ScriptObject[] args)
            {
                if (m_Enumerator.MoveNext()) {
                    ScriptTable table = m_Script.CreateTable();
                    table.SetValue("key", m_Script.CreateObject(m_Index++));
                    table.SetValue("value", m_Enumerator.Current);
                    return table;
                }
                return null;
            }
        }
        private class ArrayKPairs : ScorpioHandle
        {
            List<ScriptObject>.Enumerator m_Enumerator;
            int m_Index = 0;
            public ArrayKPairs(ScriptArray obj)
            {
                m_Index = 0;
                m_Enumerator = obj.GetIterator();
            }
            public object Call(ScriptObject[] args)
            {
                if (m_Enumerator.MoveNext())
                    return m_Index++;
                return null;
            }
        }
        private class ArrayVPairs : ScorpioHandle
        {
            List<ScriptObject>.Enumerator m_Enumerator;
            public ArrayVPairs(ScriptArray obj)
            {
                m_Enumerator = obj.GetIterator();
            }
            public object Call(ScriptObject[] args)
            {
                if (m_Enumerator.MoveNext())
                    return m_Enumerator.Current;
                return null;
            }
        }
        private class TablePairs : ScorpioHandle
        {
            Script m_Script;
            Dictionary<object, ScriptObject>.Enumerator m_Enumerator;
            public TablePairs(Script script, ScriptTable obj)
            {
                m_Script = script;
                m_Enumerator = obj.GetIterator();
            }
            public object Call(ScriptObject[] args)
            {
                if (m_Enumerator.MoveNext()) {
                    ScriptTable table = m_Script.CreateTable();
                    KeyValuePair<object, ScriptObject> v = m_Enumerator.Current;
                    table.SetValue("key", m_Script.CreateObject(v.Key));
                    table.SetValue("value", v.Value);
                    return table;
                }
                return null;
            }
        }
        private class TableKPairs : ScorpioHandle
        {
            Dictionary<object, ScriptObject>.Enumerator m_Enumerator;
            public TableKPairs(ScriptTable obj)
            {
                m_Enumerator = obj.GetIterator();
            }
            public object Call(ScriptObject[] args)
            {
                if (m_Enumerator.MoveNext())
                    return m_Enumerator.Current.Key;
                return null;
            }
        }
        private class TableVPairs : ScorpioHandle
        {
            Dictionary<object, ScriptObject>.Enumerator m_Enumerator;
            public TableVPairs(ScriptTable obj)
            {
                m_Enumerator = obj.GetIterator();
            }
            public object Call(ScriptObject[] args)
            {
                if (m_Enumerator.MoveNext())
                    return m_Enumerator.Current.Value;
                return null;
            }
        }
        private class UserdataPairs : ScorpioHandle
        {
            Script m_Script;
            System.Collections.IEnumerator m_Enumerator;
            public UserdataPairs(Script script, ScriptUserdata obj)
            {
                m_Script = script;
                object value = obj.Value;
                System.Collections.IEnumerable ienumerable = value as System.Collections.IEnumerable;
                if (ienumerable == null) throw new ExecutionException(m_Script, "pairs 只支持继承 IEnumerable 的类");
                m_Enumerator = ienumerable.GetEnumerator();
            }
            public object Call(ScriptObject[] args)
            {
                if (m_Enumerator.MoveNext())
                    return m_Enumerator.Current;
                return null;
            }
        }
        public static void Load(Script script)
        {
            script.SetObjectInternal("print", script.CreateFunction(new print()));
            script.SetObjectInternal("pairs", script.CreateFunction(new pairs(script)));
            script.SetObjectInternal("kpairs", script.CreateFunction(new kpairs(script)));
            script.SetObjectInternal("vpairs", script.CreateFunction(new vpairs(script)));
            script.SetObjectInternal("type", script.CreateFunction(new type()));
            script.SetObjectInternal("is_null", script.CreateFunction(new is_null()));
            script.SetObjectInternal("is_bool", script.CreateFunction(new is_bool()));
            script.SetObjectInternal("is_number", script.CreateFunction(new is_number()));
            script.SetObjectInternal("is_double", script.CreateFunction(new is_double()));
            script.SetObjectInternal("is_long", script.CreateFunction(new is_long()));
            script.SetObjectInternal("is_int", script.CreateFunction(new is_int()));
            script.SetObjectInternal("is_string", script.CreateFunction(new is_string()));
            script.SetObjectInternal("is_function", script.CreateFunction(new is_function()));
            script.SetObjectInternal("is_array", script.CreateFunction(new is_array()));
            script.SetObjectInternal("is_table", script.CreateFunction(new is_table()));
            script.SetObjectInternal("is_enum", script.CreateFunction(new is_enum()));
            script.SetObjectInternal("is_userdata", script.CreateFunction(new is_userdata()));
            script.SetObjectInternal("branchtype", script.CreateFunction(new branchtype()));
            script.SetObjectInternal("typeof", script.CreateFunction(new userdatatype()));
            script.SetObjectInternal("tonumber", script.CreateFunction(new tonumber(script)));
            script.SetObjectInternal("tolong", script.CreateFunction(new tolong(script)));
            script.SetObjectInternal("toint", script.CreateFunction(new toint(script)));
            script.SetObjectInternal("toenum", script.CreateFunction(new toenum(script)));
            script.SetObjectInternal("tostring", script.CreateFunction(new tostring(script)));
            script.SetObjectInternal("clone", script.CreateFunction(new clone()));
            script.SetObjectInternal("require", script.CreateFunction(new require(script)));
            script.SetObjectInternal("import", script.CreateFunction(new require(script)));
            script.SetObjectInternal("using", script.CreateFunction(new require(script)));

            script.SetObjectInternal("load_assembly", script.CreateFunction(new load_assembly(script)));
            script.SetObjectInternal("load_assembly_safe", script.CreateFunction(new load_assembly_safe(script)));
            script.SetObjectInternal("push_assembly", script.CreateFunction(new push_assembly(script)));
            script.SetObjectInternal("import_type", script.CreateFunction(new import_type(script)));
            script.SetObjectInternal("generic_type", script.CreateFunction(new generic_type(script)));
            script.SetObjectInternal("generic_method", script.CreateFunction(new generic_method(script)));
        }
        private class print : ScorpioHandle
        {
            public object Call(ScriptObject[] args) {
                for (int i = 0; i < args.Length; ++i) {
                    ScriptExtensions.print(args[i].ToString());
                }
                return null;
            }
        }
        private class pairs : ScorpioHandle
        {
            private Script m_script;
            public pairs(Script script)
            {
                m_script = script;
            }
            public object Call(ScriptObject[] args)
            {
                ScriptObject obj = args[0];
                if (obj is ScriptArray)
                    return m_script.CreateFunction(new ArrayPairs(m_script, (ScriptArray)obj));
                else if (obj is ScriptTable)
                    return m_script.CreateFunction(new TablePairs(m_script, (ScriptTable)obj));
                else if (obj is ScriptUserdata)
                    return m_script.CreateFunction(new UserdataPairs(m_script, (ScriptUserdata)obj));
                throw new ExecutionException(m_script, "pairs必须用语table或array或者继承IEnumerable的userdata 类型");
            }
        }
        private class kpairs : ScorpioHandle
        {
            private Script m_script;
            public kpairs(Script script)
            {
                m_script = script;
            }
            public object Call(ScriptObject[] args)
            {
                ScriptObject obj = args[0];
                if (obj is ScriptArray)
                    return m_script.CreateFunction(new ArrayKPairs((ScriptArray)obj));
                else if (obj is ScriptTable)
                    return m_script.CreateFunction(new TableKPairs((ScriptTable)obj));
                throw new ExecutionException(m_script, "kpairs必须用语table或array类型");
            }
        }
        private class vpairs : ScorpioHandle
        {
            private Script m_script;
            public vpairs(Script script)
            {
                m_script = script;
            }
            public object Call(ScriptObject[] args)
            {
                ScriptObject obj = args[0];
                if (obj is ScriptArray)
                    return m_script.CreateFunction(new ArrayVPairs((ScriptArray)obj));
                else if (obj is ScriptTable)
                    return m_script.CreateFunction(new TableVPairs((ScriptTable)obj));
                throw new ExecutionException(m_script, "vpairs必须用语table或array类型");
            }
        }
        private class type : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return (int)args[0].Type;
            }
        }
        private class is_null : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return args[0] is ScriptNull;
            }
        }
        private class is_bool : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return args[0] is ScriptBoolean;
            }
        }
        private class is_number : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return args[0] is ScriptNumber;
            }
        }
        private class is_double : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return args[0] is ScriptNumberDouble;
            }
        }
        private class is_long : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return args[0] is ScriptNumberLong;
            }
        }
        private class is_int : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return args[0] is ScriptNumberInt;
            }
        }
        private class is_string : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return args[0] is ScriptString;
            }
        }
        private class is_function : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return args[0] is ScriptFunction;
            }
        }
        private class is_array : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return args[0] is ScriptArray;
            }
        }
        private class is_table : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return args[0] is ScriptTable;
            }
        }
        private class is_enum : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return args[0] is ScriptEnum;
            }
        }
        private class is_userdata : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return args[0] is ScriptUserdata;
            }
        }
        private class branchtype : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return args[0].BranchType;
            }
        }
        private class userdatatype : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return ((ScriptUserdata)args[0]).ValueType;
            }
        }
        private class tonumber : ScorpioHandle
        {
            private Script m_script;
            public tonumber(Script script)
            {
                m_script = script;
            }
            public object Call(ScriptObject[] args)
            {
                ScriptObject obj = args[0];
                Util.Assert(obj is ScriptNumber || obj is ScriptString || obj is ScriptEnum, m_script, "tonumber 不能从类型 " + obj.Type + " 转换成Number类型");
                return m_script.CreateDouble(Util.ToDouble(obj.ObjectValue));
            }
        }
        private class tolong : ScorpioHandle
        {
            private Script m_script;
            public tolong(Script script)
            {
                m_script = script;
            }
            public object Call(ScriptObject[] args)
            {
                ScriptObject obj = args[0];
                Util.Assert(obj is ScriptNumber || obj is ScriptString || obj is ScriptEnum, m_script, "tolong 不能从类型 " + obj.Type + " 转换成Long类型");
                return m_script.CreateLong(Util.ToInt64(obj.ObjectValue));
            }
        }
        private class toint : ScorpioHandle
        {
            private Script m_script;
            public toint(Script script)
            {
                m_script = script;
            }
            public object Call(ScriptObject[] args)
            {
                ScriptObject obj = args[0];
                Util.Assert(obj is ScriptNumber || obj is ScriptString || obj is ScriptEnum, m_script, "toint 不能从类型 " + obj.Type + " 转换成int类型");
                return m_script.CreateInt(Util.ToInt32(obj.ObjectValue));
            }
        }
        private class toenum : ScorpioHandle
        {
            private Script m_script;
            public toenum(Script script)
            {
                m_script = script;
            }
            public object Call(ScriptObject[] args)
            {
                Util.Assert(args.Length == 2, m_script, "toenum 第一个参数是枚举类 第二个参数必须是number类型");
                ScriptUserdata obj = args[0] as ScriptUserdata;
                ScriptNumber number = args[1] as ScriptNumber;
                Util.Assert(obj != null && number != null, m_script, "toenum 第一个参数是枚举类 第二个参数必须是number类型");
                return m_script.CreateEnum(Enum.ToObject(obj.ValueType, number.ToInt32()));
            }
        }
        private class tostring : ScorpioHandle
        {
            private Script m_script;
            public tostring(Script script)
            {
                m_script = script;
            }
            public object Call(ScriptObject[] args)
            {
                ScriptObject obj = args[0];
                if (obj is ScriptString) return obj;
                return m_script.CreateString(obj.ToString());
            }
        }
        private class clone : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return args[0].Clone();
            }
        }
        private class require : ScorpioHandle
        {
            private Script m_script;
            public require(Script script)
            {
                m_script = script;
            }
            public object Call(ScriptObject[] args)
            {
                ScriptString str = args[0] as ScriptString;
                Util.Assert(str != null, m_script, "require 参数必须是 string");
                return m_script.LoadSearchPathFile(str.Value);
            }
        }
        private class load_assembly : ScorpioHandle
        {
            private Script m_script;
            public load_assembly(Script script)
            {
                m_script = script;
            }
            public object Call(ScriptObject[] args)
            {
                ScriptString str = args[0] as ScriptString;
                Util.Assert(str != null, m_script, "load_assembly 参数必须是 string");
                m_script.PushAssembly(Assembly.Load(new AssemblyName(str.Value)));
                return null;
            }
        }
        private class load_assembly_safe : ScorpioHandle
        {
            private Script m_script;
            public load_assembly_safe(Script script)
            {
                m_script = script;
            }
            public object Call(ScriptObject[] args)
            {
                try {
                    ScriptString str = args[0] as ScriptString;
                    Util.Assert(str != null, m_script, "load_assembly 参数必须是 string");
                    m_script.PushAssembly(Assembly.Load(new AssemblyName(str.Value)));
                } catch (System.Exception ) { }
                return null;
            }
        }
        private class push_assembly : ScorpioHandle
        {
            private Script m_script;
            public push_assembly(Script script)
            {
                m_script = script;
            }
            public object Call(ScriptObject[] args)
            {
                ScriptUserdata assembly = args[0] as ScriptUserdata;
                Util.Assert(assembly != null, m_script, "push_assembly 参数必须是 Assembly 类型");
                m_script.PushAssembly(assembly.ObjectValue as Assembly);
                return null;
            }
        }
        private class import_type : ScorpioHandle
        {
            private Script m_script;
            public import_type(Script script)
            {
                m_script = script;
            }
            public object Call(ScriptObject[] args)
            {
                ScriptString str = args[0] as ScriptString;
                Util.Assert(str != null, m_script, "import_type 参数必须是 string");
                return m_script.LoadType(str.Value);
            }
        }
        private class generic_type : ScorpioHandle
        {
            private Script m_script;
            public generic_type(Script script)
            {
                m_script = script;
            }
            public object Call(ScriptObject[] args)
            {
                ScriptUserdata userdata = args[0] as ScriptUserdata;
                Util.Assert(userdata != null, m_script, "generic_type 第1个参数必须是 userdata");
                Type[] types = new Type[args.Length - 1];
                for (int i = 1; i < args.Length; ++i) {
                    ScriptUserdata type = args[i] as ScriptUserdata;
                    Util.Assert(type != null, m_script, "generic_type 第" + (i+1) + "参数必须是 userdata");
                    types[i - 1] = type.ValueType;
                }
                return m_script.GetUserdataFactory().GetScorpioType(userdata.ValueType).MakeGenericType(types);
            }
        }
        private class generic_method : ScorpioHandle
        {
            private Script m_script;
            public generic_method(Script script)
            {
                m_script = script;
            }
            public object Call(ScriptObject[] args)
            {
                ScriptFunction func = args[0] as ScriptFunction;
                Util.Assert(func != null, m_script, "generic_method 第1个参数必须是 function");
                ScorpioMethod method = func.Method;
                Util.Assert(method != null, m_script, "generic_method 第1个参数必须是 程序函数");
                Type[] types = new Type[args.Length - 1];
                for (int i = 1; i < args.Length; ++i) {
                    ScriptUserdata type = args[i] as ScriptUserdata;
                    Util.Assert(type != null, m_script, "generic_method 第" + (i + 1) + "参数必须是 userdata");
                    types[i - 1] = type.ValueType;
                }
                return method.MakeGenericMethod(types);
            }
        }
    }
}
