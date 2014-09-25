using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.Exception;
namespace Scorpio.Library
{
    public class LibraryBasis
    {
        private class ArrayPair : ScorpioHandle
        {
            List<ScriptObject>.Enumerator m_ListEnumerator;
            public ArrayPair(ScriptObject obj)
            {
                m_ListEnumerator = ((ScriptArray)obj).GetIterator();
            }
            public object run(object[] args)
            {
                if (m_ListEnumerator.MoveNext())
                    return m_ListEnumerator.Current;
                return null;
            }
        }
        private class TablePair : ScorpioHandle
        {
            Dictionary<String, ScriptObject>.Enumerator m_TableEnumerator;
            public TablePair(ScriptObject obj)
            {
                m_TableEnumerator = ((ScriptTable)obj).GetIterator();
            }
            public object run(object[] args)
            {
                if (m_TableEnumerator.MoveNext())
                {
                    KeyValuePair<string, ScriptObject> v = m_TableEnumerator.Current;
                    ScriptTable table = new ScriptTable();
                    table.SetValue("key", new ScriptString(v.Key));
                    table.SetValue("value", v.Value);
                    return table;
                }
                return null;
            }
        }
        public static void Load(Script script)
        {
            script.SetObjectInternal("print", script.CreateFunction(new print()));
            script.SetObjectInternal("pair", script.CreateFunction(new pair(script)));
            script.SetObjectInternal("tonumber", script.CreateFunction(new tonumber(script)));
            script.SetObjectInternal("tostring", script.CreateFunction(new tostring(script)));
        }
        private class pair : ScorpioHandle
        {
            private Script m_script;
            public pair(Script script)
            {
                m_script = script;
            }
            public object run(object[] args)
            {
                ScriptObject obj = args[0] as ScriptObject;
                if (obj is ScriptArray)
                    return m_script.CreateFunction(new ArrayPair(obj));
                else if (obj is ScriptTable)
                    return m_script.CreateFunction(new TablePair(obj));
                throw new ExecutionException("pair必须用语table或array类型");
            }
        }
        private class print : ScorpioHandle
        {
            public object run(object[] args)
            {
                for (int i = 0; i < args.Length; ++i) {
                    Console.WriteLine(args[i].ToString());
                }
                return null;
            }
        }
        public class tonumber : ScorpioHandle
        {
            private Script m_script;
            public tonumber(Script script)
            {
                m_script = script;
            }
            public object run(object[] args)
            {
                ScriptObject obj = args[0] as ScriptObject;
                if (obj is ScriptNumber) return obj;
                return m_script.CreateNumber(obj.ObjectValue);
            }
        }
        public class tostring : ScorpioHandle
        {
            private Script m_script;
            public tostring(Script script)
            {
                m_script = script;
            }
            public object run(object[] args)
            {
                ScriptObject obj = args[0] as ScriptObject;
                if (obj is ScriptString) return obj;
                return m_script.CreateString(obj.ToString());
            }
        }
    }
}
