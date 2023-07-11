namespace Scorpio.Proto {
    public class ProtoStringBuilder {
        public static ScriptType Load(Script script, ScriptType ret) {
            var functions = new (string, ScorpioHandle)[] {
                ("length", new length()),
                ("setLength", new setLength()),
                ("clear", new clear()),
                ("append", new append()),
                ("appendFormat", new appendFormat()),
                ("insert", new insert()),
                ("remove", new remove()),
                ("replace", new replace()),
            };
            ret.SetFunctions(script, functions);
            return ret;
        }
        private class length : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((double)thisObject.Get<ScriptStringBuilder>().Builder.Length);
            }
        }
        private class setLength : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptStringBuilder>().Builder.Length = args[0].ToInt32();
                return thisObject.Reference();
            }
        }
        private class clear : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptStringBuilder>().Builder.Clear();
                return thisObject.Reference();
            }
        }
        private class append : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var builder = thisObject.Get<ScriptStringBuilder>().Builder;
                for (var i = 0; i < length; ++i) {
                    builder.Append(args[i].Value);
                }
                return thisObject.Reference();
            }
        }
        private class appendFormat : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var objs = new object[length - 1];
                for (var i = 1; i < length; ++i) {
                    objs[i - 1] = args[i].Value;
                }
                thisObject.Get<ScriptStringBuilder>().Builder.AppendFormat(args[0].ToString(), objs);
                return thisObject.Reference();
            }
        }
        private class insert : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptStringBuilder>().Builder.Insert(args[0].ToInt32(), args[1].Value);
                return thisObject.Reference();
            }
        }
        private class remove : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptStringBuilder>().Builder.Remove(args[0].ToInt32(), args[1].ToInt32());
                return thisObject.Reference();
            }
        }
        private class replace : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptStringBuilder>().Builder.Replace(args[0].ToString(), args[1].ToString());
                return thisObject.Reference();
            }
        }
    }
}
