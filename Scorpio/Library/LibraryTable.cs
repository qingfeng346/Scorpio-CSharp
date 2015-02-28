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
            script.SetObjectInternal("table", Table);
        }
        private class count : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return ((ScriptTable)args[0]).Count();
            }
        }
        private class clear : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                ((ScriptTable)args[0]).Clear();
                return null;
            }
        }
    }
}
