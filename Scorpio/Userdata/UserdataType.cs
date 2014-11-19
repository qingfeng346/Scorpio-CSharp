using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Scorpio.Exception;
using Scorpio.Variable;
namespace Scorpio.Userdata
{
    /// <summary> 保存一个类的所有元素 </summary>
    public class UserdataType
    {
        private class UserdataField
        {
            public string name;
            public Type fieldType;
            public FieldInfo field;
            public MethodInfo getMethod;
            public MethodInfo setMethod;
            public object GetValue(object obj)
            {
                if (field != null)
                    return field.GetValue(obj);
                else if (getMethod != null)
                    return getMethod.Invoke(obj, null);
                throw new ScriptException("变量 [" + name + "] 不支持GetValue");
            }
            public void SetValue(object obj, object val)
            {
                if (field != null)
                    field.SetValue(obj, val);
                else if (setMethod != null)
                    setMethod.Invoke(obj, new object[] { val });
                else
                    throw new ScriptException("变量 [" + name + "] 不支持SetValue");
            }
        }
        private Script m_Script;                                        //脚本系统
        private Type m_Type;                                            //类型
        private bool m_InitializeConstructor;                           //是否初始化过所有构造函数
        private bool m_InitializeMethods;                               //是否初始化过所有函数
        private UserdataMethod m_Constructor;                            //所有构造函数
        private MethodInfo[] m_Methods;                                 //所有函数
        private Dictionary<string, UserdataField> m_FieldInfos;   //所有的变量 以及 get set函数
        private Dictionary<string, ScriptUserdata> m_NestedTypes;       //所有的类中类
        private Dictionary<string, UserdataMethod> m_Functions;          //所有的函数
        public UserdataType(Script script, Type type)
        {
            m_Script = script;
            m_Type = type;
            m_InitializeConstructor = false;
            m_InitializeMethods = false;
            m_FieldInfos = new Dictionary<string, UserdataField>();
            m_NestedTypes = new Dictionary<string, ScriptUserdata>();
            m_Functions = new Dictionary<string, UserdataMethod>();
        }
        private void InitializeConstructor()
        {
            if (m_InitializeConstructor == true) return;
            m_InitializeConstructor = true;
            m_Constructor = new UserdataMethod(m_Type, m_Type.ToString(), m_Type.GetConstructors());
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
            for (int i = 0; i < m_Methods.Length; ++i)
            {
                if (m_Methods[i].Name.Equals(name))
                {
                    UserdataMethod method = new UserdataMethod(m_Type, name, m_Methods);
                    m_Functions.Add(name, method);
                    return method;
                }
            }
            return null;
        }
        private UserdataField GetField(string name)
        {
            if (m_FieldInfos.ContainsKey(name))
                return m_FieldInfos[name];
            UserdataField field = new UserdataField();
            field.name = name;
            FieldInfo fInfo = m_Type.GetField(name);
            if (fInfo != null)
            {
                field.field = fInfo;
                field.fieldType = fInfo.FieldType;
                m_FieldInfos.Add(name, field);
                return field;
            }
            PropertyInfo pInfo = m_Type.GetProperty(name, Script.BindingFlag);
            if (pInfo != null)
            {
                field.getMethod = pInfo.GetGetMethod();
                field.setMethod = pInfo.GetSetMethod();
                field.fieldType = pInfo.PropertyType;
                m_FieldInfos.Add(name, field);
                return field;
            }
            return null;
        }
        /// <summary> 创建一个实例 </summary>
        public object CreateInstance(ScriptObject[] parameters)
        {
            InitializeConstructor();
            return m_Constructor.Call(null, parameters);
        }
        /// <summary> 获得一个类变量 </summary>
        public object GetValue(object obj, string name)
        {
            if (m_Functions.ContainsKey(name)) return new ScorpioMethod(obj, name, m_Functions[name]);
            if (m_NestedTypes.ContainsKey(name)) return m_NestedTypes[name];
            UserdataField field = GetField(name);
            if (field != null) return m_Script.CreateObject(field.GetValue(obj));
            Type nestedType = m_Type.GetNestedType(name, Script.BindingFlag);
            if (nestedType != null)
            {
                ScriptUserdata ret = m_Script.CreateUserdata(nestedType);
                m_NestedTypes.Add(name, ret);
                return ret;
            }
            UserdataMethod func = GetMethod(name);
            if (func != null) return new ScorpioMethod(obj, name, func);
            throw new ScriptException("GetValue Type[" + m_Type.ToString() + "] 变量 [" + name + "] 不存在");
        }
        /// <summary> 设置一个类变量 </summary>
        public void SetValue(object obj, string name, ScriptObject value)
        {
            UserdataField field = GetField(name);
            if (field == null) throw new ScriptException("SetValue Type[" + m_Type + "] 变量 [" + name + "] 不存在");
            field.SetValue(obj, Util.ChangeType(value, field.fieldType));
        }
    }
}
