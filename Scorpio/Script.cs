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
        private const string GLOBAL_TABLE = "_G";
        private IScriptUserdataFactory m_UserdataFactory = null;                //Userdata工厂
        private ScriptTable m_GlobalTable = new ScriptTable();                  //全局Table
        private List<StackInfo> m_StackInfoStack = new List<StackInfo>();       //堆栈数据
        private StackInfo m_StackInfo = new StackInfo();                        //最近堆栈数据
        public ScriptObject LoadFile(String strFileName)
        {
            try {
                FileStream stream = File.OpenRead(strFileName);
                long length = stream.Length;
                byte[] buffer = new byte[length];
                stream.Read(buffer, 0, Convert.ToInt32(length));
                stream.Close();
                return LoadString(Path.GetFileName(strFileName), Encoding.UTF8.GetString(buffer));
            } catch (System.Exception e) {
                throw new ScriptException("load file [" + strFileName + "] is error : " + e.ToString());
            }
        }
        public ScriptObject LoadString(String strBuffer)
        {
            return LoadString("", strBuffer);
        }
        public ScriptObject LoadString(String strName, String strBuffer)
        {
            string strBreviary = "";
            try {
                m_StackInfoStack.Clear();
                ScriptLexer scriptLexer = new ScriptLexer(strBuffer);
                strBreviary = string.IsNullOrEmpty(strName) ? scriptLexer.GetBreviary() : strName;
                ScriptParser scriptParser = new ScriptParser(this, scriptLexer.GetTokens(), strBreviary);
                ScriptExecutable scriptExecutable = scriptParser.Parse();
                return new ScriptContext(this, scriptExecutable, null, Executable_Block.Context).Execute();
            } catch (System.Exception e) {
                throw new ScriptException("load buffer [" + strBreviary + "] is error : " + e.ToString());
            }
        }
        internal void SetStackInfo(StackInfo info)
        {
            m_StackInfo = info;
        }
        internal void PushStackInfo()
        {
            m_StackInfoStack.Add(m_StackInfo);
        }
        public string GetStackInfo()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Source [ " + m_StackInfo.Breviary + "] Line [" + m_StackInfo.Line + "]");
            for (int i = m_StackInfoStack.Count - 1; i >= 0;--i ) {
                builder.AppendLine("        Source [" + m_StackInfoStack[i].Breviary + "] Line [" + m_StackInfoStack[i].Line + "]");
            }
            return builder.ToString();
        }
        public bool HasValue(String key)
        {
            return (key == GLOBAL_TABLE) || m_GlobalTable.HasValue(key);
        }
        public ScriptObject GetValue(string key)
        {
            if (key == GLOBAL_TABLE)
                return m_GlobalTable;
            return m_GlobalTable.GetValue(key);
        }
        public void SetObject(string key, object value)
        {
            m_GlobalTable.SetValue(key, CreateObject(value));
        }
        internal void SetObjectInternal(string key, ScriptObject value)
        {
            m_GlobalTable.SetValue(key, value);
        }
        public ScriptObject Call(String strName, params object[] args)
        {
            ScriptObject obj = m_GlobalTable.GetValue(strName);
            if (obj is ScriptNull) throw new ScriptException("找不到变量[" + strName + "]");
            int length = args.Length;
            ScriptObject[] parameters = new ScriptObject[length];
            for (int i = 0; i < length;++i ) {
                parameters[i] = CreateObject(args[i]);
            }
            m_StackInfoStack.Clear();
            return obj.Call(parameters);
        }
        public ScriptObject CreateObject(object value)
        {
            if (value == null)
                return ScriptNull.Instance;
            else if (value is ScriptObject)
                return (ScriptObject)value;
            else if (value is ScorpioFunction)
                return CreateFunction((ScorpioFunction)value);
            else if (value is ScorpioHandle)
                return CreateFunction((ScorpioHandle)value);
            else if (value is Delegate)
                return CreateFunction((Delegate)value);
            else if (value is ScorpioMethod)
                return CreateFunction((ScorpioMethod)value);
            else if (Util.IsBoolObject(value))
                return CreateBool((bool)value);
            else if (Util.IsStringObject(value))
                return CreateString((string)value);
            else if (Util.IsNumberObject(value))
                return CreateNumber(value);
            else if (Util.IsEnumObject(value))
                return CreateEnum(value);
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
            if (Util.IsLongObject(value))
                return new ScriptNumberLong(this, (long)value);
            else if (Util.IsULongObject(value))
                return new ScriptNumberULong(this, (ulong)value);
            else if (Util.IsDoubleObject(value))
                return new ScriptNumberDouble(this, (double)value);
            return new ScriptNumberDouble(this, Convert.ToDouble(value));
        }
        public ScriptEnum CreateEnum(object value)
        {
            return new ScriptEnum(this, value);
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
