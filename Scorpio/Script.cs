using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scorpio.Collections;
using Scorpio.Runtime;
using Scorpio.Compiler;
using Scorpio.Exception;
using Scorpio.Library;
namespace Scorpio
{
    //脚本类
    public class Script
    {
        //全局变量
        private VariableDictionary m_GlobalObject = new VariableDictionary();
        public ReadOnlyDictionary<String, ScriptObject> GlobalObject { get { return ReadOnlyDictionary<String, ScriptObject>.AsReadOnly(m_GlobalObject); } }
        public void LoadFile(String strFileName)
        {
            try {
                FileStream stream = File.OpenRead(strFileName);
                long length = stream.Length;
                byte[] buffer = new byte[length];
                stream.Read(buffer, 0, Convert.ToInt32(length));
                stream.Close();
                LoadString(strFileName,Encoding.UTF8.GetString(buffer));
            } catch (System.Exception e) {
                throw new ScriptException("load file [" + strFileName + "] is error : " + e.ToString());
            }
        }
        public ScriptObject LoadString(String strName, String strBuffer)
        {
            string strBreviary = "";
            try {
                ScriptLexer scriptLexer = new ScriptLexer(strBuffer);
                strBreviary = string.IsNullOrEmpty(strName) ? scriptLexer.GetBreviary() : strName;
                ScriptParser scriptParser = new ScriptParser(this, scriptLexer.GetTokens(), strBreviary);
                ScriptExecutable scriptExecutable = scriptParser.Parse();
                return new ScriptContext(this, scriptExecutable, null, Executable_Block.Context).Execute();
            } catch (System.Exception e) {
                throw new ScriptException("load buffer [" + strBreviary + "] is error : " + e.ToString());
            }
        }
        public bool HasObject(String strName)
        {
            return m_GlobalObject.ContainsKey(strName);
        }
        public ScriptObject GetObject(string strName)
        {
            return m_GlobalObject.ContainsKey(strName) ? m_GlobalObject[strName] : ScriptNull.Instance;
        }
        public void SetObject(string strName, ScriptObject obj)
        {
            if (obj == null) return;
            Util.AssignObject(m_GlobalObject, strName, obj);
        }
        public void RegisterFunction(String strName, ScorpioFunction Function)
        {
            m_GlobalObject[strName] = new ScriptFunction(strName, Function);
        }
        public void RegisterFunction(String strName, ScorpioHandle handle)
        {
            m_GlobalObject[strName] = new ScriptFunction(strName, handle);
        }
        public void RegisterFunction(String strName, Delegate dele)
        {
            m_GlobalObject[strName] = new ScriptFunction(strName, dele);
        }
        public ScriptObject CallFunction(String strName, params ScriptObject[] args)
        {
            if (m_GlobalObject.ContainsKey(strName))
            {
                ScriptFunction func = m_GlobalObject[strName] as ScriptFunction;
                if (func != null)
                    return func.Call(args);
            }
            return null;
        }
        public void LoadLibrary()
        {
            LibraryBasis.Load(this);
            LibraryArray.Load(this);
            LibraryString.Load(this);
            LibraryTable.Load(this);
        }
    }
}
