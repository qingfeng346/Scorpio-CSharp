using Scorpio.Tools;
using Scorpio.Exception;
using System.Collections;
using System.Collections.Generic;
namespace Scorpio {
    public class ScriptType : ScriptObject, IEnumerable<KeyValuePair<string, ScriptValue>> {
        protected Dictionary<string, ScriptValue> m_Values = new Dictionary<string, ScriptValue>();   //所有的函数
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
            ScriptValue value;
            return m_Values.TryGetValue(key, out value) ? value : m_Prototype.GetValue(key);
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            var ret = new ScriptValue(new ScriptInstance(ObjectType.Instance, ThisValue));
            var constructor = GetValue(ScriptOperator.Constructor).Get<ScriptFunction>();
            if (constructor != null) {
                constructor.Call(ret, parameters, length);
            }
            return ret;
        }
        public IEnumerator<KeyValuePair<string, ScriptValue>> GetEnumerator() { return m_Values.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return m_Values.GetEnumerator(); }
        public override string ToString() { return $"Class<{TypeName}>"; }
    }
    //自带类型的原表,不支持动态申请,只能已特定形式申请变量
    public class ScriptBasicType : ScriptType {
        public ScriptBasicType(string typeName, ScriptValue parentType) : base(typeName, parentType) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            throw new ExecutionException($"Class<{TypeName}>不支持自定义构造");
        }
    }
    //Object原表
    public class ScriptTypeObject : ScriptBasicType {
        public ScriptTypeObject(string typeName) : base(typeName, ScriptValue.Null) { }
        public override ScriptValue Prototype { set { throw new ExecutionException("Class<Object>不支持设置 Prototype"); } }
        public override ScriptValue GetValue(string key) {
            ScriptValue value;
            return m_Values.TryGetValue(key, out value) ? value : ScriptValue.Null;
        }
    }
}
