namespace Scorpio.Proto {
    public class ProtoBoolean {
        public static ScriptType Load(Script script, ScriptValue parentType) {
            var ret = script.CreateType("Bool", parentType);
            return ret;
        }
    }
}
