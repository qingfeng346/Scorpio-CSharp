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
            script.SetObjectInternal("table", Table);
        }
        private class count : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return ((ScriptTable)args[0]).Count();
            }
        }
    }
}
