using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Scorpio.Function;
using Scorpio.Compiler;
using Scorpio.Exception;
namespace Scorpio
{
    //脚本table类型
    public class ScriptTable : ScriptObject
    {
        public override ObjectType Type { get { return ObjectType.Table; } }
        private Dictionary<object, ScriptObject> m_listObject = new Dictionary<object, ScriptObject>();  //所有的数据(函数和数据都在一个数组)
        public ScriptTable(Script script) : base(script) { }
        public override void SetValue(object key, ScriptObject value)
        {
            Util.SetObject(m_listObject, key, value);
        }
        public override ScriptObject GetValue(object key)
        {
            return m_listObject.ContainsKey(key) ? m_listObject[key] : m_Script.Null;
        }
        public override ScriptObject AssignCompute(TokenType type, ScriptObject value) {
            if (type != TokenType.AssignPlus) { return base.AssignCompute(type, value); }
            ScriptTable table = value as ScriptTable;
            if (table == null) throw new ExecutionException(m_Script, "table [+=] 操作只支持两个table " + value.Type);
            ScriptObject obj = null;
            ScriptScriptFunction func = null;
            foreach (KeyValuePair<object, ScriptObject> pair in table.m_listObject) {
                obj = pair.Value.Clone();
                if (obj is ScriptScriptFunction) {
                    func = (ScriptScriptFunction)obj;
                    if (!func.IsStaticFunction) func.SetTable(this);
                }
                m_listObject[pair.Key] = obj;
            }
            return this;
        }
        public override ScriptObject Compute(TokenType type, ScriptObject value) {
            if (type != TokenType.Plus) { return base.Compute(type, value); }
            ScriptTable table = value as ScriptTable;
            if (table == null) throw new ExecutionException(m_Script, "table [+] 操作只支持两个table " + value.Type);
            ScriptTable ret = m_Script.CreateTable();
            ScriptObject obj = null;
            ScriptScriptFunction func = null;
            foreach (KeyValuePair<object, ScriptObject> pair in m_listObject) {
                obj = pair.Value.Clone();
                if (obj is ScriptScriptFunction) {
                    func = (ScriptScriptFunction)obj;
                    if (!func.IsStaticFunction) func.SetTable(ret);
                }
                ret.m_listObject[pair.Key] = obj;
            }
            foreach (KeyValuePair<object, ScriptObject> pair in table.m_listObject) {
                obj = pair.Value.Clone();
                if (obj is ScriptScriptFunction) {
                    func = (ScriptScriptFunction)obj;
                    if (!func.IsStaticFunction) func.SetTable(ret);
                }
                ret.m_listObject[pair.Key] = obj;
            }
            return ret;
        }
        public bool HasValue(object key)
        {
			if (key == null) return false;
            return m_listObject.ContainsKey(key);
        }
        public int Count()
        {
            return m_listObject.Count;
        }
        public void Clear()
        {
            m_listObject.Clear();
        }
        public void Remove(object key) {
            m_listObject.Remove(key);
        }
		public ScriptArray GetKeys() {
			ScriptArray ret = m_Script.CreateArray ();
			foreach (KeyValuePair<object, ScriptObject> pair in m_listObject) {
				ret.Add(m_Script.CreateObject(pair.Key));
			}
			return ret;
		}
		public ScriptArray GetValues() {
			ScriptArray ret = m_Script.CreateArray ();
			foreach (KeyValuePair<object, ScriptObject> pair in m_listObject) {
				ret.Add(pair.Value.Assign());
			}
			return ret;
		}
        public Dictionary<object, ScriptObject>.Enumerator GetIterator() {
            return m_listObject.GetEnumerator();
        }
        public override ScriptObject Clone() {
            ScriptTable ret = m_Script.CreateTable();
            ScriptObject obj = null;
            ScriptScriptFunction func = null;
            foreach (KeyValuePair<object, ScriptObject> pair in m_listObject) {
                if (pair.Value == this) {
                    ret.m_listObject[pair.Key] = ret;
                } else {
                    obj = pair.Value.Clone();
                    if (obj is ScriptScriptFunction) {
                        func = (ScriptScriptFunction)obj;
                        if (!func.IsStaticFunction) func.SetTable(ret);
                    }
                    ret.m_listObject[pair.Key] = obj;
                }
            }
            return ret;
        }
        public override string ToString() { return "Table"; }
        public override string ToJson()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");
            bool first = true;
            foreach (KeyValuePair<object, ScriptObject> pair in m_listObject) {
                if (first)
                    first = false;
                else
                    builder.Append(",");
                builder.Append("\"");
                builder.Append(pair.Key);
                builder.Append("\":");
                builder.Append(pair.Value.ToJson());
            }
            builder.Append("}");
            return builder.ToString();
        }
    }
}
