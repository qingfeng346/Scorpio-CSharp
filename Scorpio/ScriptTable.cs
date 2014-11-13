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
        public override ObjectType Type { get { return ObjectType.Table; } }
        private Dictionary<object, ScriptObject> m_listObject = new Dictionary<object, ScriptObject>();  //所有的数据(函数和数据都在一个数组)
        public ScriptTable(Script script) : base(script) { }
        public override void SetValue(int key, ScriptObject value)
        {
            SetValue_impl(key, value);
        }
        public override ScriptObject GetValue(int key)
        {
            return GetValue_impl(key);
        }
        public override void SetValue(string key, ScriptObject value)
        {
            SetValue_impl(key, value);
        }
        public override ScriptObject GetValue(string key)
        {
            return GetValue_impl(key);
        }
        public override void SetValue(object key, ScriptObject value)
        {
            SetValue_impl(key, value);
        }
        public override ScriptObject GetValue(object key)
        {
            return GetValue_impl(key);
        }
        public void SetValue_impl(object key, ScriptObject scriptObject)
        {
            Util.SetObject(m_listObject, key, scriptObject);
        }
        public ScriptObject GetValue_impl(object key)
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
        public Dictionary<object, ScriptObject>.Enumerator GetIterator()
        {
            return m_listObject.GetEnumerator();
        }
        public override ScriptObject Clone()
        {
            ScriptTable ret = Script.CreateTable();
            ScriptObject obj = null;
            ScriptFunction func = null;
            foreach (KeyValuePair<object, ScriptObject> pair in m_listObject) {
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
