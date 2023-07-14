using System.Collections;
using System.Collections.Generic;
using Scorpio.Exception;
using Scorpio.Library;
using Scorpio.Tools;

namespace Scorpio {
    public class ScriptInstance : ScriptObject, IEnumerable<KeyValuePair<string, ScriptValue>> {
        protected ScriptType m_Prototype = null;
        protected ScorpioStringDictionary m_Values;              //所有的数据(函数和数据都在一个数组)
        public ScriptInstance(Script script) : base(script, ObjectType.Instance) {
            m_Values = new ScorpioStringDictionary();
        }
        public ScriptInstance(Script script, ObjectType objectType) : base(script, objectType) {
            m_Values = new ScorpioStringDictionary();
        }
        public ScriptInstance SetPrototype(ScriptType protoType) {
            m_Prototype = protoType;
            ScriptObjectReference.Reference(m_Prototype.Index);
            return this;
        }
        public override string ValueTypeName => $"Object<{m_Prototype}>";            //变量名称
        public ScriptType Prototype => m_Prototype;
        //ThisValue没有占用引用计数
        public ScriptValue ThisValue {
            get {
                ScorpioUtil.CommonThisValue.index = Index;
                return ScorpioUtil.CommonThisValue;
            }
        }
        public void SetCapacity(int capacity) {
            m_Values.SetCapacity(capacity);
        }
        protected void Release() {
            ScriptObjectReference.Free(m_Prototype.Index);
            m_Prototype = null;
            m_Values.Clear();
        }
        public override void Free() {
            DelRecord();
            Release();
            m_Script.Free(this);
        }
        public override void gc() {
            Release();
        }

