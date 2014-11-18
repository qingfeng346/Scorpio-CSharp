using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
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
        public const string DynamicDelegateName = "__DynamicDelegate__";
        public const string Version = "0.0.6beta";
        public const BindingFlags BindingFlag = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
        private const string GLOBAL_TABLE = "_G";               //全局table
        private const string GLOBAL_VERSION = "_VERSION";       //版本号
        private IScriptUserdataFactory m_UserdataFactory = null;                //Userdata工厂
        private ScriptTable m_GlobalTable;                                      //全局Table
        private List<StackInfo> m_StackInfoStack = new List<StackInfo>();       //堆栈数据
        private List<Assembly> m_Assembly = new List<Assembly>();               //所有代码集合
        private Dictionary<Type, UserdataType> m_Types = new Dictionary<Type, UserdataType>();    //所有的类集合
        private StackInfo m_StackInfo = new StackInfo();                        //最近堆栈数据
        public ScriptObject LoadFile(String strFileName)
        {
            return LoadFile(strFileName, Encoding.UTF8);
        }
        public ScriptObject LoadFile(String fileName, string encoding)
        {
            return LoadFile(fileName, Encoding.GetEncoding(encoding));
        }
        public ScriptObject LoadFile(String fileName, Encoding encoding)
        {
            try {
                return LoadString(fileName, Util.GetFileString(fileName, encoding));
            } catch (System.Exception e) {
                throw new ScriptException("load file [" + fileName + "] is error : " + e.ToString());
            }
        }
        public ScriptObject LoadString(String strBuffer)
        {
            return LoadString("", strBuffer);
        }
        public ScriptObject LoadString(String strBreviary, String strBuffer)
        {
            return LoadString(strBreviary, strBuffer, null);
        }
        internal ScriptObject LoadString(String strBreviary, String strBuffer, ScriptContext context)
        {
            try {
                m_StackInfoStack.Clear();
                ScriptLexer scriptLexer = new ScriptLexer(strBuffer);
                strBreviary = Util.IsNullOrEmpty(strBreviary) ? scriptLexer.GetBreviary() : strBreviary;
                ScriptParser scriptParser = new ScriptParser(this, scriptLexer.GetTokens(), strBreviary);
                ScriptExecutable scriptExecutable = scriptParser.Parse();
                return new ScriptContext(this, scriptExecutable, context, Executable_Block.Context).Execute();
            } catch (System.Exception e) {
                throw new ScriptException("load buffer [" + strBreviary + "] is error : " + e.ToString());
            }
        }
        public void PushAssembly(Assembly assembly)
        {
            if (assembly == null) return;
            if (!m_Assembly.Contains(assembly))
                m_Assembly.Add(assembly);
        }
        public ScriptObject LoadType(string str)
        {
            for (int i = 0; i < m_Assembly.Count;++i )
            {
                Type type = m_Assembly[i].GetType(str, false);
                if (type != null) return CreateUserdata(type);
            }
            {
                Type type = Type.GetType(str, false);
                if (type != null) return CreateUserdata(type);
            }
            return ScriptNull.Instance;
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
            return m_GlobalTable.HasValue(key);
        }
        public ScriptObject GetValue(string key)
        {
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
            return new ScriptString(this, value);
        }
        public ScriptNumber CreateNumber(object value)
        {
            return Util.IsLongObject(value) ? CreateLong((long)value) : CreateDouble(Util.ToDouble(value));
        }
        public ScriptNumber CreateDouble(double value)
        {
            return new ScriptNumberDouble(this, value);
        }
        public ScriptNumber CreateLong(long value)
        {
            return new ScriptNumberLong(this, value);
        }
        public ScriptEnum CreateEnum(object value)
        {
            return new ScriptEnum(this, value);
        }
        public ScriptUserdata CreateUserdata(object value)
        {
            return m_UserdataFactory.create(this, value);
        }
        internal ScriptArray CreateArray()
        {
            return new ScriptArray(this);
        }
        internal ScriptTable CreateTable()
        {
            return new ScriptTable(this);
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
        public ScriptFunction CreateFunction(ScorpioMethod value)
        {
            return new ScriptFunction(this, value);
        }
        internal UserdataType GetScorpioType(Type type)
        {
            if (m_Types.ContainsKey(type))
                return m_Types[type];
            UserdataType scorpioType = new UserdataType(this, type);
            m_Types.Add(type, scorpioType);
            return scorpioType;
        }
        public void LoadLibrary()
        {
            m_UserdataFactory = new DefaultScriptUserdataFactory();
            m_GlobalTable = CreateTable();
            m_GlobalTable.SetValue(GLOBAL_TABLE, m_GlobalTable);
            m_GlobalTable.SetValue(GLOBAL_VERSION, CreateString(Version));
            PushAssembly(Util.MSCORLIB_ASSEMBLY);
            PushAssembly(GetType().Assembly);
            LibraryBasis.Load(this);
            LibraryArray.Load(this);
            LibraryString.Load(this);
            LibraryTable.Load(this);
        }
    }
}
