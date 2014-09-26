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
        public void SetValue(object strName, ScriptObject scriptObject)
        {
            m_listObject[strName] = scriptObject;
        }
        public ScriptObject GetValue(object strName)
        {
            if (m_listObject.ContainsKey(strName))
                return m_listObject[strName];
            return ScriptNull.Instance;
        }
        public bool HasValue(object strName)
        {
            return m_listObject.ContainsKey(strName);
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
