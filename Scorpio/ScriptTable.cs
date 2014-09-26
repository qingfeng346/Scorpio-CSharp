using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Scorpio.Collections;
namespace Scorpio
{
    //脚本table类型
    public class ScriptTable : ScriptObject
    {
        private TableDictionary m_listObject;                                   //所有的数据(函数和数据都在一个数组)
        public override ObjectType Type { get { return ObjectType.Table; } }
        public ScriptTable()
        {
            m_listObject = new TableDictionary();
        }
        public void SetValue(object key, ScriptObject scriptObject)
        {
            Util.AssignObject(m_listObject, key, scriptObject);
        }
        public ScriptObject GetValue(object key)
        {
            return m_listObject.ContainsKey(key) ? m_listObject[key] : ScriptNull.Instance;
        }
        public bool HasValue(object key)
        {
            return m_listObject.ContainsKey(key);
        }
        public int Count()
        {
            return m_listObject.Count;
        }
        public TableDictionary.Enumerator GetIterator()
        {
            return m_listObject.GetEnumerator();
        }
        public override string ToString() { return "Table"; }
    }
}
