using System;
using System.Collections;
using Scorpio.Exception;
using System.Runtime.InteropServices;
using Scorpio.Tools;
using Scorpio.Userdata;

namespace Scorpio
{
    [StructLayout(LayoutKind.Explicit, Pack = 2)]
    public struct ScriptValue {
        private const int ParameterLength = 128; //函数参数最大数量
        public static readonly ScriptValue[] Parameters = new ScriptValue[ParameterLength]; //函数调用共用数组
        public static readonly ScriptValue[] EMPTY = new ScriptValue[0];
        public static readonly ScriptValue Null = new ScriptValue();
        public static readonly ScriptValue True = new ScriptValue(true);
        public static readonly ScriptValue False = new ScriptValue(false);
        public static readonly ScriptValue Zero = new ScriptValue((double)0);
        public static readonly ScriptValue InvalidIndex = new ScriptValue((double)-1);

        public const byte nullValueType = 0;        //null
        public const byte scriptValueType = 1;      //脚本变量
        public const byte trueValueType = 2;        //true
        public const byte falseValueType = 3;       //false
        public const byte stringValueType = 4;      //string
        public const byte int8ValueType = 11;       //
        public const byte uint8ValueType = 12;      //
        public const byte int16ValueType = 13;      //
        public const byte uint16ValueType = 14;     //
        public const byte int32ValueType = 15;      //
        public const byte uint32ValueType = 16;     //
        public const byte int64ValueType = 17;      //long
        public const byte uint64ValueType = 18;     //
        public const byte charValueType = 19;

        public const byte floatValueType = 20;      //
        public const byte doubleValueType = 21;     //double

        [FieldOffset(0)] private double _doubleValue;
        [FieldOffset(0)] private long _longValue;
        [FieldOffset(0)] private int _index;
        [FieldOffset(8)] public byte _valueType;
        public byte valueType {
            get => _valueType;
            private set => _valueType = value;
        }
        public bool boolValue {
            set {
                Free();
                _valueType = value ? trueValueType : falseValueType;
                _longValue = 0;
            }
        }
        public double doubleValue {
            get => _doubleValue;
            set {
                Free();
                _valueType = doubleValueType;
                _doubleValue = value;
            }
        }
        public long longValue {
            get => _longValue;
            set {
                Free();
                _valueType = int64ValueType;
                _longValue = value;
            }
        }
        public int scriptValueIndex {
            get => _index;
            set {
                Free();
                _index = value;
                _valueType = scriptValueType;
                ScriptObjectReference.Reference(_index);
            }
        }
        public int stringValueIndex {
            get => _index;
            set {
                Free();
                _index = value;
                _valueType = stringValueType;
                StringReference.Reference(_index);
            }
        }
        public string stringValue => StringReference.GetValue(_index);
        public ScriptObject scriptValue => ScriptObjectReference.GetValue(_index);
        public void SetNull() {
            Free();
            _valueType = nullValueType;
        }
        public void SetTrue() {
            Free();
            _valueType = trueValueType;
            _longValue = 0;
        }
        public void SetFalse() {
            Free();
            _valueType = falseValueType;
            _longValue = 0;
        }
        public void SetScriptValue(ScriptObject value) {
            Free();
            _valueType = scriptValueType;
            _index = ScriptObjectReference.Alloc(value);
        }
        public void SetStringValue(string value) {
            Free();
            _valueType = stringValueType;
            _index = StringReference.Alloc(value);
        }
        public void CopyFrom(ScriptValue value) {
            Free();
            _valueType = value._valueType;
            _longValue = value._longValue;
            if (_valueType == stringValueType) {
                StringReference.Reference(_index);
            } else if (_valueType == scriptValueType) {
                ScriptObjectReference.Reference(_index);
            }
        }
        public void Set(ScriptValue value) {
            Free();
            _valueType = value._valueType;
            _longValue = value._longValue;
        }
        public ScriptValue Reference() {
            if (_valueType == stringValueType) {
                StringReference.Reference(_index);
            } else if (_valueType == scriptValueType) {
                ScriptObjectReference.Reference(_index);
            }
            return this;
        }
        //只释放
        public void Release() {
            if (_valueType == stringValueType) {
                StringReference.Free(_index);
            } else if (_valueType == scriptValueType) {
                ScriptObjectReference.Free(_index);
            }
        }
        //释放并设置为null
        public void Free() {
            if (_valueType == stringValueType) {
                StringReference.Free(_index);
            } else if (_valueType == scriptValueType) {
                ScriptObjectReference.Free(_index);
            }
            _valueType = 0;
        }
        public ScriptValue(double value) {
            this._valueType = doubleValueType;
            this._index = 0;
            this._longValue = 0;
            this._doubleValue = value;
        }
        public ScriptValue(long value) {
            this._valueType = int64ValueType;
            this._index = 0;
            this._doubleValue = 0;
            this._longValue = value;
        }
        public ScriptValue(string value) {
            this._doubleValue = 0;
            this._longValue = 0;
            if (value == null) {
                this._valueType = nullValueType;
                this._index = 0;
            } else {
                this._valueType = stringValueType;
                this._index = StringReference.Alloc(value);
            }
        }
        public ScriptValue(ScriptObject value) {
            this._doubleValue = 0;
            this._longValue = 0;
            if (value == null) {
                this._valueType = nullValueType;
                this._index = 0;
            } else {
                this._valueType = scriptValueType;
                this._index = ScriptObjectReference.Alloc(value);
            }
        }
        internal ScriptValue(char value) {
            this._valueType = charValueType;
            this._index = 0;
            this._doubleValue = 0;
            this._longValue = value;
        }
        private ScriptValue(int value) {
            this._valueType = int32ValueType;
            this._index = 0;
            this._doubleValue = 0;
            this._longValue = value;
        }
        private ScriptValue(bool value) {
            this._valueType = value ? trueValueType : falseValueType;
            this._index = 0;
            this._doubleValue = 0;
            this._longValue = 0;
        }
        private ScriptValue(sbyte value) {
            this._valueType = int8ValueType;
            this._index = 0;
            this._doubleValue = 0;
            this._longValue = value;
        }
        private ScriptValue(byte value) {
            this._valueType = uint8ValueType;
            this._index = 0;
            this._doubleValue = 0;
            this._longValue = value;
        }
        private ScriptValue(short value) {
            this._valueType = int16ValueType;
            this._index = 0;
            this._doubleValue = 0;
            this._longValue = value;
        }
        private ScriptValue(ushort value) {
            this._valueType = uint16ValueType;
            this._index = 0;
            this._doubleValue = 0;
            this._longValue = value;
        }
        private ScriptValue(uint value) {
            this._valueType = uint32ValueType;
            this._index = 0;
            this._doubleValue = 0;
            this._longValue = value;
        }
        private ScriptValue(ulong value) {
            this._valueType = uint64ValueType;
            this._index = 0;
            this._doubleValue = 0;
            this._longValue = (long)value;
        }
        private ScriptValue(float value) {
            this._valueType = floatValueType;
            this._index = 0;
            this._longValue = 0;
            this._doubleValue = value;
        }
        private ScriptValue(string value, bool noReference) {
            this._doubleValue = 0;
            this._longValue = 0;
            if (value == null) {
                this._valueType = nullValueType;
                this._index = 0;
            } else {
                this._valueType = stringValueType;
                this._index = StringReference.GetIndex(value);
            }
        }
        internal ScriptValue(ScriptObject value, bool noReference) {
            this._doubleValue = 0;
            this._longValue = 0;
            if (value == null) {
                this._valueType = nullValueType;
                this._index = 0;
            } else {
                this._valueType = scriptValueType;
                this._index = ScriptObjectReference.GetIndex(value);
            }
        }


