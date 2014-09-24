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
        public virtual void Assign(ScriptObject obj) {  }
        public virtual ScriptObject Plus(ScriptObject obj) { return null; }
        public virtual ScriptObject Minus(ScriptObject obj) { return null; }
        public virtual ScriptObject Multiply(ScriptObject obj) { return null; }
        public virtual ScriptObject Divide(ScriptObject obj) { return null; }
        public virtual ScriptObject Modulo(ScriptObject obj) { return null; }
        public abstract ObjectType Type { get; }
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
