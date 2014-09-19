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
            script.SetObject("table", Table);
        }
        static LibraryTable()
        {
            Table.SetValue("count", new ScriptFunction(new count()));
        }
        private class count : ScorpioHandle
        {
            public object run(object[] args)
            {
                if (args.Length > 0 && args[0] is ScriptTable)
                    return ((ScriptTable)args[0]).Count();
                return 0;
            }
        }
    }
}
