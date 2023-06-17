using Scorpio.Exception;
using Scorpio.Tools;
using System.Collections;
using System.Collections.Generic;
namespace Scorpio {
    public class ScriptType : ScriptObject, IEnumerable<ScorpioKeyValue<string, ScriptValue>> {
        protected ScorpioStringDictionary<ScriptValue> m_Values;             //所有的函数
        protected ScorpioStringDictionary<ScriptValue> m_GetProperties;      //所有的get函数
        protected ScriptFunction m_EqualFunction;                       //==函数重载
        protected ScriptValue m_PrototypeValue;                         //基类
        protected ScriptType m_Prototype;                               //基类
        public ScriptType(Script script) : base(script, ObjectType.Type) {
            m_Values = new ScorpioStringDictionary<ScriptValue>();
            m_GetProperties = new ScorpioStringDictionary<ScriptValue>();
        }
        public void Set(string typeName, ScriptValue parentValue) {
            TypeName = typeName;
            m_PrototypeValue.CopyFrom(parentValue);
            m_Prototype = parentValue.Get<ScriptType>();
        }
        public void SetFunctionCapacity(int capacity) {
            m_Values.SetCapacity(capacity);
        }
        public void SetGetPropertyCapacity(int capacity) {
            m_GetProperties.SetCapacity(capacity);
        }
        public string TypeName { get; private set; }        //Type名称
        public virtual ScriptType Prototype => m_Prototype;
        public ScriptValue PrototypeValue => m_PrototypeValue;
        public virtual ScriptFunction EqualFunction => m_EqualFunction ?? m_Prototype?.EqualFunction;
        protected void Release() {
            m_PrototypeValue.Free();
            m_Prototype = null;
            m_Values.Free();
            m_GetProperties.Free();
        }
        public override void Free() {
            Release();
            m_Script.Free(this);
        }
        public override void gc() {
            Release();
        }
        public void AddGetProperty(string key, ScriptFunction function) {
            if (m_GetProperties.TryGetValue(key, out var result)) {
                result.Free();
            }
            m_GetProperties[key] = new ScriptValue(function);
        }
        public virtual ScriptValue GetValue(string key, ScriptInstance instance) {
            if (m_Values.TryGetValue(key, out var value)) {
                return value;
            } else if (m_GetProperties.Count > 0 && m_GetProperties.TryGetValue(key, out var get)) {
                var ret = get.Get<ScriptFunction>().Call(new ScriptValue(instance, true), ScriptValue.EMPTY, 0);
                ret.Release();
                return ret;
            }
            return m_Prototype.GetValue(key, instance);
        }
        public virtual ScriptValue GetValueNoDefault(string key) {
            return m_Values.TryGetValue(key, out var value) ? value : m_Prototype.GetValueNoDefault(key);
        }
        public void SetValue(string key, ScriptObject scriptObject) {
            SetValueNoReference(key, new ScriptValue(scriptObject));
        }
        #region GetValue SetValue 重载
        public override void SetValue(string key, ScriptValue value) {
            if (m_Values.TryGetValue(key, out var result)) {
                result.Free();
            }
            m_Values[key] = value.Reference();
            if (key == ScriptOperator.Equal) {
                m_EqualFunction = value.Get<ScriptFunction>();
            }
        }
        public override ScriptValue GetValue(string key) {
            return m_Values.TryGetValue(key, out var value) ? value : m_Prototype.GetValue(key);
        }
        #endregion
        public void SetValueNoReference(string key, ScriptValue value) {
            if (m_Values.TryGetValue(key, out var result)) {
                result.Free();
            }
            m_Values[key] = value;
            if (key == ScriptOperator.Equal) {
                m_EqualFunction = value.Get<ScriptFunction>();
            }
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            var instance = m_Script.NewInstance();
            instance.SetPrototypeValue(new ScriptValue(this, true));
            var ret = new ScriptValue(instance);
            var constructor = GetValue(ScriptOperator.Constructor).Get<ScriptFunction>();
            if (constructor != null) {
                constructor.Call(ret, parameters, length);
            }
            return ret;
        }
        public IEnumerator<ScorpioKeyValue<string, ScriptValue>> GetEnumerator() { return m_Values.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return m_Values.GetEnumerator(); }
        public override string ToString() { return $"Class<{TypeName}>"; }
    }
    //Object原表, GetValue 找不到就返回 null
    internal class ScriptTypeObject : ScriptType {
        internal ScriptTypeObject(Script script, string typeName) : base(script) {
            Set(typeName, ScriptValue.Null);
        }
        public override void Free() {
            Release();
        }
        public override ScriptFunction EqualFunction => m_EqualFunction;
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(m_Script.NewInstance().SetPrototypeValue(script.TypeObjectValue));
        }
        public override ScriptValue GetValueNoDefault(string key) {
            return ScriptValue.Null;
        }
        public override ScriptValue GetValue(string key, ScriptInstance instance) {
            if (m_Values.TryGetValue(key, out var value)) {
                return value;
            } else if (m_GetProperties.Count > 0 && m_GetProperties.TryGetValue(key, out var get)) {
                var ret = get.Get<ScriptFunction>().Call(new ScriptValue(instance, true), ScriptValue.EMPTY, 0);
                ret.Release();
                return ret;
            }
            return ScriptValue.Null;
        }
        public override ScriptValue GetValue(string key) {
            return m_Values.TryGetValue(key, out var value) ? value : ScriptValue.Null;
        }
    }
    //自带基础类型的原表,不支持动态申请,只能已特定形式申请变量, number, string, bool, function 等
    internal class ScriptTypePrimitive : ScriptType {
        public ScriptTypePrimitive(Script script, string typeName, ScriptValue parentValue) : base(script) {
            Set(typeName, parentValue);
        }
        public override void Free() {
            Release();
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            throw new ExecutionException($"Class<{TypeName}>不支持构造");
        }
    }
    internal abstract class ScriptTypeBasic<T> : ScriptType where T : ScriptObject {
        internal ScriptTypeBasic(Script script, string typeName, ScriptValue parentType) : base(script) {
            Set(typeName, parentType);
        }
        public override void Free() {
            Release();
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(New());
        }
        protected abstract T New();
    }
    //Array类型原表
    internal class ScriptTypeBasicArray : ScriptTypeBasic<ScriptArray> {
        internal ScriptTypeBasicArray(Script script, string typeName, ScriptValue parentType) : base(script, typeName, parentType) { }
        protected override ScriptArray New() {
            return m_Script.NewArray();
        }
    }
    //Map原表
    internal class ScriptTypeBasicMap : ScriptTypeBasic<ScriptMapObject> {
        internal ScriptTypeBasicMap(Script script, string typeName, ScriptValue parentType) : base(script, typeName, parentType) { }
        protected override ScriptMapObject New() {
            return m_Script.NewMapObject();
        }
    }
    //HashSet
    internal class ScriptTypeBasicHashSet : ScriptTypeBasic<ScriptHashSet> {
        internal ScriptTypeBasicHashSet(Script script, string typeName, ScriptValue parentType) : base(script, typeName, parentType) { }
        protected override ScriptHashSet New() {
            return m_Script.NewHashSet();
        }
    }
    //StringBuilding原表
    internal class ScriptTypeBasicStringBuilder : ScriptTypeBasic<ScriptStringBuilder> {
        internal ScriptTypeBasicStringBuilder(Script script, string typeName, ScriptValue parentType) : base(script, typeName, parentType) { }
        protected override ScriptStringBuilder New() {
            return m_Script.NewStringBuilder();
        }
    }
}
