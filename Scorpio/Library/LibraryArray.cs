using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.Library
{
    public class LibraryArray
    {
        public static ScriptTable Table = new ScriptTable();
        public static void Load(Script script)
        {
            script.SetObject("array", Table);
        }
        static LibraryArray()
        {
            Table.SetValue("count", new ScriptFunction(new count()));
            Table.SetValue("insert", new ScriptFunction(new insert()));
            Table.SetValue("add", new ScriptFunction(new add()));
        }
        private class count : ScorpioHandle
        {
            public object run(object[] args) {
                if (args.Length > 0 && args[0] is ScriptArray)
                    return ((ScriptArray)args[0]).Count();
                return 0;
            }
        }
        private class insert : ScorpioHandle
        {
            public object run(object[] args) {
                if (args.Length > 2) {
                    ScriptArray array = (ScriptArray)args[0];
                    int index = ((ScriptNumber)args[1]).ToInt32();
                    ScriptObject obj = (ScriptObject)args[2];
                    array.Insert(index, obj);
                }
                return null;
            }
        }
        private class add : ScorpioHandle
        {
            public object run(object[] args) {
                if (args.Length > 1) {
                    ScriptArray array = (ScriptArray)args[0];
                    ScriptObject obj = (ScriptObject)args[1];
                    array.Add(obj);
                }
                return null;
            }
        }
    }
}
