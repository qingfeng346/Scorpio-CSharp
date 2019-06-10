using Scorpio.Tools;
using Scorpio.Exception;
namespace Scorpio {
    public class ScriptType : ScriptObject {
        protected ScorpioDictionaryString<ScriptValue> m_Values = new ScorpioDictionaryString<ScriptValue>();   //所有的函数
        protected ScriptValue m_Prototype = ScriptValue.Null;
        public ScriptType(string typeName, ScriptValue parentType) : base(ObjectType.Type) {
            TypeName = typeName;
            m_Prototype = parentType;
        }
        public string TypeName { get; private set; }        //Type名称
        public virtual ScriptValue Prototype { get { return m_Prototype; } set { m_Prototype = value; } }
        public override void SetValue(string key, ScriptValue value) {
            if (value.valueType == ScriptValue.nullValueType) {
                m_Values.Remove(key);
            } else {
                m_Values[key] = value;
            }
        }
        public override ScriptValue GetValue(string key) {
            return m_Values.ContainsKey(key) ? m_Values[key] : m_Prototype.GetValue(key);
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            var ret = new ScriptValue(new ScriptInstance(ObjectType.Instance, new ScriptValue(this)));
            var constructor = GetValue(ScriptOperator.Constructor).Get<ScriptFunction>();
            if (constructor != null) {
                constructor.Call(ret, parameters, length);
            }
            return ret;
        }
        public override string ToString() { return "Class<" + TypeName + ">"; }
    }
    public class ScriptTypeObject : ScriptType {
        public ScriptTypeObject(string typeName) : base(typeName, ScriptValue.Null) { }
        public override ScriptValue Prototype { set { throw new ExecutionException("Class<Object>不支持设置 Prototype"); } }
        public override ScriptValue GetValue(string key) {
            return m_Values.ContainsKey(key) ? m_Values[key] : ScriptValue.Null;
        }
    }
}
