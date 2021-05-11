using System.Collections;
using System.Collections.Generic;
using Scorpio.Tools;
namespace Scorpio {
    public class ScriptInstance : ScriptObject, IEnumerable<KeyValuePair<string, ScriptValue>> {
        internal Dictionary<string, ScriptValue> m_Values = new Dictionary<string, ScriptValue>();         //所有的数据(函数和数据都在一个数组)
        protected ScriptValue m_Prototype = ScriptValue.Null;
        protected ScriptInstance(ObjectType objectType) : base(objectType) { }
        public ScriptInstance(ObjectType objectType, ScriptValue prototype) : base(objectType) {
            m_Prototype = prototype;
        }
        public override string ValueTypeName => $"Object<{m_Prototype}>";            //变量名称
        public ScriptValue Prototype { get { return m_Prototype; } set { m_Prototype = value; } }
        public override ScriptValue GetValue(string key) {
            return m_Values.TryGetValue(key, out var value) ? value : m_Prototype.GetValue(key);
        }
        public override void SetValue(string key, ScriptValue value) {
            m_Values[key] = value;
        }
        public virtual bool HasValue(string key) {
            return m_Values.ContainsKey(key);
        }
        public ScriptArray GetKeys(Script script) {
            var ret = new ScriptArray(script);
            foreach (var pair in m_Values) {
                ret.Add(ScriptValue.CreateValue(pair.Key));
            }
            return ret;
        }
        public IEnumerator<KeyValuePair<string, ScriptValue>> GetEnumerator() { return m_Values.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return m_Values.GetEnumerator(); }
        public override bool Less(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Less).Get<ScriptFunction>();
            if (func != null) {
                ScriptValue.Parameters[0] = obj;
                return func.Call(ThisValue, ScriptValue.Parameters, 1).valueType == ScriptValue.trueValueType;
            }
            return base.Less(obj);
        }
        public override bool LessOrEqual(ScriptValue obj) {
            var func = GetValue(ScriptOperator.LessOrEqual).Get<ScriptFunction>();
            if (func != null) {
                ScriptValue.Parameters[0] = obj;
                return func.Call(ThisValue, ScriptValue.Parameters, 1).valueType == ScriptValue.trueValueType;
            }
            return base.LessOrEqual(obj);
        }
        public override bool Greater(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Greater).Get<ScriptFunction>();
            if (func != null) {
                ScriptValue.Parameters[0] = obj;
                return func.Call(ThisValue, ScriptValue.Parameters, 1).valueType == ScriptValue.trueValueType;
            }
            return base.Greater(obj);
        }
        public override bool GreaterOrEqual(ScriptValue obj) {
            var func = GetValue(ScriptOperator.GreaterOrEqual).Get<ScriptFunction>();
            if (func != null) {
                ScriptValue.Parameters[0] = obj;
                return func.Call(ThisValue, ScriptValue.Parameters, 1).valueType == ScriptValue.trueValueType;
            }
            return base.GreaterOrEqual(obj);
        }
        public override bool Equals(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Equal).Get<ScriptFunction>();
            if (func != null) {
                ScriptValue.Parameters[0] = obj;
                return func.Call(ThisValue, ScriptValue.Parameters, 1).valueType == ScriptValue.trueValueType;
            }
            return base.Equals(obj);
        }

        public override ScriptValue Plus(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Plus).Get<ScriptFunction>();
            if (func != null) {
                ScriptValue.Parameters[0] = obj;
                return func.Call(ThisValue, ScriptValue.Parameters, 1);
            }
            return base.Plus(obj);
        }
        public override ScriptValue Minus(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Minus).Get<ScriptFunction>();
            if (func != null) {
                ScriptValue.Parameters[0] = obj;
                return func.Call(ThisValue, ScriptValue.Parameters, 1);
            }
            return base.Minus(obj);
        }
        public override ScriptValue Multiply(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Multiply).Get<ScriptFunction>();
            if (func != null) {
                ScriptValue.Parameters[0] = obj;
                return func.Call(ThisValue, ScriptValue.Parameters, 1);
            }
            return base.Multiply(obj);
        }
        public override ScriptValue Divide(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Divide).Get<ScriptFunction>();
            if (func != null) {
                ScriptValue.Parameters[0] = obj;
                return func.Call(ThisValue, ScriptValue.Parameters, 1);
            }
            return base.Divide(obj);
        }
        public override ScriptValue Modulo(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Modulo).Get<ScriptFunction>();
            if (func != null) {
                ScriptValue.Parameters[0] = obj;
                return func.Call(ThisValue, ScriptValue.Parameters, 1);
            }
            return base.Modulo(obj);
        }
        public override ScriptValue InclusiveOr(ScriptValue obj) {
            var func = GetValue(ScriptOperator.InclusiveOr).Get<ScriptFunction>();
            if (func != null) {
                ScriptValue.Parameters[0] = obj;
                return func.Call(ThisValue, ScriptValue.Parameters, 1);
            }
            return base.InclusiveOr(obj);
        }
        public override ScriptValue Combine(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Combine).Get<ScriptFunction>();
            if (func != null) {
                ScriptValue.Parameters[0] = obj;
                return func.Call(ThisValue, ScriptValue.Parameters, 1);
            }
            return base.Combine(obj);
        }
        public override ScriptValue XOR(ScriptValue obj) {
            var func = GetValue(ScriptOperator.XOR).Get<ScriptFunction>();
            if (func != null) {
                ScriptValue.Parameters[0] = obj;
                return func.Call(ThisValue, ScriptValue.Parameters, 1);
            }
            return base.XOR(obj);
        }
        public override ScriptValue Shi(ScriptValue obj) {
            var func = m_Prototype.GetValue(ScriptOperator.Shi).Get<ScriptFunction>();
            if (func != null) {
                ScriptValue.Parameters[0] = obj;
                return func.Call(ThisValue, ScriptValue.Parameters, 1);
            }
            return base.Shi(obj);
        }
        public override ScriptValue Shr(ScriptValue obj) {
            var func = GetValue(ScriptOperator.Shr).Get<ScriptFunction>();
            if (func != null) {
                ScriptValue.Parameters[0] = obj;
                return func.Call(ThisValue, ScriptValue.Parameters, 1);
            }
            return base.Shr(obj);
        }
        public ScriptValue Call(ScriptValue parameter1) {
            ScriptValue.Parameters[0] = parameter1;
            return Call(ScriptValue.Null, ScriptValue.Parameters, 1);
        }
        public ScriptValue Call(ScriptValue parameter1, ScriptValue parameter2) {
            ScriptValue.Parameters[0] = parameter1;
            ScriptValue.Parameters[1] = parameter2;
            return Call(ScriptValue.Null, ScriptValue.Parameters, 2);
        }
        public ScriptValue Call(ScriptValue parameter1, ScriptValue parameter2, ScriptValue parameter3) {
            ScriptValue.Parameters[0] = parameter1;
            ScriptValue.Parameters[1] = parameter2;
            ScriptValue.Parameters[2] = parameter3;
            return Call(ScriptValue.Null, ScriptValue.Parameters, 3);
        }
        public ScriptValue Call(ScriptValue parameter1, ScriptValue parameter2, ScriptValue parameter3, ScriptValue parameter4) {
            ScriptValue.Parameters[0] = parameter1;
            ScriptValue.Parameters[1] = parameter2;
            ScriptValue.Parameters[2] = parameter3;
            ScriptValue.Parameters[3] = parameter4;
            return Call(ScriptValue.Null, ScriptValue.Parameters, 4);
        }
        public ScriptValue Call(ScriptValue[] parameters, int length) {
            return Call(ScriptValue.Null, parameters, length);
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            var func = GetValue(ScriptOperator.Invoke).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(ThisValue, parameters, length);
            }
            return base.Call(thisObject, parameters, length);
        }
        public override string ToString() { return $"Object<{m_Prototype}>"; }
        internal virtual void ToJson(JsonSerializer jsonSerializer) {
            var builder = jsonSerializer.m_Builder;
            builder.Append("{");
            var first = true;
            foreach (var pair in m_Values) {
                if (first) { first = false; } else { builder.Append(","); }
                jsonSerializer.Serializer(pair.Key);
                builder.Append(":");
                jsonSerializer.Serializer(pair.Value);
            }
            builder.Append("}");
        }
    }
}
