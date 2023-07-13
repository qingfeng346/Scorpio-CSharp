using Scorpio.Runtime;
using Scorpio.Tools;

namespace Scorpio.Function {
    public abstract class ScriptScriptFunctionBase : ScriptFunction {
        protected ScriptContext m_Context;
        protected InternalValue[] m_internalValues;               //父级内部变量
        public InternalValue[] InternalValues => m_internalValues;
        public ScriptScriptFunctionBase(Script script) : base(script) { }
        public ScriptScriptFunctionBase SetContext(ScriptContext context) {
            m_Context = context;
            if (context.m_FunctionData.internalCount > 0) {
                m_internalValues = m_Script.NewIntervalValues();
            }
            return this;
        }
        public override void Free() {
            DelRecord();
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
        public override bool Equals(ScriptValue obj) {
            var func = obj.Get<ScriptScriptFunctionBase>();
            return func != null && ReferenceEquals(m_Context, func.m_Context);
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
            DelRecord();
            ReleaseInternal();
            Release();
            m_BindObject.Free();
            m_Context = null;
        }
        public override void gc() {
            Release();
            m_BindObject.Free();
        }
        public override ScriptValue BindObject => m_BindObject;
        public override bool Equals(ScriptValue obj) {
            var func = obj.Get<ScriptScriptBindFunctionBase>();
            return func != null && ReferenceEquals(m_Context, func.m_Context) && m_BindObject.Equals(func.m_BindObject);
        }
    }
}
