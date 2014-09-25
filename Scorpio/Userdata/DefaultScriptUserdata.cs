using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Scorpio.Variable;
using Scorpio.Exception;
namespace Scorpio.Userdata
{
    //语言数据
    public class DefaultScriptUserdata : ScriptUserdata
    {
        private class Field
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
        public override object Value { get; protected set; }
        public override Type ValueType { get; protected set; }
        private bool m_IsEnum = false;
        private Dictionary<string, Field> FieldInfos = new Dictionary<string, Field>();                         //所有的变量 以及 get set函数
        private Dictionary<string, ScriptUserdata> NestedTypes = new Dictionary<string, ScriptUserdata>();      //所有的类中类
        private Dictionary<string, ScriptFunction> Functions = new Dictionary<string, ScriptFunction>();        //所有的函数
        private Dictionary<string, ScriptEnum> Enums = new Dictionary<string, ScriptEnum>();                    //如果是枚举的话 所有枚举的值
        public DefaultScriptUserdata(Script script, object value)
        {
            this.Script = script;
            this.Value = value;
            this.ValueType = (Value is Type) ? (Type)value : value.GetType();
            m_IsEnum = ValueType.IsEnum;
            if (m_IsEnum) {
                Array values = Enum.GetValues(ValueType);
                foreach (var v in values) {
                    Enums[v.ToString()] = script.CreateEnum(v);
                }
            }
        }
        private Field GetField(string strName)
        {
            if (FieldInfos.ContainsKey(strName))
                return FieldInfos[strName];
            Field field = new Field();
            field.name = strName;
            FieldInfo info = ValueType.GetField(strName);
            if (info != null)
            {
                field.field = info;
                field.fieldType = info.FieldType;
                FieldInfos.Add(strName, field);
                return field;
            }
            MethodInfo method = ValueType.GetMethod("get_" + strName);
            if (method != null)
            {
                field.getMethod = method;
                field.fieldType = method.ReturnType;
                field.setMethod = ValueType.GetMethod("set_" + strName);
                FieldInfos.Add(strName, field);
                return field;
            }
            method = ValueType.GetMethod("set_" + strName);
            if (method != null)
            {
                field.setMethod = method;
                field.fieldType = method.GetParameters()[0].ParameterType;
                field.getMethod = ValueType.GetMethod("get_" + strName);
                FieldInfos.Add(strName, field);
                return field;
            }
            return null;
        }
        public override ScriptObject GetValue(string strName)
        {
            if (m_IsEnum) {
                if (!Enums.ContainsKey(strName)) throw new ScriptException("Enum[" + ValueType.ToString() + "] Element[" + strName + "] 不存在");
                return Enums[strName];
            } else {
                if (Functions.ContainsKey(strName))
                    return Functions[strName];
                if (NestedTypes.ContainsKey(strName))
                    return NestedTypes[strName];
                Field field = GetField(strName);
                if (field != null) return Script.CreateObject(field.GetValue(Value));
                MethodInfo method = ValueType.GetMethod(strName);
                if (method != null)
                {
                    ScriptFunction func = Script.CreateFunction(new ScorpioMethod(ValueType, strName, Value));
                    Functions[strName] = func;
                    return func;
                }
                Type nestedType = ValueType.GetNestedType(strName);
                if (nestedType != null) {
                    ScriptUserdata ret = Script.CreateUserdata(nestedType);
                    NestedTypes.Add(strName, ret);
                    return ret;
                }
                throw new ScriptException("Type[" + ValueType.ToString() + "] Variable[" + strName + "] 不存在");
            }
        }
        public override void SetValue(string strName, ScriptObject value)
        {
            if (m_IsEnum) {
                throw new ScriptException("Enum 不支持 SetValue");
            } else {
                Field field = GetField(strName);
                if (field == null) throw new ScriptException("Type[" + ValueType + "] 变量 [" + strName + "] 不存在");
                field.SetValue(Value, Util.ChangeType(value, field.fieldType));
            }
        }
        public override string ToString() { return Value.ToString(); }
    }
}
