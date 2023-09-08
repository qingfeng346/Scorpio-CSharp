using Scorpio.Exception;
using Scorpio.Tools;
using System.Collections;
using System.Collections.Generic;
namespace Scorpio {
    public class ScriptType : ScriptObject, IEnumerable<KeyValuePair<string, ScriptValue>> {
        internal ScorpioStringDictionary<ScriptValue> m_Values;             //所有的函数
        internal ScorpioStringDictionary<ScriptFunction> m_GetProperties;   //所有的get函数
        protected ScriptFunction m_EqualFunction;                           //==函数重载
        protected ScriptType m_Prototype;                                   //基类
        protected Script m_Script;
        public ScriptType(string typeName, ScriptType parentType, Script script)
#if SCORPIO_DEBUG
            : base()
#endif
            {
            m_Script = script;
            TypeName = typeName;
            m_Prototype = parentType;
            m_Values = new ScorpioStringDictionary<ScriptValue>();
            m_GetProperties = new ScorpioStringDictionary<ScriptFunction>();
        }
        public Script script => m_Script;
        public string TypeName { get; private set; }        //Type名称
        public virtual ScriptType Prototype { get { return m_Prototype; } set { m_Prototype = value ?? m_Script.TypeObject; } }
        public virtual ScriptFunction EqualFunction => m_EqualFunction ?? m_Prototype.EqualFunction;
        internal void SetFunctions(Script script, (string, ScorpioHandle)[] functions) {
            SetFunctionCapacity(functions.Length);
            foreach (var (name, func) in functions) {
                SetValue(name, script.CreateFunction(func));
            }
        }
        public void SetFunctionCapacity(int capacity) {
            m_Values.SetCapacity(capacity);
        }
        public void SetGetPropertyCapacity(int capacity) {
            m_GetProperties.SetCapacity(capacity);
        }
        public void AddGetProperty(string key, ScriptFunction value) {
            m_GetProperties[key] = value;
        }
        public void RemoveGetProperty(string key) {
            m_GetProperties.Remove(key);
        }
        public virtual ScriptValue GetValue(string key, ScriptInstanceBase instance) {
            if (m_Values.TryGetValue(key, out var value)) {
                return value;
            } else if (m_GetProperties.TryGetValue(key, out var get)) {
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
            var ret = new ScriptValue(new ScriptInstance(this));
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
        internal ScriptTypeObject(Script script, string typeName) : base(typeName, null, script) { }
        public override ScriptType Prototype { set { throw new ExecutionException("Class<Object>不支持设置 Prototype"); } }
        public override ScriptFunction EqualFunction => m_EqualFunction;
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(new ScriptInstance(m_Script.TypeObject));
        }
        public override ScriptValue GetValueNoDefault(string key) {
            return default;
        }
        public override ScriptValue GetValue(string key, ScriptInstanceBase instance) {
            if (m_Values.TryGetValue(key, out var value)) {
                return value;
            } else if (m_GetProperties.TryGetValue(key, out var get)) {
                return get.CallNoParameters(new ScriptValue(instance));
            }
            return default;
        }
        public override ScriptValue GetValue(string key) {
            return m_Values.TryGetValue(key, out var value) ? value : default;
        }
    }
    //自带基础类型的原表,不支持动态申请,只能已特定形式申请变量, number, string, bool, function 等
    internal class ScriptTypePrimitive : ScriptType {
        public ScriptTypePrimitive(string typeName, ScriptType parentType, Script script) : base(typeName, parentType, script) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            throw new ExecutionException($"Class<{TypeName}>不支持构造");
        }
    }
    //Array类型原表
    internal class ScriptTypeBasicArray : ScriptType {
        internal ScriptTypeBasicArray(Script script, string typeName, ScriptType parentType) : base(typeName, parentType, script) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(new ScriptArray(m_Script, parameters, length));
        }
    }
    //Map原表
    internal class ScriptTypeBasicMap : ScriptType {
        internal ScriptTypeBasicMap(Script script, string typeName, ScriptType parentType) : base(typeName, parentType, script) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(new ScriptMapObject(m_Script, parameters, length));
        }
    }
    //HashSet原表
    internal class ScriptTypeBasicHashSet : ScriptType {
        internal ScriptTypeBasicHashSet(Script script, string typeName, ScriptType parentType) : base(typeName, parentType, script) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(new ScriptHashSet(m_Script, parameters, length));
        }
    }
    //StringBuilding原表
    internal class ScriptTypeBasicStringBuilder : ScriptType {
        internal ScriptTypeBasicStringBuilder(Script script, string typeName, ScriptType parentType) : base(typeName, parentType, script) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(new ScriptStringBuilder(m_Script, parameters, length));
        }
    }
}
