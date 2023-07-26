using Scorpio.Runtime;
using System.Collections;
namespace Scorpio.Function {
    public class ScriptScriptAsyncFunction : ScriptScriptFunctionBase {
        public ScriptScriptAsyncFunction(ScriptContext context) : base(context) { }
        public override ScriptFunction SetBindObject(ScriptValue bindObject) {
            return new ScriptScriptAsyncBindFunction(m_Context, bindObject);
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
            return ScriptValue.CreateValue(m_Script.StartCoroutine(Execute(thisObject, parameters, length)));
        }
        internal override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return ScriptValue.CreateValue(m_Script.StartCoroutine(Execute(thisObject, parameters, length, baseType)));
        }
        internal override ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(Execute(thisObject, parameters, length));
        }
        internal override ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return ScriptValue.CreateValue(Execute(thisObject, parameters, length, baseType));
        }
    }
    public class ScriptScriptAsyncBindFunction : ScriptScriptBindFunctionBase {
        public ScriptScriptAsyncBindFunction(ScriptContext context, ScriptValue bindObject) : base(context, bindObject) { }
        public override ScriptFunction SetBindObject(ScriptValue bindObject) {
            return new ScriptScriptAsyncBindFunction(m_Context, bindObject);
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
            return ScriptValue.CreateValue(m_Script.StartCoroutine(Execute(parameters, length)));
        }
        internal override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return ScriptValue.CreateValue(m_Script.StartCoroutine(Execute(parameters, length, baseType)));
        }
        internal override ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(Execute(parameters, length));
        }
        internal override ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return ScriptValue.CreateValue(Execute(parameters, length, baseType));
        }
    }
}