        //此函数为运行时调用
        internal ScriptValue GetValue(ScriptValue value, Script script) {
            switch (value.valueType) {
                case stringValueType:
                    return GetValue(value.stringValue, script);
                case doubleValueType:
                    return GetValue(value.doubleValue);
                case int64ValueType:
                    return GetValue(value.longValue);
                case floatValueType:
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType:
                case uint64ValueType:
                case charValueType:
                case trueValueType:
                case falseValueType:
                    return GetValue(value.Value);
                case scriptValueType: {
                    var userdata = value.Get<ScriptUserdataObject>();
                    if (userdata != null) {
                        return GetValue(userdata.Value);
                    }
                    throw new ExecutionException($"不支持当前类型作为变量 : {value.ValueTypeName}");
                }
                default:
                    throw new ExecutionException($"不支持当前类型作为变量 : {value.ValueTypeName}");
            }
        }
        //此函数为运行时调用
        internal void SetValue(ScriptValue key, ScriptValue value) {
            switch (key.valueType) {
                case stringValueType:
                    SetValue(key.stringValue, value);
                    return;
                case doubleValueType:
                    SetValue(key.doubleValue, value);
                    return;
                case int64ValueType:
                    SetValue(key.longValue, value);
                    return;
                case floatValueType:
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType:
                case uint64ValueType:
                case charValueType:
                case trueValueType:
                case falseValueType:
                    SetValue(key.Value, value);
                    return;
                case scriptValueType: {
                    var userdata = key.Get<ScriptUserdataObject>();
                    if (userdata != null) {
                        SetValue(userdata.Value, value);
                        return;
                    }
                    throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量:{key.ValueTypeName}");
                }
                default:
                    throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量:{key.ValueTypeName}");
            }
        }
        //此函数为运行时调用，传入script 可以获取 基础类型的原表变量
        internal ScriptValue GetValueByIndex(int key, Script script) {
            switch (valueType) {
                case scriptValueType:
                    return scriptValue.GetValueByIndex(key);
                case doubleValueType:
                case int64ValueType:
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
                case int64ValueType:
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
            if (valueType == scriptValueType) {
                return scriptValue.call(thisObject, args);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持函数调用");
            }
        }
        //调用无参函数
        public ScriptValue Call(ScriptValue thisObject) { 
            return Call(thisObject, EMPTY, 0);
        }
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
                    case doubleValueType: return _doubleValue;
                    case int64ValueType: return _longValue;
                    case nullValueType: return null;
                    case trueValueType: return true;
                    case falseValueType: return false;
                    case stringValueType: return stringValue;
                    case scriptValueType: return scriptValue.Value;
                    case floatValueType: return (float)_doubleValue;
                    case int8ValueType: return (sbyte)_longValue;
                    case uint8ValueType: return (byte)_longValue;
                    case int16ValueType: return (short)_longValue;
                    case uint16ValueType: return (ushort)_longValue;
                    case int32ValueType: return (int)_longValue;
                    case uint32ValueType: return (uint)_longValue;
                    case uint64ValueType: return (ulong)_longValue;
                    case charValueType: return (char)_longValue;
                    default: throw new ExecutionException($"未知的数据类型 : {valueType}");
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
                    case floatValueType:
                    case doubleValueType:
                    case int8ValueType:
                    case uint8ValueType:
                    case int16ValueType:
                    case uint16ValueType:
                    case int32ValueType:
                    case uint32ValueType:
                    case int64ValueType:
                    case uint64ValueType:
                    case charValueType:
                        return "Number";
                    case stringValueType:
                        return "String";
                    default:
                        return scriptValue.ValueTypeName;
                }
            }
        }
        public T ToNumber<T>() where T : struct, IConvertible {
            switch (valueType) {
                case floatValueType:
                case doubleValueType:
                    return (T)Convert.ChangeType(_doubleValue, typeof(T));
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType:
                case int64ValueType:
                case uint64ValueType:
                case charValueType:
                    return (T)Convert.ChangeType(_longValue, typeof(T));
                default:
                    return (T)Convert.ChangeType(Value, typeof(T));
            }
        }
        public int ToInt32() {
            switch (valueType) {
                case floatValueType:
                case doubleValueType:
                    return (int)_doubleValue;
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType:
                case int64ValueType:
                case uint64ValueType:
                case charValueType:
                    return (int)_longValue;
                default:
                    return Convert.ToInt32(Value);
            }
        }
        public double ToDouble() {
            switch (valueType) {
                case floatValueType:
                case doubleValueType:
                    return _doubleValue;
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType:
                case int64ValueType:
                case uint64ValueType:
                case charValueType:
                    return _longValue;
                default:
                    return Convert.ToDouble(Value);
            }
        }
        public long ToLong() {
            switch (valueType) {
                case floatValueType:
                case doubleValueType:
                    return (long)_doubleValue;
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType:
                case int64ValueType:
                case uint64ValueType:
                case charValueType:
                    return _longValue;
                default:
                    return Convert.ToInt64(Value);
            }
        }
        public char ToChar() {
            switch (valueType) {
                case floatValueType:
                case doubleValueType:
                    return (char)_doubleValue;
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType:
                case int64ValueType:
                case uint64ValueType:
                case charValueType:
                    return (char)_longValue;
                case stringValueType:
                    return stringValue[0];
                default:
                    return Convert.ToChar(Value);
            }
        }
        public T Get<T>() where T : ScriptObject {
            return valueType == scriptValueType ? (scriptValue as T) : null;
        }
        public bool IsNull => valueType == nullValueType;
        public bool IsTrue => valueType == trueValueType;
        public bool IsFalse => valueType == falseValueType;
        public bool IsNumber => valueType == doubleValueType || valueType == int64ValueType;
        public bool IsString => valueType == stringValueType;
        public bool IsScriptObject => valueType == scriptValueType;

        
        public override string ToString() {
            switch (valueType) {
                case stringValueType: return stringValue;
                case floatValueType:
                case doubleValueType: 
                    return _doubleValue.ToString();
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType:
                case int64ValueType:
                case uint64ValueType:
                    return _longValue.ToString();
                case charValueType:
                    return ((char)_longValue).ToString();
                case trueValueType: return "true";
                case falseValueType: return "false";
                case nullValueType: return "null";
                case scriptValueType: return scriptValue.ToString();
                default: return "";
            }
        }
        public override int GetHashCode() {
            switch (valueType) {
                case nullValueType: return 0;
                case trueValueType: return true.GetHashCode();
                case falseValueType: return false.GetHashCode();
                case floatValueType:
                case doubleValueType: 
                    return _doubleValue.GetHashCode();
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType:
                case int64ValueType:
                case uint64ValueType:
                case charValueType:
                    return _longValue.GetHashCode();
                case stringValueType: return stringValue.GetHashCode();
                default: return scriptValue.GetHashCode();
            }
        }
        public override bool Equals(object obj) {
            if (obj == null) { 
                return valueType == nullValueType;
            } else if (obj is ScriptValue) {
                return Equals((ScriptValue)obj);
            } else if (obj is long) {
                return valueType == int64ValueType && _longValue == (long)obj;
            } else if (obj is double) {
                return valueType == doubleValueType && _doubleValue == (double)obj;
            } else if (obj is string) {
                return valueType == stringValueType && stringValue == (string)obj;
            } else if (obj is bool) {
                return (bool)obj ? valueType == trueValueType : valueType == falseValueType;
            } else if (obj is ScriptObject) {
                return valueType == scriptValueType && scriptValue.Value.Equals(obj);
            } else {
                return false;
            }
        }
        public bool Equals(ScriptValue value) {
            if (valueType != value.valueType) { return false; }
            switch (valueType) {
                case floatValueType:
                case doubleValueType:
                    return _doubleValue == value._doubleValue;
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType:
                case int64ValueType:
                case uint64ValueType:
                case charValueType:
                    return _longValue == value._longValue;
                case stringValueType: return stringValue == value.stringValue;
                case scriptValueType: return scriptValue.Equals(value);
                default: return true;
            }
        }

        public static ScriptValue CreateValue(Script script, object value) {
            if (value == null)
                return Null;
            else if (value is ScriptValue)
                //需要增加一次引用计数
                return ((ScriptValue)value).Reference();
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
            else if (value is ulong)
                return new ScriptValue((long)(ulong)value);
            else if (value is ScriptObject)
                return new ScriptValue((ScriptObject)value);
            else if (value is Type)
                //需要增加一次引用计数
                return script.GetUserdataTypeValue((Type)value).Reference();
            else if (value is Delegate)
                return new ScriptValue(script.NewUserdataDelegate().Set((Delegate)value));
            else if (value is Enum)
                return new ScriptValue(Convert.ToInt64(value));
            else if (value is IList)
                return new ScriptValue(script.NewUserdataArray().Set(script.GetUserdataType(value.GetType()), (IList)value));
            return new ScriptValue(script.NewUserdataObject().Set(script.GetUserdataType(value.GetType()), value));
        }
        //不占用引用的value,如果是新创建的scriptobject和string,创建后会立即加入释放列表
        public static ScriptValue CreateValueNoReference(Script script, object value) {
            if (value == null)
                return Null;
            else if (value is ScriptValue)
                return (ScriptValue)value;
            else if (value is bool)
                return (bool)value ? True : False;
            else if (value is string)
                return new ScriptValue((string)value, true);
            else if (value is double)
                return new ScriptValue((double)value);
            else if (value is int || value is float || value is byte || value is sbyte || value is short || value is ushort || value is uint)
                return new ScriptValue(Convert.ToDouble(value));
            else if (value is long)
                return new ScriptValue((long)value);
            else if (value is ulong)
                return new ScriptValue((long)(ulong)value);
            else if (value is ScriptObject)
                return new ScriptValue((ScriptObject)value, true);
            else if (value is Type)
                return script.GetUserdataTypeValue((Type)value);
            else if (value is Delegate)
                return new ScriptValue(script.NewUserdataDelegate().Set((Delegate)value), true);
            else if (value is Enum)
                return new ScriptValue(Convert.ToInt64(value));
            else if (value is IList)
                return new ScriptValue(script.NewUserdataArray().Set(script.GetUserdataType(value.GetType()), (IList)value), true);
            return new ScriptValue(script.NewUserdataObject().Set(script.GetUserdataType(value.GetType()), value), true);
        }
        public static ScriptValue CreateNumber(object value) {
            if (value is double)
                return new ScriptValue((double)value);
            else if (value is long)
                return new ScriptValue((long)value);
            else if (value is char)
                return new ScriptValue((char)value);
            else if (value is int)
                return new ScriptValue((int)value);
            else if (value is char)
                return new ScriptValue((char)value);
            else if (value is sbyte)
                return new ScriptValue((sbyte)value);
            else if (value is byte)
                return new ScriptValue((byte)value);
            else if (value is short)
                return new ScriptValue((short)value);
            else if (value is ushort)
                return new ScriptValue((ushort)value);
            else if (value is uint)
                return new ScriptValue((uint)value);
            else if (value is ulong)
                return new ScriptValue((ulong)value);
            else if (value is float)
                return new ScriptValue((float)value);
            throw new ExecutionException($"未知的Number类型:{value?.GetType()?.FullName}");
        }
    }
}
