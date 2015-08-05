using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Library
{
    public class LibraryMath
    {
        public static void Load(Script script) {
            ScriptTable Table = script.CreateTable();
            script.SetObjectInternal("math", Table);
        }
    }
}
