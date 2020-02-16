using System.IO;
using System.Text;
using System.Reflection;
using Scorpio.Instruction;
using Scorpio.Exception;
using System.Collections.Generic;
using Scorpio.Function;
using Scorpio.Library;
using Scorpio.Runtime;
using Scorpio.Proto;
using Scorpio.Userdata;
using Scorpio.Serialize;
namespace Scorpio {
    public partial class Script {
        /// <summary> 反射获取变量和函数的属性 </summary>
        public const BindingFlags BindingFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
        /// <summary> 文本默认编码格式 </summary>
        public static readonly Encoding UTF8 = Encoding.UTF8;
        private const string Undefined = "Undefined";                   //Undefined
        private const string GLOBAL_NAME = "_G";                        //全局对象
        private const string GLOBAL_SCRIPT = "_SCRIPT";                 //Script对象
        private const string GLOBAL_VERSION = "_VERSION";               //版本号
        private List<string> m_SearchPath = new List<string>();         //request所有文件的路径集合

        /// <summary> 所有类型的基类 </summary>
        public ScriptType TypeObject { get; private set; }
        /// <summary> 所有类型的基类 </summary>
        public ScriptValue TypeObjectValue { get; private set; }

        /// <summary> bool类型的原表 </summary>
        public ScriptType TypeBoolean { get; private set; }
        /// <summary> bool类型的原表 </summary>
        public ScriptValue TypeBooleanValue { get; private set; }

        /// <summary> number类型的原表 </summary>
        public ScriptType TypeNumber { get; private set; }
        /// <summary> number类型的原表 </summary>
        public ScriptValue TypeNumberValue { get; private set; }

        /// <summary> string类型的原表 </summary>
        public ScriptType TypeString { get; private set; }
        /// <summary> string类型的原表 </summary>
        public ScriptValue TypeStringValue { get; private set; }

        /// <summary> array类型的原表 </summary>
        public ScriptType TypeArray { get; private set; }
        /// <summary> array类型的原表 </summary>
        public ScriptValue TypeArrayValue { get; private set; }

        /// <summary> map类型的原表 </summary>
        public ScriptType TypeMap { get; private set; }
        /// <summary> map类型的原表 </summary>
        public ScriptValue TypeMapValue { get; private set; }

        /// <summary> function类型的原表 </summary>
        public ScriptType TypeFunction { get; private set; }
        /// <summary> function类型的原表 </summary>
        public ScriptValue TypeFunctionValue { get; private set; }

        /// <summary> 全局变量 </summary>
        public ScriptGlobal Global { get; private set; }


