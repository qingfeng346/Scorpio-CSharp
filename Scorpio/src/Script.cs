using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Scorpio.Instruction;
using Scorpio.Exception;
using Scorpio.Function;
using Scorpio.Library;
using Scorpio.Runtime;
using Scorpio.Proto;
using Scorpio.Serialize;
using Scorpio.Tools;
using Scorpio.Compile.Compiler;
using System.Runtime.CompilerServices;

namespace Scorpio {
    public partial class Script {
        /// <summary> 反射获取变量和函数的属性 </summary>
        public const BindingFlags BindingFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
        public const int StringStage = 256;
        private const string GLOBAL_NAME = "_G";                        //全局对象
        private const string GLOBAL_SCRIPT = "_SCRIPT";                 //Script对象
        private const string GLOBAL_VERSION = "_VERSION";               //版本号
        private const string GLOBAL_ARGS = "_ARGS";                     //命令行参数
        /// <summary> 按文本读取时,文本文件的编码 </summary>
        public static Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary> request文件的搜索路径集合 </summary>
        private string[] m_SearchPaths;
        public string[] SearchPaths => m_SearchPaths;
        /// <summary> 所有类型的基类 </summary>
        /// <summary> 所有类型的基类 </summary>
        private ScriptType m_TypeObject;
        private ScriptValue m_TypeObjectValue;
        public ScriptType TypeObject => m_TypeObject;
        public ScriptValue TypeObjectValue => m_TypeObjectValue;

        /// <summary> 所有基础类型数据 </summary>
        private ScriptType m_TypeBool, m_TypeNumber, m_TypeString, m_TypeArray, m_TypeMap, m_TypeFunction, m_TypeStringBuilder, m_TypeHashSet;
        private ScriptValue m_TypeValueBool, m_TypeValueNumber, m_TypeValueString, m_TypeValueArray, m_TypeValueMap, m_TypeValueFunction, m_TypeValueStringBuilder, m_TypeValueHashSet;
        /// <summary> bool类型的原表 </summary>
        public ScriptType TypeBoolean => m_TypeBool;
        public ScriptValue TypeBooleanValue => m_TypeValueBool;

        /// <summary> number类型的原表 </summary>
        public ScriptType TypeNumber => m_TypeNumber;
        public ScriptValue TypeNumberValue => m_TypeValueNumber;

        /// <summary> string类型的原表 </summary>
        public ScriptType TypeString => m_TypeString;
        public ScriptValue TypeStringValue => m_TypeValueString;

        /// <summary> array类型的原表 </summary>
        public ScriptType TypeArray => m_TypeArray;
        public ScriptValue TypeArrayValue => m_TypeValueArray;

        /// <summary> map类型的原表 </summary>
        public ScriptType TypeMap => m_TypeMap;
        public ScriptValue TypeMapValue => m_TypeValueMap;

        /// <summary>  </summary>
        public ScriptType TypeHashSet => m_TypeHashSet;
        public ScriptValue TypeHashSetValue => m_TypeValueHashSet;

        /// <summary> function类型的原表 </summary>
        public ScriptType TypeFunction => m_TypeFunction;
        public ScriptValue TypeFunctionValue => m_TypeValueFunction;

        /// <summary> stringBuilder类型的原表 </summary>
        public ScriptType TypeStringBuilder => m_TypeStringBuilder;
        public ScriptValue TypeStringBuilderValue => m_TypeValueStringBuilder;

