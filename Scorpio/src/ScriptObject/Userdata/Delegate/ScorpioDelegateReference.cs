#if SCORPIO_DEBUG
using Scorpio.Tools;
using System;
namespace Scorpio {
    public class ScorpioDelegateReference {
        private ScriptObject scriptObject;
        private ulong index;
        public ScorpioDelegateReference(ScriptObject scriptObject, Type type) {
            this.scriptObject = scriptObject;
            index = ScorpioProfiler.AddCustomReference(scriptObject.Id, $"Delegate : {type.FullName}");
        }
        ~ScorpioDelegateReference() {
            ScorpioProfiler.DelCustomReference(scriptObject.Id, index);
        }
        public ScriptValue call(params object[] args) {
            return scriptObject.call(ScriptValue.Null, args);
        }
    }
}
#endif
