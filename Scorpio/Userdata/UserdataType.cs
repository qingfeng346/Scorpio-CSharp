#if SCORPIO_UWP && !UNITY_EDITOR
#define UWP
#endif
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Scorpio;
using Scorpio.Exception;
using Scorpio.Variable;
using Scorpio.Compiler;
namespace Scorpio.Userdata
{
    /// <summary> 保存一个类的所有元素 </summary>
    public abstract class UserdataType
    {
        protected Script m_Script;                                        //脚本系统
        protected Type m_Type;                                            //类型
        public UserdataType(Script script, Type type) {
            m_Script = script;
            m_Type = type;
        }
        /// <summary> 初始化泛型类 </summary>
        public ScriptUserdata MakeGenericType(Type[] parameters) {
            Type[] types = m_Type.GetGenericArguments();
            if (types.Length != parameters.Length)
                throw new ExecutionException(m_Script, m_Type + " 泛型类个数错误 需要:" + types.Length + " 传入:" + parameters.Length);
            int length = types.Length;
            for (int i = 0; i < length; ++i) {
#if UWP
                if (!types[i].GetTypeInfo().BaseType.IsAssignableFrom(parameters[i]))
                    throw new ExecutionException(m_Script, m_Type + "泛型类第" + (i + 1) + "个参数失败 需要:" + types[i].GetTypeInfo().BaseType + " 传入:" + parameters[i]);
#else
                if (!types[i].BaseType.IsAssignableFrom(parameters[i]))
                    throw new ExecutionException(m_Script, m_Type + "泛型类第" + (i + 1) + "个参数失败 需要:" + types[i].BaseType + " 传入:" + parameters[i]);
#endif
            }
            return m_Script.CreateUserdata(m_Type.MakeGenericType(parameters));
        }
        /// <summary> 创建一个实例 </summary>
        public abstract object CreateInstance(ScriptObject[] parameters);
        /// <summary> 获得运算符重载的函数 </summary>
        public abstract UserdataMethod GetComputeMethod(TokenType type);
        /// <summary> 获得一个类变量 </summary>
        public abstract object GetValue(object obj, string name);
        /// <summary> 设置一个类变量 </summary>
        public abstract void SetValue(object obj, string name, ScriptObject value);
        
    }
    public class ReflectUserdataType : UserdataType {
		private bool m_InitializeConstructor;                           //是否初始化过所有构造函数
		private bool m_InitializeMethods;                               //是否初始化过所有函数
        private UserdataMethod m_Constructor;                           //所有构造函数
        private MethodInfo[] m_Methods;                                 //所有函数
        private Dictionary<string, UserdataVariable> m_Variables;       //所有的变量 FieldInfo,PropertyInfo,EventInfo
        private Dictionary<string, ScriptUserdata> m_NestedTypes;       //所有的类中类
        private Dictionary<string, UserdataMethod> m_Functions;         //所有的函数
        private Dictionary<string, ScorpioMethod> m_ScorpioMethods;     //所有的静态函数和类函数（不包含对象函数）
        public ReflectUserdataType(Script script, Type type) : base(script, type) {
			m_InitializeConstructor = false;
			m_InitializeMethods = false;
            m_Variables = new Dictionary<string, UserdataVariable>();
            m_NestedTypes = new Dictionary<string, ScriptUserdata>();
            m_Functions = new Dictionary<string, UserdataMethod>();
            m_ScorpioMethods = new Dictionary<string, ScorpioMethod>();
        }
		private void InitializeConstructor() {
			if (m_InitializeConstructor == true) return;
			m_InitializeConstructor = true;
			m_Constructor = new ReflectUserdataMethod(m_Script, m_Type, m_Type.ToString(), m_Type.GetConstructors());
		}
		private void InitializeMethods() {
			if (m_InitializeMethods == true) return;
			m_InitializeMethods = true;
			m_Methods = m_Type.GetMethods(Script.BindingFlag);
		}
        private UserdataMethod GetMethod(string name) {
			InitializeMethods();
            for (int i = 0; i < m_Methods.Length; ++i) {
                if (m_Methods[i].Name.Equals(name)) {
                    UserdataMethod method = new ReflectUserdataMethod(m_Script, m_Type, name, m_Methods);
                    m_Functions.Add(name, method);
                    return method;
                }
            }
            return null;
        }
        private ScorpioMethod GetMethod(object obj, string name, UserdataMethod method) {
            if (method.IsStatic) {
                return m_ScorpioMethods[name] = new ScorpioStaticMethod(name, method);
            } else if (obj == null) {
                return m_ScorpioMethods[name] = new ScorpioTypeMethod(m_Script, name, method, m_Type);
            }
            return new ScorpioObjectMethod(obj, name, method);
        }
        private UserdataVariable GetVariable(string name) {
            if (m_Variables.ContainsKey(name))
                return m_Variables[name];
            FieldInfo fInfo = m_Type.GetField(name);
            if (fInfo != null) return m_Variables[name] = new UserdataField(m_Script, fInfo);
            PropertyInfo pInfo = m_Type.GetProperty(name, Script.BindingFlag);
            if (pInfo != null) return m_Variables[name] = new UserdataProperty(m_Script, pInfo);
            EventInfo eInfo = m_Type.GetEvent(name);
            if (eInfo != null) return m_Variables[name] = new UserdataEvent(m_Script, eInfo);
            return null;
        }
        /// <summary> 创建一个实例 </summary>
        public override object CreateInstance(ScriptObject[] parameters) {
			InitializeConstructor();
            return m_Constructor.Call(null, parameters);
        }
        /// <summary> 获得运算符重载的函数 </summary>
        public override UserdataMethod GetComputeMethod(TokenType type) {
            switch (type) {
            case TokenType.Plus: return GetMethod("op_Addition");
            case TokenType.Minus: return GetMethod("op_Subtraction");
            case TokenType.Multiply: return GetMethod("op_Multiply");
            case TokenType.Divide: return GetMethod("op_Division");
            default: return null;
            }
        }
        /// <summary> 获得一个类变量 </summary>
        public override object GetValue(object obj, string name) {
            if (m_ScorpioMethods.ContainsKey(name)) return m_ScorpioMethods[name];
            if (m_Functions.ContainsKey(name)) return GetMethod(obj, name, m_Functions[name]);
            if (m_NestedTypes.ContainsKey(name)) return m_NestedTypes[name];
            UserdataVariable variable = GetVariable(name);
            if (variable != null) return variable.GetValue(obj);
            Type nestedType = m_Type.GetNestedType(name, Script.BindingFlag);
            if (nestedType != null) {
                ScriptUserdata ret = m_Script.CreateUserdata(nestedType);
                m_NestedTypes.Add(name, ret);
                return ret;
            }
            UserdataMethod func = GetMethod(name);
            if (func != null) return GetMethod(obj, name, func);
            throw new ExecutionException(m_Script, "GetValue Type[" + m_Type.ToString() + "] 变量 [" + name + "] 不存在");
        }
        /// <summary> 设置一个类变量 </summary>
        public override void SetValue(object obj, string name, ScriptObject value) {
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
        public override object CreateInstance(ScriptObject[] parameters) {
            return m_Constructor.Call(null, parameters);
        }

        public override UserdataMethod GetComputeMethod(TokenType type) {
            switch (type) {
            case TokenType.Plus: return m_Value.GetValue(null, "op_Addition") as UserdataMethod;
            case TokenType.Minus: return m_Value.GetValue(null, "op_Subtraction") as UserdataMethod;
            case TokenType.Multiply: return m_Value.GetValue(null, "op_Multiply") as UserdataMethod;
            case TokenType.Divide: return m_Value.GetValue(null, "op_Division") as UserdataMethod;
            default: return null;
            }
        }

        public override object GetValue(object obj, string name) {
            return m_Value.GetValue(obj, name);
        }

        public override void SetValue(object obj, string name, ScriptObject value) {
            m_Value.SetValue(obj, name, value);
        }
    }
}
