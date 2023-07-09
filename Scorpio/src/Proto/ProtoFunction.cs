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
                return thisObject.Get<ScriptFunction>().BindObject;
            }
        }
        private class call : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var count = length - 1;
                var pars = new ScriptValue[count];
                for (var i = 0 ; i < count; ++i) {
                    pars[i] = args[i + 1];
                }
                return thisObject.Call(args[0], pars, count);
            }
        }
    }
}
