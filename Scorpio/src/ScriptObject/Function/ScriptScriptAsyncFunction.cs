using Scorpio.Runtime;
using Scorpio.Tools;
namespace Scorpio.Function {
    public class ScriptScriptAsyncFunction : ScriptFunction {
        protected ScriptContext m_Context;
        protected InternalValue[] m_internalValues;               //父级内部变量
        public ScriptScriptAsyncFunction(Script script) : base(script, "") { }
        public ScriptScriptAsyncFunction SetContext(ScriptContext context) {
            m_Context = context;
            m_internalValues = new InternalValue[context.internalCount];
            return this;
        }
        public void SetInternal(int index, InternalValue value) {
            m_internalValues[index] = value;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(m_Script.StartCoroutine(m_Context.ExecuteCoroutine(thisObject, parameters.CloneParameters(length), length, m_internalValues)));
        }
        internal override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return ScriptValue.CreateValue(m_Script.StartCoroutine(m_Context.ExecuteCoroutine(thisObject, parameters.CloneParameters(length), length, m_internalValues, baseType)));
        }
        internal override ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(m_Context.ExecuteCoroutine(thisObject, parameters.CloneParameters(length), length, m_internalValues));
        }
        internal override ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return ScriptValue.CreateValue(m_Context.ExecuteCoroutine(thisObject, parameters.CloneParameters(length), length, m_internalValues, baseType));
        }
//#if SCORPIO_DEBUG
//        public ScriptContext Context {
//            get { return m_Context; }
//            set {
//                m_Context = value;
//                m_internalValues = new InternalValue[value.internalCount];
//            }
//        }
//        public override ScriptFunction SetBindObject(ScriptValue obj) {
//            return new ScriptScriptAsyncBindFunction(this, obj);
//        }
//#else
        public override ScriptFunction SetBindObject(ScriptValue obj) {
            return m_Script.NewAsyncLambdaFunction().SetContext(m_Context, obj);
        }
//#endif
    }
    public class ScriptScriptAsyncLambdaFunction : ScriptScriptAsyncFunction {
        private ScriptValue m_BindObject = ScriptValue.Null;
        public ScriptScriptAsyncLambdaFunction(Script script) : base(script) {
        }
        public ScriptScriptAsyncLambdaFunction SetContext(ScriptContext context, ScriptValue bindObject) {
            SetContext(context);
            m_BindObject = bindObject;
            return this;
        }
        public override ScriptValue BindObject => m_BindObject;
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(m_Script.StartCoroutine(m_Context.ExecuteCoroutine(m_BindObject, parameters.CloneParameters(length), length, m_internalValues)));
        }
        internal override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return ScriptValue.CreateValue(m_Script.StartCoroutine(m_Context.ExecuteCoroutine(m_BindObject, parameters.CloneParameters(length), length, m_internalValues, baseType)));
        }
        internal override ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(m_Context.ExecuteCoroutine(m_BindObject, parameters.CloneParameters(length), length, m_internalValues));
        }
        internal override ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return ScriptValue.CreateValue(m_Context.ExecuteCoroutine(m_BindObject, parameters.CloneParameters(length), length, m_internalValues, baseType));
        }
        public override bool Equals(ScriptValue obj) {
            var func = obj.Get<ScriptScriptAsyncLambdaFunction>();
            return func != null && ReferenceEquals(m_Context, func.m_Context) && m_BindObject.Equals(func.m_BindObject);
        }
    }
//#if SCORPIO_DEBUG
//    public class ScriptScriptAsyncBindFunction : ScriptScriptAsyncFunction {
//        private ScriptScriptAsyncFunction m_Function;
//        private ScriptValue m_BindObject = ScriptValue.Null;
//        public ScriptScriptAsyncBindFunction(ScriptScriptAsyncFunction function, ScriptValue bindObject) : base(function.Context) {
//            m_Function = function;
//            m_BindObject = bindObject;
//        }
//        public override ScriptValue BindObject => m_BindObject;
//        public new void SetInternal(int index, InternalValue value) {
//            m_Function.SetInternal(index, value);
//        }
//        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
//            return m_Function.Call(m_BindObject, parameters, length);
//        }
//        internal override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
//            return m_Function.Call(m_BindObject, parameters, length, baseType);
//        }
//        internal override ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length) {
//            return m_Function.CallAsync(m_BindObject, parameters, length);
//        }
//        internal override ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
//            return m_Function.CallAsync(m_BindObject, parameters, length, baseType);
//        }
//        public override bool Equals(ScriptValue obj) {
//            var func = obj.Get<ScriptScriptAsyncBindFunction>();
//            return func != null && ReferenceEquals(m_Function.Context, func.m_Context) && m_BindObject.Equals(func.m_BindObject);
//        }
//    }
//#else
    //public class ScriptScriptAsyncBindFunction : ScriptScriptAsyncFunction {
    //    public override bool Equals(ScriptValue obj) {
    //        var func = obj.Get<ScriptScriptAsyncBindFunction>();
    //        return func != null && ReferenceEquals(m_Context, func.m_Context) && m_BindObject.Equals(func.m_BindObject);
    //    }
    //}
//#endif
}
