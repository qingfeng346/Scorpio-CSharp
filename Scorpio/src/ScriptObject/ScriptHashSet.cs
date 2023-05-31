﻿using System;
using System.Collections;
using System.Collections.Generic;
using Scorpio.Tools;
using Scorpio.Library;
namespace Scorpio {
    public class ScriptHashSet : ScriptInstance, IEnumerable<ScriptValue> {
        private Script m_Script;
        public HashSet<ScriptValue> m_Objects = new HashSet<ScriptValue>();
        public ScriptHashSet(Script script) : base(ObjectType.HashSet, script.TypeHashSet) { m_Script = script; }
        public new IEnumerator<ScriptValue> GetEnumerator() { return m_Objects.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return m_Objects.GetEnumerator(); }
        public Script getScript() { return m_Script; }
        public override void Free() {
            base.Free();
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
            return m_Objects.RemoveWhere(match);
        }
        public int Count => m_Objects.Count;
        public void UnionWith(IEnumerable<ScriptValue> other) {
            m_Objects.UnionWith(other);
        }
        public void ExceptWith(IEnumerable<ScriptValue> other) {
            m_Objects.ExceptWith(other);
        }
        public void IntersectWith(IEnumerable<ScriptValue> other) {
            m_Objects.IntersectWith(other);
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
        public void SymmetricExceptWith(IEnumerable<ScriptValue> other) {
            m_Objects.SymmetricExceptWith(other);
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
