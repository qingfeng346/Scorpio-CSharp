using Scorpio.Runtime;
namespace Scorpio.Function {
    public abstract class ScriptScriptFunctionBase : ScriptFunction {
        protected ScriptContext m_Context;
        protected InternalValue[] m_internalValues;               //父级内部变量
        public ScriptScriptFunctionBase(ScriptContext context) : base(context.m_script) {
            m_Context = context;
            if (context.m_FunctionData.internalCount > 0) {
                m_internalValues = new InternalValue[context.m_FunctionData.internalCount];
            }
        }
        public void SetInternal(int index, InternalValue value) {
            m_internalValues[index] = value;
        }
#if SCORPIO_DEBUG
        public ScriptContext Context {
            get { return m_Context; }
            set {
                m_Context = value;
                m_internalValues = new InternalValue[value.m_FunctionData.internalCount];
            }
        }
#endif
    }
    public abstract class ScriptScriptBindFunctionBase : ScriptScriptFunctionBase {
        protected ScriptValue m_BindObject;
        public ScriptScriptBindFunctionBase(ScriptContext context, ScriptValue bindObject) : base(context) {
            m_BindObject = bindObject;
        }
        public override ScriptValue BindObject => m_BindObject;
        public override bool Equals(ScriptValue obj) {
            var func = obj.Get<ScriptScriptBindFunctionBase>();
            return func != null && ReferenceEquals(m_Context, func.m_Context) && m_BindObject.Equals(func.m_BindObject);
        }
    }
}
