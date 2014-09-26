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
        public override ObjectType Type { get { return ObjectType.Table; } }
        private TableDictionary m_listObject = new TableDictionary();  //所有的数据(函数和数据都在一个数组)
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
        public override ScriptObject Clone()
        {
            ScriptTable ret = new ScriptTable();
            ScriptObject obj = null;
            ScriptFunction func = null;
            foreach (var pair in m_listObject) {
                obj = pair.Value.Clone();
                if (obj is ScriptFunction) {
                    func = (ScriptFunction)obj;
                    if (!func.IsStatic) func.SetTable(ret);
                }
                ret.m_listObject[pair.Key] = obj;
            }
            return ret;
        }
        public override string ToString() { return "Table"; }
    }
}
