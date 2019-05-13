using System.IO;
using System.Text;
using System.Reflection;
using Scorpio.Compiler;
using Scorpio.Exception;
using System.Collections.Generic;
using Scorpio.Commons;
using Scorpio.Function;
using Scorpio.Library;
using Scorpio.Runtime;
using Scorpio.Proto;
using Scorpio.Userdata;
using System;
namespace Scorpio {
    public class Script {
        //反射获取变量和函数的属性
        public const BindingFlags BindingFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        private static readonly Encoding UTF8 = Encoding.UTF8;          //默认编码格式
        private const string Undefined = "Undefined";                   //Undefined
        private const string GLOBAL_NAME = "_G";                        //全局对象
        private const string GLOBAL_SCRIPT = "_SCRIPT";                 //Script对象
        private const string GLOBAL_VERSION = "_VERSION";               //版本号
        private List<Assembly> m_Assembly = new List<Assembly>();       //所有代码集合
        private List<string> m_SearchPath = new List<string>();         //request所有文件的路径集合

        private Dictionary<Type, UserdataType> m_Types = new Dictionary<Type, UserdataType>();                                      //所有的类集合
        private Dictionary<Type, ScriptValue> m_UserdataTypes = new Dictionary<Type, ScriptValue>();                                //所有的类集合
        private Dictionary<Type, ScorpioFastReflectClass> m_FastReflectClass = new Dictionary<Type, ScorpioFastReflectClass>();     //所有去反射类集合
        

        public ScriptType TypeObject { get; private set; }                      //所有类型的基类
        public ScriptValue TypeObjectValue { get; private set; }                //所有类型的基类

        public ScriptType TypeBoolean { get; private set; }                     //所有bool的基类
        public ScriptValue TypeBooleanValue { get; private set; }               //所有bool的基类

        public ScriptType TypeNumber { get; private set; }                      //所有number的基类
        public ScriptValue TypeNumberValue { get; private set; }                //所有number的基类

        public ScriptType TypeString { get; private set; }                      //所有string的基类
        public ScriptValue TypeStringValue { get; private set; }                //所有string的基类

        public ScriptType TypeArray { get; private set; }                       //所有array的基类
        public ScriptValue TypeArrayValue { get; private set; }                 //所有array的基类

        public ScriptType TypeMap { get; private set; }                         //所有map的基类
        public ScriptValue TypeMapValue { get; private set; }                   //所有map的基类

        public ScriptType TypeFunction { get; private set; }                    //所有function的基类
        public ScriptValue TypeFunctionValue { get; private set; }              //所有function的基类

        public ScriptGlobal Global { get; private set; }                        //全局变量


