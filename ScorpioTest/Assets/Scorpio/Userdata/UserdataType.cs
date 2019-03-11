using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Scorpio;
using Scorpio.Exception;
using Scorpio.Variable;
using Scorpio.Compiler;
namespace Scorpio.Userdata {
    /// <summary> 保存一个类的所有元素 </summary>
    public abstract class UserdataType {
        protected Script m_Script;                                                              //脚本系统
        protected Type m_Type;                                                                  //类型
        protected Dictionary<string, string> m_Rename = new Dictionary<string, string>();       //变量重命名
        protected Dictionary<TokenType, ScorpioMethod> m_ComputeMethods = new Dictionary<TokenType, ScorpioMethod>();       //运算符重载函数
        protected Dictionary<TokenType, string> m_ComputeNames = new Dictionary<TokenType, string>();                       //运算符重载名字
        public UserdataType(Script script, Type type) {
            m_Script = script;
            m_Type = type;
            m_ComputeNames[TokenType.Plus] = "op_Addition";
            m_ComputeNames[TokenType.Minus] = "op_Subtraction";
            m_ComputeNames[TokenType.Multiply] = "op_Multiply";
            m_ComputeNames[TokenType.Divide] = "op_Division";
        }
        /// <summary> 初始化泛型类 </summary>
        public ScriptUserdata MakeGenericType(Type[] parameters) {
            Type[] types = m_Type.GetTypeInfo().GetGenericArguments();
            if (types.Length != parameters.Length)
                throw new ExecutionException(m_Script, m_Type + " 泛型类个数错误 需要:" + types.Length + " 传入:" + parameters.Length);
            int length = types.Length;
            for (int i = 0; i < length; ++i) {
                if (!types[i].GetTypeInfo().BaseType.GetTypeInfo().IsAssignableFrom(parameters[i]))
                    throw new ExecutionException(m_Script, m_Type + "泛型类第" + (i + 1) + "个参数失败 需要:" + types[i].GetTypeInfo().BaseType + " 传入:" + parameters[i]);
            }
            return m_Script.CreateUserdata(m_Type.MakeGenericType(parameters));
        }
        public void Rename(string name1, string name2) {
            m_Rename[name2] = name1;
        }
        public Type GetVariableType(string name) {
            return m_Rename.ContainsKey(name) ? GetVariableType_impl(m_Rename[name]) : GetVariableType_impl(name);
        }
        public object GetValue(object obj, string name) {
            return m_Rename.ContainsKey(name) ? GetValue_impl(obj, m_Rename[name]) : GetValue_impl(obj, name);
        }
        public void SetValue(object obj, string name, ScriptObject value) {
            if (m_Rename.ContainsKey(name)) {
                SetValue_impl(obj, m_Rename[name], value);
            } else {
                SetValue_impl(obj, name, value);
            }
        }
        /// <summary> 获得运算符重载的函数 </summary>
        public ScorpioMethod GetComputeMethod(TokenType type) {
            if (m_ComputeMethods.ContainsKey(type)) {
                return m_ComputeMethods[type];
            } else {
                ScorpioMethod method = GetComputeMethod_impl(type);
                m_ComputeMethods.Add(type, method);
                return method;
            }
        }
        /// <summary> 添加一个扩展函数 </summary>
        public abstract void AddExtensionMethod(MethodInfo method);
        /// <summary> 创建一个实例 </summary>
        public abstract object CreateInstance(ScriptObject[] parameters);
        /// <summary> 获得运算符重载的函数 </summary>
        protected abstract ScorpioMethod GetComputeMethod_impl(TokenType type);
        /// <summary> 获取一个变量的类型,只能获取 Field Property Event </summary>
        protected abstract Type GetVariableType_impl(string name);
        /// <summary> 获得一个类变量 </summary>
        protected abstract object GetValue_impl(object obj, string name);
        /// <summary> 设置一个类变量 </summary>
        protected abstract void SetValue_impl(object obj, string name, ScriptObject value);
    }
    public class ReflectUserdataType : UserdataType {
        private bool m_InitializeConstructor;                           //是否初始化过所有构造函数
        private bool m_InitializeMethods;                               //是否初始化过所有函数
        private UserdataMethod m_Constructor;                           //所有构造函数
        private List<MethodInfo> m_Methods;                             //所有函数 包含扩展函数
        private Dictionary<string, UserdataVariable> m_Variables;       //所有的变量 FieldInfo,PropertyInfo,EventInfo
        private Dictionary<string, ScriptUserdata> m_NestedTypes;       //所有的类中类
        private Dictionary<string, UserdataMethod> m_Functions;         //所有的函数
        public ReflectUserdataType(Script script, Type type) : base(script, type) {
            m_InitializeConstructor = false;
            m_InitializeMethods = false;
            m_Methods = new List<MethodInfo>();
            m_Variables = new Dictionary<string, UserdataVariable>();
            m_NestedTypes = new Dictionary<string, ScriptUserdata>();
            m_Functions = new Dictionary<string, UserdataMethod>();
        }
        private void InitializeConstructor() {
            if (m_InitializeConstructor == true) return;
            m_InitializeConstructor = true;
            m_Constructor = new ReflectUserdataMethod(m_Script, m_Type, m_Type.ToString(), m_Type.GetTypeInfo().GetConstructors(Script.BindingFlag));
        }
        private void InitializeMethods() {
            if (m_InitializeMethods == true) return;
            m_InitializeMethods = true;
            m_Methods.AddRange(m_Type.GetTypeInfo().GetMethods(Script.BindingFlag));
        }
        private UserdataMethod GetMethod(string name) {
            InitializeMethods();
            foreach (var method in m_Methods) {
                if (method.Name.Equals(name)) {
                    ReflectUserdataMethod ret = new ReflectUserdataMethod(m_Script, m_Type, name, m_Methods);
                    m_Functions.Add(name, ret);
                    return ret;
                }
            }
            return null;
        }
        private UserdataVariable GetVariable(string name) {
            if (m_Variables.ContainsKey(name))
                return m_Variables[name];
            FieldInfo fInfo = m_Type.GetTypeInfo().GetField(name, Script.BindingFlag);
            if (fInfo != null) return m_Variables[name] = new UserdataField(m_Script, fInfo);
            PropertyInfo pInfo = m_Type.GetTypeInfo().GetProperty(name, Script.BindingFlag);
            if (pInfo != null) return m_Variables[name] = new UserdataProperty(m_Script, pInfo);
            EventInfo eInfo = m_Type.GetTypeInfo().GetEvent(name, Script.BindingFlag);
            if (eInfo != null) return m_Variables[name] = new UserdataEvent(m_Script, eInfo);
            return null;
        }
        private ScriptUserdata GetNestedType(string name) {
            Type nestedType = m_Type.GetTypeInfo().GetNestedType(name, Script.BindingFlag);
            if (nestedType != null) {
                ScriptUserdata ret = m_Script.CreateUserdata(nestedType);
                m_NestedTypes.Add(name, ret);
                return ret;
            }
            return null;
        }
        
