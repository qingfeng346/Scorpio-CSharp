using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Proto {
    public class ProtoFunction {
        public static ScriptType Load(Script script, ScriptType parentType) {
            var ret = script.CreateType("Function", parentType);
            ret.SetValue("bind", script.CreateFunction(new bind()));
            return ret;
        }
        private class bind : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(thisObject.Get<ScriptFunction>().SetBindObject(args[0]));
            }
        }
    }
}