        public Script() {
            Global = new ScriptGlobal(this);
            Global.SetValue(GLOBAL_NAME, new ScriptValue(Global));
            Global.SetValue(GLOBAL_SCRIPT, CreateObject(this));
            Global.SetValue(GLOBAL_VERSION, CreateObject(typeof(Version)));

            TypeObject = ProtoObject.Load(this);
            TypeObjectValue = new ScriptValue(TypeObject);
            Global.SetValue(TypeObject.TypeName, TypeObjectValue);

            TypeBoolean = ProtoBoolean.Load(this, TypeObject);
            TypeBooleanValue = new ScriptValue(TypeBoolean);
            Global.SetValue(TypeBoolean.TypeName, TypeBooleanValue);

            TypeNumber = ProtoNumber.Load(this, TypeObject);
            TypeNumberValue = new ScriptValue(TypeNumber);
            Global.SetValue(TypeNumber.TypeName, TypeNumberValue);

            TypeString = ProtoString.Load(this, TypeObject);
            TypeStringValue = new ScriptValue(TypeString);
            Global.SetValue(TypeString.TypeName, TypeStringValue);

            TypeArray = ProtoArray.Load(this, TypeObject);
            TypeArrayValue = new ScriptValue(TypeArray);
            Global.SetValue(TypeArray.TypeName, TypeArrayValue);

            TypeMap = ProtoMap.Load(this, TypeObject);
            TypeMapValue = new ScriptValue(TypeMap);
            Global.SetValue(TypeMap.TypeName, TypeMapValue);

            TypeFunction = ProtoFunction.Load(this, TypeObject);
            TypeFunctionValue = new ScriptValue(TypeFunction);
            Global.SetValue(TypeFunction.TypeName, TypeFunctionValue);


            PushAssembly(typeof(object).GetTypeInfo().Assembly);                        //mscorlib.dll
            PushAssembly(typeof(System.Net.Sockets.Socket).GetTypeInfo().Assembly);     //System.dll
            PushAssembly(GetType().GetTypeInfo().Assembly);                             //当前所在的程序集
            LoadLibrary();
        }
        public void LoadLibrary() {
            LibraryBasis.Load(this);
            LibraryJson.Load(this);
            LibraryMath.Load(this);
            LibraryUserdata.Load(this);
            LibraryIO.Load(this);
        }
        public ScriptValue LoadFile(string fileName) {
            return LoadFile(fileName, UTF8);
        }
        public ScriptValue LoadFile(string fileName, Encoding encoding) {
            using (var stream = File.OpenRead(fileName)) {
                var length = stream.Length;
                var buffer = new byte[length];
                stream.Read(buffer, 0, buffer.Length);
                return LoadBuffer(fileName, buffer, encoding);
            }
        }
        public ScriptValue LoadBuffer(byte[] buffer) {
            return LoadBuffer(Undefined, buffer, UTF8);
        }
        public ScriptValue LoadBuffer(string breviary, byte[] buffer) {
            return LoadBuffer(breviary, buffer, UTF8);
        }
        public ScriptValue LoadBuffer(string breviary, byte[] buffer, Encoding encoding) {
            if (buffer == null || buffer.Length == 0) { return ScriptValue.Null; }
            return LoadString(breviary, encoding.GetString(buffer, 0, buffer.Length));
        }
        public ScriptValue LoadString(string buffer) {
            return LoadString(Undefined, buffer);
        }
        public ScriptValue LoadString(string breviary, string buffer) {
            try {
                if (Util.IsNullOrEmpty(buffer)) return ScriptValue.Null;
                var scriptLexer = new ScriptLexer(buffer, breviary);
                breviary = scriptLexer.Breviary;
                var tokens = scriptLexer.GetTokens();
                if (tokens.Count == 0) { return ScriptValue.Null; }
                var scriptParser = new ScriptParser(tokens, breviary);
                var functionData = scriptParser.Parse();
                var contexts = new ScriptContext[scriptParser.Functions.Count];
                for (int i = 0; i < scriptParser.Functions.Count; ++i) {
                    contexts[i] = new ScriptContext(this, breviary, scriptParser.Functions[i], scriptParser.ConstDouble.ToArray(), scriptParser.ConstLong.ToArray(),
                        scriptParser.ConstString.ToArray(), contexts);
                }
                return new ScriptContext(this, breviary, functionData, scriptParser.ConstDouble.ToArray(), scriptParser.ConstLong.ToArray(), scriptParser.ConstString.ToArray(), contexts).Execute(ScriptValue.Null, null, 0, null);
            } catch (System.Exception e) {
                throw new ScriptException("load string [" + breviary + "] is error : " + e.ToString());
            }
        }
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
            throw new ExecutionException("require 找不到文件 : " + fileName);
        }
        public void SetGlobal(string key, ScriptValue value) {
            Global.SetValue(key, value);
        }
        public void PushAssembly(Assembly assembly) {
            if (assembly == null) return;
            if (!m_Assembly.Contains(assembly))
                m_Assembly.Add(assembly);
        }
        public Type LoadType(string name) {
            for (int i = 0; i < m_Assembly.Count; ++i) {
                Type type = m_Assembly[i].GetType(name);
                if (type != null) return type;
            }
            return Type.GetType(name, false, false);
        }
        public UserdataType GetType(Type type) {
            if (m_Types.ContainsKey(type))
                return m_Types[type];
            if (m_FastReflectClass.ContainsKey(type)) {
                return m_Types[type] = new UserdataTypeFastReflect(this, type, m_FastReflectClass[type]);
            } else {
                return m_Types[type] = new UserdataTypeReflect(this, type);
            }
        }
        public ScriptValue GetUserdataType(string name) {
            Type type = LoadType(name);
            if (type == null) return ScriptValue.Null;
            return GetUserdataType(type);
        }
        public ScriptValue GetUserdataType(Type type) {
            if (m_UserdataTypes.ContainsKey(type))
                return m_UserdataTypes[type];
            if (Util.IsDelegate(type))
                return m_UserdataTypes[type] = new ScriptValue(new ScriptUserdataDelegateType(this, type));
            else if (type.IsEnum)
                return m_UserdataTypes[type] = new ScriptValue(new ScriptUserdataEnumType(this, type));
            else
                return m_UserdataTypes[type] = new ScriptValue(new ScriptUserdataType(this, type, GetType(type)));
        }
        public ScriptValue CreateObject(object value) {
            if (value == null)
                return ScriptValue.Null;
            else if (value is bool)
                return (bool)value ? ScriptValue.True : ScriptValue.False;
            else if (value is string)
                return new ScriptValue((string)value);
            else if (value is long)
                return new ScriptValue((long)value);
            else if (value is double)
                return new ScriptValue((double)value);
            else if (value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is float || value is decimal)
                return new ScriptValue(Util.ToDouble(value));
            else if (value is ScriptValue)
                return (ScriptValue)value;
            else if (value is ScriptObject)
                return new ScriptValue((ScriptObject)value);
            else if (value is ScorpioHandle)
                return new ScriptValue(new ScriptHandleFunction(this, (ScorpioHandle)value));
            else if (value is Type)
                return GetUserdataType((Type)value);
            else if (value is Delegate)
                return new ScriptValue(new ScriptUserdataDelegate(this, (Delegate)value));
            else if (value is Enum)
                return new ScriptValue(value);
            return new ScriptValue(new ScriptUserdataObject(this, value, GetType(value.GetType())));
        }
        //加载扩展函数
        public void LoadExtension(string type) {
            LoadExtension(LoadType(type));
        }
        public void LoadExtension(Type type) {
            if (type == null) return;
            if (!Util.IsExtensionType(type)) return;
            var methods = type.GetMethods(BindingFlag);
            foreach (var method in methods) {
                if (Util.IsExtensionMethod(method)) {
                    GetType(method.GetParameters()[0].ParameterType).AddExtensionMethod(method);
                }
            }
        }
        //添加一个去反射类
        public void PushFastReflectClass(Type type, ScorpioFastReflectClass value) {
            if (value == null || type == null) { return; }
            m_FastReflectClass[type] = value;
        }
        public ScriptMap CreateMap() { return new ScriptMap(this); }
        public ScriptType CreateType(string typeName, ScriptType parentType) { return new ScriptType(this, typeName, parentType); }
        public ScriptValue CreateFunction(ScorpioHandle value) { return new ScriptValue(new ScriptHandleFunction(this, value)); }
    }
}
