using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.Library
{
    public class LibraryTable
    {
        public static ScriptTable Table = new ScriptTable();
        public static void Load(Script script)
        {
            Table.SetValue("count", script.CreateFunction(new count()));
            script.SetObjectInternal("table", Table);
        }
        private class count : ScorpioHandle
        {
            public object run(object[] args)
            {
                return ((ScriptTable)args[0]).Count();
            }
        }
    }
}
