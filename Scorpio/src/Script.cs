using System.IO;
using System.Text;
using System.Reflection;
using Scorpio.Compiler;
using Scorpio.Exception;
using System.Collections.Generic;
using Scorpio.Tools;
using Scorpio.Function;
using Scorpio.Library;
using Scorpio.Runtime;
using Scorpio.Proto;
using Scorpio.Userdata;
using Scorpio.Serialize;
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
        private List<string> m_SearchPath = new List<string>();         //request所有文件的路径集合

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
            Global = new ScriptGlobal();
            Global.SetValue(GLOBAL_NAME, new ScriptValue(Global));
            Global.SetValue(GLOBAL_SCRIPT, ScriptValue.CreateObject(this));
            Global.SetValue(GLOBAL_VERSION, ScriptValue.CreateObject(typeof(Version)));

            TypeObject = ProtoObject.Load(this);
            TypeObjectValue = new ScriptValue(TypeObject);
            Global.SetValue(TypeObject.TypeName, TypeObjectValue);

            TypeBoolean = ProtoBoolean.Load(this, TypeObjectValue);
            TypeBooleanValue = new ScriptValue(TypeBoolean);
            Global.SetValue(TypeBoolean.TypeName, TypeBooleanValue);

            TypeNumber = ProtoNumber.Load(this, TypeObjectValue);
            TypeNumberValue = new ScriptValue(TypeNumber);
            Global.SetValue(TypeNumber.TypeName, TypeNumberValue);

            TypeString = ProtoString.Load(this, TypeObjectValue);
            TypeStringValue = new ScriptValue(TypeString);
            Global.SetValue(TypeString.TypeName, TypeStringValue);

            TypeArray = ProtoArray.Load(this, TypeObjectValue);
            TypeArrayValue = new ScriptValue(TypeArray);
            Global.SetValue(TypeArray.TypeName, TypeArrayValue);

            TypeMap = ProtoMap.Load(this, TypeObjectValue);
            TypeMapValue = new ScriptValue(TypeMap);
            Global.SetValue(TypeMap.TypeName, TypeMapValue);

            TypeFunction = ProtoFunction.Load(this, TypeObjectValue);
            TypeFunctionValue = new ScriptValue(TypeFunction);
            Global.SetValue(TypeFunction.TypeName, TypeFunctionValue);


            TypeManager.PushAssembly(typeof(object).GetTypeInfo().Assembly);                        //mscorlib.dll
            TypeManager.PushAssembly(typeof(System.Net.Sockets.Socket).GetTypeInfo().Assembly);     //System.dll
            TypeManager.PushAssembly(GetType().GetTypeInfo().Assembly);                             //当前所在的程序集
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
            if (buffer[0] == 0)
                return Execute(breviary, SerializeUtil.Deserialize(buffer));
            else
                return Execute(breviary, SerializeUtil.Serialize(breviary, encoding.GetString(buffer, 0, buffer.Length)));
        }
        public ScriptValue LoadString(string buffer) {
            return LoadString(Undefined, buffer);
        }
        public ScriptValue LoadString(string breviary, string buffer) {
            if (buffer == null || buffer.Length == 0) { return ScriptValue.Null; }
            return Execute(breviary, SerializeUtil.Serialize(breviary, buffer));
        }
        ScriptValue Execute(string breviary, SerializeData data) {
            var contexts = new ScriptContext[data.Functions.Length];
            for (int i = 0; i < data.Functions.Length; ++i) {
                contexts[i] = new ScriptContext(this, breviary, data.Functions[i], data.ConstDouble, data.ConstLong, data.ConstString, contexts);
            }
            return new ScriptContext(this, breviary, data.Context, data.ConstDouble, data.ConstLong, data.ConstString, contexts).Execute(ScriptValue.Null, null, 0, null);
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
        public ScriptArray CreateArray() { return new ScriptArray(this); }
        public ScriptMap CreateMap() { return new ScriptMap(this); }
        public ScriptType CreateType(string typeName, ScriptValue parentType) { return new ScriptType(typeName, parentType); }
        public ScriptValue CreateFunction(ScorpioHandle value) { return new ScriptValue(new ScriptHandleFunction(this, value)); }

        public ScriptValue call(string name, params object[] args) {
            return Global.GetValue(name).call(ScriptValue.Null, args);
        }
        public ScriptValue Call(String name, ScriptValue[] args, int length) {
            return Global.GetValue(name).Call(ScriptValue.Null, args, length);
        }
    }
}
