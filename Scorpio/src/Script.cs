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
namespace Scorpio {
    //脚本类
    public class Script {
        public const string DynamicDelegateName = "__DynamicDelegate__";
        public const BindingFlags BindingFlag = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
        private const string GLOBAL_TABLE = "_G";                   //全局table
        private const string GLOBAL_VERSION = "_VERSION";           //版本号
        private const string GLOBAL_SCRIPT = "_SCRIPT";             //Script对象
        private static readonly Encoding UTF8 = Encoding.UTF8;      //默认编码格式
        private ScriptTable m_GlobalTable;                                      //全局Table
        private Stack<StackInfo> m_StackInfoStack = new Stack<StackInfo>();     //堆栈数据
        private List<Assembly> m_Assembly = new List<Assembly>();               //所有代码集合
        private List<String> m_SearchPath = new List<String>();                 //request所有文件的路径集合
        private List<String> m_Defines = new List<String>();                    //所有Define
        private Dictionary<Type, IScorpioFastReflectClass> m_FastReflectClass = new Dictionary<Type, IScorpioFastReflectClass>();   //快速反射集合
        private Dictionary<Type, ScriptUserdataEnum> m_Enums = new Dictionary<Type, ScriptUserdataEnum>();                          //所有枚举集合
        private Dictionary<Type, ScriptUserdataDelegateType> m_Delegates = new Dictionary<Type, ScriptUserdataDelegateType>();      //所有委托类型集合
        private Dictionary<Type, ScriptUserdataObjectType> m_Types = new Dictionary<Type, ScriptUserdataObjectType>();              //所有的类集合
        private Dictionary<Type, UserdataType> m_UserdataTypes = new Dictionary<Type, UserdataType>();                              //所有的类集合
        private StackInfo m_StackInfo = new StackInfo();                        //最近堆栈数据
        private ScriptNull m_Null;                                              //null对象
        private ScriptBoolean m_True;                                           //true对象
        private ScriptBoolean m_False;                                          //false对象
        public ScriptNull Null { get { return m_Null; } }                       //null对象
        public ScriptBoolean True { get { return m_True; } }                    //true对象
        public ScriptBoolean False { get { return m_False; } }                  //false对象
        public Script() {
            m_Null = new ScriptNull(this);
            m_True = new ScriptBoolean(this, true);
            m_False = new ScriptBoolean(this, false);
            m_GlobalTable = CreateTable();
            m_GlobalTable.SetValue(GLOBAL_TABLE, m_GlobalTable);
            m_GlobalTable.SetValue(GLOBAL_VERSION, CreateObject(typeof(Version)));
            m_GlobalTable.SetValue(GLOBAL_SCRIPT, CreateObject(this));
            PushAssembly(typeof(object).GetTypeInfo().Assembly);                        //mscorlib.dll
            PushAssembly(typeof(System.Net.Sockets.Socket).GetTypeInfo().Assembly);     //System.dll
            PushAssembly(GetType().GetTypeInfo().Assembly);                             //当前所在的程序集
        }
        public void LoadLibrary() {
            LibraryBasis.Load(this);
            LibraryArray.Load(this);
            LibraryString.Load(this);
            LibraryTable.Load(this);
            LibraryJson.Load(this);
            LibraryMath.Load(this);
            LibraryFunc.Load(this);
            LibraryUserdata.Load(this);
        }
        public ScriptObject LoadFile(String strFileName) {
            return LoadFile(strFileName, UTF8);
        }
        public ScriptObject LoadFile(String fileName, Encoding encoding) {
            using (FileStream stream = File.OpenRead(fileName)) {
                long length = stream.Length;
                byte[] buffer = new byte[length];
                stream.Read(buffer, 0, buffer.Length);
                return LoadBuffer(fileName, buffer, encoding);
            }
        }
        public ScriptObject LoadBuffer(byte[] buffer) {
            return LoadBuffer("Undefined", buffer, UTF8);
        }
        public ScriptObject LoadBuffer(String strBreviary, byte[] buffer) {
            return LoadBuffer(strBreviary, buffer, UTF8);
        }
        public ScriptObject LoadBuffer(String strBreviary, byte[] buffer, Encoding encoding) {
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
        public ScriptObject LoadString(String strBuffer) {
            return LoadString("", strBuffer);
        }
        public ScriptObject LoadString(String strBreviary, String strBuffer) {
            return LoadString(strBreviary, strBuffer, null, true);
        }
        internal ScriptObject LoadString(String strBreviary, String strBuffer, ScriptContext context, bool clearStack) {
            try {
                if (Util.IsNullOrEmpty(strBuffer)) return m_Null;
                if (clearStack) m_StackInfoStack.Clear();
                ScriptLexer scriptLexer = new ScriptLexer(strBuffer, strBreviary);
                strBreviary = scriptLexer.GetBreviary();
                return Load(strBreviary, scriptLexer.GetTokens(), context);
            } catch (System.Exception e) {
                throw new ScriptException("load string [" + strBreviary + "] is error : " + e.ToString());
            }
        }
        public ScriptObject LoadTokens(List<Token> tokens) {
            return LoadTokens("Undefined", tokens);
        }
        public ScriptObject LoadTokens(String strBreviary, List<Token> tokens) {
            try {
                if (tokens.Count == 0) return m_Null;
                m_StackInfoStack.Clear();
                return Load(strBreviary, tokens, null);
            } catch (System.Exception e) {
                throw new ScriptException("load tokens [" + strBreviary + "] is error : " + e.ToString());
            }
        }
        private ScriptObject Load(String strBreviary, List<Token> tokens, ScriptContext context) {
            if (tokens.Count == 0) return m_Null;
            ScriptParser scriptParser = new ScriptParser(this, tokens, strBreviary);
            ScriptExecutable scriptExecutable = scriptParser.Parse();
            return new ScriptContext(this, scriptExecutable, context, Executable_Block.Context).Execute();
        }
        public void PushSearchPath(string path) {
            if (!m_SearchPath.Contains(path))
                m_SearchPath.Add(path);
        }
        public ScriptObject LoadSearchPathFile(String fileName) {
            for (int i = 0; i < m_SearchPath.Count; ++i) {
                string file = m_SearchPath[i] + "/" + fileName;
                if (File.Exists(file))
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
        public void PushAssembly(Assembly assembly) {
            if (assembly == null) return;
            if (!m_Assembly.Contains(assembly))
                m_Assembly.Add(assembly);
        }
        public Type GetType(string str) {
            for (int i = 0; i < m_Assembly.Count; ++i) {
                Type type = m_Assembly[i].GetType(str);
                if (type != null) return type;
            }
            return Type.GetType(str, false, false);
        }
        public ScriptObject LoadType(string str) {
            Type type = GetType(str);
            if (type == null) return m_Null;
            return CreateUserdata(type);
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
        internal void SetStackInfo(StackInfo info) {
            m_StackInfo = info;
        }
        public StackInfo GetCurrentStackInfo() {
            return m_StackInfo;
        }
        internal void PushStackInfo() {
            m_StackInfoStack.Push(m_StackInfo);
        }
        internal void PopStackInfo() {
            if (m_StackInfoStack.Count > 0)
                m_StackInfoStack.Pop();
        }
        public void ClearStackInfo() {
            m_StackInfoStack.Clear();
        }
        public string GetStackInfo() {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Source [" + m_StackInfo.Breviary + "] Line [" + m_StackInfo.Line + "]");
            foreach (StackInfo info in m_StackInfoStack) {
                builder.AppendLine("        Source [" + info.Breviary + "] Line [" + info.Line + "]");
            }
            return builder.ToString();
        }
        public ScriptTable GetGlobalTable() {
            return m_GlobalTable;
        }
        public bool HasValue(string key) {
            return m_GlobalTable.HasValue(key);
        }
        public ScriptObject GetValue(string key) {
            return m_GlobalTable.GetValue(key);
        }
        public void SetValue(string key, object value) {
            m_GlobalTable.SetValue(key, CreateObject(value));
        }
        public void SetObject(string key, object value) {
            m_GlobalTable.SetValue(key, CreateObject(value));
        }
        internal void SetObjectInternal(string key, ScriptObject value) {
            m_GlobalTable.SetValue(key, value);
        }
        public object Call(String strName, params object[] args) {
            ScriptObject obj = m_GlobalTable.GetValue(strName);
            if (obj is ScriptNull) throw new ScriptException("找不到变量[" + strName + "]");
            int length = args.Length;
            ScriptObject[] parameters = new ScriptObject[length];
            for (int i = 0; i < length; ++i) {
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
        public ScriptObject CreateObject(object value) {
            if (value == null)
                return m_Null;
            else if (value is bool)
                return CreateBool((bool)value);
            else if (value is string)
                return new ScriptString(this, (string)value);
            else if (value is long)
                return new ScriptNumberLong(this, (long)value);
            else if (value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is float || value is double || value is decimal)
                return new ScriptNumberDouble(this, Util.ToDouble(value));
            else if (value is ScriptObject)
                return (ScriptObject)value;
            else if (value is ScorpioFunction)
                return new ScriptDelegateFunction(this, (ScorpioFunction)value);
            else if (value is ScorpioHandle)
                return new ScriptHandleFunction(this, (ScorpioHandle)value);
            else if (value is ScorpioMethod)
                return new ScriptMethodFunction(this, (ScorpioMethod)value);
            else if (value.GetType().GetTypeInfo().IsEnum)
                return new ScriptEnum(this, value);
            return CreateUserdata(value);
        }
        public ScriptBoolean CreateBool(bool value) {
            return value ? True : False;
        }
        public ScriptString CreateString(string value) {
            return new ScriptString(this, value);
        }
        public ScriptNumber CreateDouble(double value) {
            return new ScriptNumberDouble(this, value);
        }
        public ScriptArray CreateArray() {
            return new ScriptArray(this);
        }
        public ScriptTable CreateTable() {
            return new ScriptTable(this);
        }
        public ScriptFunction CreateFunction(ScorpioHandle value) {
            return new ScriptHandleFunction(this, value);
        }
        public ScriptUserdata CreateUserdata(object obj) {
            Type type = obj as Type;
            if (type != null) {
                if (type.GetTypeInfo().IsEnum)
                    return GetEnum(type);
                else if (Util.IsDelegateType(type))
                    return GetDelegate(type);
                else
                    return GetType(type);
            }
            if (obj is Delegate)
                return new ScriptUserdataDelegate(this, (Delegate)obj);
            else if (obj is BridgeEventInfo)
                return new ScriptUserdataEventInfo(this, (BridgeEventInfo)obj);
            return new ScriptUserdataObject(this, obj, GetScorpioType(obj.GetType()));
        }
        public ScriptUserdata GetEnum(Type type) {
            if (m_Enums.ContainsKey(type))
                return m_Enums[type];
            ScriptUserdataEnum ret = new ScriptUserdataEnum(this, type);
            m_Enums.Add(type, ret);
            return ret;
        }
        public ScriptUserdata GetDelegate(Type type) {
            if (m_Delegates.ContainsKey(type))
                return m_Delegates[type];
            ScriptUserdataDelegateType ret = new ScriptUserdataDelegateType(this, type);
            m_Delegates.Add(type, ret);
            return ret;
        }
        public ScriptUserdataObjectType GetType(Type type) {
            if (m_Types.ContainsKey(type))
                return m_Types[type];
            ScriptUserdataObjectType ret = new ScriptUserdataObjectType(this, type, GetScorpioType(type));
            m_Types.Add(type, ret);
            return ret;
        }
        public UserdataType GetScorpioType(Type type) {
            if (m_UserdataTypes.ContainsKey(type))
                return m_UserdataTypes[type];
            UserdataType scorpioType = null;
            if (ContainsFastReflectClass(type)) {
                scorpioType = new FastReflectUserdataType(this, type, GetFastReflectClass(type));
            } else {
                scorpioType = new ReflectUserdataType(this, type);
            }
            m_UserdataTypes.Add(type, scorpioType);
            return scorpioType;
        }
        public void LoadExtension(string type) {
            LoadExtension(GetType(type));
        }
        public void LoadExtension(Type type) {
            if (type == null) return;
            if (!Util.IsExtensionType(type)) return;
            var methods = type.GetMethods(BindingFlag);
            foreach (var method in methods) {
                if (Util.IsExtensionMethod(method)) {
                    GetScorpioType(method.GetParameters()[0].ParameterType).AddExtensionMethod(method);
                }
            }
        }
    }
}