        private ScriptGlobal m_Global;
        /// <summary> 全局变量 </summary>
        public ScriptGlobal Global => m_Global;
        public int MainThreadId { get; private set; }
        private ScorpioJsonSerializer m_JsonSerializer;
        private ScorpioJsonDeserializer m_JsonDeserializer;
        public bool IsShutdown { get; private set; }
        //private int ConstStringIndex = 0;
        //public string[] ConstString;
        //public Dictionary<string, int> StringIndex;
        public Script() {
            m_SearchPaths = new string[0];
            m_Global = new ScriptGlobal();
            m_JsonSerializer = new ScorpioJsonSerializer();
            m_JsonDeserializer = new ScorpioJsonDeserializer(this);
            m_TypeObject = new ScriptTypeObject(this, "Object");
            m_TypeObjectValue = new ScriptValue(TypeObject);
            m_Global.SetValue(m_TypeObject.TypeName, m_TypeObjectValue);

            AddPrimitivePrototype("Bool", ref m_TypeBool, ref m_TypeValueBool);
            AddPrimitivePrototype("Number", ref m_TypeNumber, ref m_TypeValueNumber);
            AddPrimitivePrototype("String", ref m_TypeString, ref m_TypeValueString);
            AddPrimitivePrototype("Function", ref m_TypeFunction, ref m_TypeValueFunction);

            AddBasicPrototype(m_TypeArray = new ScriptTypeBasicArray(this, "Array", m_TypeObject), ref m_TypeValueArray);
            AddBasicPrototype(m_TypeMap = new ScriptTypeBasicMap(this, "Map", m_TypeObject), ref m_TypeValueMap);
            AddBasicPrototype(m_TypeStringBuilder = new ScriptTypeBasicStringBuilder(this, "StringBuilder", m_TypeObject), ref m_TypeValueStringBuilder);
            AddBasicPrototype(m_TypeHashSet = new ScriptTypeBasicHashSet(this, "HashSet", m_TypeObject), ref m_TypeValueHashSet);

            m_Global.SetValue(GLOBAL_NAME, new ScriptValue(m_Global));
            m_Global.SetValue(GLOBAL_SCRIPT, ScriptValue.CreateValue(this));
            m_Global.SetValue(GLOBAL_VERSION, ScriptValue.CreateValue(typeof(Version)));

            ProtoObject.Load(this, TypeObject);
            ProtoBoolean.Load(this, TypeBoolean);
            ProtoNumber.Load(this, TypeNumber);
            ProtoString.Load(this, TypeString);
            ProtoArray.Load(this, TypeArray);
            ProtoMap.Load(this, TypeMap);
            ProtoFunction.Load(this, TypeFunction);
            ProtoStringBuilder.Load(this, TypeStringBuilder);
            ProtoHashSet.Load(this, TypeHashSet);

            PushAssembly(typeof(object));                        //mscorlib.dll
            PushAssembly(typeof(System.Net.Sockets.Socket));     //System.dll
            PushAssembly(GetType());                             //当前所在的程序集

            LibraryBasis.Load(this);
            LibraryType.Load(this);
            LibraryJson.Load(this);
            LibraryMath.Load(this);
            LibraryUserdata.Load(this);
            LibraryIO.Load(this);
            LibraryCoroutine.Load(this);
            MainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            //ConstString = new string[StringStage];
            //StringIndex = new Dictionary<string, int>();
        }
        void AddPrimitivePrototype(string name, ref ScriptType type, ref ScriptValue typeValue) {
            type = new ScriptTypePrimitive(name, m_TypeObject, this);
            typeValue = new ScriptValue(type);
            Global.SetValue(name, typeValue);
        }
        void AddBasicPrototype(ScriptType type, ref ScriptValue typeValue) {
            typeValue = new ScriptValue(type);
            m_Global.SetValue(type.TypeName, typeValue);
        }
        public ScriptMap AddLibrary(string libraryName, (string, ScorpioHandle)[] functions, int number = 0) {
            var map = new ScriptMapStringPolling(this, functions.Length + number);
            foreach (var (name, func) in functions) {
                map.SetValue(name, CreateFunction(func));
            }
            SetGlobal(libraryName, new ScriptValue(map));
            return map;
        }
        public void Shutdown() {
            if (IsShutdown) return;
            IsShutdown = true;
            m_Global.Shutdown();
            m_TypeObject = m_TypeBool = m_TypeNumber = m_TypeString = m_TypeArray = m_TypeMap = m_TypeFunction = m_TypeStringBuilder = null;
            m_TypeObjectValue = m_TypeValueBool = m_TypeValueNumber = m_TypeValueString = m_TypeValueArray = m_TypeValueMap = m_TypeValueFunction = m_TypeValueStringBuilder = default;
        }
        public void ClearStack() {
            Array.Clear(ScorpioUtil.Parameters, 0, ScorpioUtil.Parameters.Length);
            if (ScriptContext.MaxVariableValueIndex > ScriptContext.VariableValueIndex) {
                for (var i = ScriptContext.VariableValueIndex; i < ScriptContext.MaxVariableValueIndex; ++i) {
                    Array.Clear(ScriptContext.VariableValues[i], 0, ScriptContext.VariableValues[i].Length);
                    Array.Clear(ScriptContext.StackValues[i], 0, ScriptContext.StackValues[i].Length);
                }
            }
            if (ScriptContext.AsyncValuePoolLength > ScriptContext.MinAsyncValueIndex) {
                for (var i = ScriptContext.MinAsyncValueIndex; i < ScriptContext.AsyncValuePoolLength; ++i) {
                    Array.Clear(ScriptContext.AsyncValuePool[i].variable, 0, ScriptContext.AsyncValuePool[i].variable.Length);
                    Array.Clear(ScriptContext.AsyncValuePool[i].stack, 0, ScriptContext.AsyncValuePool[i].stack.Length);
                }
            }
            ScriptContext.MaxVariableValueIndex = 0;
            ScriptContext.MinAsyncValueIndex = int.MaxValue;
        }
        /// <summary> 压入一个搜索路径,使用 require 时会搜索此路径 </summary>
        /// <param name="path">绝对路径</param>
        public void PushSearchPath(string path) {
            if (!Array.Exists(m_SearchPaths, _ => _ == path)) {
                Array.Resize(ref m_SearchPaths, m_SearchPaths.Length + 1);
                m_SearchPaths[m_SearchPaths.Length - 1] = path;
            }
        }
        public string SearchFile(string fileName, CompileOption option) {
            if (File.Exists(fileName)) { 
                return fileName;
            }
            for (int i = 0; i < m_SearchPaths.Length; ++i) {
                string file = Path.Combine(m_SearchPaths[i], fileName);
                if (File.Exists(file)) {
                    return file;
                }
            }
            if (option != null) {
                foreach (var path in option.searchPaths) {
                    string file = Path.Combine(path, fileName);
                    if (File.Exists(file)) {
                        return file;
                    }
                }
            }
            return null;
        }
        /// <summary> 压入程序集 </summary>
        public void PushReferencedAssemblies(Assembly assembly) {
            foreach (var assemblyName in assembly.GetReferencedAssemblies ()) {
                PushAssembly (Assembly.Load (assemblyName));
            }
        }
        /// <summary> 压入程序集 </summary>
        public void PushAssembly(Type type) {
            PushAssembly(Assembly.GetAssembly(type));
        }
        /// <summary> 压入程序集 </summary>
        public void PushAssembly(AssemblyName assemblyName) {
            PushAssembly(Assembly.Load(assemblyName));
        }
        /// <summary> 压入程序集 </summary>
        public void PushAssembly(Assembly assembly) {
            ScorpioTypeManager.PushAssembly(assembly);
        }
        /// <summary> 设置函数指针仓库 </summary>
        public void SetDelegateFactory(IScorpioDelegateFactory factory) {
            ScorpioDelegateFactoryManager.SetFactory(factory);
        }
        /// <summary> 设置快速反射类 </summary>
        public void SetFastReflectClass(Type type, IScorpioFastReflectClass fastReflectClass) {
            ScorpioTypeManager.SetFastReflectClass(type, fastReflectClass);
        }
        /// <summary> 关联扩展函数 </summary>
        public void LoadExtension(Type type) {
            ScorpioTypeManager.LoadExtension(type);
        }
        /// <summary> 设置一个全局变量 </summary>
        /// <param name="key">名字</param>
        /// <param name="value">值</param>
        public void SetGlobal(string key, ScriptValue value) {
            m_Global.SetValue(key, value);
        }
        /// <summary> 获得一个全局变量 </summary>
        /// <param name="key">名字</param>
        /// <returns>值</returns>
        public ScriptValue GetGlobal(string key) {
            return m_Global.GetValue(key);
        }
        /// <summary> 是否包含一个全局变量 </summary>
        /// <param name="key">名字</param>
        /// <returns>是否包含</returns>
        public bool HasGlobal(string key) {
            return m_Global.HasValue(key);
        }
        public void SetArgs(string[] args) {
            var array = new ScriptArray(this);
            for (var i = 0; i < args.Length; ++i) {
                array.Add(new ScriptValue(args[i]));
            }
            m_Global.SetValue(GLOBAL_ARGS, new ScriptValue(array));
        }
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
        #region Load
        /// <summary> 使用字符串方式加载文件 </summary>
        public ScriptValue LoadFileByString(string fileName, CompileOption compileOption = null) {
            var fullFileName = SearchFile(fileName, compileOption);
            if (fullFileName == null) {
                throw new System.Exception($"can't found file : {fileName}");
            }
            using (var stream = File.OpenRead(fullFileName)) {
                return LoadStreamByString(fileName, stream, (int)stream.Length, compileOption);
            }
        }
        /// <summary> 使用字节码方式加载文件 </summary>
        public ScriptValue LoadFileByIL(string fileName, CompileOption compileOption = null) {
            var fullFileName = SearchFile(fileName, compileOption);
            if (fullFileName == null) {
                throw new System.Exception($"can't found file : {fileName}");
            }
            using (var stream = File.OpenRead(fullFileName)) {
                return LoadStreamByIL(stream);
            }
        }
        /// <summary> 使用字符串方式二进制 </summary>
        public ScriptValue LoadBufferByString(string breviary, byte[] buffer, CompileOption compileOption = null) {
            return LoadBufferByString(breviary, buffer, 0, buffer.Length, compileOption);
        }
        /// <summary> 使用字符串方式二进制 </summary>
        public ScriptValue LoadBufferByString(string breviary, byte[] buffer, int index, int count, CompileOption compileOption = null) {
            return LoadString(breviary, Encoding.GetString(buffer, index, count), compileOption);
        }
        /// <summary> 使用字节码方式二进制,IL不需要breviary和compileOption </summary>
        public ScriptValue LoadBufferByIL(byte[] buffer) {
            return LoadBufferByIL(buffer, 0, buffer.Length);
        }
        /// <summary> 使用字节码方式二进制 </summary>
        public ScriptValue LoadBufferByIL(byte[] buffer, int index, int count) {
            return Execute(Deserializer.Deserialize(buffer, index, count));
        }
        /// <summary> 使用字节码方式加载流 </summary>
        public ScriptValue LoadStreamByIL(Stream stream) {
            return Execute(Deserializer.Deserialize(stream));
        }
        /// <summary> 使用字符串方式加载流 </summary>
        public ScriptValue LoadStreamByString(string breviary, Stream stream, int count, CompileOption compileOption = null) {
            var buffer = new byte[count];
            stream.ReadBytes(buffer);
            return LoadString(breviary, Encoding.GetString(buffer), compileOption);
        }
        /// <summary> 加载一段文本 </summary>
        public ScriptValue LoadString(string buffer, CompileOption compileOption = null) {
            return LoadString(null, buffer, compileOption);
        }
        /// <summary> 加载一段文本 </summary>
        public ScriptValue LoadString(string breviary, string buffer, CompileOption compileOption = null) {
            if (buffer == null || buffer.Length == 0) { return ScriptValue.Null; }
            return Execute(Serializer.Serialize(breviary, buffer, m_SearchPaths, compileOption));
        }
        /// <summary> 加载一个文件,自动判断是IL或String </summary>
        public ScriptValue LoadFile(string fileName, CompileOption compileOption = null) {
            var fullFileName = SearchFile(fileName, compileOption);
            if (fullFileName == null) {
                throw new System.Exception($"can't found file : {fileName}");
            }
            using (var stream = File.OpenRead(fullFileName)) {
                var buffer = new byte[stream.Length];
                stream.ReadBytes(buffer);
                return LoadBuffer(fileName, buffer, compileOption);
            }
        }
        /// <summary> 加载一段数据,自动判断是IL或String </summary>
        public ScriptValue LoadBuffer(byte[] buffer, CompileOption compileOption = null) {
            return LoadBuffer(null, buffer, compileOption);
        }
        /// <summary> 加载一段数据,自动判断是IL或String </summary>
        public ScriptValue LoadBuffer(byte[] buffer, int index, int count, CompileOption compileOption = null) {
            return LoadBuffer(null, buffer, index, count, compileOption);
        }
        /// <summary> 加载一段数据,自动判断是IL或String </summary>
        public ScriptValue LoadBuffer(string breviary, byte[] buffer, CompileOption compileOption = null) {
            return LoadBuffer(breviary, buffer, 0, buffer.Length, compileOption);
        }
        /// <summary> 加载一段数据,自动判断是IL或String </summary>
        public ScriptValue LoadBuffer(string breviary, byte[] buffer, int index, int count, CompileOption compileOption = null) {
            if (count <= 0) { 
                return ScriptValue.Null;
            } else if (count > 6 && buffer[index] == 0 && BitConverter.ToInt32(buffer, index + 1) == int.MaxValue) {
                return LoadBufferByIL(buffer, index, count);
            } else {
                return LoadBufferByString(breviary, buffer, index, count, compileOption);
            }
        }
        //void ParseConstString(SerializeData data) {
        //    var length = data.ConstString.Length;
        //    var flagLength = data.NoContext.Length;
        //    for (var i = 0; i < length; ++i) {
        //        if (flagLength == 0 || (data.NoContext[i] & ScriptConstValue.StringFlag) != 0) {
        //            var str = data.ConstString[i];
        //            if (!StringIndex.ContainsKey(str)) {
        //                if (ConstStringIndex == ConstString.Length) {
        //                    Array.Resize(ref ConstString, ConstStringIndex + StringStage);
        //                }
        //                str = string.Intern(str);
        //                StringIndex[str] = ConstStringIndex;
        //                ConstString[ConstStringIndex++] = str;
        //            }
        //        }
        //    }
        //    length = data.Functions.Length;
        //    for (var i = 0; i < length; ++i) {
        //        var func = data.Functions[i];
        //        for (var j = 0; j < func.scriptInstructions.Length; ++j) {
        //            var instruction = func.scriptInstructions[j];
        //            switch (instruction.opcode) {
        //                case Opcode.LoadConstString:
        //                case Opcode.LoadValueString:
        //                case Opcode.LoadGlobalString:
        //                case Opcode.StoreValueStringAssign:
        //                case Opcode.StoreGlobalStringAssign:
        //                case Opcode.StoreValueString:
        //                case Opcode.StoreGlobalString:
        //                    if (flagLength == 0 || (data.NoContext[instruction.opvalue] & ScriptConstValue.StringFlag) != 0) {
        //                        instruction.opvalue = StringIndex[data.ConstString[instruction.opvalue]];
        //                    }
        //                    break;
        //            }
        //        }
        //    }
        //    length = data.Classes.Length;
        //    for (var i = 0; i < length; ++i) {
        //        var cl = data.Classes[i];
        //        cl.name = StringIndex[data.ConstString[cl.name]];
        //        if (cl.parent > 0) {
        //            cl.parent = StringIndex[data.ConstString[cl.parent]];
        //        }
        //        for (var j = 0; j < cl.functions.Length; ++j) {
        //            var func = cl.functions[j];
        //            long index = StringIndex[data.ConstString[func >> 32]];
        //            cl.functions[j] = (index << 32) | (func & 0xFFFFFFFFL);
        //        }
        //    }
        //}
        void OptimizeConstString(SerializeData data) {
            var length = data.ConstString.Length;
            var flagLength = data.NoContext.Length;
            for (var i = 0; i < length; ++i) {
                if (flagLength != 0 && (data.NoContext[i] & ScriptConstValue.StringFlag) == 0) {
                    data.ConstString[i] = null;
                } else {
                    data.ConstString[i] = string.Intern(data.ConstString[i]);
                }
            }
        }
        /// <summary> 执行IL </summary>
        public ScriptValue Execute(SerializeData[] datas) {
            ScriptValue result = ScriptValue.Null;
            int length = datas.Length;
            for (var j = 0; j < length; ++j) {
                SerializeData data = datas[j];
                //ParseConstString(data);
                var contexts = new ScriptContext[data.Functions.Length];
                for (int i = 0; i < data.Functions.Length; ++i) {
                    contexts[i] = new ScriptContext(this, data.Breviary, data.Functions[i], data.ConstDouble, data.ConstLong, data.ConstString, contexts, data.Classes);
                }
                result = new ScriptContext(this, data.Breviary, data.Context, data.ConstDouble, data.ConstLong, data.ConstString, contexts, data.Classes).Execute(ScriptValue.Null, null, 0, null);
                OptimizeConstString(data);
            }
            return result;
        }
        #if SCORPIO_DEBUG
        public ScriptValue Execute(SerializeData[] datas, ref List<ScriptContext[]> refContexts) {
            ScriptValue result = ScriptValue.Null;
            int length = datas.Length;
            for (var j = 0; j < length; ++j) {
                SerializeData data = datas[j];
                //ParseConstString(data);
                var contexts = new ScriptContext[data.Functions.Length];
                for (int i = 0; i < data.Functions.Length; ++i) {
                    contexts[i] = new ScriptContext(this, data.Breviary, data.Functions[i], data.ConstDouble, data.ConstLong, data.ConstString, contexts, data.Classes);
                }
                refContexts.Add(contexts);
                result = new ScriptContext(this, data.Breviary, data.Context, data.ConstDouble, data.ConstLong, data.ConstString, contexts, data.Classes).Execute(ScriptValue.Null, null, 0, null);
            }
            return result;
        }
#endif
        #endregion
        #region Stack
        private StackInfo[] m_StackInfos = new StackInfo[128];          //堆栈信息
        private StackInfo m_Stack = new StackInfo();
        private int m_StackLength = 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PushStackInfo(string breviary, int line) {
            m_StackInfos[m_StackLength].Breviary = breviary;
            m_StackInfos[m_StackLength++].Line = line;
            m_Stack.Breviary = breviary;
            m_Stack.Line = line;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PopStackInfo() {
            --m_StackLength;
        }
        /// <summary> 最近的堆栈调用 </summary>
        public StackInfo GetStackInfo() {
            return m_Stack;
        }
        /// <summary> 调用堆栈 </summary>
        public StackInfo[] GetStackInfos() {
            var stackInfos = new StackInfo[m_StackLength];
            for (var i = m_StackLength - 1; i >= 0; --i) {
                stackInfos[i] = m_StackInfos[i];
            }
            return stackInfos;
        }
        #endregion
        #region Json
        public string ToJson(ScriptValue scriptValue) {
            using (m_JsonSerializer) {
                return m_JsonSerializer.ToJson(scriptValue);
            }
        }
        public string ToJson(ScriptObject scriptObject) {
            using (m_JsonSerializer) {
                return m_JsonSerializer.ToJson(scriptObject);
            }
        }
        public ScriptValue ParseJson(string buffer, bool supportLong, bool supportIntern) {
            using (m_JsonDeserializer) {
                return m_JsonDeserializer.Parse(buffer, supportLong, supportIntern);
            }
        }
        #endregion
        public ScriptConst LoadConst(string fileName) {
            var keys = new HashSet<string>(Global.GetKeys());
            LoadFile(fileName);
            var scriptConst = new ScriptConst();
            foreach (var pair in Global) {
                if (keys.Contains(pair.Key)) { continue; }
                AddConst(scriptConst, pair.Key, pair.Value);
            }
            return scriptConst;
        }
        void AddConst(ScriptConst scriptConst, string key, ScriptValue value) {
            switch (value.valueType) {
                case ScriptValue.nullValueType:
                    scriptConst.Add(key, null);
                    break;
                case ScriptValue.trueValueType:
                    scriptConst.Add(key, true);
                    break;
                case ScriptValue.falseValueType:
                    scriptConst.Add(key, false);
                    break;
                case ScriptValue.doubleValueType:
                    scriptConst.Add(key, value.doubleValue);
                    break;
                case ScriptValue.longValueType:
                    scriptConst.Add(key, value.longValue);
                    break;
                case ScriptValue.stringValueType:
                    scriptConst.Add(key, value.stringValue);
                    break;
                case ScriptValue.scriptValueType:
                    var map = value.Get<ScriptMap>();
                    if (map != null) {
                        var con = new ScriptConst();
                        foreach (var pair in map) {
                            AddConst(con, pair.Key.ToString(), pair.Value);
                        }
                        scriptConst.Add(key, con);
                    } else {
                        throw new ExecutionException($"变量{key}不是基础常量:{value.ValueTypeName}");
                    }
                    break;
                default:
                    throw new ExecutionException($"变量{key}不是基础常量:{value.ValueTypeName}");
            }
        }
    }
}
