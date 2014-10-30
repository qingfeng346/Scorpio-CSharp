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
        public virtual ScriptObject Assign() { return this; }                           // 赋值
        //设置变量
        public virtual void SetValue(int key, ScriptObject value) { throw new ExecutionException("类型[" + Type + "]不支持获取变量(int)"); }
        //获取变量
        public virtual ScriptObject GetValue(int key) { throw new ExecutionException("类型[" + Type + "]不支持设置变量(int)"); }
        //设置变量
        public virtual void SetValue(string key, ScriptObject value) { throw new ExecutionException("类型[" + Type + "]不支持获取变量(string)"); }
        //获取变量
        public virtual ScriptObject GetValue(string key) { throw new ExecutionException("类型[" + Type + "]不支持设置变量(string)"); }
        //设置变量
        public virtual void SetValue(object key, ScriptObject value) { throw new ExecutionException("类型[" + Type + "]不支持获取变量(object)"); }
        //获取变量
        public virtual ScriptObject GetValue(object key) { throw new ExecutionException("类型[" + Type + "]不支持设置变量(object)"); }
        //调用无参函数
        public ScriptObject Call() { return Call(NOPARAMETER); }
        //调用函数
        public virtual ScriptObject Call(ScriptObject[] parameters) { throw new ExecutionException("类型[" + Type + "]不支持函数调用"); }
        public virtual ScriptObject Clone() { return this; }                            // 复制一个变量
        public override string ToString() { return ObjectValue.ToString(); }            // ToString
        public ScriptObject(Script script) { Script = script; }                         // 构图函数
        public Script Script { get; protected set; }                                    // 所在脚本对象
        public abstract ObjectType Type { get; }                                        // 变量类型
        public virtual int BranchType { get { return 0; } }                             // 分支类型
        public virtual object ObjectValue { get { return this; } }                      // 变量值
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
