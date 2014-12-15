using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Library
{
    public class LibraryJson
    {
        public static void Load(Script script)
        {
            ScriptTable Table = script.CreateTable();
            Table.SetValue("encode", script.CreateFunction(new encode()));
            Table.SetValue("decode", script.CreateFunction(new decode(script)));
            script.SetObjectInternal("json", Table);
        }
        private class encode : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return args[0].ToJson();
            }
        }
        private class decode : ScorpioHandle
        {
            private Script m_Script;
            public decode(Script script)
            {
                m_Script = script;
            }
            public object Call(ScriptObject[] args)
            {
                return m_Script.LoadString("return " + args[0].ToString());
            }
        }
    }
}