        public override void AddExtensionMethod(MethodInfo method) {
            if (!m_Methods.Contains(method))
                m_Methods.Add(method);
        }
        /// <summary> 创建一个实例 </summary>
        public override object CreateInstance(ScriptObject[] parameters) {
            InitializeConstructor();
            return m_Constructor.Call(null, parameters);
        }
        /// <summary> 获得运算符重载的函数 </summary>
        protected override ScorpioMethod GetComputeMethod_impl(TokenType type) {
            if (m_ComputeNames.ContainsKey(type)) {
                object ret = GetValue(null, m_ComputeNames[type]);
                if (ret is UserdataMethod) {
                    return new ScorpioStaticMethod(m_ComputeNames[type], (UserdataMethod)ret);
                }
            }
            return null;
        }
        protected override Type GetVariableType_impl(string name) {
            var variable = GetVariable(name);
            return variable != null ? variable.FieldType : null;
        }
        /// <summary> 获得一个类变量 </summary>
        protected override object GetValue_impl(object obj, string name) {
            if (m_Functions.ContainsKey(name)) return m_Functions[name];
            if (m_NestedTypes.ContainsKey(name)) return m_NestedTypes[name];
            UserdataVariable variable = GetVariable(name);
            if (variable != null) return variable.GetValue(obj);
            ScriptUserdata nestedType = GetNestedType(name);
            if (nestedType != null) return nestedType;
            UserdataMethod func = GetMethod(name);
            if (func != null) return func;
            throw new ExecutionException(m_Script, "GetValue Type[" + m_Type.ToString() + "] 变量 [" + name + "] 不存在");
        }
        /// <summary> 设置一个类变量 </summary>
        protected override void SetValue_impl(object obj, string name, ScriptObject value) {
            UserdataVariable variable = GetVariable(name);
            if (variable == null) throw new ExecutionException(m_Script, "SetValue Type[" + m_Type + "] 变量 [" + name + "] 不存在");
            try {
                variable.SetValue(obj, Util.ChangeType(m_Script, value, variable.FieldType));
            } catch (System.Exception e) {
                throw new ExecutionException(m_Script, "SetValue 出错 源类型:" + (value == null || value.IsNull ? "null" : value.ObjectValue.GetType().Name) + " 目标类型:" + variable.FieldType.Name + " : " + e.ToString());
            }
        }
    }
    public class FastReflectUserdataType : UserdataType {
        private IScorpioFastReflectClass m_Value;
        private FastReflectUserdataMethod m_Constructor;
        public FastReflectUserdataType(Script script, Type type, IScorpioFastReflectClass value) : base(script, type) {
            m_Value = value;
            m_Constructor = value.GetConstructor();
        }
        public override void AddExtensionMethod(MethodInfo method) {

        }
        public override object CreateInstance(ScriptObject[] parameters) {
            return m_Constructor.Call(null, parameters);
        }
        protected override ScorpioMethod GetComputeMethod_impl(TokenType type) {
            return m_ComputeNames.ContainsKey(type) ? m_Value.GetValue(null, m_ComputeNames[type]) as ScorpioMethod : null;
        }
        protected override Type GetVariableType_impl(string name) {
            return m_Value.GetVariableType(name);
        }
        protected override object GetValue_impl(object obj, string name) {
            return m_Value.GetValue(obj, name);
        }
        protected override void SetValue_impl(object obj, string name, ScriptObject value) {
            m_Value.SetValue(obj, name, value);
        }
    }
}
