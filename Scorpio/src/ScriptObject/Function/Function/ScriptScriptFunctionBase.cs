using Scorpio.Runtime;
using Scorpio.Tools;

namespace Scorpio.Function {
    public abstract class ScriptScriptFunctionBase : ScriptFunction {
        protected ScriptContext m_Context;
        protected InternalValue[] m_internalValues;               //父级内部变量
        public ScriptScriptFunctionBase(Script script) : base(script) { }
        public ScriptScriptFunctionBase SetContext(ScriptContext context) {
            m_Context = context;
            if (context.m_FunctionData.internalCount > 0) {
                m_internalValues = m_Script.NewIntervalValues();
            }
            return this;
        }
        public override void Free() {
            base.Free();
            ReleaseInternal();
            Release();
            m_Context = null;
        }
        protected void ReleaseInternal() {
            ScorpioUtil.Free(m_Script, m_internalValues, m_Context.m_FunctionData.internalCount);
            m_internalValues = null;
        }
        public void SetInternal(int index, InternalValue value) {
            m_internalValues[index] = value.Reference();
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
            base.Free();
            ReleaseInternal();
            Release();
            m_BindObject.Free();
            m_Context = null;
        }
        public override ScriptValue BindObject => m_BindObject;
    }
}
