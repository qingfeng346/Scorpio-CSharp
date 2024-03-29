using System;
using Scorpio.Exception;
using Scorpio.Tools;
namespace Scorpio {
    public abstract class ScriptObject {
#if SCORPIO_DEBUG
        private static ulong AutoId = 0;
        public readonly ulong Id;
        // 构造函数
        public ScriptObject() {
            Id = AutoId++;
            ScorpioProfiler.AddRecord(Id, this);
        }
        ~ScriptObject() {
            ScorpioProfiler.DelRecord(Id);
        }
#endif
        public virtual object Value => this;                        //值
        public virtual Type ValueType => GetType();                 //值类型
        public virtual Type Type => GetType();                      //获取类型
        public virtual string ValueTypeName => GetType().Name;      //类型名称
        public ScriptValue ThisValue { 
            get {
                ScorpioUtil.CommonThisValue.scriptValue = this;
                return ScorpioUtil.CommonThisValue;
            }
        }
        //获取变量
        public virtual ScriptValue GetValueByIndex(int key) { throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Index : {key}"); }
        public virtual ScriptValue GetValue(string key) { throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 String : {key}"); }
        public virtual ScriptValue GetValue(double key) { throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Double : {key}"); }
        public virtual ScriptValue GetValue(long key) { throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Long : {key}"); }
        public virtual ScriptValue GetValue(object key) { throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Object : {key}"); }

        //设置变量
        public virtual void SetValueByIndex(int key, ScriptValue value) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持获取变量 Index : {key}"); }
        public virtual void SetValue(string key, ScriptValue value) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持设置变量 String : {key}"); }
        public virtual void SetValue(double key, ScriptValue value) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持设置变量 Double : {key}"); }
        public virtual void SetValue(long key, ScriptValue value) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持设置变量 Long : {key}"); }
        public virtual void SetValue(object key, ScriptValue value) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持设置变量 Object : {key}"); }

        //比较运算符比较
        public virtual bool Less(ScriptValue obj) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持 [<] 运算"); }
        public virtual bool LessOrEqual(ScriptValue obj) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持 [<=] 运算"); }
        public virtual bool Greater(ScriptValue obj) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持 [>] 运算"); }
        public virtual bool GreaterOrEqual(ScriptValue obj) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持 [>=] 运算"); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override bool Equals(object obj) { return Equals(ScriptValue.CreateValue(obj)); }
        public virtual bool Equals(ScriptValue obj) { return obj.valueType == ScriptValue.scriptValueType && obj.scriptValue == this; }
        public bool EqualReference(ScriptValue obj) { return obj.valueType == ScriptValue.scriptValueType && ReferenceEquals(obj.scriptValue, this); }
        public override string ToString() { return base.ToString(); }
        internal static int CombineHashCodes(int h1, int h2) {
            return ((h1 << 5) + h1) ^ h2;
        }
        //运算符
        public virtual ScriptValue Plus(ScriptValue obj) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持 [+] 运算"); }
        public virtual ScriptValue Minus(ScriptValue obj) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持 [-] 运算"); }
        public virtual ScriptValue Multiply(ScriptValue obj) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持 [*] 运算"); }
        public virtual ScriptValue Divide(ScriptValue obj) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持 [/] 运算"); }
        public virtual ScriptValue Modulo(ScriptValue obj) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持 [%] 运算"); }
        public virtual ScriptValue InclusiveOr(ScriptValue obj) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持 [|] 运算"); }
        public virtual ScriptValue Combine(ScriptValue obj) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持 [&] 运算"); }
        public virtual ScriptValue XOR(ScriptValue obj) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持 [^] 运算"); }
        public virtual ScriptValue Shi(ScriptValue obj) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持 [<<] 运算"); }
        public virtual ScriptValue Shr(ScriptValue obj) { throw new ExecutionException($"Object类型[{ValueTypeName}]不支持 [>>] 运算"); }

        //调用函数
        public ScriptValue call(ScriptValue thisObject, params object[] args) {
            var length = args.Length;
            var parameters = ScorpioUtil.Parameters;
            for (var i = 0; i < length; ++i) parameters[i] = ScriptValue.CreateValue(args[i]);
            return Call(thisObject, parameters, length);
        }
        //调用无参函数
        public ScriptValue CallNoParameters(ScriptValue thisObject) { return Call(thisObject, ScorpioUtil.VALUE_EMPTY, 0); }
        //调用函数
        public virtual ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) { throw new ExecutionException($"类型[{ValueTypeName}]不支持函数调用"); }
        internal virtual ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) { throw new ExecutionException($"类型[{ValueTypeName}]不支持base函数调用"); }
        internal virtual ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length) { return Call(thisObject, parameters, length); }
        internal virtual ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) { return Call(thisObject, parameters, length, baseType); }
        public virtual ScriptObject Clone(bool deep) { return this; }                   // 复制一个变量 是否深层复制
    }
}
