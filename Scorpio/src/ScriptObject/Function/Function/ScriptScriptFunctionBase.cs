using Scorpio.Runtime;
using Scorpio.Tools;

namespace Scorpio.Function {
    public abstract class ScriptScriptFunctionBase : ScriptFunction {
        protected ScriptContext m_Context;
        protected InternalValue[] m_internalValues;               //父级内部变量
        public ScriptScriptFunctionBase(Script script) : base(script) { }
        public ScriptScriptFunctionBase SetContext(ScriptContext context) {
            m_Context = context;
            m_internalValues = new InternalValue[context.internalCount];
            return this;
        }
        public override void Free() {
            ReleaseInternal();
            Release();
        }
        protected void ReleaseInternal() {
            ScorpioUtil.FreeInternal(m_internalValues);
            m_internalValues = null;
        }
        public void SetInternal(int index, InternalValue value) {
            if (m_internalValues[index] != null)
                m_internalValues[index].value.Free();
            value.value.Reference();
            m_internalValues[index] = value;
        }
    }
    public abstract class ScriptScriptBindFunctionBase : ScriptScriptFunctionBase {
        protected ScriptValue m_BindObject = ScriptValue.Null;
        public ScriptScriptBindFunctionBase(Script script) : base(script) {
        }
        public ScriptScriptBindFunctionBase SetContext(ScriptContext context, ScriptValue bindObject) {
            SetContext(context);
            m_BindObject.CopyFrom(bindObject);
            return this;
        }
        public override void Free() {
            ReleaseInternal();
            Release();
            m_BindObject.Free();
        }
        public override ScriptValue BindObject => m_BindObject;
    }
}
