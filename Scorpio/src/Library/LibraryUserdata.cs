using Scorpio.Userdata;
namespace Scorpio.Library {
    public partial class LibraryUserdata {
        public static void Load(Script script) {
            var map = new ScriptMapString(script);
            map.SetValue("fieldTypeOf", script.CreateFunction(new fieldTypeOf()));
            map.SetValue("isType", script.CreateFunction(new isType()));
            map.SetValue("extend", script.CreateFunction(new extend()));
            script.SetGlobal("userdata", new ScriptValue(map));
        }
        private class fieldTypeOf : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return TypeManager.GetUserdataType(TypeManager.GetType(args[0].scriptValue.Type).GetVariableType(args[1].ToString()));
            }
        }
        private class isType : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length > 1 && args[0].valueType == ScriptValue.scriptValueType && args[1].valueType == ScriptValue.scriptValueType) {
                    return args[1].scriptValue.Type.IsAssignableFrom(args[0].scriptValue.Type) ? ScriptValue.True : ScriptValue.False;
                }
                return ScriptValue.False;
            }
        }
        private class extend : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                TypeManager.GetType(args[0].scriptValue.Type).SetValue(args[1].ToString(), args[2]);
                return ScriptValue.Null;
            }
        }
    }
}
