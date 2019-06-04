using System.Collections.Generic;
using Scorpio.Tools;
namespace Scorpio {
    public class ScriptType : ScriptObject {
        private ScorpioDictionaryString<ScriptValue> m_Values = new ScorpioDictionaryString<ScriptValue>();   //所有的函数
        public ScriptType(string typeName, ScriptType parentType) : base(ObjectType.Type) {
            TypeName = typeName;
            if (parentType != null)
                m_Values[ScriptValue.Prototype] = new ScriptValue(parentType);
        }
        public string TypeName { get; private set; }        //Type名称
        public override void SetValue(string key, ScriptValue value) {
            if (value.valueType == ScriptValue.nullValueType) {
                m_Values.Remove(key);
            } else {
                m_Values[key] = value;
            }
        }
        public override ScriptValue GetValue(string key) {
            return m_Values.ContainsKey(key) ? m_Values[key] : (m_Values.ContainsKey(ScriptValue.Prototype) ? m_Values[ScriptValue.Prototype].GetValue(key) : ScriptValue.Null);
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            var ret = new ScriptValue(new ScriptInstance(ObjectType.Instance, this));
            var constructor = GetOperator(ScriptOperator.Constructor);
            if (constructor != null) {
                constructor.Call(ret, parameters, length);
            }
            return ret;
        }
        public ScriptFunction GetOperator(string oper) {
            var value = GetValue(oper);
            return value.valueType == ScriptValue.scriptValueType ? value.scriptValue as ScriptFunction : null;
        }
        public override string ToString() { return "Class<" + TypeName + ">"; }
    }
}
