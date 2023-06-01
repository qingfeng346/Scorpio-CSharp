using System;
using System.Collections;
using System.Collections.Generic;
using Scorpio.Tools;
using Scorpio.Library;
using Scorpio.Exception;
namespace Scorpio {
    public class ScriptHashSet : ScriptInstance, IEnumerable<ScriptValue> {
        public HashSet<ScriptValue> m_Objects = new HashSet<ScriptValue>();
        public ScriptHashSet(Script script) : base(script, ObjectType.HashSet) {
            Set(script.TypeHashSet);
        }
        public new IEnumerator<ScriptValue> GetEnumerator() { return m_Objects.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return m_Objects.GetEnumerator(); }
        public Script getScript() { return m_Script; }
        public override void Free() {
            Release();
            Clear();
            m_Script.Free(this);
        }
        public void Add(ScriptValue item) {
            if (!m_Objects.Contains(item)) {
                m_Objects.Add(new ScriptValue(item));
            }
        }
        public void Clear() {
            foreach (var value in m_Objects) {
                value.Free();
            }
            m_Objects.Clear();
        }
        public bool Contains(ScriptValue item) {
            return m_Objects.Contains(item);
        }
        public bool Remove(ScriptValue item) {
            if (m_Objects.Remove(item)) {
                item.Free();
                return true;
            }
            return false;
        }
        public int RemoveWhere(Predicate<ScriptValue> match) {
            return m_Objects.RemoveWhere(value => {
                if (match(value)) {
                    value.Free();
                    return true;
                }
                return false;
            });
        }
        public int Count => m_Objects.Count;
        public void UnionWith(IEnumerable<ScriptValue> other) {
            foreach (var element in other) {
                Add(element);
            }
        }
        public void ExceptWith(IEnumerable<ScriptValue> other) {
            foreach (var element in other) {
                Remove(element);
            }
        }
        public void IntersectWith(IEnumerable<ScriptValue> other) {
            //m_Objects.IntersectWith(other);
            throw new ExecutionException("未实现 IntersectWith");
        }
        public void SymmetricExceptWith(IEnumerable<ScriptValue> other) {
            //m_Objects.SymmetricExceptWith(other);
            throw new ExecutionException("未实现 SymmetricExceptWith");
        }
        public bool IsProperSubsetOf(IEnumerable<ScriptValue> other) {
            return m_Objects.IsProperSubsetOf(other);
        }
        public bool IsProperSupersetOf(IEnumerable<ScriptValue> other) {
            return m_Objects.IsProperSupersetOf(other);
        }
        public bool IsSubsetOf(IEnumerable<ScriptValue> other) {
            return m_Objects.IsSubsetOf(other);
        }
        public bool IsSupersetOf(IEnumerable<ScriptValue> other) {
            return m_Objects.IsSupersetOf(other);
        }
        public bool Overlaps(IEnumerable<ScriptValue> other) {
            return m_Objects.Overlaps(other);
        }
        public bool SetEquals(IEnumerable<ScriptValue> other) {
            return m_Objects.SetEquals(other);
        }
        public void TrimExcess() {
            m_Objects.TrimExcess();
        }
        public Array ToArray(Type type) {
            var array = Array.CreateInstance(type, m_Objects.Count);
            var index = 0;
            foreach (var value in m_Objects) {
                array.SetValue(value.ChangeType(type), index++);
            }
            return array;
        }
        public override string ToString() {
            using (var serializer = new ScorpioJsonSerializer()) {
                return serializer.ToJson(this);
            }
        }
        internal override void ToJson(ScorpioJsonSerializer jsonSerializer) {
            var builder = jsonSerializer.m_Builder;
            builder.Append("[");
            var first = true;
            foreach (var value in m_Objects) {
                if (first) { first = false; } else { builder.Append(","); }
                jsonSerializer.Serializer(value);
            }
            builder.Append("]");
        }
    }
}
