using Scorpio.Function;

namespace Scorpio.Library
{
    public partial class LibraryUserdata {
        public static void Load(Script script) {
            var functions = new (string, ScorpioHandle)[] {
                ("fieldTypeOf", new fieldTypeOf(script)),
                ("isType", new isType()),
                ("extend", new extend(script)),
                ("bind", new bind()),
            };
            script.AddLibrary("userdata", functions);
        }
        private class fieldTypeOf : ScorpioHandle {
            private Script script;
            public fieldTypeOf(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return script.GetUserdataTypeValue(script.GetUserdataType(args[0].GetScriptValue.Type).GetVariableType(args[1].ToString())).Reference();
            }
        }
        private class isType : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length > 1 && args[0].valueType == ScriptValue.scriptValueType && args[1].valueType == ScriptValue.scriptValueType) {
                    return args[1].GetScriptValue.Type.IsAssignableFrom(args[0].GetScriptValue.Type) ? ScriptValue.True : ScriptValue.False;
                }
                return ScriptValue.False;
            }
        }
        private class extend : ScorpioHandle {
            public Script script;
            public extend(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                script.GetUserdataType(args[0].GetScriptValue.Type).SetValue(args[1].ToString(), args[2]);
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
