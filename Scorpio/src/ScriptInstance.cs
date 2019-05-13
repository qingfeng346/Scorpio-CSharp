using System.Collections.Generic;
using Scorpio.Commons;
namespace Scorpio {
    public class ScriptInstance : ScriptObject {
        protected ScorpioDictionaryString<ScriptValue> m_Values = new ScorpioDictionaryString<ScriptValue>();         //所有的数据(函数和数据都在一个数组)
        public ScriptInstance(Script script, ObjectType objectType, ScriptType type) : base(script, objectType) {
            m_Values[ScriptValue.Prototype] = new ScriptValue(type);
        }
        public override string ValueTypeName { get { return ToString(); } }            //变量名称
        public ScriptType Class { get { return m_Values[ScriptValue.Prototype].scriptValue as ScriptType; } }
        public override ScriptValue GetValue(string key) {
            return m_Values.ContainsKey(key) ? m_Values[key] : (m_Values.ContainsKey(ScriptValue.Prototype) ? m_Values[ScriptValue.Prototype].GetValue(key, m_Script) : ScriptValue.Null);
        }
        public override void SetValue(string key, ScriptValue value) {
            if (value.valueType == ScriptValue.nullValueType) {
                m_Values.Remove(key);
            } else {
                m_Values[key] = value;
            }
        }
        public bool HasValue(string key) {
            return m_Values.ContainsKey(key);
        }
        public override bool Less(ScriptValue obj) {
            var type = Class;
            var func = type != null ? Class.GetOperator(ScriptOperator.Less) : null;
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1).valueType == ScriptValue.trueValueType;
            }
            return base.Less(obj);
        }
        public override bool LessOrEqual(ScriptValue obj) {
            var type = Class;
            var func = type != null ? Class.GetOperator(ScriptOperator.LessOrEqual) : null;
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1).valueType == ScriptValue.trueValueType;
            }
            return base.LessOrEqual(obj);
        }
        public override bool Greater(ScriptValue obj) {
            var type = Class;
            var func = type != null ? Class.GetOperator(ScriptOperator.Greater) : null;
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1).valueType == ScriptValue.trueValueType;
            }
            return base.Greater(obj);
        }
        public override bool GreaterOrEqual(ScriptValue obj) {
            var type = Class;
            var func = type != null ? Class.GetOperator(ScriptOperator.GreaterOrEqual) : null;
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1).valueType == ScriptValue.trueValueType;
            }
            return base.GreaterOrEqual(obj);
        }
        public override bool Equals(ScriptValue obj) {
            var type = Class;
            var func = type != null ? Class.GetOperator(ScriptOperator.Equal) : null;
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1).valueType == ScriptValue.trueValueType;
            }
            return base.Equals(obj);
        }

        public override ScriptValue Plus(ScriptValue obj) {
            var type = Class;
            var func = type != null ? Class.GetOperator(ScriptOperator.Plus) : null;
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.Plus(obj);
        }
        public override ScriptValue Minus(ScriptValue obj) {
            var type = Class;
            var func = type != null ? Class.GetOperator(ScriptOperator.Minus) : null;
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.Minus(obj);
        }
        public override ScriptValue Multiply(ScriptValue obj) {
            var type = Class;
            var func = type != null ? Class.GetOperator(ScriptOperator.Multiply) : null;
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.Multiply(obj);
        }
        public override ScriptValue Divide(ScriptValue obj) {
            var type = Class;
            var func = type != null ? Class.GetOperator(ScriptOperator.Divide) : null;
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.Divide(obj);
        }
        public override ScriptValue Modulo(ScriptValue obj) {
            var type = Class;
            var func = type != null ? Class.GetOperator(ScriptOperator.Modulo) : null;
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.Modulo(obj);
        }
        public override ScriptValue InclusiveOr(ScriptValue obj) {
            var type = Class;
            var func = type != null ? Class.GetOperator(ScriptOperator.InclusiveOr) : null;
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.InclusiveOr(obj);
        }
        public override ScriptValue Combine(ScriptValue obj) {
            var type = Class;
            var func = type != null ? Class.GetOperator(ScriptOperator.Combine) : null;
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.Combine(obj);
        }
        public override ScriptValue XOR(ScriptValue obj) {
            var type = Class;
            var func = type != null ? Class.GetOperator(ScriptOperator.XOR) : null;
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.XOR(obj);
        }
        public override ScriptValue Shi(ScriptValue obj) {
            var type = Class;
            var func = type != null ? Class.GetOperator(ScriptOperator.Shi) : null;
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.Shi(obj);
        }
        public override ScriptValue Shr(ScriptValue obj) {
            var type = Class;
            var func = type != null ? Class.GetOperator(ScriptOperator.Shr) : null;
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.Shr(obj);
        }

        public ScriptValue Call(ScriptValue[] parameters, int length) {
            return Call(ScriptValue.Null, parameters, length);
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            var type = Class;
            var func = type != null ? Class.GetOperator(ScriptOperator.Invoke) : null;
            if (func != null) {
                return func.Call(new ScriptValue(this), parameters, length);
            }
            return base.Call(thisObject, parameters, length);
        }
        public override string ToString() { return Class != null ? $"Object<{Class.TypeName}>" : "Object<null>"; }
    }
}
