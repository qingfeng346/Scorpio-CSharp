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
        private String m_strBreviary = "";
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
                throw new ScriptException(e, "load file " + strFileName + " is error ");
            }
        }
        public ScriptObject LoadString(String strName, String strBuffer)
        {
            try {
                ScriptLexer scriptLexer = new ScriptLexer(strBuffer);
                m_strBreviary = scriptLexer.GetBreviary();
                List<Token> tokens = scriptLexer.GetTokens();
                ScriptParser scriptParser = new ScriptParser(this, tokens);
                ScriptExecutable scriptExecutable = scriptParser.Parse();
                return new ScriptContext(this, scriptExecutable, null, Executable_Block.Context).Execute();
            } catch (System.Exception e) {
                throw new ScriptException(e, "load buffer " + m_strBreviary + " is error ");
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
        public ScriptObject CallFunction(String strName, params ScriptObject[] args)
        {
            if (m_GlobalObject.ContainsKey(strName))
            {
                ScriptObject obj = m_GlobalObject[strName];
                if (obj.IsFunction)
                    return ((ScriptFunction)obj).Call(args);
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
