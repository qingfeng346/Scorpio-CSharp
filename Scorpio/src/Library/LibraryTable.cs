using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.Library
{
    public class LibraryTable
    {
        public static void Load(Script script)
        {
            ScriptTable Table = script.CreateTable();
            Table.SetValue("count", script.CreateFunction(new count()));
            Table.SetValue("clear", script.CreateFunction(new clear()));
            Table.SetValue("remove", script.CreateFunction(new remove()));
            Table.SetValue("containskey", script.CreateFunction(new containskey()));
			Table.SetValue("keys", script.CreateFunction(new keys()));
			Table.SetValue("values", script.CreateFunction(new values()));
            script.SetObjectInternal("table", Table);
        }
        private class count : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                return ((ScriptTable)args[0]).Count();
            }
        }
        private class clear : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                ((ScriptTable)args[0]).Clear();
                return null;
            }
        }
        private class remove : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                ((ScriptTable)args[0]).Remove(args[1].ObjectValue);
                return null;
            }
        }
        private class containskey : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                return ((ScriptTable)args[0]).HasValue(args[1].ObjectValue);
            }
        }
		private class keys : ScorpioHandle {
			public object Call(ScriptObject[] args) {
				return ((ScriptTable)args [0]).GetKeys ();
			}
		}
		private class values : ScorpioHandle {
			public object Call(ScriptObject[] args) {
				return ((ScriptTable)args [0]).GetValues ();
			}
		}
    }
}
