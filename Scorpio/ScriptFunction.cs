using System;
using System.Collections.Generic;
using Scorpio.Runtime;
using Scorpio.Variable;
using Scorpio.Exception;
namespace Scorpio
{
    //C#函数指针
    public delegate object ScorpioFunction(ScriptObject[] Parameters);
    //C#类执行
    public interface ScorpioHandle {
        object Call(ScriptObject[] Parameters);
    }
    /// <summary> 函数类型 </summary>

    //脚本函数类型
    public class ScriptFunction : ScriptObject
    {
        public enum FunstionType {
            //脚本函数
            Script,
            //注册的C函数
            Function,
            //注册的C函数
            Handle,
            //函数
            Method,
        }
        public FunstionType FunctionType { get; private set; }                  //函数类型 （是 脚本函数 还是 程序函数）
        public bool IsStatic { get; private set; }                              //是否是静态函数（不是table内部函数）
        private ScorpioScriptFunction m_ScriptFunction;                         //脚本函数
        private ScorpioFunction m_Function;                                     //程序函数指针
        private ScorpioHandle m_Handle;                                         //程序函数执行类
        private ScorpioMethod m_Method;                                         //程序函数
        private ScriptContext m_ParentContext;                                  //父级堆栈
        public ScorpioMethod Method { get { return m_Method; } }                //返回程序函数对象
        private Dictionary<String, ScriptObject> m_stackObject = new Dictionary<String, ScriptObject>();    //函数变量
        public override ObjectType Type { get { return ObjectType.Function; } }
        public ScriptFunction(Script script, ScorpioFunction function) : this(script, "", function) { }
        public ScriptFunction(Script script, String strName, ScorpioFunction function) : base(script) {
            this.m_Function = function;
            Initialize(strName, FunstionType.Function);
        }
        public ScriptFunction(Script script, ScorpioHandle handle) : this(script, handle.GetType().FullName, handle) { }
        public ScriptFunction(Script script, String strName, ScorpioHandle handle) : base(script) {
            this.m_Handle = handle;
            Initialize(strName, FunstionType.Handle);
        }
        public ScriptFunction(Script script, ScorpioMethod method) : this(script, method.MethodName, method) { }
        public ScriptFunction(Script script, String strName, ScorpioMethod method) : base(script)
        {
            this.m_Method = method;
            Initialize(strName, FunstionType.Method);
        }
        internal ScriptFunction(Script script, String strName, ScorpioScriptFunction function) : base(script)
        {
            this.IsStatic = true;
            this.m_ScriptFunction = function;
            Initialize(strName, FunstionType.Script);
        }
        private void Initialize(String strName, FunstionType funcType)
        {
            Name = strName;
            FunctionType = funcType;
            m_ParentContext = null;
        }
        public void SetTable(ScriptTable table) {
            if (FunctionType == FunstionType.Script) {
                IsStatic = false;
                m_stackObject["this"] = table;
                m_stackObject["self"] = table;
            }
        }
        public override void SetValue(object key, ScriptObject value) {
            if (!(key is string)) throw new ExecutionException(this.Script, "Function SetValue只支持String类型 key值为:" + key);
            m_stackObject[(string)key] = value;
        }
        public override ScriptObject GetValue(object key) {
            if (!(key is string)) throw new ExecutionException(this.Script, "Function GetValue只支持String类型 key值为:" + key);
            string skey = (string)key;
            return m_stackObject.ContainsKey(skey) ? m_stackObject[skey] : Script.Null;
        }
        public ScriptFunction SetParentContext(ScriptContext context) {
            if (FunctionType == FunstionType.Script)
                m_ParentContext = context;
            return this;
        }
        public override object Call(ScriptObject[] parameters) {
            if (FunctionType == FunstionType.Script) {
                return m_ScriptFunction.Call(m_ParentContext, m_stackObject, parameters);
            } else {
                try {
                    if (FunctionType == FunstionType.Handle){
                        return m_Handle.Call(parameters);
                    } else if (FunctionType == FunstionType.Function) {
                        return m_Function(parameters);
                    } else if (FunctionType == FunstionType.Method) {
                        return m_Method.Call(parameters);
                    }
                } catch (System.Exception ex) {
                    throw new ExecutionException(Script, "CallFunction [" + Name + "] is error : " + ex.ToString());
                }
            }
            return null;
        }
        public override ScriptObject Clone()
        {
            if (FunctionType != FunstionType.Script)
                return base.Clone();
            ScriptFunction ret = new ScriptFunction(Script, Name, m_ScriptFunction);
            ret.IsStatic = IsStatic;
            return ret;
        }
        public override string ToString() { return "Function(" + Name + ")"; }
        public override string ToJson() { return "\"Function\""; }
    }
}
