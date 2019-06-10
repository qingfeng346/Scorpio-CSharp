namespace Scorpio.Proto {
    public class ProtoBoolean {
        public static ScriptType Load(Script script, ScriptValue parentType) {
            var ret = new ScriptType("Bool", parentType);
            return ret;
        }
    }
}
