using System.Collections;

namespace Scorpio.Function {
    public class ScriptScriptAsyncFunction : ScriptScriptFunctionBase {
        public ScriptScriptAsyncFunction(Script script) : base(script) { }
        public override void Free() {
            base.Free();
            m_Script.Free(this);
        }
        IEnumerator Execute(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            //协程先进入一次函数,赋值局部变量
            var enumerator = m_Context.ExecuteCoroutine(thisObject, parameters, length, m_internalValues);
            enumerator.MoveNext();
            return enumerator;
        }
        IEnumerator Execute(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            //协程先进入一次函数,赋值局部变量
            var enumerator = m_Context.ExecuteCoroutine(thisObject, parameters, length, m_internalValues, baseType);
            enumerator.MoveNext();
            return enumerator;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(m_Script, m_Script.StartCoroutine(Execute(thisObject, parameters, length)));
        }
        internal override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return ScriptValue.CreateValue(m_Script, m_Script.StartCoroutine(Execute(thisObject, parameters, length, baseType)));
        }
        internal override ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(m_Script, Execute(thisObject, parameters, length));
        }
        internal override ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return ScriptValue.CreateValue(m_Script, Execute(thisObject, parameters, length, baseType));
        }
        public override ScriptFunction SetBindObject(ScriptValue obj) {
            return m_Script.NewAsyncBindFunction().SetContext(m_Context, obj);
        }
        public override string ToString() { return $"AsyncFunction<{FunctionName}>"; }
    }
    public class ScriptScriptAsyncBindFunction : ScriptScriptBindFunctionBase {
        public ScriptScriptAsyncBindFunction(Script script) : base(script) { }
        public override void Free() {
            base.Free();
            m_Script.Free(this);
        }
        IEnumerator Execute(ScriptValue[] parameters, int length) {
            //协程先进入一次函数,赋值局部变量
            var enumerator = m_Context.ExecuteCoroutine(m_BindObject, parameters, length, m_internalValues);
            enumerator.MoveNext();
            return enumerator;
        }
        IEnumerator Execute(ScriptValue[] parameters, int length, ScriptType baseType) {
            //协程先进入一次函数,赋值局部变量
            var enumerator = m_Context.ExecuteCoroutine(m_BindObject, parameters, length, m_internalValues, baseType);
            enumerator.MoveNext();
            return enumerator;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(m_Script, m_Script.StartCoroutine(Execute(parameters, length)));
        }
        internal override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return ScriptValue.CreateValue(m_Script, m_Script.StartCoroutine(Execute(parameters, length, baseType)));
        }
        internal override ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(m_Script, Execute(parameters, length));
        }
        internal override ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return ScriptValue.CreateValue(m_Script, Execute(parameters, length, baseType));
        }
        public override bool Equals(ScriptValue obj) {
            var func = obj.Get<ScriptScriptAsyncBindFunction>();
            return func != null && ReferenceEquals(m_Context, func.m_Context) && m_BindObject.Equals(func.m_BindObject);
        }
        public override ScriptFunction SetBindObject(ScriptValue obj) {
            return m_Script.NewAsyncBindFunction().SetContext(m_Context, obj);
        }
        public override string ToString() { return $"AsyncBindFunction<{FunctionName}>"; }
    }
}
