using System;
using System.Collections;
using Scorpio.Userdata;
using Scorpio.Exception;
using System.Runtime.InteropServices;
namespace Scorpio {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ScriptValue {
        private const int ParameterLength = 128; //函数参数最大数量
        public static ScriptValue[] Parameters = new ScriptValue[ParameterLength]; //函数调用共用数组
        public static readonly ScriptValue[] EMPTY = new ScriptValue[0];
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

        [FieldOffset(0)] private object _objectValue;
        [FieldOffset(0)] private string _stringValue;
        [FieldOffset(0)] private ScriptObject _scriptValue;
        [FieldOffset(8)] private double _doubleValue;
        [FieldOffset(8)] private long _longValue;
        [FieldOffset(16)] private byte _valueType;
        public byte valueType { get => _valueType; private set => _valueType = value; }
        public double doubleValue { 
            get => _doubleValue;
            set {
                _valueType = doubleValueType;
                _doubleValue = value;
                _objectValue = null;
            }
        }
        public long longValue {
            get => _longValue;
            set {
                _valueType = longValueType;
                _longValue = value;
                _objectValue = null;
            }
        }
        public object objectValue {
            get => _objectValue;
            set {
                _valueType = objectValueType;
                _objectValue = value;
            }
        }
        public string stringValue {
            get => _stringValue;
            set {
                _stringValue = value;
                _valueType = stringValueType;
            }
        }
        public ScriptObject scriptValue {
            get => _scriptValue;
            set {
                _scriptValue = value;
                _valueType = scriptValueType;
            }
        }
        public bool boolValue {
            set {
                _valueType = value ? trueValueType : falseValueType;
                _objectValue = null;
            }
        }
        public void SetNull() {
            _valueType = nullValueType;
            _objectValue = null;
        }
        public void SetTrue() {
            _valueType = trueValueType;
            _objectValue = null;
        }
        public void SetFalse() {
            _valueType = falseValueType;
            _objectValue = null;
        }
        private ScriptValue(bool value) {
            this._valueType = value ? trueValueType : falseValueType;
            this._doubleValue = 0;
            this._longValue = 0;
            this._stringValue = null;
            this._objectValue = null;
            this._scriptValue = null;
        }
        public ScriptValue(double value) {
            this._valueType = doubleValueType;
            this._longValue = 0;
            this._doubleValue = value;
            this._stringValue = null;
            this._objectValue = null;
            this._scriptValue = null;
        }
        private ScriptValue(long value) {
            this._valueType = longValueType;
            this._doubleValue = 0;
            this._longValue = value;
            this._stringValue = null;
            this._objectValue = null;
            this._scriptValue = null;
        }
        public ScriptValue(string value) {
            this._valueType = value == null ? nullValueType : stringValueType;
            this._doubleValue = 0;
            this._longValue = 0;
            this._scriptValue = null;
            this._objectValue = null;
            this._stringValue = value;
        }
        public ScriptValue(ScriptObject value) {
            this._valueType = value == null ? nullValueType : scriptValueType;
            this._doubleValue = 0;
            this._longValue = 0;
            this._stringValue = null;
            this._objectValue = null;
            this._scriptValue = value;
        }
        private ScriptValue(sbyte value) {
            this._valueType = objectValueType;
            this._doubleValue = 0;
            this._longValue = 0;
            this._stringValue = null;
            this._scriptValue = null;
            this._objectValue = value;
        }
        private ScriptValue(byte value) {
            this._valueType = objectValueType;
            this._doubleValue = 0;
            this._longValue = 0;
            this._stringValue = null;
            this._scriptValue = null;
            this._objectValue = value;
        }
        private ScriptValue(short value) {
            this._valueType = objectValueType;
            this._doubleValue = 0;
            this._longValue = 0;
            this._stringValue = null;
            this._scriptValue = null;
            this._objectValue = value;
        }
        private ScriptValue(ushort value) {
            this._valueType = objectValueType;
            this._doubleValue = 0;
            this._longValue = 0;
            this._stringValue = null;
            this._scriptValue = null;
            this._objectValue = value;
        }
        private ScriptValue(int value) {
            this._valueType = objectValueType;
            this._doubleValue = 0;
            this._longValue = 0;
            this._stringValue = null;
            this._scriptValue = null;
            this._objectValue = value;
        }
        private ScriptValue(uint value) {
            this._valueType = objectValueType;
            this._doubleValue = 0;
            this._longValue = 0;
            this._stringValue = null;
            this._scriptValue = null;
            this._objectValue = value;
        }
        private ScriptValue(ulong value) {
            this._valueType = objectValueType;
            this._doubleValue = 0;
            this._longValue = 0;
            this._stringValue = null;
            this._scriptValue = null;
            this._objectValue = value;
        }
        private ScriptValue(char value) {
            this._valueType = objectValueType;
            this._doubleValue = 0;
            this._longValue = 0;
            this._stringValue = null;
            this._scriptValue = null;
            this._objectValue = value;
        }
        private ScriptValue(float value) {
            this._valueType = objectValueType;
            this._doubleValue = 0;
            this._longValue = 0;
            this._stringValue = null;
            this._scriptValue = null;
            this._objectValue = value;
        }
        private ScriptValue(decimal value) {
            this._valueType = objectValueType;
            this._doubleValue = 0;
            this._longValue = 0;
            this._stringValue = null;
            this._scriptValue = null;
            this._objectValue = value;
        }
        //私有,创建枚举
        private ScriptValue(object value) {
            this._valueType = objectValueType;
            this._doubleValue = 0;
            this._longValue = 0;
            this._stringValue = null;
            this._scriptValue = null;
            this._objectValue = value;
        }

