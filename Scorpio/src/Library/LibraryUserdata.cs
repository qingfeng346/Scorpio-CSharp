using Scorpio.Function;
namespace Scorpio.Library {
    public partial class LibraryUserdata {
        public static void Load(Script script) {
            var functions = new (string, ScorpioHandle)[] {
                ("fieldTypeOf", new fieldTypeOf()),
                ("isType", new isType()),
                ("extend", new extend()),
                ("bind", new bind()),
            };
            script.AddLibrary("userdata", functions);
        }
        private class fieldTypeOf : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return ScorpioTypeManager.GetUserdataType(ScorpioTypeManager.GetType(args[0].scriptValue.Type).GetVariableType(args[1].ToString()));
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
                ScorpioTypeManager.GetType(args[0].scriptValue.Type).SetValue(args[1].ToString(), args[2]);
                return ScriptValue.Null;
            }
        }
        private class bind : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].Get<ScriptMethodFunction>().Bind(args[1]));
            }
        }
    }
}
