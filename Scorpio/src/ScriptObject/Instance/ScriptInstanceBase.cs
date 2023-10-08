using Scorpio.Exception;
using Scorpio.Library;
using Scorpio.Tools;

namespace Scorpio {
    public abstract class ScriptInstanceBase : ScriptObject {
        protected ScriptType m_Prototype;
        public ScriptInstanceBase(ScriptType prototype)
#if SCORPIO_DEBUG
            : base()
#endif
            {
            m_Prototype = prototype;
        }
        public Script script => m_Prototype.script;
        public override string ValueTypeName => $"Object<{m_Prototype}>";            //变量名称
        public ScriptType Prototype { get { return m_Prototype; } set { m_Prototype = value ?? script.TypeObject; } }
        public virtual bool HasValue(string key) { return false; }
        public virtual void DelValue(string key) { }
        public virtual void ClearVariables() { }
        #region GetValue SetValue重载
        public override ScriptValue GetValue(string key) {
            return m_Prototype.GetValue(key, this);
        }
        #endregion
        #region 运算符重载
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
        bool CallCompare(string operatorIndex, ScriptValue obj) {
            var func = m_Prototype.GetValue(operatorIndex).Get<ScriptFunction>();
            if (func == null) throw new ExecutionException($"类型[{ValueTypeName}]不支持 [{operatorIndex}] 运算");
            ScorpioUtil.Parameters[0] = obj;
            return func.Call(ThisValue, ScorpioUtil.Parameters, 1).valueType == ScriptValue.trueValueType;
        }
        ScriptValue CallOperator(string operatorIndex, ScriptValue obj) {
            var func = m_Prototype.GetValue(operatorIndex).Get<ScriptFunction>();
            if (func == null) throw new ExecutionException($"类型[{ValueTypeName}]不支持 [{operatorIndex}] 运算");
            ScorpioUtil.Parameters[0] = obj;
            return func.Call(ThisValue, ScorpioUtil.Parameters, 1);
        }
        public override int GetHashCode() {
            var func = m_Prototype.GetValueNoDefault(ScriptOperator.getHashCode).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(ThisValue, ScorpioUtil.VALUE_EMPTY, 0).ToInt32();
            }
            return base.GetHashCode();
        }
        public override bool Equals(ScriptValue obj) {
            if (m_Prototype.EqualFunction != null) {
                ScorpioUtil.Parameters[0] = obj;
                return m_Prototype.EqualFunction.Call(ThisValue, ScorpioUtil.Parameters, 1).valueType == ScriptValue.trueValueType;
            }
            return base.Equals(obj);
        }
        #endregion
        #region Call
        public ScriptValue Call(ScriptValue parameter1) {
            ScorpioUtil.Parameters[0] = parameter1;
            return Call(ScriptValue.Null, ScorpioUtil.Parameters, 1);
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
            if (func != null) {
                return func.Call(ThisValue, parameters, length);
            }
            return base.Call(thisObject, parameters, length);
        }
        #endregion
        public override string ToString() {
            var func = m_Prototype.GetValueNoDefault(ScriptOperator.toString).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(ThisValue, ScorpioUtil.VALUE_EMPTY, 0).ToString();
            }
            return $"Object<{m_Prototype}>";
        }
        internal abstract void SerializerJson(ScorpioJsonSerializer jsonSerializer);
    }
}
