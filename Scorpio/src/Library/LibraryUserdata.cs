namespace Scorpio.Library
{
    public partial class LibraryUserdata {
        public static void Load(Script script) {
            var functions = new (string, ScorpioHandle)[] {
                ("fieldTypeOf", new fieldTypeOf(script)),
                ("isType", new isType()),
                ("extend", new extend(script)),
            };
            var map = script.NewMapString();
            map.SetCapacity(functions.Length);
            foreach (var (name, func) in functions) {
                map.SetValueNoReference(name, script.CreateFunction(func));
            }
            script.SetGlobalNoReference("userdata", map);
        }
        private class fieldTypeOf : ScorpioHandle {
            private Script script;
            public fieldTypeOf(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return script.GetUserdataTypeValue(script.GetUserdataType(args[0].scriptValue.Type).GetVariableType(args[1].ToString())).Reference();
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
            public Script script;
            public extend(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                script.GetUserdataType(args[0].scriptValue.Type).SetValue(args[1].ToString(), args[2]);
                return ScriptValue.Null;
            }
        }
    }
}
