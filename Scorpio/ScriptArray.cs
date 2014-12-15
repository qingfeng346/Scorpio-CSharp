using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.CodeDom;
using Scorpio.Exception;
namespace Scorpio
{
    //脚本数组类型
    public class ScriptArray : ScriptObject
    {
        public override ObjectType Type { get { return ObjectType.Array; } }
        public List<ScriptObject> m_listObject = new List<ScriptObject>();
        public ScriptArray(Script script) : base(script) { }
        public override ScriptObject GetValue(int index)
        {
            if (index < 0 || index >= m_listObject.Count)
                throw new ExecutionException("index is < 0 or out of count ");
            return m_listObject[index];
        }
        public override void SetValue(int index, ScriptObject obj)
        {
            if (index < 0 || index >= m_listObject.Count)
                throw new ExecutionException("index is < 0 or out of count ");
            m_listObject[index] = obj;
        }
        public void Add(ScriptObject obj)
        {
            m_listObject.Add(obj);
        }
        public void Insert(int index, ScriptObject obj)
        {
            m_listObject.Insert(index, obj);
        }
        public void Clear()
        {
            m_listObject.Clear();
        }
        public int Count()
        {
            return m_listObject.Count;
        }
        public List<ScriptObject>.Enumerator GetIterator()
        {
            return m_listObject.GetEnumerator();
        }
        public override ScriptObject Clone()
        {
            ScriptArray ret = Script.CreateArray();
            for (int i = 0; i < m_listObject.Count; ++i) {
                ret.m_listObject.Add(m_listObject[i].Clone());
            }
            return ret;
        }
        public override string ToString() { return "Array"; }
        public override string ToJson()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[");
            for (int i = 0; i < m_listObject.Count;++i )
            {
                if (i != 0) builder.Append(",");
                builder.Append(m_listObject[i].ToJson());
            }
            builder.Append("]");
            return builder.ToString();
        }
    }
}
