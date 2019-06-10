namespace Scorpio.Proto {
    public class ProtoNumber {
        public static ScriptType Load(Script script, ScriptValue parentType) {
            var ret = new ScriptType("Number", parentType);
            return ret;
        }
    }
}
