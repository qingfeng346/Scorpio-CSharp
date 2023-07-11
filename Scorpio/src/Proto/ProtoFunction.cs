using System;

namespace Scorpio.Proto {
    public class ProtoFunction {
        public static ScriptType Load(Script script, ScriptType ret) {
            var functions = new (string, ScorpioHandle)[] {
                ("bind", new bind()),
                ("bindObject", new bindObject()),
                ("call", new call()),
            };
            ret.SetFunctions(script, functions);
            return ret;
        }
        private class bind : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(thisObject.Get<ScriptFunction>().SetBindObject(args[0]));
            }
        }
        private class bindObject : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptFunction>().BindObject.Reference();
            }
        }
        private class call : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var thisValue = args[0];
                Array.Copy(args, 1, args, 0, length - 1);
                return thisObject.Call(thisValue, args, length - 1);
            }
        }
    }
}
