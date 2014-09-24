using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scorpio.Collections;
using Scorpio.Runtime;
using Scorpio.Compiler;
using Scorpio.Exception;
using Scorpio.Library;
using Scorpio.Userdata;
using Scorpio.Variable;
namespace Scorpio
{
    //脚本类
    public class Script
    {
        //全局变量
        private IScriptUserdataFactory m_UserdataFactory = null;
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
        public void SetObject(string strName, object value)
        {
            if (value == null) return;
            Util.AssignObject(m_GlobalObject, strName, CreateObject(value));
        }
        internal void SetObjectInternal(string strName, ScriptObject value)
        {
            if (value == null) return;
            Util.AssignObject(m_GlobalObject, strName, value);
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
        public ScriptObject CreateObject(object value)
        {
            if (value == null)
                return ScriptNull.Instance;
            else if (value is ScriptObject)
                return (ScriptObject)value;
            else if (value is ScriptFunction)
                return CreateFunction((ScorpioFunction)value);
            else if (value is ScorpioHandle)
                return CreateFunction((ScorpioHandle)value);
            else if (value is Delegate)
                return CreateFunction((Delegate)value);
            else if (value is ScorpioMethod)
                return CreateFunction((ScorpioMethod)value);
            Type type = value.GetType();
            if (Util.IsBool(type))
                return CreateBool((bool)value);
            else if (Util.IsString(type))
                return CreateString((string)value);
            else if (Util.IsNumber(type))
                return CreateNumber(value);
            return CreateUserdata(value);
        }
        public ScriptBoolean CreateBool(bool value)
        {
            return ScriptBoolean.Get(value);
        }
        public ScriptString CreateString(string value)
        {
            return new ScriptString(value);
        }
        public ScriptNumber CreateNumber(object value)
        {
            return new ScriptNumber(value);
        }
        public ScriptUserdata CreateUserdata(object value)
        {
            return m_UserdataFactory.create(this, value);
        }
        internal ScriptFunction CreateFunction(string name, ScorpioScriptFunction value)
        {
            return new ScriptFunction(this, name, value);
        }
        public ScriptFunction CreateFunction(ScorpioFunction value)
        {
            return new ScriptFunction(this, value);
        }
        public ScriptFunction CreateFunction(ScorpioHandle value)
        {
            return new ScriptFunction(this, value);
        }
        public ScriptFunction CreateFunction(Delegate value)
        {
            return new ScriptFunction(this, value);
        }
        public ScriptFunction CreateFunction(ScorpioMethod value)
        {
            return new ScriptFunction(this, value);
        }
        public void LoadLibrary()
        {
            m_UserdataFactory = new DefaultScriptUserdataFactory();
            LibraryBasis.Load(this);
            LibraryArray.Load(this);
            LibraryString.Load(this);
            LibraryTable.Load(this);
        }
    }
}
