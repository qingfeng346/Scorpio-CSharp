using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.Library
{
    public class LibraryBasis
    {
        private class TablePair : ScorpioHandle
        {
            int type = 0;
            List<ScriptObject>.Enumerator m_ListEnumerator;
            Dictionary<String, ScriptObject>.Enumerator m_TableEnumerator;
            public TablePair(ScriptObject obj)
            {
                if (obj.IsArray) {
                    type = 1;
                    m_ListEnumerator = ((ScriptArray)obj).GetIterator();
                } else if (obj.IsTable) {
                    type = 2;
                    m_TableEnumerator = ((ScriptTable)obj).GetIterator();
                }
            }
            public object run(object[] args)
            {
                if (type == 1) {
                    if (m_ListEnumerator.MoveNext())
                        return m_ListEnumerator.Current;
                } else if (type == 2) {
                    if (m_TableEnumerator.MoveNext()) {
                        KeyValuePair<string, ScriptObject> v = m_TableEnumerator.Current;
                        ScriptTable table = new ScriptTable();
                        table.SetValue("key", new ScriptString(v.Key));
                        table.SetValue("value", v.Value);
                        return table;
                    }
                }
                return null;
            }
        }
        public static void Load(Script script)
        {
            script.SetObject("print", new ScriptFunction(new print()));
            script.SetObject("pair", new ScriptFunction(new pair()));
        }
        private class pair : ScorpioHandle
        {
            public object run(object[] args)
            {
                return new ScriptFunction(new TablePair((ScriptObject)args[0]));
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
    }
}
