using System;
using System.Collections;
using Scorpio.Userdata;
using Scorpio.Exception;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Scorpio.Tools;

namespace Scorpio {
    [StructLayout(LayoutKind.Explicit)]
    public struct ScriptValue {
        public static readonly ScriptValue Null = new ScriptValue();
        public static readonly ScriptValue True = new ScriptValue(true);
        public static readonly ScriptValue False = new ScriptValue(false);
        public static readonly ScriptValue Zero = new ScriptValue((double)0);
        public static readonly ScriptValue InvalidIndex = new ScriptValue((double)-1);


        public const byte nullValueType = 0;        //null
        public const byte scriptValueType = 1;      //脚本变量
        public const byte doubleValueType = 2;      //double
        public const byte longValueType = 3;        //long
        public const byte trueValueType = 4;        //true
        public const byte falseValueType = 5;       //false
        public const byte stringValueType = 6;      //string
        public const byte objectValueType = 7;      //除了 double long 以外的number类型 和 枚举

        [FieldOffset(0)] public string stringValue;
        [FieldOffset(0)] public ScriptObject scriptValue;
        [FieldOffset(0)] public object objectValue;
        [FieldOffset(8)] public double doubleValue;
        [FieldOffset(8)] public long longValue;
        [FieldOffset(16)] public byte valueType;
        #region 构造函数
        public ScriptValue(bool value) {
            this.valueType = value ? trueValueType : falseValueType;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null;
            this.objectValue = null;
            this.scriptValue = null;
        }
        public ScriptValue(double value) {
            this.valueType = doubleValueType;
            this.longValue = 0;
            this.doubleValue = value;
            this.stringValue = null;
            this.objectValue = null;
            this.scriptValue = null;
        }
        public ScriptValue(long value) {
            this.valueType = longValueType;
            this.doubleValue = 0;
            this.longValue = value;
            this.stringValue = null;
            this.objectValue = null;
            this.scriptValue = null;
        }
        public ScriptValue(string value) {
            this.valueType = value == null ? nullValueType : stringValueType;
            this.doubleValue = 0;
            this.longValue = 0;
            this.scriptValue = null;
            this.objectValue = null;
            this.stringValue = value;
        }
        public ScriptValue(ScriptObject value) {
            this.valueType = value == null ? nullValueType : scriptValueType;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null;
            this.objectValue = null;
            this.scriptValue = value;
        }
        #endregion
        #region 创建其他number类型和枚举
        internal ScriptValue(sbyte value) {
            this.valueType = objectValueType;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null;
            this.scriptValue = null;
            this.objectValue = value;
        }
        internal ScriptValue(byte value) {
            this.valueType = objectValueType;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null;
            this.scriptValue = null;
            this.objectValue = value;
        }
        internal ScriptValue(short value) {
            this.valueType = objectValueType;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null;
            this.scriptValue = null;
            this.objectValue = value;
        }
        internal ScriptValue(ushort value) {
            this.valueType = objectValueType;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null;
            this.scriptValue = null;
            this.objectValue = value;
        }
        internal ScriptValue(int value) {
            this.valueType = objectValueType;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null;
            this.scriptValue = null;
            this.objectValue = value;
        }
        internal ScriptValue(uint value) {
            this.valueType = objectValueType;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null;
            this.scriptValue = null;
            this.objectValue = value;
        }
        internal ScriptValue(ulong value) {
            this.valueType = objectValueType;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null;
            this.scriptValue = null;
            this.objectValue = value;
        }
        internal ScriptValue(char value) {
            this.valueType = objectValueType;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null;
            this.scriptValue = null;
            this.objectValue = value;
        }
        internal ScriptValue(float value) {
            this.valueType = objectValueType;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null;
            this.scriptValue = null;
            this.objectValue = value;
        }
        internal ScriptValue(decimal value) {
            this.valueType = objectValueType;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null;
            this.scriptValue = null;
            this.objectValue = value;
        }
        //私有,创建枚举
        private ScriptValue(object value) {
            this.valueType = objectValueType;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null;
            this.scriptValue = null;
            this.objectValue = value;
        }
        #endregion
        #region 仅运行时调用
        //此函数为运行时调用，传入script 可以获取 基础类型的原表变量
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ScriptValue GetValueByIndex(int key, Script script) {
            switch (valueType) {
                case scriptValueType:
                    return scriptValue.GetValueByIndex(key);
                case doubleValueType:
                case longValueType:
                    return script.TypeNumber.GetValueByIndex(key);
                case stringValueType:
                    return script.TypeString.GetValueByIndex(key);
                case trueValueType:
                case falseValueType:
                    return script.TypeBoolean.GetValueByIndex(key);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Index : [{key}]");
            }
        }
        //此函数为运行时调用，传入script 可以获取 基础类型的原表变量
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ScriptValue GetValue(string key, Script script) {
            switch (valueType) {
                case scriptValueType:
                    return scriptValue.GetValue(key);
                case doubleValueType:
                case longValueType:
                    return script.TypeNumber.GetValue(key);
                case stringValueType:
                    return script.TypeString.GetValue(key);
                case trueValueType:
                case falseValueType:
                    return script.TypeBoolean.GetValue(key);
                case nullValueType:
                    throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 String : [{key}]");
                default:
                    return script.TypeObject.GetValue(key);
            }
        }
        #endregion
        #region GetValue SetValue
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue GetValue(string key) {
            if (valueType == scriptValueType) {
                return scriptValue.GetValue(key);
            }
            throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 String : [{key}]");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue GetValue(double key) {
            switch (valueType) {
                case scriptValueType: return scriptValue.GetValue(key);
                case stringValueType: return new ScriptValue(stringValue[(int)key]);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Double : [{key}]");
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue GetValue(long key) {
            switch (valueType) {
                case scriptValueType: return scriptValue.GetValue(key);
                case stringValueType: return new ScriptValue(stringValue[(int)key]);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Long : [{key}]");
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue GetValue(object key) {
            switch (valueType) {
                case scriptValueType: return scriptValue.GetValue(key);
                case stringValueType: return new ScriptValue(stringValue[Convert.ToInt32(key)]);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Object : [{key}]");
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValueByIndex(int key, ScriptValue value) {
            if (valueType == scriptValueType) {
                scriptValue.SetValueByIndex(key, value);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 Index : [{key}]");
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(string key, ScriptValue value) {
            if (valueType == scriptValueType) {
                scriptValue.SetValue(key, value);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 String : [{key}]");
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(double key, ScriptValue value) {
            if (valueType == scriptValueType) {
                scriptValue.SetValue(key, value);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 Double : [{key}]");
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(long key, ScriptValue value) {
            if (valueType == scriptValueType) {
                scriptValue.SetValue(key, value);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 Long : [{key}]");
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(object key, ScriptValue value) {
            if (valueType == scriptValueType) {
                scriptValue.SetValue(key, value);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 Object : [{key}]");
            }
        }
        #endregion
        //调用函数
        public ScriptValue call(ScriptValue thisObject, params object[] args) {
            var length = args.Length;
            var parameters = ScorpioUtil.Parameters;
            for (var i = 0; i < length; ++i) parameters[i] = CreateValue(args[i]);
            return Call(thisObject, parameters, length);
        }
        //调用无参函数
        public ScriptValue Call(ScriptValue thisObject) { return Call(thisObject, ScorpioUtil.VALUE_EMPTY, 0); }
        //调用函数
        public ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            if (valueType == scriptValueType) {
                return scriptValue.Call(thisObject, parameters, length);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持函数调用");
            }
        }
        //调用base函数
        internal ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            if (valueType == scriptValueType) {
                return scriptValue.Call(thisObject, parameters, length, baseType);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持base函数调用");
            }
        }
        //await 调用异步函数
        internal ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            if (valueType == scriptValueType) {
                return scriptValue.CallAsync(thisObject, parameters, length);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持函数调用");
            }
        }
        //await 调用异步base函数
        internal ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            if (valueType == scriptValueType) {
                return scriptValue.CallAsync(thisObject, parameters, length, baseType);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持base函数调用");
            }
        }
        //传入参数
        public object Value {
            get {
                switch (valueType) {
                    case doubleValueType: return doubleValue;
                    case longValueType: return longValue;
                    case nullValueType: return null;
                    case trueValueType: return true;
                    case falseValueType: return false;
                    case stringValueType: return stringValue;
                    case objectValueType: return objectValue;
                    default: return scriptValue.Value;
                }
            }
        }
        public string ValueTypeName {
            get {
                switch (valueType) {
                    case nullValueType:
                        return "Null";
                    case trueValueType:
                    case falseValueType:
                        return "Boolean";
                    case doubleValueType:
                    case longValueType:
                        return "Number";
                    case stringValueType:
                        return "String";
                    case objectValueType:
                        return objectValue.GetType().FullName;
                    default:
                        return scriptValue.ValueTypeName;
                }
            }
        }
        public int ToInt32() {
            switch (valueType) {
                case doubleValueType: return (int)doubleValue;
                case longValueType: return (int)longValue;
                case objectValueType: return Convert.ToInt32(objectValue);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持转换为 int32");
            }
        }
        public double ToDouble() {
            switch (valueType) {
                case doubleValueType: return doubleValue;
                case longValueType: return longValue;
                case objectValueType: return Convert.ToDouble(objectValue);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持转换为 double");
            }
        }
        public long ToLong() {
            switch (valueType) {
                case doubleValueType: return (long)doubleValue;
                case longValueType: return longValue;
                case objectValueType: return Convert.ToInt64(objectValue);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持转换为 long");
            }
        }
        public ulong ToULong() {
            switch (valueType) {
                case doubleValueType: return (ulong)doubleValue;
                case longValueType: return (ulong)longValue;
                case objectValueType: return Convert.ToUInt64(objectValue);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持转换为 ulong");
            }
        }
        public char ToChar() {
            switch (valueType) {
                case doubleValueType: return (char)doubleValue;
                case longValueType: return (char)longValue;
                case objectValueType: return Convert.ToChar(objectValue);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持转换为 char");
            }
        }
        public T Get<T>() where T : ScriptObject {
            return valueType == scriptValueType ? (scriptValue as T) : null;
        }
        public ScriptObject Get() {
            return valueType == scriptValueType ? scriptValue : null;
        }
        public T ToNumber<T>() where T : struct, IConvertible {
            switch (valueType) {
                case doubleValueType:
                    return (T)Convert.ChangeType(doubleValue, typeof(T));
                case longValueType:
                    return (T)Convert.ChangeType(longValue, typeof(T));
                default:
                    return (T)Convert.ChangeType(Value, typeof(T));
            }
        }

        public bool IsNull => valueType == nullValueType;
        public bool IsTrue => valueType == trueValueType;
        public bool IsFalse => valueType == falseValueType;
        public bool IsNumber => valueType == doubleValueType || valueType == longValueType;
        public bool IsString => valueType == stringValueType;
        public bool IsScriptObject => valueType == scriptValueType;

        
        public override string ToString() {
            switch (valueType) {
                case stringValueType: return stringValue;
                case doubleValueType: return doubleValue.ToString();
                case longValueType: return longValue.ToString();
                case trueValueType: return "true";
                case falseValueType: return "false";
                case nullValueType: return "null";
                case scriptValueType: return scriptValue.ToString();
                case objectValueType: return objectValue.ToString();
                default: return "";
            }
        }
        public override int GetHashCode() {
            switch (valueType) {
                case nullValueType: return 0;
                case trueValueType: return true.GetHashCode();
                case falseValueType: return false.GetHashCode();
                case doubleValueType: return doubleValue.GetHashCode();
                case longValueType: return longValue.GetHashCode();
                case stringValueType: return stringValue.GetHashCode();
                case objectValueType: return objectValue.GetHashCode();
                default: return scriptValue.GetHashCode();
            }
        }
        public override bool Equals(object obj) {
            if (obj == null) { 
                return valueType == nullValueType;
            } else if (obj is ScriptValue) {
                return Equals((ScriptValue)obj);
            } else if (obj is long) {
                return valueType == longValueType && longValue == (long)obj;
            } else if (obj is double) {
                return valueType == doubleValueType && doubleValue == (double)obj;
            } else if (obj is string) {
                return valueType == stringValueType && stringValue == (string)obj;
            } else if (obj is bool) {
                return (bool)obj ? valueType == trueValueType : valueType == falseValueType;
            } else if (obj is ScriptObject) {
                return valueType == scriptValueType && scriptValue.Value.Equals(obj);
            } else {
                return valueType == objectValueType && objectValue.Equals(obj);
            }
        }
        public bool Equals(ScriptValue value) {
            if (valueType != value.valueType) { return false; }
            switch (valueType) {
                case doubleValueType: return doubleValue == value.doubleValue;
                case longValueType: return longValue == value.longValue;
                case stringValueType: return stringValue == value.stringValue;
                case scriptValueType: return scriptValue.Equals(value);
                case objectValueType: return objectValue.Equals(value.objectValue);
                default: return true;
            }
        }

        public static ScriptValue CreateValue(object value) {
            if (value == null)
                return Null;
            else if (value is ScriptValue)
                return (ScriptValue)value;
            else if (value is bool)
                return (bool)value ? True : False;
            else if (value is string)
                return new ScriptValue((string)value);
            else if (value is double)
                return new ScriptValue((double)value);
            else if (value is int || value is float || value is byte || value is sbyte || value is short || value is ushort || value is uint)
                return new ScriptValue(Convert.ToDouble(value));
            else if (value is long)
                return new ScriptValue((long)value);
            else if (value is ulong || value is decimal)
                return new ScriptValue(value);
            else if (value is ScriptObject)
                return new ScriptValue((ScriptObject)value);
            else if (value is Type)
                return ScorpioTypeManager.GetUserdataType((Type)value);
            else if (value is Delegate)
                return new ScriptValue(new ScriptUserdataDelegate((Delegate)value));
            else if (value is Enum)
                return new ScriptValue(value);
            else if (value is IList)
                return new ScriptValue(new ScriptUserdataArray((IList)value, ScorpioTypeManager.GetType(value.GetType())));
            return new ScriptValue(new ScriptUserdataObject(value, ScorpioTypeManager.GetType(value.GetType())));
        }
    }
}
