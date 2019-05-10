using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Proto {
    public class ProtoNumber {
        public static ScriptType Load(Script script, ScriptType parentType) {
            var ret = script.CreateType("Number", parentType);
            return ret;
        }
    }
}
