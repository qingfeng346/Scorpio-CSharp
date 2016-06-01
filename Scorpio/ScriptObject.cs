using System;
using Scorpio.Exception;
using Scorpio.Compiler;
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
        // 无参                            
        private static readonly ScriptObject[] NOPARAMETER = new ScriptObject[0];
        // Object名字
        public String Name { get; set; }
        // 赋值
        public virtual ScriptObject Assign() { return this; }                           
        //设置变量
        public virtual void SetValue(object key, ScriptObject value) { throw new ExecutionException(m_Script, "类型[" + Type + "]不支持设置变量[" + key + "]"); }
        //获取变量
        public virtual ScriptObject GetValue(object key) { throw new ExecutionException(m_Script, "类型[" + Type + "]不支持获取变量[" + key + "]"); }
        public object call(params object[] args)
        {
            int length = args.Length;
            ScriptObject[] parameters = new ScriptObject[length];
            for (int i = 0; i < length; ++i) parameters[i] = m_Script.CreateObject(args[i]);
            return Call(parameters);
        }
        //调用无参函数
        public object Call() { return Call(NOPARAMETER); }
        //调用函数
        public virtual object Call(ScriptObject[] parameters) { throw new ExecutionException(m_Script, "类型[" + Type + "]不支持函数调用[" + Name + "]"); }
        //两个数值比较 > >= < <=
        public virtual bool Compare(TokenType type, ScriptObject obj) { throw new ExecutionException(m_Script, "类型[" + Type + "]不支持值比较[" + type + "]"); }
        //运算符或者位运算 + - * / % | & ^ >> <<
        public virtual ScriptObject Compute(TokenType type, ScriptObject obj) { throw new ExecutionException(m_Script, "类型[" + Type + "]不支持运算符[" + type + "]"); }
        //运算符或者位运算赋值运算 += -= *= /= %= |= &= ^= >>= <<=
        public virtual ScriptObject AssignCompute(TokenType type, ScriptObject obj) { throw new ExecutionException(m_Script, "类型[" + Type + "]不支持赋值运算符[" + type + "]"); }
        //逻辑运算符 逻辑运算时 Object 算 true 或者 false
        public virtual bool LogicOperation() { return true; }
        // 复制一个变量
        public virtual ScriptObject Clone() { return this; }
        // ToJson
        public virtual string ToJson() { return ObjectValue.ToString(); }
        // ToString
        public override string ToString() { return ObjectValue.ToString(); }
        // Equals
        public override bool Equals(object obj) {                                       
            if (obj == null) return false;
            if (!(obj is ScriptObject)) return false;
            if (ObjectValue == this) return obj == this;
            return ObjectValue.Equals(((ScriptObject)obj).ObjectValue);
        }
        // GetHashCode
        public override int GetHashCode() {                                             
            return base.GetHashCode();
        }
        protected Script m_Script;
        public ScriptObject(Script script) { m_Script = script; }                      // 构图函数
        public Script Script { get { return m_Script; } }                               // 所在脚本对象
        public abstract ObjectType Type { get; }                                        // 变量类型
        public virtual int BranchType { get { return 0; } }                             // 分支类型
        public virtual object ObjectValue { get { return this; } }                      // 变量值
        public virtual object KeyValue { get { return this; } }                         // 作为key值
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
