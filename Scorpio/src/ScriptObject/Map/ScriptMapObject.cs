using System.Collections;
using System.Collections.Generic;
using Scorpio.Tools;

namespace Scorpio {
    //脚本map类型
    public class ScriptMapObject : ScriptMap, IEnumerable<KeyValuePair<object, ScriptValue>> {
        private Dictionary<object, ScriptValue> m_Objects;  //所有的数据(函数和数据都在一个数组)
        public ScriptMapObject(Script script) : base(script) {
            m_Objects = new Dictionary<object, ScriptValue>();
        }
        public override IEnumerator<KeyValuePair<object, ScriptValue>> GetEnumerator() { return m_Objects.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public override void Free() {
            DelRecord();
            Release();
            var clear = m_Objects.Count > ScorpioUtil.EMPTY_LIMIT;
            Clear();
            if (clear) m_Objects = new Dictionary<object, ScriptValue>();
            m_Script.Free(this);
        }
        public override void gc() {
            Clear();
        }
        #region GetValue SetValue 重载
        public override ScriptValue GetValue(string key) {
            return m_Objects.TryGetValue(key, out var value) ? value : m_Prototype.GetValue(key);
        }
        public override ScriptValue GetValue(double key) {
            return m_Objects.TryGetValue(key, out var value) ? value : ScriptValue.Null;
        }
        public override ScriptValue GetValue(long key) {
            return m_Objects.TryGetValue(key, out var value) ? value : ScriptValue.Null;
        }
        public override ScriptValue GetValue(object key) {
            return m_Objects.TryGetValue(key, out var value) ? value : ScriptValue.Null;
        }
        public override void SetValue(string key, ScriptValue value) {
            if (m_Objects.TryGetValue(key, out var result)) {
                result.Free();
            }
            //正常引用计数
            m_Objects[key] = value.Reference();
        }
        public override void SetValue(double key, ScriptValue value) {
            if (m_Objects.TryGetValue(key, out var result)) {
                result.Free();
            }
            //正常引用计数
            m_Objects[key] = value.Reference();
        }
        public override void SetValue(long key, ScriptValue value) {
            if (m_Objects.TryGetValue(key, out var result)) {
                result.Free();
            }
            //正常引用计数
            m_Objects[key] = value.Reference();
        }
        public override void SetValue(object key, ScriptValue value) {
            if (m_Objects.TryGetValue(key, out var result)) {
                result.Free();
            }
            //正常引用计数
            m_Objects[key] = value.Reference();
        }
        #endregion
        #region ScriptInstance 重载
        public override void SetValueNoReference(string key, ScriptValue value) {
            if (m_Objects.TryGetValue(key, out var result)) {
                result.Free();
            }
            //不计数
            m_Objects[key] = value;
        }
        public override void SetValueNoReference(object key, ScriptValue value) {
            if (m_Objects.TryGetValue(key, out var result)) {
                result.Free();
            }
            //不计数
            m_Objects[key] = value;
        }
        public override bool HasValue(string key) {
            return m_Objects.ContainsKey(key);
        }
        public override void ClearVariables() {
            base.ClearVariables();
            m_Objects.Free();
        }
        public override void DelValue(string key) {
            Remove(key);
        }
        #endregion
        #region ScriptMap重载
        public override bool ContainsKey(object key) {
            if (key == null) return false;
            return m_Objects.ContainsKey(key);
        }
        public override bool ContainsValue(ScriptValue value) {
            return m_Objects.ContainsValue(value);
        }
        public override int Count() {
            return m_Objects.Count;
        }
        public override void Clear() {
            m_Objects.Free();
        }
        public override void Remove(object key) {
            if (m_Objects.TryGetValue(key, out var result)) {
                result.Free();
                m_Objects.Remove(key);
            }
        }
        public override ScriptArray GetKeys() {
            var ret = m_Script.NewArray();
            foreach (var pair in m_Objects) {
                ret.AddNoReference(ScriptValue.CreateValue(m_Script, pair.Key));
            }
            return ret;
        }
        public override ScriptArray GetValues() {
            var ret = m_Script.NewArray();
            ret.SetArrayCapacity(m_Objects.Count);
            foreach (var pair in m_Objects) {
                ret.Add(pair.Value);
            }
            return ret;
        }
        public override ScriptMap NewCopy() {
            var ret = m_Script.NewMapObject();
            foreach (var pair in m_Objects) {
                ret.m_Objects[pair.Key] = pair.Value.Reference();
            }
            return ret;
        }
        #endregion
        public override ScriptObject Clone(bool deep) {
            var ret = m_Script.NewMapObject();
            if (deep) {
                foreach (var pair in m_Objects) {
                    var value = pair.Value;
                    if (value.valueType == ScriptValue.scriptValueType) {
                        var scriptObject = value.scriptValue;
                        if (scriptObject != this && (scriptObject is ScriptArray || scriptObject is ScriptMap)) {
                            ret.m_Objects[pair.Key] = new ScriptValue(scriptObject.Clone(true));
                        } else {
                            ret.m_Objects[pair.Key] = pair.Value.Reference();
                        }
                    } else {
                        ret.m_Objects[pair.Key] = pair.Value.Reference();
                    }
                }
            } else {
                foreach (var pair in m_Objects) {
                    ret.m_Objects[pair.Key] = pair.Value.Reference();
                }
            }
            return ret;
        }
    }
}