        #region 重载 GetValue SetValue
        public override ScriptValue GetValue(string key) {
            return m_Values.TryGetValue(key, out var value) ? value : m_Prototype.GetValue(key, this);
        }
        public override void SetValue(string key, ScriptValue value) {
            m_Values[key] = value;
        }
        #endregion
        public void SetValueNoReference(string key, ScriptObject scriptObject) {
            SetValueNoReference(key, new ScriptValue(scriptObject));
        }
        public virtual void SetValueNoReference(string key, ScriptValue value) {
            m_Values.SetValue(key, value);
        }
        public virtual void SetValueNoReference(object key, ScriptValue value) {
            throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 Object : {key}");
        }
        public virtual bool HasValue(string key) {
            return m_Values.ContainsKey(key);
        }
        public virtual void DelValue(string key) {
            m_Values.Remove(key);
        }
        public virtual void ClearVariables() {
            m_Values.Clear();
        }
        public IEnumerator<KeyValuePair<string, ScriptValue>> GetEnumerator() => m_Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_Values.GetEnumerator();
        public override ScriptValue Plus(ScriptValue obj) {
            return CallOperator(ScriptOperator.Plus, obj);
        }
        public override ScriptValue Minus(ScriptValue obj) {
            return CallOperator(ScriptOperator.Minus, obj);
        }
        public override ScriptValue Multiply(ScriptValue obj) {
            return CallOperator(ScriptOperator.Multiply, obj);
        }
        public override ScriptValue Divide(ScriptValue obj) {
            return CallOperator(ScriptOperator.Divide, obj);
        }
        public override ScriptValue Modulo(ScriptValue obj) {
            return CallOperator(ScriptOperator.Modulo, obj);
        }
        public override ScriptValue InclusiveOr(ScriptValue obj) {
            return CallOperator(ScriptOperator.InclusiveOr, obj);
        }
        public override ScriptValue Combine(ScriptValue obj) {
            return CallOperator(ScriptOperator.Combine, obj);
        }
        public override ScriptValue XOR(ScriptValue obj) {
            return CallOperator(ScriptOperator.XOR, obj);
        }
        public override ScriptValue Shi(ScriptValue obj) {
            return CallOperator(ScriptOperator.Shi, obj);
        }
        public override ScriptValue Shr(ScriptValue obj) {
            return CallOperator(ScriptOperator.Shr, obj);
        }
        public override bool Greater(ScriptValue obj) {
            return CallCompare(ScriptOperator.Greater, obj);
        }
        public override bool GreaterOrEqual(ScriptValue obj) {
            return CallCompare(ScriptOperator.GreaterOrEqual, obj);
        }
        public override bool Less(ScriptValue obj) {
            return CallCompare(ScriptOperator.Less, obj);
        }
        public override bool LessOrEqual(ScriptValue obj) {
            return CallCompare(ScriptOperator.LessOrEqual, obj);
        }
        public override bool Equals(ScriptValue obj) {
            if (m_Prototype.EqualFunction != null) {
                var parameters = ScorpioUtil.Parameters;
                parameters[0] = obj;
                return m_Prototype.EqualFunction.Call(ThisValue, parameters, 1).valueType == ScriptValue.trueValueType;
            }
            return base.Equals(obj);
        }
        bool CallCompare(string operatorIndex, ScriptValue obj) {
            var func = m_Prototype.GetValue(operatorIndex).Get<ScriptFunction>();
            if (func == null) throw new ExecutionException($"类型[{ValueTypeName}]不支持 [{operatorIndex}] 运算");
            var parameters = ScorpioUtil.Parameters;
            parameters[0] = obj;
            return func.Call(ThisValue, parameters, 1).valueType == ScriptValue.trueValueType;
        }
        ScriptValue CallOperator(string operatorIndex, ScriptValue obj) {
            var func = m_Prototype.GetValue(operatorIndex).Get<ScriptFunction>();
            if (func == null) throw new ExecutionException($"类型[{ValueTypeName}]不支持 [{operatorIndex}] 运算");
            var parameters = ScorpioUtil.Parameters;
            parameters[0] = obj;
            return func.Call(ThisValue, parameters, 1);
        }
        public ScriptValue Call(ScriptValue parameter1) {
            var parameters = ScorpioUtil.Parameters;
            parameters[0] = parameter1;
            return Call(ScriptValue.Null, parameters, 1);
        }
        public ScriptValue Call(ScriptValue parameter1, ScriptValue parameter2) {
            var parameters = ScorpioUtil.Parameters;
            parameters[0] = parameter1;
            parameters[1] = parameter2;
            return Call(ScriptValue.Null, parameters, 2);
        }
        public ScriptValue Call(ScriptValue parameter1, ScriptValue parameter2, ScriptValue parameter3) {
            var parameters = ScorpioUtil.Parameters;
            parameters[0] = parameter1;
            parameters[1] = parameter2;
            parameters[2] = parameter3;
            return Call(ScriptValue.Null, parameters, 3);
        }
        public ScriptValue Call(ScriptValue parameter1, ScriptValue parameter2, ScriptValue parameter3, ScriptValue parameter4) {
            var parameters = ScorpioUtil.Parameters;
            parameters[0] = parameter1;
            parameters[1] = parameter2;
            parameters[2] = parameter3;
            parameters[3] = parameter4;
            return Call(ScriptValue.Null, parameters, 4);
        }
        public ScriptValue Call(ScriptValue[] parameters, int length) {
            return Call(ScriptValue.Null, parameters, length);
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            var func = m_Prototype.GetValue(ScriptOperator.Invoke).Get<ScriptFunction>();
            if (func == null) throw new ExecutionException($"类型[{ValueTypeName}]不支持函数调用");
            return func.Call(ThisValue, parameters, length);
        }
        public override string ToString() {
            var func = m_Prototype.GetValueNoDefault(ScriptOperator.toString).Get<ScriptFunction>();
            if (func == null) return $"Object<{m_Prototype}>";
            var value = func.Call(ThisValue, ScorpioUtil.VALUE_EMPTY, 0);
            value.Release();
            return value.ToString();
        }
        internal virtual void SerializerString(ScorpioStringSerializer jsonSerializer) {
            jsonSerializer.m_Builder.Append($"\"{ToString()}\"");
        }
        internal virtual void SerializerJson(ScorpioJsonSerializer jsonSerializer) {
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