        //此函数为运行时调用，传入script 可以获取 基础类型的原表变量
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
        public ScriptValue GetValue(string key) {
            if (valueType == scriptValueType) {
                return scriptValue.GetValue(key);
            }
            throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 String : [{key}]");
        }
        public ScriptValue GetValue(double key) {
            switch (valueType) {
                case scriptValueType: return scriptValue.GetValue(key);
                case stringValueType: return new ScriptValue(stringValue[(int)key]);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Double : [{key}]");
            }
        }
        public ScriptValue GetValue(long key) {
            switch (valueType) {
                case scriptValueType: return scriptValue.GetValue(key);
                case stringValueType: return new ScriptValue(stringValue[(int)key]);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Long : [{key}]");
            }
        }
        public ScriptValue GetValue(object key) {
            switch (valueType) {
                case scriptValueType: return scriptValue.GetValue(key);
                case stringValueType: return new ScriptValue(stringValue[Convert.ToInt32(key)]);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Object : [{key}]");
            }
        }
        public void SetValueByIndex(int key, ScriptValue value) {
            if (valueType == scriptValueType) {
                scriptValue.SetValueByIndex(key, value);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 Index : [{key}]");
            }
        }
        public void SetValue(string key, ScriptValue value) {
            if (valueType == scriptValueType) {
                scriptValue.SetValue(key, value);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 String : [{key}]");
            }
        }
        public void SetValue(double key, ScriptValue value) {
            if (valueType == scriptValueType) {
                scriptValue.SetValue(key, value);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 Double : [{key}]");
            }
        }
        public void SetValue(long key, ScriptValue value) {
            if (valueType == scriptValueType) {
                scriptValue.SetValue(key, value);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 Long : [{key}]");
            }
        }
        public void SetValue(object key, ScriptValue value) {
            if (valueType == scriptValueType) {
                scriptValue.SetValue(key, value);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 Object : [{key}]");
            }
        }
        //调用函数
        public ScriptValue call(ScriptValue thisObject, params object[] args) {
            var length = args.Length;
            var parameters = Parameters;
            for (var i = 0; i < length; ++i) parameters[i] = CreateValue(args[i]);
            return Call(thisObject, parameters, length);
        }
        //调用无参函数
        public ScriptValue Call(ScriptValue thisObject) { return Call(thisObject, EMPTY, 0); }
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
        public sbyte ToInt8() {
            switch (valueType) {
                case doubleValueType: return (sbyte)doubleValue;
                case longValueType: return (sbyte)longValue;
                case objectValueType: return Convert.ToSByte(objectValue);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持转换为 int8");
            }
        }
        public byte ToUint8() {
            switch (valueType) {
                case doubleValueType: return (byte)doubleValue;
                case longValueType: return (byte)longValue;
                case objectValueType: return Convert.ToByte(objectValue);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持转换为 uint8");
            }
        }
        public short ToInt16() {
            switch (valueType) {
                case doubleValueType: return (short)doubleValue;
                case longValueType: return (short)longValue;
                case objectValueType: return Convert.ToInt16(objectValue);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持转换为 int16");
            }
        }
        public ushort ToUint16() {
            switch (valueType) {
                case doubleValueType: return (ushort)doubleValue;
                case longValueType: return (ushort)longValue;
                case objectValueType: return Convert.ToUInt16(objectValue);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持转换为 uint16");
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
        public uint ToUint32() {
            switch (valueType) {
                case doubleValueType: return (uint)doubleValue;
                case longValueType: return (uint)longValue;
                case objectValueType: return Convert.ToUInt32(objectValue);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持转换为 uint32");
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
        public float ToFloat() {
            switch (valueType) {
                case doubleValueType: return (float)doubleValue;
                case longValueType: return (float)longValue;
                case objectValueType: return Convert.ToSingle(objectValue);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持转换为 float");
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
        public char ToChar() {
            switch (valueType) {
                case doubleValueType: return (char)doubleValue;
                case longValueType: return (char)longValue;
                case objectValueType: return Convert.ToChar(objectValue);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持转换为 char");
            }
        }
        public decimal ToDecimal() {
            switch (valueType) {
                case doubleValueType: return (decimal)doubleValue;
                case longValueType: return (decimal)longValue;
                case objectValueType: return Convert.ToDecimal(objectValue);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持转换为 decimal");
            }
        }
        public T Get<T>() where T : ScriptObject {
            return valueType == scriptValueType ? (scriptValue as T) : null;
        }
        public ScriptObject Get() {
            return valueType == scriptValueType ? scriptValue : null;
        }
        public bool IsNull { get { return valueType == nullValueType; } }
        public bool IsTrue { get { return valueType == trueValueType; } }
        public bool IsFalse { get { return valueType == falseValueType; } }
        public bool IsNumber { get { return valueType == doubleValueType || valueType == longValueType; } }
        public bool IsString { get { return valueType == stringValueType; } }
        public bool IsScriptObject { get { return valueType == scriptValueType; } }

        
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

        public static implicit operator ScriptValue(sbyte value) => new ScriptValue(value);
        public static implicit operator ScriptValue(byte value) => new ScriptValue(value);
        public static implicit operator ScriptValue(short value) => new ScriptValue(value);
        public static implicit operator ScriptValue(ushort value) => new ScriptValue(value);
        public static implicit operator ScriptValue(int value) => new ScriptValue(value);
        public static implicit operator ScriptValue(uint value) => new ScriptValue(value);
        public static implicit operator ScriptValue(ulong value) => new ScriptValue(value);
        public static implicit operator ScriptValue(float value) => new ScriptValue(value);
        public static implicit operator ScriptValue(char value) => new ScriptValue(value);
        public static implicit operator ScriptValue(decimal value) => new ScriptValue(value);

        public static implicit operator ScriptValue(bool value) => new ScriptValue(value);
        public static implicit operator ScriptValue(long value) => new ScriptValue(value);
        public static implicit operator ScriptValue(double value) => new ScriptValue(value);
        //public static implicit operator ScriptValue(string value) => new ScriptValue(value);
        //public static implicit operator ScriptValue(ScriptObject value) => new ScriptValue(value);
    }
}
