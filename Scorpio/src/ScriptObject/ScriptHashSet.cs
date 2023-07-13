using System;
using System.Collections;
using System.Collections.Generic;
using Scorpio.Tools;
using Scorpio.Library;
using System.Linq;

namespace Scorpio {
    public class ScriptHashSet : ScriptInstance, IEnumerable<ScriptValue> {
        private static List<ScriptValue> tempList = new List<ScriptValue>();
        public HashSet<ScriptValue> m_Objects = new HashSet<ScriptValue>();
        public ScriptHashSet(Script script) : base(script, ObjectType.HashSet) { }
        public new IEnumerator<ScriptValue> GetEnumerator() { return m_Objects.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return m_Objects.GetEnumerator(); }
        public override void Alloc() {
            AddRecord();
            SetPrototype(script.TypeHashSet);
        }
        public override void Free() {
            DelRecord();
            Release();
            Clear();
            m_Script.Free(this);
        }
        public override void gc() {
            Clear();
        }
        public void Add(ScriptValue item) {
            if (!m_Objects.Contains(item)) {
                m_Objects.Add(item.Reference());
            }
        }
        public void AddNoReference(ScriptValue item) {
            if (!m_Objects.Contains(item)) {
                m_Objects.Add(item);
            } else {
                item.Release();
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
        //添加一个集合
        public void UnionWith(IEnumerable<ScriptValue> other) {
            foreach (var element in other) {
                Add(element);
            }
        }
        //删除一个集合
        public void ExceptWith(IEnumerable<ScriptValue> other) {
            foreach (var element in other) {
                Remove(element);
            }
        }
        //两个集合内重复的元素
        public void IntersectWith(IEnumerable<ScriptValue> other) {
            tempList.Clear();
            foreach (var element in m_Objects) {
                if (!other.Contains(element)) {
                    tempList.Add(element);
                }
            }
            for (var i = 0; i < tempList.Count;++i) {
                Remove(tempList[i]);
            }
        }
        //两个集合内不重复的所有元素
        public void SymmetricExceptWith(IEnumerable<ScriptValue> other) {
            tempList.Clear();
            foreach (var element in other) {
                if (m_Objects.Contains(element)) {
                    tempList.Add(element);
                } else {
                    Add(element);
                }
            }
            for (var i = 0; i < tempList.Count; ++i) {
                Remove(tempList[i]);
            }
        }
        //当前集合是否全部包含在other内,  this是other的真子集(两个集合内容长度一致返回false)
        public bool IsProperSubsetOf(IEnumerable<ScriptValue> other) {
            return m_Objects.IsProperSubsetOf(other);
        }
        //当前集合是否全部包含other内的元素  other是this的真子集(两个集合内容长度一致返回false)
        public bool IsProperSupersetOf(IEnumerable<ScriptValue> other) {
            return m_Objects.IsProperSupersetOf(other);
        }
        //当前集合是否全部包含在other内,  this是other的子集
        public bool IsSubsetOf(IEnumerable<ScriptValue> other) {
            return m_Objects.IsSubsetOf(other);
        }
        //当前集合是否全部包含other内的元素  other是this的子集
        public bool IsSupersetOf(IEnumerable<ScriptValue> other) {
            return m_Objects.IsSupersetOf(other);
        }
        //两个集合是否有相同的元素
        public bool Overlaps(IEnumerable<ScriptValue> other) {
            return m_Objects.Overlaps(other);
        }
        //两个集合内容和长度一致,两个集合相同
        public bool SetEquals(IEnumerable<ScriptValue> other) {
            return m_Objects.SetEquals(other);
        }
        //清理冗余的分配
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
            return m_Script.ToString(this);
        }
        internal override void ToString(ScorpioStringSerializer serializer) {
            var builder = serializer.m_Builder;
            builder.Append("[");
            var first = true;
            foreach (var value in m_Objects) {
                if (first) { first = false; } else { builder.Append(","); }
                serializer.Serializer(value);
            }
            builder.Append("]");
        }
        internal override void ToJson(ScorpioJsonSerializer serializer) {
            var builder = serializer.m_Builder;
            builder.Append("[");
            var first = true;
            foreach (var value in m_Objects) {
                if (first) { first = false; } else { builder.Append(","); }
                serializer.Serializer(value);
            }
            builder.Append("]");
        }
    }
}
