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
        public object Value { get; private set; }
        public Type ValueType { get; private set; }
        private Dictionary<string, FieldInfo> FieldInfos = new Dictionary<string, FieldInfo>();
        private Dictionary<string, ScriptFunction> Functions = new Dictionary<string, ScriptFunction>();
        public ScriptUserdata(object value)
        {
            Type = ObjectType.UserData;
            Value = value;
            ValueType = (Value is Type) ? (Type)value : value.GetType();
        }
        private FieldInfo GetField(string strName)
        {
            if (FieldInfos.ContainsKey(strName))
                return FieldInfos[strName];
            FieldInfo info = ValueType.GetField(strName);
            if (info != null) FieldInfos.Add(strName, info);
            return info;
        }
        public ScriptObject GetValue(string strName)
        {
            if (Functions.ContainsKey(strName))
                return Functions[strName];
            FieldInfo field = GetField(strName);
            if (field != null) return ScriptObject.CreateObject(field.GetValue(Value));
            ScriptFunction func = new ScriptFunction("", new ScorpioMethod(ValueType, strName, Value));
            Functions[strName] = func;
            return func;
        }
        public void SetValue(string strName, ScriptObject value)
        {
            FieldInfo field = GetField(strName);
            if (field == null) throw new ScriptException("Type[" + ValueType + "] 变量 [" + strName + "] 不存在");
            field.SetValue(Value, Util.ChangeType(value, field.FieldType));
        }
        public override string ToString() { return "Userdata"; }
    }
}
