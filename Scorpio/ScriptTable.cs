using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Scorpio
{
    //脚本table类型
    public class ScriptTable : ScriptObject
    {
        private Dictionary<String, ScriptObject> m_listObject;               //所有的数据(函数和数据都在一个数组)
        public ScriptTable()
        {
            Type = ObjectType.Table;
            m_listObject = new Dictionary<String, ScriptObject>();
        }
        public void SetValue(String strName, ScriptObject scriptObject)
        {
            m_listObject[strName] = scriptObject;
        }
        public ScriptObject GetValue(string strName)
        {
            if (HasValue(strName))
                return m_listObject[strName];
            return ScriptNull.Instance;
        }
        public bool HasValue(String strName)
        {
            return m_listObject.ContainsKey(strName);
        }
        public int Count()
        {
            return m_listObject.Count;
        }
        public Dictionary<String, ScriptObject>.Enumerator GetIterator()
        {
            return m_listObject.GetEnumerator();
        }
        public override string ToString() { return "Table"; }
    }
}
