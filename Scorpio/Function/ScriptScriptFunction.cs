using System;
using System.Collections.Generic;
using Scorpio;
using Scorpio.Runtime;
using Scorpio.Variable;
using Scorpio.Exception;
namespace Scorpio.Function {
    public class ScriptScriptFunction : ScriptFunction {
        private ScorpioScriptFunction m_ScriptFunction;                         //脚本函数
        private ScriptContext m_ParentContext;                                  //父级堆栈
        private bool m_IsStaticFunction;                                        //是否是静态函数(不是table内部函数)
        private Dictionary<String, ScriptObject> m_stackObject = new Dictionary<String, ScriptObject>();    //函数变量
        public bool IsStaticFunction { get { return m_IsStaticFunction; } }
        internal ScriptScriptFunction(Script script, String name, ScorpioScriptFunction function) : base(script, name)
        {
            this.m_IsStaticFunction = true;
            this.m_ScriptFunction = function;
        }
        public override int GetParamCount() { return m_ScriptFunction.GetParameterCount(); }
        public override bool IsParams() { return m_ScriptFunction.IsParams(); }
        public override bool IsStatic() { return m_IsStaticFunction; }
        public override ScriptArray GetParams() { return m_ScriptFunction.GetParameters(); }
        public override void SetValue(object key, ScriptObject value) {
            if (!(key is string)) throw new ExecutionException(this.m_Script, "Function SetValue只支持String类型 key值为:" + key);
            m_stackObject[(string)key] = value;
        }
        public override ScriptObject GetValue(object key) {
            if (!(key is string)) throw new ExecutionException(this.m_Script, "Function GetValue只支持String类型 key值为:" + key);
            string skey = (string)key;
            return m_stackObject.ContainsKey(skey) ? m_stackObject[skey] : m_Script.Null;
        }
        public void SetTable(ScriptTable table) {
            m_IsStaticFunction = false;
            m_stackObject["this"] = table;
            m_stackObject["self"] = table;
        }
        public ScriptScriptFunction SetParentContext(ScriptContext context) {
            m_ParentContext = context;
            return this;
        }
        public ScriptScriptFunction Create() {
            ScriptScriptFunction ret = new ScriptScriptFunction(m_Script, Name, m_ScriptFunction);
            ret.m_IsStaticFunction = IsStaticFunction;
            return ret;
        }
        public override object Call(ScriptObject[] parameters) {
            return m_ScriptFunction.Call(m_ParentContext, m_stackObject, parameters);
        }
        public override ScriptObject Clone() {
            return Create();
        }
    }
}