        public Script() {
            Global = new ScriptGlobal();
            
            TypeObject = new ScriptTypeObject("Object");
            TypeObjectValue = new ScriptValue(TypeObject);
            Global.SetValue(TypeObject.TypeName, TypeObjectValue);

            TypeBoolean = new ScriptBasicType("Bool", TypeObjectValue);
            TypeBooleanValue = new ScriptValue(TypeBoolean);
            Global.SetValue(TypeBoolean.TypeName, TypeBooleanValue);

            TypeNumber = new ScriptBasicType("Number", TypeObjectValue);
            TypeNumberValue = new ScriptValue(TypeNumber);
            Global.SetValue(TypeNumber.TypeName, TypeNumberValue);

            TypeString = new ScriptBasicType("String", TypeObjectValue);
            TypeStringValue = new ScriptValue(TypeString);
            Global.SetValue(TypeString.TypeName, TypeStringValue);

            TypeArray = new ScriptBasicType("Array", TypeObjectValue);
            TypeArrayValue = new ScriptValue(TypeArray);
            Global.SetValue(TypeArray.TypeName, TypeArrayValue);

            TypeMap = new ScriptBasicType("Map", TypeObjectValue);
            TypeMapValue = new ScriptValue(TypeMap);
            Global.SetValue(TypeMap.TypeName, TypeMapValue);

            TypeFunction = new ScriptBasicType("Function", TypeObjectValue);
            TypeFunctionValue = new ScriptValue(TypeFunction);
            Global.SetValue(TypeFunction.TypeName, TypeFunctionValue);

            Global.SetValue(GLOBAL_NAME, new ScriptValue(Global));
            Global.SetValue(GLOBAL_SCRIPT, ScriptValue.CreateValue(this));
            Global.SetValue(GLOBAL_VERSION, ScriptValue.CreateValue(typeof(Version)));

            ProtoObject.Load(this, TypeObject);
            ProtoBoolean.Load(this, TypeBoolean);
            ProtoNumber.Load(this, TypeNumber);
            ProtoString.Load(this, TypeString);
            ProtoArray.Load(this, TypeArray);
            ProtoMap.Load(this, TypeMap);
            ProtoFunction.Load(this, TypeFunction);

            TypeManager.PushAssembly(typeof(object).Assembly);                        //mscorlib.dll
            TypeManager.PushAssembly(typeof(System.Net.Sockets.Socket).Assembly);     //System.dll
            TypeManager.PushAssembly(GetType().Assembly);                             //当前所在的程序集

            LibraryBasis.Load(this);
            LibraryJson.Load(this);
            LibraryMath.Load(this);
            LibraryUserdata.Load(this);
            LibraryIO.Load(this);
        }
        ScriptValue Execute(string breviary, SerializeData data) {
            var contexts = new ScriptContext[data.Functions.Length];
            for (int i = 0; i < data.Functions.Length; ++i) {
                contexts[i] = new ScriptContext(this, breviary, data.Functions[i], data.ConstDouble, data.ConstLong, data.ConstString, contexts, data.Classes);
            }
            return new ScriptContext(this, breviary, data.Context, data.ConstDouble, data.ConstLong, data.ConstString, contexts, data.Classes).Execute(ScriptValue.Null, null, 0, null);
        }
        /// <summary> 压入一个搜索路径,使用 require 时会搜索此路径 </summary>
        /// <param name="path">绝对路径</param>
        public void PushSearchPath(string path) {
            if (!m_SearchPath.Contains(path))
                m_SearchPath.Add(path);
        }
        public ScriptValue LoadSearchPathFile(string fileName) {
            for (int i = 0; i < m_SearchPath.Count; ++i) {
                string file = m_SearchPath[i] + "/" + fileName;
                if (File.Exists(file))
                    return LoadFile(file);
            }
            throw new ExecutionException($"require 找不到文件 : {fileName}");
        }
        /// <summary> 设置一个全局变量 </summary>
        /// <param name="key">名字</param>
        /// <param name="value">值</param>
        public void SetGlobal(string key, ScriptValue value) {
            Global.SetValue(key, value);
        }
        /// <summary> 获得一个全局变量 </summary>
        /// <param name="key">名字</param>
        /// <returns>值</returns>
        public ScriptValue GetGlobal(string key) {
            return Global.GetValue(key);
        }
        /// <summary> 是否包含一个全局变量 </summary>
        /// <param name="key">名字</param>
        /// <returns>是否包含</returns>
        public bool HasGlobal(string key) {
            return Global.HasValue(key);
        }
        /// <summary> 创建一个空的array </summary>
        public ScriptArray CreateArray() { return new ScriptArray(this); }
        /// <summary> 创建一个空的map </summary>
        public ScriptMap CreateMap() { return new ScriptMap(this); }
        /// <summary> 创建一个类 </summary>
        /// <param name="typeName">类名</param>
        /// <param name="parentType">类数据</param>
        public ScriptType CreateType(string typeName, ScriptValue parentType) { return new ScriptType(typeName, parentType); }
        /// <summary> 创建一个Function </summary>
        /// <param name="value">ScorpioHandle</param>
        public ScriptValue CreateFunction(ScorpioHandle value) { return new ScriptValue(new ScriptHandleFunction(this, value)); }

