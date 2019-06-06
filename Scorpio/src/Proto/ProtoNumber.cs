namespace Scorpio.Proto {
    public class ProtoNumber {
        public static ScriptType Load(Script script, ScriptValue parentType) {
            var ret = script.CreateType("Number", parentType);
            return ret;
        }
    }
}
