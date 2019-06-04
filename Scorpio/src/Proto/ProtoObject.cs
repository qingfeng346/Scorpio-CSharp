namespace Scorpio.Proto {
    public class ProtoObject {
        public static ScriptType Load(Script script) {
            var ret = script.CreateType("Object", null);
            ret.SetValue("toString", script.CreateFunction(new toString()));
            return ret;
        }
        private class toString : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(thisObject.ToString());
            }
        }
    }
}