        /// <summary> 调用一个全局函数 </summary>
        /// <param name="name">函数名</param>
        /// <param name="args">参数</param>
        /// <returns>函数返回值</returns>
        public ScriptValue call(string name, params object[] args) {
            return Global.GetValue(name).call(ScriptValue.Null, args);
        }
        /// <summary> 调用一个全局函数 </summary>
        /// <param name="name">函数名</param>
        /// <param name="args">参数</param>
        /// <param name="length">参数个数</param>
        /// <returns>函数返回值</returns>
        public ScriptValue Call(string name, ScriptValue[] args, int length) {
            return Global.GetValue(name).Call(ScriptValue.Null, args, length);
        }
        /// <summary> 加载一个文件 </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns>返回值</returns>
        public ScriptValue LoadFile(string fileName) {
            return LoadFile(fileName, UTF8);
        }
        /// <summary> 加载一个文件 </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="encoding">文件编码</param>
        /// <returns>返回值</returns>
        public ScriptValue LoadFile(string fileName, Encoding encoding) {
            using (var stream = File.OpenRead(fileName)) {
                var length = stream.Length;
                var buffer = new byte[length];
                stream.Read(buffer, 0, buffer.Length);
                return LoadBuffer(fileName, buffer, encoding);
            }
        }
        /// <summary> 加载一段文本 </summary>
        /// <param name="buffer">文本内容</param>
        /// <returns>返回值</returns>
        public ScriptValue LoadString(string buffer) {
            return LoadString(Undefined, buffer);
        }
        /// <summary> 加载一段文本 </summary>
        /// <param name="breviary">摘要</param>
        /// <param name="buffer">文本内容</param>
        /// <returns>返回值</returns>
        public ScriptValue LoadString(string breviary, string buffer) {
            if (buffer == null || buffer.Length == 0) { return ScriptValue.Null; }
            return Execute(breviary, Serializer.Serialize(breviary, buffer, null));
        }
        /// <summary> 加载一段数据 </summary>
        /// <param name="buffer">数据内容</param>
        /// <returns>返回值</returns>
        public ScriptValue LoadBuffer(byte[] buffer) {
            return LoadBuffer(Undefined, buffer, UTF8);
        }
        /// <summary> 加载一段数据 </summary>
        /// <param name="breviary">摘要</param>
        /// <param name="buffer">数据内容</param>
        /// <returns>返回值</returns>
        public ScriptValue LoadBuffer(string breviary, byte[] buffer) {
            return LoadBuffer(breviary, buffer, UTF8);
        }
        /// <summary> 加载一段数据 </summary>
        /// <param name="breviary">摘要</param>
        /// <param name="buffer">数据内容</param>
        /// <param name="encoding">如果数据内容时文本时,文本的编码类型</param>
        /// <returns>返回值</returns>
        public ScriptValue LoadBuffer(string breviary, byte[] buffer, Encoding encoding) {
            if (buffer == null || buffer.Length == 0) { return ScriptValue.Null; }
            if (buffer[0] == 0)
                return Execute(breviary, Deserializer.Deserialize(buffer));
            else
                return Execute(breviary, Serializer.Serialize(breviary, encoding.GetString(buffer, 0, buffer.Length), null));
        }


#if SCORPIO_DEBUG
        private Stack<StackInfo> m_StackInfos = new Stack<StackInfo>(); //堆栈信息
        internal void PushStackInfo(string breviary, int line) {
            m_StackInfos.Push(new StackInfo() { Breviary = breviary, Line = line });
        }
        internal void PopStackInfo() {
            m_StackInfos.Pop();
        }
        /// <summary> 最近的堆栈调用 </summary>
        public StackInfo GetStackInfo() {
            return m_StackInfos.Count > 0 ? m_StackInfos.Peek() : default;
        }
        /// <summary> 调用堆栈 </summary>
        public StackInfo[] GetStackInfos() {
            return m_StackInfos.ToArray();
        }
#else
        private readonly static StackInfo[] EmptyStackInfos = new StackInfo[0];
        /// <summary> 最近的堆栈调用 </summary>
        public StackInfo GetStackInfo() {
            return default;
        }
        /// <summary> 调用堆栈 </summary>
        public StackInfo[] GetStackInfos() {
            return EmptyStackInfos;
        }
#endif
    }
}
