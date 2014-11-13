using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Scorpio;
using Scorpio.Variable;
using Scorpio.Exception;
namespace Scorpio.Userdata
{
    //语言数据
    public class DefaultScriptUserdataObject : ScriptUserdata
    {
        private class ScriptUserdataField
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
        private ScorpioMethod m_Constructor;
        private Dictionary<string, ScriptUserdataField> m_FieldInfos;                 //所有的变量 以及 get set函数
        private Dictionary<string, ScriptUserdata> m_NestedTypes;       //所有的类中类
        private Dictionary<string, ScriptFunction> m_Functions;         //所有的函数
        public DefaultScriptUserdataObject(Script script, object value) : base(script)
        {
            this.Value = value;
            this.ValueType = (Value is Type) ? (Type)Value : Value.GetType();
            m_FieldInfos = new Dictionary<string, ScriptUserdataField>();
            m_NestedTypes = new Dictionary<string, ScriptUserdata>();
            m_Functions = new Dictionary<string, ScriptFunction>();
            m_Constructor = new ScorpioMethod(ValueType.ToString(), ValueType.GetConstructors());
            MethodInfo[] methods = ValueType.GetMethods(Script.BindingFlag);
            for (int i = 0; i < methods.Length;++i )
            {
                string name = methods[i].Name;
                if (!m_Functions.ContainsKey(name))
                    m_Functions.Add(name, Script.CreateFunction(new ScorpioMethod(ValueType, name, Value)));
            }
        }
        private ScriptUserdataField GetField(string strName)
        {
            if (m_FieldInfos.ContainsKey(strName))
                return m_FieldInfos[strName];
            ScriptUserdataField field = new ScriptUserdataField();
            field.name = strName;
            FieldInfo info = ValueType.GetField(strName);
            if (info != null)
            {
                field.field = info;
                field.fieldType = info.FieldType;
                m_FieldInfos.Add(strName, field);
                return field;
            }
            MethodInfo method = ValueType.GetMethod("get_" + strName);
            if (method != null)
            {
                field.getMethod = method;
                field.fieldType = method.ReturnType;
                field.setMethod = ValueType.GetMethod("set_" + strName);
                m_FieldInfos.Add(strName, field);
                return field;
            }
            method = ValueType.GetMethod("set_" + strName);
            if (method != null)
            {
                field.setMethod = method;
                field.fieldType = method.GetParameters()[0].ParameterType;
                field.getMethod = ValueType.GetMethod("get_" + strName);
                m_FieldInfos.Add(strName, field);
                return field;
            }
            return null;
        }
        public override ScriptObject Call(ScriptObject[] parameters)
        {
            return Script.CreateObject(m_Constructor.Call(parameters));
        }
        public override ScriptObject GetValue(string strName)
        {
            if (m_Functions.ContainsKey(strName))
                return m_Functions[strName];
            if (m_NestedTypes.ContainsKey(strName))
                return m_NestedTypes[strName];
            ScriptUserdataField field = GetField(strName);
            if (field != null) return Script.CreateObject(field.GetValue(Value));
            Type nestedType = ValueType.GetNestedType(strName, Script.BindingFlag);
            if (nestedType != null) {
                ScriptUserdata ret = Script.CreateUserdata(nestedType);
                m_NestedTypes.Add(strName, ret);
                return ret;
            }
            throw new ScriptException("Type[" + ValueType.ToString() + "] Variable[" + strName + "] 不存在");
        }
        public override void SetValue(string strName, ScriptObject value)
        {
            ScriptUserdataField field = GetField(strName);
            if (field == null) throw new ScriptException("Type[" + ValueType + "] 变量 [" + strName + "] 不存在");
            field.SetValue(Value, Util.ChangeType(value, field.fieldType));
        }
    }
}
