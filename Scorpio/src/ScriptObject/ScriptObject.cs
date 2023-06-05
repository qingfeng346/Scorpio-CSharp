using System;
using Scorpio.Exception;
using Scorpio.Tools;

namespace Scorpio {
    public enum ObjectType {
        Type,           //原表
        Array,          //数组
        Map,            //MAP
        Function,       //函数
        StringBuilder,  //StringBuilder
        HashSet,        //HashSet
        Instance,       //原表实例
        Enum,           //枚举
        Namespace,      //namespace
        UserData,       //普通类
        Global,         //全局变量保存类,只有_G是这个类型
    }
    public abstract class ScriptObject : IPool {
        private static uint AutomaticId = 0;
        protected Script m_Script;
        public uint Id { get; private set; }
        // 构图函数
        public ScriptObject(Script script, ObjectType objectType) {
            m_Script = script;
            ObjectType = objectType;
            Id = AutomaticId++;
        }
        public virtual void Alloc() { }
        public abstract void Free();
        public ObjectType ObjectType { get; private set; }              //类型
        public virtual object Value => this;                            //值
        public virtual Type ValueType => GetType();                     //值类型
        public virtual Type Type => GetType();                          //获取类型
        public virtual string ValueTypeName => ObjectType.ToString();   //类型名称
        public Script script => m_Script;
        //ThisValue没有占用引用计数
        protected ScriptValue ThisValue {
            get {
                using var ret = new ScriptValue(this);
                return ret;
            }
        }
        
        //获取变量
        public virtual ScriptValue GetValueByIndex(int key) { throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Index : {key}"); }
        public virtual ScriptValue GetValue(string key) { throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 String : {key}"); }
        public virtual ScriptValue GetValue(double key) { throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Double : {key}"); }
        public virtual ScriptValue GetValue(long key) { throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Long : {key}"); }
        public virtual ScriptValue GetValue(object key) { throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Object : {key}"); }

        //设置变量
        public virtual void SetValueByIndex(int key, ScriptValue value) { throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Index : {key}"); }
        public virtual void SetValue(string key, ScriptValue value) { throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 String : {key}"); }
        public virtual void SetValue(double key, ScriptValue value) { throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 Double : {key}"); }
        public virtual void SetValue(long key, ScriptValue value) { throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 Long : {key}"); }
        public virtual void SetValue(object key, ScriptValue value) { throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 Object : {key}"); }

        //比较运算符比较
        public virtual bool Less(ScriptValue obj) { throw new ExecutionException($"类型[{ValueTypeName}]不支持 [<] 运算"); }
        public virtual bool LessOrEqual(ScriptValue obj) { throw new ExecutionException($"类型[{ValueTypeName}]不支持 [<=] 运算"); }
        public virtual bool Greater(ScriptValue obj) { throw new ExecutionException($"类型[{ValueTypeName}]不支持 [>] 运算"); }
        public virtual bool GreaterOrEqual(ScriptValue obj) { throw new ExecutionException($"类型[{ValueTypeName}]不支持 [>=] 运算"); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override bool Equals(object obj) { using (var value = ScriptValue.CreateValue(m_Script, obj)) { return Equals(value); } }
        public virtual bool Equals(ScriptValue obj) { return obj.valueType == ScriptValue.scriptValueType && obj.scriptValue == this; }
        public bool EqualReference(ScriptValue obj) { return obj.valueType == ScriptValue.scriptValueType && ReferenceEquals(obj.scriptValue, this); }
        public override string ToString() { return base.ToString(); }

        //运算符
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
            using (var parameters = ScorpioParameters.Get()) {
                for (var i = 0; i < length; ++i)
                    parameters[i] = ScriptValue.CreateValue(m_Script, args[i]);
                return Call(thisObject, parameters.values, length);
            }
        }
        //调用无参函数
        public ScriptValue CallNoParameters(ScriptValue thisObject) { return Call(thisObject, ScriptValue.EMPTY, 0); }
        //调用函数
        public virtual ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) { throw new ExecutionException($"类型[{ValueTypeName}]不支持函数调用"); }
        internal virtual ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) { throw new ExecutionException($"类型[{ValueTypeName}]不支持base函数调用"); }
        internal virtual ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length) { return Call(thisObject, parameters, length); }
        internal virtual ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) { return Call(thisObject, parameters, length, baseType); }
        public virtual ScriptObject Clone(bool deep) { return this; }                   // 复制一个变量 是否深层复制

        public bool IsFunction => ObjectType == ObjectType.Function;
        public bool IsArray => ObjectType == ObjectType.Array;
        public bool IsMap => ObjectType == ObjectType.Map;
        public bool IsType => ObjectType == ObjectType.Type;
        public bool IsInstance => ObjectType == ObjectType.Instance;
        public bool IsEnum => ObjectType == ObjectType.Enum;
        public bool IsUserData => ObjectType == ObjectType.UserData;
        public bool IsStringBuilder => ObjectType == ObjectType.StringBuilder;
    }
}
