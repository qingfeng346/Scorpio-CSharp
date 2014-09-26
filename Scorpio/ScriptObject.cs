using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.CodeDom;
using Scorpio.Exception;

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
        Enum,           //枚举
        UserData,       //普通类
    }
    //脚本数据类型
    public abstract class ScriptObject
    {
        private static readonly ScriptObject[] NOPARAMETER = new ScriptObject[0];       // 没有参数
        public virtual void Assign(ScriptObject obj) { }                                // 赋值
        public virtual ScriptObject Plus(ScriptObject obj) { return null; }             // +=
        public virtual ScriptObject Minus(ScriptObject obj) { return null; }            // -=
        public virtual ScriptObject Multiply(ScriptObject obj) { return null; }         // *=
        public virtual ScriptObject Divide(ScriptObject obj) { return null; }           // /=
        public virtual ScriptObject Modulo(ScriptObject obj) { return null; }           // %=
        public ScriptObject Call() { return Call(NOPARAMETER); }                        // 无参调用
        public virtual ScriptObject Call(ScriptObject[] parameters) { throw new ExecutionException("类型 " + Type + " 不支持函数调用"); }        //函数调用
        public virtual ScriptObject Clone() { return this; }                            // 复制一个变量
        public abstract ObjectType Type { get; }                                        // 变量类型
        public virtual int BranchType { get { return 0; } }                             // 分支类型
        public virtual object ObjectValue { get { return null; } }                      // 变量值
        public bool IsPrimitive { get { return IsBoolean || IsNumber || IsString; } }
        public bool IsNull { get { return (Type == ObjectType.Null); } }
        public bool IsBoolean { get { return (Type == ObjectType.Boolean); } }
        public bool IsNumber { get { return (Type == ObjectType.Number); } }
        public bool IsString { get { return (Type == ObjectType.String); } }
        public bool IsFunction { get { return (Type == ObjectType.Function); } }
        public bool IsArray { get { return (Type == ObjectType.Array); } }
        public bool IsTable { get { return (Type == ObjectType.Table); } }
        public bool IsEnum { get { return (Type == ObjectType.Enum); } }
        public bool IsUserData { get { return (Type == ObjectType.UserData); } }
    }
}
