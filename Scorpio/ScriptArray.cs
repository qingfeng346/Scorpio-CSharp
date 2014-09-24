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
        public List<ScriptObject> m_listObject;
        public override ObjectType Type { get { return ObjectType.Array; } }
        public ScriptArray()
        {
            m_listObject = new List<ScriptObject>();
        }
        public ScriptObject GetValue(int index, CodeObject member)
        {
            if (index < 0 || index >= m_listObject.Count)
                throw new ExecutionException("index is < 0 or out of count ", member);
            return m_listObject[index];
        }
        public void SetValue(int index, ScriptObject obj, CodeObject member)
        {
            if (index < 0 || index >= m_listObject.Count)
                throw new ExecutionException("index is < 0 or out of count ", member);
            m_listObject[index] = obj;
        }
        public ScriptObject GetValue(int index)
        {
            return m_listObject[index];
        }
        public void SetValue(int index, ScriptObject obj)
        {
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
        public int Count()
        {
            return m_listObject.Count;
        }
        public List<ScriptObject>.Enumerator GetIterator()
        {
            return m_listObject.GetEnumerator();
        }
        public override string ToString() { return "Array"; }
    }
}
