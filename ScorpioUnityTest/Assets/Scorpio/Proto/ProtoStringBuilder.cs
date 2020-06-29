namespace Scorpio.Proto {
    public class ProtoStringBuilder {
        public static ScriptType Load(Script script, ScriptType ret) {
            ret.SetValue("length", script.CreateFunction(new length()));
            ret.SetValue("setLength", script.CreateFunction(new setLength()));
            ret.SetValue("clear", script.CreateFunction(new clear()));
            ret.SetValue("append", script.CreateFunction(new append()));
            ret.SetValue("appendFormat", script.CreateFunction(new appendFormat()));
            ret.SetValue("insert", script.CreateFunction(new insert()));
            ret.SetValue("remove", script.CreateFunction(new remove()));
            ret.SetValue("replace", script.CreateFunction(new replace()));
            return ret;
        }
        private class length : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(thisObject.Get<ScriptStringBuilder>().Builder.Length);
            }
        }
        private class setLength : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptStringBuilder>().Builder.Length = args[0].ToInt32();
                return thisObject;
            }
        }
        private class clear : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptStringBuilder>().Builder.Clear();
                return thisObject;
            }
        }
        private class append : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var builder = thisObject.Get<ScriptStringBuilder>().Builder;
                for (var i = 0; i < length; ++i) {
                    builder.Append(args[i].Value);
                }
                return thisObject;
            }
        }
        private class appendFormat : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var objs = new object[length - 1];
                for (var i = 1; i < length; ++i) {
                    objs[i - 1] = args[i].Value;
                }
                thisObject.Get<ScriptStringBuilder>().Builder.AppendFormat(args[0].ToString(), objs);
                return thisObject;
            }
        }
        private class insert : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptStringBuilder>().Builder.Insert(args[0].ToInt32(), args[1].Value);
                return thisObject;
            }
        }
        private class remove : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptStringBuilder>().Builder.Remove(args[0].ToInt32(), args[1].ToInt32());
                return thisObject;
            }
        }
        private class replace : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptStringBuilder>().Builder.Replace(args[0].ToString(), args[1].ToString());
                return thisObject;
            }
        }
    }
}
