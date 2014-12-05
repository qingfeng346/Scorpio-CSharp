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
            ScriptTable m_Table;
            public ArrayPairs(Script script, ScriptArray obj)
            {
                m_Script = script;
                m_Index = 0;
                m_Table = m_Script.CreateTable();
                m_Enumerator = obj.GetIterator();
            }
            public object Call(ScriptObject[] args)
            {
                if (m_Enumerator.MoveNext())
                {
                    m_Table.SetValue("key", m_Script.CreateObject(m_Index++));
                    m_Table.SetValue("value", m_Enumerator.Current);
                    return m_Table;
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
            ScriptTable m_Table;
            public TablePairs(Script script, ScriptTable obj)
            {
                m_Script = script;
                m_Table = m_Script.CreateTable();
                m_Enumerator = obj.GetIterator();
            }
            public object Call(ScriptObject[] args)
            {
                if (m_Enumerator.MoveNext())
                {
                    KeyValuePair<object, ScriptObject> v = m_Enumerator.Current;
                    m_Table.SetValue("key", m_Script.CreateObject(v.Key));
                    m_Table.SetValue("value", v.Value);
                    return m_Table;
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
            System.Collections.IEnumerator m_Enumerator;
            public UserdataPairs(ScriptUserdata obj)
            {
                object value = obj.Value;
                System.Collections.IEnumerable ienumerable = value as System.Collections.IEnumerable;
                if (ienumerable == null) throw new ExecutionException("pairs 只支持继承 IEnumerable 的类");
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
            script.SetObjectInternal("branchtype", script.CreateFunction(new branchtype()));
            script.SetObjectInternal("typeof", script.CreateFunction(new userdatatype()));
            script.SetObjectInternal("tonumber", script.CreateFunction(new tonumber(script)));
            script.SetObjectInternal("tolong", script.CreateFunction(new tolong(script)));
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
            public object Call(ScriptObject[] args)
            {
                for (int i = 0; i < args.Length; ++i) {
                    Console.WriteLine(args[i].ToString());
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
                    return m_script.CreateFunction(new UserdataPairs((ScriptUserdata)obj));
                throw new ExecutionException("pairs必须用语table或array或者继承IEnumerable的userdata 类型");
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
                throw new ExecutionException("kpairs必须用语table或array类型");
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
                throw new ExecutionException("vpairs必须用语table或array类型");
            }
        }
        private class type : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return args[0].Type;
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
                if (obj is ScriptNumber || obj is ScriptString || obj is ScriptEnum)
                    return m_script.CreateNumber(Util.ToDouble(obj.ObjectValue));
                throw new ExecutionException("不能从类型 " + obj.Type + " 转换成Number类型");
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
                if (obj is ScriptNumber || obj is ScriptString || obj is ScriptEnum)
                    return m_script.CreateNumber(Util.ToInt64(obj.ObjectValue));
                throw new ExecutionException("不能从类型 " + obj.Type + " 转换成Long类型");
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
                Util.Assert(str != null, "require 参数必须是 string");
                return m_script.LoadFile(m_script.GetValue("searchpath") + "/" + str.Value);
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
                if (str == null) throw new ExecutionException("load_assembly 参数必须是 string");
                m_script.PushAssembly(Assembly.Load(str.Value));
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
                    if (str == null) throw new ExecutionException("load_assembly 参数必须是 string");
                    m_script.PushAssembly(Assembly.Load(str.Value));
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
                if (assembly == null) throw new ExecutionException("push_assembly 参数必须是 Assembly 类型");
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
                if (str == null) throw new ExecutionException("import_type 参数必须是 string");
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
                if (userdata == null) throw new ExecutionException("generic_type 参数必须是 userdata");
                Type[] types = new Type[args.Length - 1];
                for (int i = 1; i < args.Length; ++i)
                {
                    ScriptUserdata type = args[i] as ScriptUserdata;
                    if (userdata == null) throw new ExecutionException("generic_type 参数必须是 userdata");
                    types[i - 1] = type.ValueType;
                }
                return userdata.ValueType.MakeGenericType(types);
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
                if (args.Length < 2) throw new ExecutionException("generic_method 参数必须大于等于2个");
                ScriptFunction func = args[0] as ScriptFunction;
                if (func == null) throw new ExecutionException("generic_method 参数必须是 function");
                ScorpioMethod method = func.Method;
                if (func == null) throw new ExecutionException("generic_method 参数必须是 程序函数");
                ScriptObject[] pars = new ScriptObject[args.Length - 1];
                Array.Copy(args, 1, pars, 0, pars.Length);
                return method.MakeGenericMethod(pars);
            }
        }
    }
}
