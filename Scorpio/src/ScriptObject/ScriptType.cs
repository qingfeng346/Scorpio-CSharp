using Scorpio.Exception;
using System.Collections;
using System.Collections.Generic;
namespace Scorpio {
    public class ScriptType : ScriptObject, IEnumerable<ScorpioKeyValue<string, ScriptValue>> {
        protected ScorpioStringDictionary<ScriptValue> m_Values;             //所有的函数
        protected ScorpioStringDictionary<ScriptFunction> m_GetProperties;   //所有的get函数
        protected ScriptFunction m_EqualFunction;                       //==函数重载
        protected ScriptType m_Prototype;                               //基类
        public ScriptType(string typeName, ScriptType parentType) : base(ObjectType.Type) {
            TypeName = typeName;
            m_Prototype = parentType;
            m_Values = new ScorpioStringDictionary<ScriptValue>();
            m_GetProperties = new ScorpioStringDictionary<ScriptFunction>();
        }
        public string TypeName { get; private set; }        //Type名称
        public virtual ScriptType Prototype { get => m_Prototype; set => m_Prototype = value; }
        public virtual ScriptFunction EqualFunction => m_EqualFunction ?? m_Prototype.EqualFunction;
        public void AddGetProperty(string key, ScriptFunction value) {
            m_GetProperties[key] = value;
        }
        public virtual ScriptValue GetValue(string key, ScriptInstance instance) {
            if (m_Values.TryGetValue(key, out var value)) {
                return value;
            } else if (m_GetProperties.Count > 0 && m_GetProperties.TryGetValue(key, out var get)) {
                return get.CallNoParameters(new ScriptValue(instance));
            }
            return m_Prototype.GetValue(key, instance);
        }
        public virtual ScriptValue GetValueNoDefault(string key) {
            return m_Values.TryGetValue(key, out var value) ? value : m_Prototype.GetValueNoDefault(key);
        }
        public override void SetValue(string key, ScriptValue value) {
            m_Values[key] = value;
            if (key == ScriptOperator.Equal) {
                m_EqualFunction = value.Get<ScriptFunction>();
            }
        }
        public override ScriptValue GetValue(string key) {
            return m_Values.TryGetValue(key, out var value) ? value : m_Prototype.GetValue(key);
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            var ret = new ScriptValue(new ScriptInstance(ObjectType.Instance, this));
            var constructor = GetValue(ScriptOperator.Constructor).Get<ScriptFunction>();
            if (constructor != null) {
                constructor.Call(ret, parameters, length);
            }
            return ret;
        }
        public IEnumerator<ScorpioKeyValue<string, ScriptValue>> GetEnumerator() => m_Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_Values.GetEnumerator();
        public override string ToString() { return $"Class<{TypeName}>"; }
    }
    //Object原表, GetValue 找不到就返回 null
    internal class ScriptTypeObject : ScriptType {
        private Script m_Script;
        internal ScriptTypeObject(Script script, string typeName) : base(typeName, null) {
            m_Script = script;
        }
        public override ScriptType Prototype { set => throw new ExecutionException("Class<Object>不支持设置 Prototype"); }
        public override ScriptFunction EqualFunction => m_EqualFunction;
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(new ScriptInstance(ObjectType.Type, m_Script.TypeObject));
        }
        public override ScriptValue GetValueNoDefault(string key) {
            return ScriptValue.Null;
        }
        public override ScriptValue GetValue(string key, ScriptInstance instance) {
            if (m_Values.TryGetValue(key, out var value)) {
                return value;
            } else if (m_GetProperties.Count > 0 && m_GetProperties.TryGetValue(key, out var get)) {
                return get.CallNoParameters(new ScriptValue(instance));
            }
            return ScriptValue.Null;
        }
        public override ScriptValue GetValue(string key) {
            return m_Values.TryGetValue(key, out var value) ? value : ScriptValue.Null;
        }
    }
    //自带基础类型的原表,不支持动态申请,只能已特定形式申请变量, number, string, bool, function 等
    internal class ScriptTypePrimitive : ScriptType {
        public ScriptTypePrimitive(string typeName, ScriptType parentType) : base(typeName, parentType) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            throw new ExecutionException($"Class<{TypeName}>不支持构造");
        }
    }
    //Array类型原表
    internal class ScriptTypeBasicArray : ScriptType {
        private Script m_Script;
        internal ScriptTypeBasicArray(Script script, string typeName, ScriptType parentType) : base(typeName, parentType) {
            m_Script = script;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(new ScriptArray(m_Script, parameters, length));
        }
    }
    //Map原表
    internal class ScriptTypeBasicMap : ScriptType {
        private Script m_Script;
        internal ScriptTypeBasicMap(Script script, string typeName, ScriptType parentType) : base(typeName, parentType) {
            m_Script = script;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(new ScriptMapObject(m_Script, parameters, length));
        }
    }
    internal class ScriptTypeBasicHashSet : ScriptType {
        private Script m_Script;
        internal ScriptTypeBasicHashSet(Script script, string typeName, ScriptType parentType) : base(typeName, parentType) {
            m_Script = script;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(new ScriptHashSet(m_Script, parameters, length));
        }
    }
    //StringBuilding原表
    internal class ScriptTypeBasicStringBuilder : ScriptType {
        private Script m_Script;
        internal ScriptTypeBasicStringBuilder(Script script, string typeName, ScriptType parentType) : base(typeName, parentType) {
            m_Script = script;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(new ScriptStringBuilder(m_Script, parameters, length));
        }
    }
}
