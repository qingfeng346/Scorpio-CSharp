using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Proto {
    public class ProtoBoolean {
        public static ScriptType Load(Script script, ScriptType parentType) {
            var ret = script.CreateType("Boolean", parentType);
            return ret;
        }
    }
}
