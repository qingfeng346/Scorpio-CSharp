using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Scorpio.Variable;
using Scorpio.Exception;
namespace Scorpio
{
    //语言数据
    public class ScriptUserdata : ScriptObject
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
        public object Value { get; private set; }
        public Type ValueType { get; private set; }
        private Dictionary<string, Field> FieldInfos = new Dictionary<string, Field>();
        private Dictionary<string, ScriptFunction> Functions = new Dictionary<string, ScriptFunction>();
        public ScriptUserdata(object value)
        {
            Type = ObjectType.UserData;
            Value = value;
            ValueType = (Value is Type) ? (Type)value : value.GetType();
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
        public ScriptObject GetValue(string strName)
        {
            if (Functions.ContainsKey(strName))
                return Functions[strName];
            Field field = GetField(strName);
            if (field != null) return ScriptObject.CreateObject(field.GetValue(Value));
            ScriptFunction func = new ScriptFunction("", new ScorpioMethod(ValueType, strName, Value));
            Functions[strName] = func;
            return func;
        }
        public void SetValue(string strName, ScriptObject value)
        {
            Field field = GetField(strName);
            if (field == null) throw new ScriptException("Type[" + ValueType + "] 变量 [" + strName + "] 不存在");
            field.SetValue(Value, Util.ChangeType(value, field.fieldType));
        }
        public override string ToString() { return "Userdata"; }
    }
}
