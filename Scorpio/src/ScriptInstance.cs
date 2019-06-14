using System.Collections.Generic;
using Scorpio.Tools;
using System.Text;
namespace Scorpio {
    public class ScriptInstance : ScriptObject {
        protected ScorpioDictionaryString<ScriptValue> m_Values = new ScorpioDictionaryString<ScriptValue>();         //所有的数据(函数和数据都在一个数组)
        protected ScriptValue m_Prototype = ScriptValue.Null;
        public ScriptInstance(ObjectType objectType, ScriptValue type) : base(objectType) {
            m_Prototype = type;
        }
        public override string ValueTypeName { get { return ToString(); } }            //变量名称
        public ScriptValue Prototype { get { return m_Prototype; } set { m_Prototype = value; } }
        public override ScriptValue GetValue(string key) {
            ScriptValue value;
            return m_Values.TryGetValue(key, out value) ? value : m_Prototype.GetValue(key);
        }
        public override void SetValue(string key, ScriptValue value) {
            m_Values[key] = value;
        }
        public virtual bool HasValue(string key) {
            return m_Values.ContainsKey(key);
        }
        public override bool Less(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Less).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1).valueType == ScriptValue.trueValueType;
            }
            return base.Less(obj);
        }
        public override bool LessOrEqual(ScriptValue obj) {
            var func = GetValue(ScriptOperator.LessOrEqual).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1).valueType == ScriptValue.trueValueType;
            }
            return base.LessOrEqual(obj);
        }
        public override bool Greater(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Greater).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1).valueType == ScriptValue.trueValueType;
            }
            return base.Greater(obj);
        }
        public override bool GreaterOrEqual(ScriptValue obj) {
            var func = GetValue(ScriptOperator.GreaterOrEqual).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1).valueType == ScriptValue.trueValueType;
            }
            return base.GreaterOrEqual(obj);
        }
        public override bool Equals(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Equal).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1).valueType == ScriptValue.trueValueType;
            }
            return base.Equals(obj);
        }

        public override ScriptValue Plus(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Plus).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.Plus(obj);
        }
        public override ScriptValue Minus(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Minus).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.Minus(obj);
        }
        public override ScriptValue Multiply(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Multiply).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.Multiply(obj);
        }
        public override ScriptValue Divide(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Divide).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.Divide(obj);
        }
        public override ScriptValue Modulo(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Modulo).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.Modulo(obj);
        }
        public override ScriptValue InclusiveOr(ScriptValue obj) {
            var func = GetValue(ScriptOperator.InclusiveOr).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.InclusiveOr(obj);
        }
        public override ScriptValue Combine(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Combine).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.Combine(obj);
        }
        public override ScriptValue XOR(ScriptValue obj) {
            var func = GetValue(ScriptOperator.XOR).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.XOR(obj);
        }
        public override ScriptValue Shi(ScriptValue obj) {
            var func = m_Prototype.GetValue(ScriptOperator.Shi).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.Shi(obj);
        }
        public override ScriptValue Shr(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Shr).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(new ScriptValue(this), new ScriptValue[] { obj }, 1);
            }
            return base.Shr(obj);
        }
        public ScriptValue Call(ScriptValue parameter1) {
            return Call(ScriptValue.Null, new ScriptValue[] { parameter1 }, 1);
        }
        public ScriptValue Call(ScriptValue parameter1, ScriptValue parameter2) {
            return Call(ScriptValue.Null, new ScriptValue[] { parameter1, parameter2 }, 2);
        }
        public ScriptValue Call(ScriptValue parameter1, ScriptValue parameter2, ScriptValue parameter3) {
            return Call(ScriptValue.Null, new ScriptValue[] { parameter1, parameter2, parameter3 }, 3);
        }
        public ScriptValue Call(ScriptValue parameter1, ScriptValue parameter2, ScriptValue parameter3, ScriptValue parameter4) {
            return Call(ScriptValue.Null, new ScriptValue[] { parameter1, parameter2, parameter3, parameter4 }, 4);
        }
        public ScriptValue Call(ScriptValue[] parameters, int length) {
            return Call(ScriptValue.Null, parameters, length);
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            var func = GetValue(ScriptOperator.Invoke).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(new ScriptValue(this), parameters, length);
            }
            return base.Call(thisObject, parameters, length);
        }
        public override string ToString() { return Prototype != null ? $"Object<{m_Prototype.ToString()}>" : "Object<null>"; }
        public override string ToJson(bool supportKeyNumber) {
            var builder = new StringBuilder();
            builder.Append("{");
            var first = true;
            foreach (var pair in m_Values) {
                var value = pair.value;
                if (value.valueType == ScriptValue.scriptValueType && (value.scriptValue is ScriptFunction || value.scriptValue == this)) { continue; }
                if (first) { first = false; } else { builder.Append(","); }
                builder.Append($"\"{pair.key}\":{value.ToJson(supportKeyNumber)}");
            }
            builder.Append("}");
            return builder.ToString();
        }
    }
}
