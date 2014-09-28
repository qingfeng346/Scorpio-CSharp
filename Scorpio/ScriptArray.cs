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
        private ScriptArray(List<ScriptObject> objs)
        {
            m_listObject = objs;
        }
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
            List<ScriptObject> objs = new List<ScriptObject>();
            for (int i = 0; i < m_listObject.Count; ++i) {
                objs.Add(m_listObject[i].Clone());
            }
            return new ScriptArray(objs);
        }
        public override string ToString() { return "Array"; }
    }
}
