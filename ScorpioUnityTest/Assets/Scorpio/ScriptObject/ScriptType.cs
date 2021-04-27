using Scorpio.Exception;
using System;
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
            return m_Values.TryGetValue(key, out var value) ? value : m_Prototype.GetValue(key);
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
    //Object原表, GetValue 找不到就返回 null
    internal class ScriptTypeObject : ScriptType {
        private Script m_Script;
        internal ScriptTypeObject(Script script, string typeName) : base(typeName, ScriptValue.Null) {
            m_Script = script;
        }
        public override ScriptValue Prototype { set { throw new ExecutionException("Class<Object>不支持设置 Prototype"); } }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(new ScriptInstance(ObjectType.Type, m_Script.TypeObjectValue));
        }
        public override ScriptValue GetValue(string key) {
            return m_Values.TryGetValue(key, out var value) ? value : ScriptValue.Null;
        }
    }
    //自带基础类型的原表,不支持动态申请,只能已特定形式申请变量, number, string, bool, function 等
    internal class ScriptTypePrimitive : ScriptType {
        public ScriptTypePrimitive(string typeName, ScriptValue parentType) : base(typeName, parentType) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            throw new ExecutionException($"Class<{TypeName}>不支持构造");
        }
    }
    //Array类型原表
    internal class ScriptTypeBasicArray : ScriptType {
        private Script m_Script;
        internal ScriptTypeBasicArray(Script script, string typeName, ScriptValue parentType) : base(typeName, parentType) {
            m_Script = script;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(new ScriptArray(m_Script, parameters, length));
        }
    }
    //Map原表
    internal class ScriptTypeBasicMap : ScriptType {
        private Script m_Script;
        internal ScriptTypeBasicMap(Script script, string typeName, ScriptValue parentType) : base(typeName, parentType) {
            m_Script = script;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(new ScriptMapObject(m_Script, parameters, length));
        }
    }
    //StringBuilding原表
    internal class ScriptTypeBasicStringBuilder : ScriptType {
        private Script m_Script;
        internal ScriptTypeBasicStringBuilder(Script script, string typeName, ScriptValue parentType) : base(typeName, parentType) {
            m_Script = script;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(new ScriptStringBuilder(m_Script, parameters, length));
        }
    }
}
