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
using Scorpio.Serialize;
using Scorpio.Function;
namespace Scorpio
{
    //脚本类
    public class Script
    {
        public const string DynamicDelegateName = "__DynamicDelegate__";
        public const string Version = "master";
        public const BindingFlags BindingFlag = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
        private const string GLOBAL_TABLE = "_G";               //全局table
        private const string GLOBAL_VERSION = "_VERSION";       //版本号
        private const string GLOBAL_SCRIPT = "_SCRIPT";         //Script对象
        private IScriptUserdataFactory m_UserdataFactory = null;                //Userdata工厂
        private ScriptTable m_GlobalTable;                                      //全局Table
        private List<StackInfo> m_StackInfoStack = new List<StackInfo>();       //堆栈数据
        private List<Assembly> m_Assembly = new List<Assembly>();               //所有代码集合
        private List<String> m_SearchPath = new List<String>();                 //request所有文件的路径集合
        private List<String> m_Defines = new List<String>();                    //所有Define
        private Dictionary<Type, IScorpioFastReflectClass> m_FastReflectClass = new Dictionary<Type, IScorpioFastReflectClass>();
        private StackInfo m_StackInfo = new StackInfo();                        //最近堆栈数据
        private ScriptNull m_Null;                                              //null对象
        private ScriptBoolean m_True;                                           //true对象
        private ScriptBoolean m_False;                                          //false对象
        public ScriptNull Null { get { return m_Null; } }                       //null对象
        public ScriptBoolean True { get { return m_True; } }                    //true对象
        public ScriptBoolean False { get { return m_False; } }                  //false对象
        public ScriptBoolean GetBoolean(bool value) { return value ? True : False;  }
        public Script() {
            m_Null = new ScriptNull(this);
            m_True = new ScriptBoolean(this, true);
            m_False = new ScriptBoolean(this, false);
            m_UserdataFactory = new DefaultScriptUserdataFactory(this);
            m_GlobalTable = CreateTable();
            m_GlobalTable.SetValue(GLOBAL_TABLE, m_GlobalTable);
            m_GlobalTable.SetValue(GLOBAL_VERSION, CreateString(Version));
            m_GlobalTable.SetValue(GLOBAL_SCRIPT, CreateObject(this));
            PushAssembly(ScriptExtensions.GetAssembly(typeof(object)));
            PushAssembly(ScriptExtensions.GetAssembly(GetType()));
        }
        public ScriptObject LoadFile(String strFileName)
        {
            return LoadFile(strFileName, Encoding.UTF8);
        }
        public ScriptObject LoadFile(String fileName, Encoding encoding)
        {
            return LoadBuffer(fileName, ScriptExtensions.GetFileBuffer(fileName), encoding);
        }
        public ScriptObject LoadBuffer(byte[] buffer)
        {
            return LoadBuffer("Undefined", buffer, Encoding.UTF8);
        }
        public ScriptObject LoadBuffer(String strBreviary, byte[] buffer)
        {
            return LoadBuffer(strBreviary, buffer, Encoding.UTF8);
        }
        public ScriptObject LoadBuffer(String strBreviary, byte[] buffer, Encoding encoding)
        {
            if (buffer == null || buffer.Length == 0) { return null; }
            try {
                if (buffer[0] == 0) {
                    return LoadTokens(strBreviary, ScorpioMaker.Deserialize(buffer));
                } else {
                    return LoadString(strBreviary, encoding.GetString(buffer, 0, buffer.Length));
                }
            } catch (System.Exception e) {
                throw new ScriptException("load buffer [" + strBreviary + "] is error : " + e.ToString());
            }
        }
        public ScriptObject LoadString(String strBuffer)
        {
            return LoadString("", strBuffer);
        }
        public ScriptObject LoadString(String strBreviary, String strBuffer)
        {
            return LoadString(strBreviary, strBuffer, null, true);
        }
        internal ScriptObject LoadString(String strBreviary, String strBuffer, ScriptContext context, bool clearStack)
        {
            try {
                if (Util.IsNullOrEmpty(strBuffer)) return m_Null;
                if (clearStack) m_StackInfoStack.Clear();
                ScriptLexer scriptLexer = new ScriptLexer(strBuffer, strBreviary);
				return Load(scriptLexer.GetBreviary(), scriptLexer.GetTokens(), context);
            } catch (System.Exception e) {
                throw new ScriptException("load buffer [" + strBreviary + "] is error : " + e.ToString());
            }
        }
        public ScriptObject LoadTokens(List<Token> tokens)
        {
            return LoadTokens("Undefined", tokens);
        }
        public ScriptObject LoadTokens(String strBreviary, List<Token> tokens)
        {
            try {
                if (tokens.Count == 0) return m_Null;
                m_StackInfoStack.Clear();
                return Load(strBreviary, tokens, null);
            } catch (System.Exception e) {
                throw new ScriptException("load tokens [" + strBreviary + "] is error : " + e.ToString());
            }
        }
        private ScriptObject Load(String strBreviary, List<Token> tokens, ScriptContext context)
        {
            if (tokens.Count == 0) return m_Null;
            ScriptParser scriptParser = new ScriptParser(this, tokens, strBreviary);
            ScriptExecutable scriptExecutable = scriptParser.Parse();
            return new ScriptContext(this, scriptExecutable, context, Executable_Block.Context).Execute();
        }
        public void PushSearchPath(string path)
        {
            if (!m_SearchPath.Contains(path))
                m_SearchPath.Add(path);
        }
        public ScriptObject LoadSearchPathFile(String fileName)
        {
            for (int i = 0; i < m_SearchPath.Count; ++i) {
                string file = m_SearchPath[i] + "/" + fileName;
                if (ScriptExtensions.FileExist(file))
                    return LoadFile(file);
            }
            throw new ExecutionException(this, "require 找不到文件 : " + fileName);
        }
        public void PushDefine(string define) {
            if (!m_Defines.Contains(define))
                m_Defines.Add(define);
        }
        public bool ContainDefine(string define) {
            return m_Defines.Contains(define);
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
                Type type = m_Assembly[i].GetType(str);
                if (type != null) return CreateUserdata(type);
            }
            {
                Type type = Type.GetType(str, false, false);
                if (type != null) return CreateUserdata(type);
            }
            return m_Null;
        }
        public void PushFastReflectClass(Type type, IScorpioFastReflectClass value) {
            m_FastReflectClass[type] = value;
        }
        public bool ContainsFastReflectClass(Type type) {
            return m_FastReflectClass.ContainsKey(type);
        }
        public IScorpioFastReflectClass GetFastReflectClass(Type type) {
            return m_FastReflectClass[type];
        }
        internal void SetStackInfo(StackInfo info)
        {
            m_StackInfo = info;
        }
        public StackInfo GetCurrentStackInfo()
        {
            return m_StackInfo;
        }
        internal void PushStackInfo()
        {
            m_StackInfoStack.Add(m_StackInfo);
        }
        public void ClearStackInfo()
        {
            m_StackInfoStack.Clear();
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
        public ScriptTable GetGlobalTable()
        {
            return m_GlobalTable;
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
        public object Call(String strName, params object[] args)
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
		public object Call(String strName, ScriptObject[] args) {
			ScriptObject obj = m_GlobalTable.GetValue(strName);
			if (obj is ScriptNull) throw new ScriptException("找不到变量[" + strName + "]");
			m_StackInfoStack.Clear();
			return obj.Call(args);
		}
        public ScriptObject CreateObject(object value)
        {
            if (value == null)
                return m_Null;
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
            return GetBoolean(value);
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
        public ScriptNumber CreateInt(int value)
        {
            return new ScriptNumberInt(this, value);
        }
        public ScriptEnum CreateEnum(object value)
        {
            return new ScriptEnum(this, value);
        }
        public ScriptUserdata CreateUserdata(object value)
        {
            return m_UserdataFactory.create(this, value);
        }
        public ScriptArray CreateArray()
        {
            return new ScriptArray(this);
        }
        public ScriptTable CreateTable()
        {
            return new ScriptTable(this);
        }
        internal ScriptScriptFunction CreateFunction(string name, ScorpioScriptFunction value)
        {
            return new ScriptScriptFunction(this, name, value);
        }
        public ScriptFunction CreateFunction(ScorpioFunction value)
        {
            return new ScriptDelegateFunction(this, value);
        }
        public ScriptFunction CreateFunction(ScorpioHandle value)
        {
            return new ScriptHandleFunction(this, value);
        }
        public ScriptFunction CreateFunction(ScorpioMethod value)
        {
            return new ScriptMethodFunction(this, value);
        }
        public IScriptUserdataFactory GetUserdataFactory() {
            return m_UserdataFactory;
        }
        public void SetUserdataFactory(IScriptUserdataFactory value) {
            m_UserdataFactory = value;
        }
        public void LoadLibrary() {
            LibraryBasis.Load(this);
            LibraryArray.Load(this);
            LibraryString.Load(this);
            LibraryTable.Load(this);
            LibraryJson.Load(this);
            LibraryMath.Load(this);
            LibraryFunc.Load(this);
        }
    }
}
