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
    public class UserdataType
    {
        private Script m_Script;                                        //脚本系统
        private Type m_Type;                                            //类型
        private bool m_InitializeConstructor;                           //是否初始化过所有构造函数
        private bool m_InitializeMethods;                               //是否初始化过所有函数
        private UserdataMethod m_Constructor;                            //所有构造函数
        private MethodInfo[] m_Methods;                                 //所有函数
        private Dictionary<string, UserdataField> m_FieldInfos;         //所有的变量 以及 get set函数
        private Dictionary<string, ScriptUserdata> m_NestedTypes;       //所有的类中类
        private Dictionary<string, UserdataMethod> m_Functions;         //所有的函数
        private Dictionary<string, ScorpioMethod> m_ScorpioMethods;     //所有的静态函数和类函数（不包含对象函数）
        public UserdataType(Script script, Type type)
        {
            m_Script = script;
            m_Type = type;
            m_InitializeConstructor = false;
            m_InitializeMethods = false;
            m_FieldInfos = new Dictionary<string, UserdataField>();
            m_NestedTypes = new Dictionary<string, ScriptUserdata>();
            m_Functions = new Dictionary<string, UserdataMethod>();
            m_ScorpioMethods = new Dictionary<string, ScorpioMethod>();
        }
        private void InitializeConstructor()
        {
            if (m_InitializeConstructor == true) return;
            m_InitializeConstructor = true;
            m_Constructor = new UserdataMethod(m_Script, m_Type, m_Type.ToString(), m_Type.GetConstructors());
        }
        private void InitializeMethods()
        {
            if (m_InitializeMethods == true) return;
            m_InitializeMethods = true;
            m_Methods = m_Type.GetMethods(Script.BindingFlag);
        }
        private UserdataMethod GetMethod(string name)
        {
            InitializeMethods();
            for (int i = 0; i < m_Methods.Length; ++i) {
                if (m_Methods[i].Name.Equals(name)) {
                    UserdataMethod method = new UserdataMethod(m_Script, m_Type, name, m_Methods);
                    m_Functions.Add(name, method);
                    return method;
                }
            }
            return null;
        }
        private ScorpioMethod GetMethod(object obj, string name, UserdataMethod method)
        {
            if (method.IsStatic) {
                return m_ScorpioMethods[name] = new ScorpioStaticMethod(name, method);
            } else if (obj == null) {
                return m_ScorpioMethods[name] = new ScorpioTypeMethod(m_Script, name, method, m_Type);
            }
            return new ScorpioObjectMethod(obj, name, method);
        }
        private UserdataField GetField(string name)
        {
            if (m_FieldInfos.ContainsKey(name))
                return m_FieldInfos[name];
            FieldInfo fInfo = m_Type.GetField(name);
            if (fInfo != null) return m_FieldInfos[name] = new UserdataField(m_Script, fInfo);
            PropertyInfo pInfo = m_Type.GetProperty(name, Script.BindingFlag);
            if (pInfo != null) return m_FieldInfos[name] = new UserdataField(m_Script, pInfo);
            EventInfo eInfo = m_Type.GetEvent(name);
            if (eInfo != null) return m_FieldInfos[name] = new UserdataField(m_Script, eInfo);
            return null;
        }
        /// <summary> 创建一个实例 </summary>
        public object CreateInstance(ScriptObject[] parameters)
        {
            InitializeConstructor();
            return m_Constructor.Call(null, parameters);
        }
        public UserdataMethod GetComputeMethod(TokenType type)
        {
            switch (type)
            {
                case TokenType.Plus: return GetMethod("op_Addition");
                case TokenType.Minus: return GetMethod("op_Subtraction");
                case TokenType.Multiply: return GetMethod("op_Multiply");
                case TokenType.Divide: return GetMethod("op_Division");
                default: return null;
            }
        }
        /// <summary> 获得一个类变量 </summary>
        public object GetValue(object obj, string name)
        {
            if (m_ScorpioMethods.ContainsKey(name)) return m_ScorpioMethods[name];
            if (m_Functions.ContainsKey(name)) return GetMethod(obj, name, m_Functions[name]);
            if (m_NestedTypes.ContainsKey(name)) return m_NestedTypes[name];
            UserdataField field = GetField(name);
            if (field != null) return field.GetValue(obj);
            Type nestedType = m_Type.GetNestedType(name, Script.BindingFlag);
            if (nestedType != null)
            {
                ScriptUserdata ret = m_Script.CreateUserdata(nestedType);
                m_NestedTypes.Add(name, ret);
                return ret;
            }
            UserdataMethod func = GetMethod(name);
            if (func != null) return GetMethod(obj, name, func);
            throw new ExecutionException(m_Script, "GetValue Type[" + m_Type.ToString() + "] 变量 [" + name + "] 不存在");
        }
        /// <summary> 设置一个类变量 </summary>
        public void SetValue(object obj, string name, ScriptObject value)
        {
            UserdataField field = GetField(name);
            if (field == null) throw new ExecutionException(m_Script, "SetValue Type[" + m_Type + "] 变量 [" + name + "] 不存在");
            try {
                field.SetValue(obj, Util.ChangeType(m_Script, value, field.FieldType));
            } catch (System.Exception e) {
                throw new ExecutionException(m_Script, "SetValue 出错 源类型:" + (value == null || value.IsNull ? "null" : value.ObjectValue.GetType().Name) + " 目标类型:" + field.FieldType.Name + " : " + e.ToString());
            }
        }
    }
}
