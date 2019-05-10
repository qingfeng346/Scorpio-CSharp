using System;
using Scorpio.Exception;
namespace Scorpio {
    public enum ObjectType {
        Type,           //原表
        Array,          //数组
        Map,            //MAP
        Function,       //函数
        Instance,       //原表实例
        Enum,           //枚举
        UserData,       //普通类
        Global,         //全局变量保存类,只有_G是这个类型
    }
    public abstract class ScriptObject {
        protected static ScriptValue[] Parameters = new ScriptValue[64];
        protected Script m_Script { get; set; }
        // 构图函数
        public ScriptObject(Script script, ObjectType objectType) {
            m_Script = script;
            ObjectType = objectType;
        }
        public ObjectType ObjectType { get; private set; }                              //类型
        public virtual object Value { get { return this; } }                            //值
        public virtual Type ValueType { get { return GetType(); } }                     //值类型
        public virtual Type Type { get { return GetType(); } }                          //获取类型
        public virtual string ValueTypeName { get { return ObjectType.ToString(); } }   //类型名称
        //获取变量
        public virtual ScriptValue GetValueByIndex(int key) { throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量[{key}]"); }
        public virtual ScriptValue GetValue(string key) { throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量[{key}]"); }
        public virtual ScriptValue GetValue(object key) { return GetValue(key as string); }

        //设置变量
        public virtual void SetValueByIndex(int key, ScriptValue value) { throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量[{key}]"); }
        public virtual void SetValue(string key, ScriptValue value) { throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量[{key}]"); }
        public virtual void SetValue(object key, ScriptValue value) { SetValue(key as string, value); }


        public virtual bool Less(ScriptValue obj) { throw new ExecutionException($"类型[{ValueTypeName}]不支持 [<] 运算"); }
        public virtual bool LessOrEqual(ScriptValue obj) { throw new ExecutionException($"类型[{ValueTypeName}]不支持 [<=] 运算"); }
        public virtual bool Greater(ScriptValue obj) { throw new ExecutionException($"类型[{ValueTypeName}]不支持 [>] 运算"); }
        public virtual bool GreaterOrEqual(ScriptValue obj) { throw new ExecutionException($"类型[{ValueTypeName}]不支持 [>=] 运算"); }
        public virtual bool Equals(ScriptValue obj) { return obj.valueType == ScriptValue.scriptValueType && obj.scriptValue == this; }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override bool Equals(object obj) { return Equals(m_Script.CreateObject(obj)); }

        public virtual ScriptValue Plus(ScriptValue obj) { throw new ExecutionException($"类型[{ValueTypeName}]不支持 [+] 运算"); }
        public virtual ScriptValue Minus(ScriptValue obj) { throw new ExecutionException($"类型[{ValueTypeName}]不支持 [-] 运算"); }
        public virtual ScriptValue Multiply(ScriptValue obj) { throw new ExecutionException($"类型[{ValueTypeName}]不支持 [*] 运算"); }
        public virtual ScriptValue Divide(ScriptValue obj) { throw new ExecutionException($"类型[{ValueTypeName}]不支持 [/] 运算"); }
        public virtual ScriptValue Modulo(ScriptValue obj) { throw new ExecutionException($"类型[{ValueTypeName}]不支持 [%] 运算"); }
        public virtual ScriptValue InclusiveOr(ScriptValue obj) { throw new ExecutionException($"类型[{ValueTypeName}]不支持 [|] 运算"); }
        public virtual ScriptValue Combine(ScriptValue obj) { throw new ExecutionException($"类型[{ValueTypeName}]不支持 [&] 运算"); }
        public virtual ScriptValue XOR(ScriptValue obj) { throw new ExecutionException($"类型[{ValueTypeName}]不支持 [^] 运算"); }
        public virtual ScriptValue Shi(ScriptValue obj) { throw new ExecutionException($"类型[{ValueTypeName}]不支持 [<<] 运算"); }
        public virtual ScriptValue Shr(ScriptValue obj) { throw new ExecutionException($"类型[{ValueTypeName}]不支持 [>>] 运算"); }

        //调用函数
        public ScriptValue call(ScriptValue thisObject, params object[] args) {
            var length = args.Length;
            var parameters = new ScriptValue[length];
            for (var i = 0; i < length; ++i) parameters[i] = m_Script.CreateObject(args[i]);
            return Call(thisObject, parameters, length);
        }
        //调用函数
        public virtual ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) { throw new ExecutionException($"类型[{ValueTypeName}]不支持函数调用"); }

        public virtual string ToJson() { return ToString(); }                           // ToJson
        public override string ToString() { return base.ToString(); }                   // ToString
        public virtual ScriptObject Clone() { return this; }                            // 复制一个变量

        public bool IsFunction { get { return (ObjectType == ObjectType.Function); } }
        public bool IsArray { get { return (ObjectType == ObjectType.Array); } }
        public bool IsMap { get { return (ObjectType == ObjectType.Map); } }
        public bool IsType { get { return (ObjectType == ObjectType.Type); } }
        public bool IsInstance { get { return (ObjectType == ObjectType.Instance); } }
        public bool IsEnum { get { return (ObjectType == ObjectType.Enum); } }
        public bool IsUserData { get { return (ObjectType == ObjectType.UserData); } }
    }
}
