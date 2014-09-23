using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.CodeDom;

namespace Scorpio
{
    public enum ObjectType
    {
        Null,           //Null
        Boolean,        //布尔
        Number,         //数字
        String,         //字符串
        Function,       //函数
        Array,          //数组
        Table,          //MAP
        UserData,       //普通类
    }
    //脚本数据类型
    public abstract class ScriptObject
    {
        public static ScriptObject CreateObject(object obj)
        {
            if (obj == null)
                return ScriptNull.Instance;
            else if (obj is ScriptObject)
                return (ScriptObject)obj;
            Type type = obj.GetType();
            if (Util.IsBool(type)) {
                return ScriptBoolean.Get((bool)obj);
            } else if (Util.IsString(type)) {
                return new ScriptString((string)obj);
            } else if (Util.IsDouble(type)) {
                return new ScriptNumber(Convert.ToDouble(obj));
            } else if (Util.IsLong(type)) {
                return new ScriptNumber(Convert.ToInt64(obj));
            } else if (Util.IsULong(type)) {
                return new ScriptNumber(Convert.ToUInt64(obj));
            }
            return new ScriptUserdata(obj);
        }
        public virtual void Assign(ScriptObject obj) {  }
        public virtual ScriptObject Plus(ScriptObject obj) { return null; }
        public virtual ScriptObject Minus(ScriptObject obj) { return null; }
        public virtual ScriptObject Multiply(ScriptObject obj) { return null; }
        public virtual ScriptObject Divide(ScriptObject obj) { return null; }
        public virtual ScriptObject Modulo(ScriptObject obj) { return null; }
        public ObjectType Type { get; protected set; }
        public bool IsNull { get { return (Type == ObjectType.Null); } }
        public bool IsBoolean { get { return (Type == ObjectType.Boolean); } }
        public bool IsNumber { get { return (Type == ObjectType.Number); } }
        public bool IsString { get { return (Type == ObjectType.String); } }
        public bool IsFunction { get { return (Type == ObjectType.Function); } }
        public bool IsArray { get { return (Type == ObjectType.Array); } }
        public bool IsTable { get { return (Type == ObjectType.Table); } }
        public bool IsUserData { get { return (Type == ObjectType.UserData); } }
    }
}
