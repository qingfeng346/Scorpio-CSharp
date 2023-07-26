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
        public static readonly ScriptValue Null = new ScriptValue();
        public static readonly ScriptValue True = new ScriptValue(true);
        public static readonly ScriptValue False = new ScriptValue(false);
        public static readonly ScriptValue Zero = new ScriptValue((double)0);
        public static readonly ScriptValue InvalidIndex = new ScriptValue((double)-1);

        public const byte nullValueType = 0;        //null
        public const byte trueValueType = 1;        //true
        public const byte falseValueType = 2;       //false
        
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

        public const byte scriptValueType = 31;      //脚本变量
        public const byte stringValueType = 32;      //string

        [FieldOffset(0)] public double doubleValue;
        [FieldOffset(0)] public long longValue;
        [FieldOffset(0)] public int index;
        [FieldOffset(8)] public byte valueType;
        public string GetStringValue(Script script) {
            return script.GetStringValue(index);
        }
        public ScriptObject GetScriptValue(Script script) {
            return script.GetObjectValue(index);
        }
        public void SetBoolValue(bool value, Script script) {
            if (valueType > 30) {
                if (valueType == stringValueType) {
                    script.FreeString(index);
                } else if (valueType == scriptValueType) {
                    script.FreeObject(index);
                }
            }
            valueType = value ? trueValueType : falseValueType;
        }
        public void SetDoubleValue(double value, Script script) {
            if (valueType > 30) {
                if (valueType == stringValueType) {
                    script.FreeString(index);
                } else if (valueType == scriptValueType) {
                    script.FreeObject(index);
                }
            }
            valueType = doubleValueType;
            doubleValue = value;
        }
        public void SetLongValue(long value, Script script) {
            if (valueType > 30) {
                if (valueType == stringValueType) {
                    script.FreeString(index);
                } else if (valueType == scriptValueType) {
                    script.FreeObject(index);
                }
            }
            valueType = int64ValueType;
            longValue = value;
        }
        public void SetNull(Script script) {
            if (valueType > 30) {
                if (valueType == stringValueType) {
                    script.FreeString(index);
                } else if (valueType == scriptValueType) {
                    script.FreeObject(index);
                }
            }
            valueType = nullValueType;
        }
        public void SetTrue(Script script) {
            if (valueType > 30) {
                if (valueType == stringValueType) {
                    script.FreeString(index);
                } else if (valueType == scriptValueType) {
                    script.FreeObject(index);
                }
            }
            valueType = trueValueType;
        }
        public void SetFalse(Script script) {
            if (valueType > 30) {
                if (valueType == stringValueType) {
                    script.FreeString(index);
                } else if (valueType == scriptValueType) {
                    script.FreeObject(index);
                }
            }
            valueType = falseValueType;
        }
        public void SetScriptValue(ScriptObject value, Script script) {
            if (valueType > 30) {
                if (valueType == stringValueType) {
                    script.FreeString(index);
                } else if (valueType == scriptValueType) {
                    script.FreeObject(index);
                }
            }
            valueType = scriptValueType;
            index = ScriptObjectReference.Alloc(value);
        }
        public void SetStringValue(string value, Script script) {
            if (valueType > 30) {
                if (valueType == stringValueType) {
                    script.FreeString(index);
                } else if (valueType == scriptValueType) {
                    script.FreeObject(index);
                }
            }
            valueType = stringValueType;
            index = script.Alloc(value);
        }
        public void CopyFrom(ScriptValue value, Script script) {
            if (valueType > 30) {
                if (valueType == stringValueType) {
                    script.FreeString(index);
                } else if (valueType == scriptValueType) {
                    script.FreeObject(index);
                }
            }
            longValue = value.longValue;
            if ((valueType = value.valueType) > 30) {
                if (valueType == stringValueType) {
                    script.ReferenceString(index);
                } else if (valueType == scriptValueType) {
                    script.ReferenceObject(index);
                }
            }
        }
        public void Set(ScriptValue value, Script script) {
            if (valueType > 30) {
                if (valueType == stringValueType) {
                    script.FreeString(index);
                } else if (valueType == scriptValueType) {
                    script.FreeObject(index);
                }
            }
            valueType = value.valueType;
            longValue = value.longValue;
        }
        public ScriptValue Reference(Script script) {
            if (valueType > 30) {
                if (valueType == stringValueType) {
                    script.ReferenceString(index);
                } else if (valueType == scriptValueType) {
                    script.ReferenceObject(index);
                }
            }
            return this;
        }
        //只释放
        public void Release(Script script) {
            if (valueType > 30) {
                if (valueType == stringValueType) {
                    script.FreeString(index);
                } else if (valueType == scriptValueType) {
                    script.FreeObject(index);
                }
            }
        }
        //释放并设置为null
        public void Free(Script script) {
            if (valueType > 30) {
                if (valueType == stringValueType) {
                    script.FreeString(index);
                } else if (valueType == scriptValueType) {
                    script.FreeObject(index);
                }
            }
            valueType = 0;
        }
        public ScriptValue(double value) {
            this.valueType = doubleValueType;
            this.index = 0;
            this.longValue = 0;
            this.doubleValue = value;
        }
        public ScriptValue(long value) {
            this.valueType = int64ValueType;
            this.index = 0;
            this.doubleValue = 0;
            this.longValue = value;
        }
        public ScriptValue(string value, Script script) {
            this.doubleValue = 0;
            this.longValue = 0;
            if (value == null) {
                this.valueType = nullValueType;
                this.index = 0;
            } else {
                this.valueType = stringValueType;
                this.index = script.Alloc(value);
            }
        }
        public ScriptValue(ScriptObject value, Script script) {
            this.doubleValue = 0;
            this.longValue = 0;
            if (value == null) {
                this.valueType = nullValueType;
                this.index = 0;
            } else {
                this.valueType = scriptValueType;
                this.index = script.Alloc(value);
            }
        }
        private ScriptValue(ScriptValue value, Script script) {
            this.doubleValue = 0;
            this.index = 0;
            this.valueType = value.valueType;
            this.longValue = value.longValue;
            if (valueType > 30) {
                if (valueType == stringValueType) {
                    script.ReferenceString(index);
                } else if (valueType == scriptValueType) {
                    script.ReferenceObject(index);
                }
            }
        }
        internal ScriptValue(char value) {
            this.valueType = charValueType;
            this.index = 0;
            this.doubleValue = 0;
            this.longValue = value;
        }
        private ScriptValue(int value) {
            this.valueType = int32ValueType;
            this.index = 0;
            this.doubleValue = 0;
            this.longValue = value;
        }
        private ScriptValue(bool value) {
            this.valueType = value ? trueValueType : falseValueType;
            this.index = 0;
            this.doubleValue = 0;
            this.longValue = 0;
        }
        private ScriptValue(sbyte value) {
            this.valueType = int8ValueType;
            this.index = 0;
            this.doubleValue = 0;
            this.longValue = value;
        }
        private ScriptValue(byte value) {
            this.valueType = uint8ValueType;
            this.index = 0;
            this.doubleValue = 0;
            this.longValue = value;
        }
        private ScriptValue(short value) {
            this.valueType = int16ValueType;
            this.index = 0;
            this.doubleValue = 0;
            this.longValue = value;
        }
        private ScriptValue(ushort value) {
            this.valueType = uint16ValueType;
            this.index = 0;
            this.doubleValue = 0;
            this.longValue = value;
        }
        private ScriptValue(uint value) {
            this.valueType = uint32ValueType;
            this.index = 0;
            this.doubleValue = 0;
            this.longValue = value;
        }
        private ScriptValue(ulong value) {
            this.valueType = uint64ValueType;
            this.index = 0;
            this.doubleValue = 0;
            this.longValue = (long)value;
        }
        private ScriptValue(float value) {
            this.valueType = floatValueType;
            this.index = 0;
            this.longValue = 0;
            this.doubleValue = value;
        }
        private ScriptValue(string value, bool noReference, Script script) {
            this.doubleValue = 0;
            this.longValue = 0;
            this.valueType = stringValueType;
            this.index = script.GetIndex(value);
        }
        private ScriptValue(ScriptObject value, bool noReference, Script script) {
            this.doubleValue = 0;
            this.longValue = 0;
            this.valueType = scriptValueType;
            this.index = script.GetIndex(value);
        }

        #region 运行时调用
        //此函数为运行时调用
        internal ScriptValue GetValueByScriptValue(ScriptValue value, Script script) {
            switch (value.valueType) {
                case stringValueType:
                    return GetValueByString(value.GetStringValue(script), script);
                case doubleValueType:
                    return GetValue(value.doubleValue, script);
                case int64ValueType:
                    return GetValue(value.longValue, script);
                case floatValueType:
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType: //uint32 用作string
                case uint64ValueType:
                case charValueType:
                case trueValueType:
                case falseValueType:
                    return GetValue(value.GetObject(script), script);
                case scriptValueType: {
                    var userdata = value.Get<ScriptUserdataObject>(script);
                    if (userdata != null) return GetValue(userdata.Value, script);
                    throw new ExecutionException($"类型[{ValueTypeName}]不支持以下类型作为变量 : {value.ValueTypeName}");
                }
                default:
                    throw new ExecutionException($"类型[{ValueTypeName}]不支持以下类型作为变量 : {value.ValueTypeName}");
            }
        }
        //此函数为运行时调用，传入script 可以获取 基础类型的原表变量
        internal ScriptValue GetValueByIndex(int key, Script script) {
            switch (valueType) {
                case scriptValueType:
                    return GetScriptValue(script).GetValueByIndex(key);
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
        internal ScriptValue GetValueByString(string key, Script script) {
            switch (valueType) {
                case scriptValueType:
                    return GetScriptValue(script).GetValue(key);
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
        //此函数为运行时调用
        internal void SetValueByScriptValue(ScriptValue key, ScriptValue value, Script script) {
            switch (key.valueType) {
                case stringValueType:
                    SetValue(key.GetStringValue(script), value, script);
                    return;
                case doubleValueType:
                    SetValue(key.doubleValue, value, script);
                    return;
                case int64ValueType:
                    SetValue(key.longValue, value, script);
                    return;
                case floatValueType:
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType: //uint32 用作 string
                case uint64ValueType:
                case charValueType:
                case trueValueType:
                case falseValueType:
                    SetValue(key.GetObject(script), value, script);
                    return;
                case scriptValueType: {
                    var userdata = key.Get<ScriptUserdataObject>(script);
                    if (userdata != null) {
                        SetValue(userdata.Value, value, script);
                        return;
                    }
                    throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量:{key.ValueTypeName}");
                }
                default:
                    throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量:{key.ValueTypeName}");
            }
        }
        internal void SetValueByIndex(int key, ScriptValue value, Script script) {
            if (valueType == scriptValueType) {
                GetScriptValue(script).SetValueByIndex(key, value);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 Index : [{key}]");
            }
        }
        #endregion
        #region GetValue SetValue
        public ScriptValue GetValue(string key, Script script) {
            if (valueType == scriptValueType) {
                return GetScriptValue(script).GetValue(key);
            }
            throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 String : [{key}]");
        }
        public ScriptValue GetValue(double key, Script script) {
            switch (valueType) {
                case scriptValueType: return GetScriptValue(script).GetValue(key);
                case stringValueType: return new ScriptValue(GetStringValue(script)[(int)key]);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Double : [{key}]");
            }
        }
        public ScriptValue GetValue(long key, Script script) {
            switch (valueType) {
                case scriptValueType: return GetScriptValue(script).GetValue(key);
                case stringValueType: return new ScriptValue(GetStringValue(script)[(int)key]);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Long : [{key}]");
            }
        }
        public ScriptValue GetValue(object key, Script script) {
            switch (valueType) {
                case scriptValueType: return GetScriptValue(script).GetValue(key);
                case stringValueType: return new ScriptValue(GetStringValue(script)[Convert.ToInt32(key)]);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 Object : [{key}]");
            }
        }

        public void SetValue(string key, ScriptValue value, Script script) {
            if (valueType == scriptValueType) {
                GetScriptValue(script).SetValue(key, value);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 String : [{key}]");
            }
        }
        public void SetValue(double key, ScriptValue value, Script script) {
            if (valueType == scriptValueType) {
                GetScriptValue(script).SetValue(key, value);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 Double : [{key}]");
            }
        }
        public void SetValue(long key, ScriptValue value, Script script) {
            if (valueType == scriptValueType) {
                GetScriptValue(script).SetValue(key, value);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 Long : [{key}]");
            }
        }
        public void SetValue(object key, ScriptValue value, Script script) {
            if (valueType == scriptValueType) {
                GetScriptValue(script).SetValue(key, value);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 Object : [{key}]");
            }
        }
        #endregion
        #region Call
        //调用函数
        public ScriptValue call(Script script, ScriptValue thisObject, params object[] args) {
            if (valueType == scriptValueType) {
                return GetScriptValue(script).call(thisObject, args);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持函数调用");
            }
        }
        //调用无参函数
        public ScriptValue Call(Script script, ScriptValue thisObject) { 
            return Call(script, thisObject, ScorpioUtil.VALUE_EMPTY, 0);
        }
        //调用函数
        public ScriptValue Call(Script script, ScriptValue thisObject, ScriptValue[] parameters, int length) {
            if (valueType == scriptValueType) {
                return GetScriptValue(script).Call(thisObject, parameters, length);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持函数调用");
            }
        }
        //调用base函数
        internal ScriptValue Call(Script script, ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            if (valueType == scriptValueType) {
                return GetScriptValue(script).Call(thisObject, parameters, length, baseType);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持base函数调用");
            }
        }
        //await 调用异步函数
        internal ScriptValue CallAsync(Script script, ScriptValue thisObject, ScriptValue[] parameters, int length) {
            if (valueType == scriptValueType) {
                return GetScriptValue(script).CallAsync(thisObject, parameters, length);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持函数调用");
            }
        }
        //await 调用异步base函数
        internal ScriptValue CallAsync(Script script, ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            if (valueType == scriptValueType) {
                return GetScriptValue(script).CallAsync(thisObject, parameters, length, baseType);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持base函数调用");
            }
        }
        #endregion
        //传入参数
        public object GetObject(Script script) {
            switch (valueType) {
                case doubleValueType: return doubleValue;
                case int64ValueType: return longValue;
                case nullValueType: return null;
                case trueValueType: return true;
                case falseValueType: return false;
                case stringValueType: return GetStringValue(script);
                case scriptValueType: return GetScriptValue(script).Value;
                case floatValueType: return (float)doubleValue;
                case int8ValueType: return (sbyte)longValue;
                case uint8ValueType: return (byte)longValue;
                case int16ValueType: return (short)longValue;
                case uint16ValueType: return (ushort)longValue;
                case int32ValueType: return (int)longValue;
                case uint32ValueType: return (uint)longValue;
                case uint64ValueType: return (ulong)longValue;
                case charValueType: return (char)longValue;
                default: throw new ExecutionException($"未知的数据类型 : {valueType}");
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
                        return "ScriptObject";
                }
            }
        }
        public T ToNumber<T>(Script script) where T : struct, IConvertible {
            switch (valueType) {
                case floatValueType:
                case doubleValueType:
                    return (T)Convert.ChangeType(doubleValue, typeof(T));
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType:
                case int64ValueType:
                case uint64ValueType:
                case charValueType:
                    return (T)Convert.ChangeType(longValue, typeof(T));
                default:
                    return (T)Convert.ChangeType(GetObject(script), typeof(T));
            }
        }
        public int ToInt32(Script script) {
            switch (valueType) {
                case floatValueType:
                case doubleValueType:
                    return (int)doubleValue;
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType:
                case int64ValueType:
                case uint64ValueType:
                case charValueType:
                    return (int)longValue;
                default:
                    return Convert.ToInt32(GetObject(script));
            }
        }
        public double ToDouble(Script script) {
            switch (valueType) {
                case floatValueType:
                case doubleValueType:
                    return doubleValue;
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType:
                case int64ValueType:
                case uint64ValueType:
                case charValueType:
                    return longValue;
                default:
                    return Convert.ToDouble(GetObject(script));
            }
        }
        public long ToLong(Script script) {
            switch (valueType) {
                case floatValueType:
                case doubleValueType:
                    return (long)doubleValue;
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType:
                case int64ValueType:
                case uint64ValueType:
                case charValueType:
                    return longValue;
                default:
                    return Convert.ToInt64(GetObject(script));
            }
        }
        public char ToChar(Script script) {
            switch (valueType) {
                case floatValueType:
                case doubleValueType:
                    return (char)doubleValue;
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType:
                case int64ValueType:
                case uint64ValueType:
                case charValueType:
                    return (char)longValue;
                case stringValueType:
                    return GetStringValue(script)[0];
                default:
                    return Convert.ToChar(GetObject(script));
            }
        }
        public T Get<T>(Script script) where T : ScriptObject { 
            return valueType == scriptValueType ? (script.GetObjectValue(index) as T) : null;
        }
        public bool IsNull => valueType == nullValueType;
        public bool IsTrue => valueType == trueValueType;
        public bool IsFalse => valueType == falseValueType;
        public bool IsNumber => valueType == doubleValueType || valueType == int64ValueType;
        public bool IsString => valueType == stringValueType;
        public bool IsScriptObject => valueType == scriptValueType;

        public string ToString(Script script) {
            switch (valueType) {
                case stringValueType: return GetStringValue(script);
                case floatValueType:
                case doubleValueType: 
                    return doubleValue.ToString();
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType:
                case int64ValueType:
                case uint64ValueType:
                    return longValue.ToString();
                case charValueType:
                    return ((char)longValue).ToString();
                case trueValueType: return "true";
                case falseValueType: return "false";
                case nullValueType: return "null";
                case scriptValueType: return GetScriptValue(script).ToString();
                default: return "";
            }
        }
        public int GetHashCode(Script script) {
            switch (valueType) {
                case nullValueType: return 0;
                case trueValueType: return true.GetHashCode();
                case falseValueType: return false.GetHashCode();
                case floatValueType:
                case doubleValueType: 
                    return doubleValue.GetHashCode();
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType:
                case int64ValueType:
                case uint64ValueType:
                case charValueType:
                    return longValue.GetHashCode();
                case stringValueType: return GetStringValue(script).GetHashCode();
                default: return GetScriptValue(script).GetHashCode();
            }
        }
        public bool Equals(ScriptValue value, Script script) {
            if (valueType != value.valueType) { return false; }
            switch (valueType) {
                case floatValueType:
                case doubleValueType:
                    return doubleValue == value.doubleValue;
                case int8ValueType:
                case uint8ValueType:
                case int16ValueType:
                case uint16ValueType:
                case int32ValueType:
                case uint32ValueType:
                case int64ValueType:
                case uint64ValueType:
                case charValueType:
                    return longValue == value.longValue;
                case stringValueType: return index == value.index;
                case scriptValueType: return index == value.index || GetScriptValue(script).Equals(value);
                default: return true;
            }
        }

        public static ScriptValue CreateValue(Script script, object value) {
            if (value == null)
                return Null;
            else if (value is ScriptValue)
                //需要增加一次引用计数
                return new ScriptValue((ScriptValue)value, script);
            else if (value is bool)
                return (bool)value ? True : False;
            else if (value is string)
                return new ScriptValue((string)value, script);
            else if (value is double)
                return new ScriptValue((double)value);
            else if (value is int || value is float || value is byte || value is sbyte || value is short || value is ushort || value is uint)
                return new ScriptValue(Convert.ToDouble(value));
            else if (value is long)
                return new ScriptValue((long)value);
            else if (value is ulong)
                return new ScriptValue((long)(ulong)value);
            else if (value is ScriptObject)
                return new ScriptValue((ScriptObject)value, script);
            else if (value is Type)
                //需要增加一次引用计数
                return script.GetUserdataTypeValue((Type)value).Reference(script);
            else if (value is Delegate)
                return new ScriptValue(script.NewUserdataDelegate().Set((Delegate)value), script);
            else if (value is Enum)
                return new ScriptValue(Convert.ToInt64(value));
            else if (value is IList)
                return new ScriptValue(script.NewUserdataArray().Set(script.GetUserdataType(value.GetType()), (IList)value), script);
            return new ScriptValue(script.NewUserdataObject().Set(script.GetUserdataType(value.GetType()), value), script);
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
                return new ScriptValue((string)value, true, script);
            else if (value is double)
                return new ScriptValue((double)value);
            else if (value is int || value is float || value is byte || value is sbyte || value is short || value is ushort || value is uint)
                return new ScriptValue(Convert.ToDouble(value));
            else if (value is long)
                return new ScriptValue((long)value);
            else if (value is ulong)
                return new ScriptValue((long)(ulong)value);
            else if (value is ScriptObject)
                return new ScriptValue((ScriptObject)value, true, script);
            else if (value is Type)
                return script.GetUserdataTypeValue((Type)value);
            else if (value is Delegate)
                return new ScriptValue(script.NewUserdataDelegate().Set((Delegate)value), true, script);
            else if (value is Enum)
                return new ScriptValue(Convert.ToInt64(value));
            else if (value is IList)
                return new ScriptValue(script.NewUserdataArray().Set(script.GetUserdataType(value.GetType()), (IList)value), true, script);
            return new ScriptValue(script.NewUserdataObject().Set(script.GetUserdataType(value.GetType()), value), true, script);
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
