using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Scorpio.Exception;
using Scorpio.Tools;
namespace Scorpio.Userdata {
    //反射类管理
    public class UserdataTypeReflect : UserdataType {
        private bool m_InitializeConstructor;                           //是否初始化过所有构造函数
        private bool m_InitializeFunctions;                             //是否初始化过所有函数
        private UserdataMethod m_Constructor;                           //所有构造函数
        private List<MethodInfo> m_Methods;                             //所有函数
        private Dictionary<string, UserdataVariable> m_Variables;       //所有的变量 FieldInfo,PropertyInfo,EventInfo
        private Dictionary<string, ScriptValue> m_NestedTypes;          //所有的内部类
        private Dictionary<string, UserdataMethodReflect> m_Functions;  //所有的函数
        public UserdataTypeReflect(Type type) : base(type) {
            m_InitializeConstructor = false;
            m_InitializeFunctions = false;
            m_Methods = new List<MethodInfo>();
            m_Variables = new Dictionary<string, UserdataVariable>();
            m_NestedTypes = new Dictionary<string, ScriptValue>();
            m_Functions = new Dictionary<string, UserdataMethodReflect>();
            InitializeConstructor();
            InitializeFunctions();
        }
        //初始化构造函数
        private void InitializeConstructor() {
            if (m_InitializeConstructor == true) return;
            m_InitializeConstructor = true;
            //GetConstructors 去掉 NonPublic 标识, 否则取函数会取出一些错误的函数例如类 System.Diagnostics.Process
            var name = string.Intern(m_Type.ToString());
            m_Constructor = new UserdataMethodReflect(m_Type, name, m_Type.GetConstructors(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy));
        }
        //初始化所有函数
        private void InitializeFunctions() {
            if (m_InitializeFunctions == true) return;
            m_InitializeFunctions = true;
            m_Methods.AddRange(m_Type.GetMethods(Script.BindingFlag));
        }
        //获取一个函数，名字相同返回值相同
        private UserdataMethodReflect GetFunction(string name) {
            var methods = m_Methods.FindAll((method) => method.Name == name);
            if (methods.Count > 0) {
                name = string.Intern(name);
                return m_Functions[name] = new UserdataMethodReflect(m_Type, name, methods.ToArray());
            }
            return null;
        }
        //获取一个内部类
        private ScriptValue GetNestedType(string name) {
            var nestedType = m_Type.GetNestedType(name, Script.BindingFlag);
            if (nestedType != null) {
                return m_NestedTypes[string.Intern(name)] = TypeManager.GetUserdataType(nestedType);
            }
            return ScriptValue.Null;
        }
        //获取一个变量
        private UserdataVariable GetVariable(string name) {
            if (m_Variables.TryGetValue(name, out var value))
                return value;
            name = string.Intern(name);
            FieldInfo fInfo = m_Type.GetField(name, Script.BindingFlag);
            if (fInfo != null) return m_Variables[name] = new UserdataField(fInfo);
            PropertyInfo pInfo = m_Type.GetProperty(name, Script.BindingFlag);
            if (pInfo != null) return m_Variables[name] = new UserdataProperty(m_Type, pInfo);
            //EventInfo eInfo = m_Type.GetTypeInfo().GetEvent(name, Script.BindingFlag);
            //if (eInfo != null) return m_Variables[name] = new UserdataEvent(m_Script, eInfo);
            return null;
        }
        //添加一个扩展函数
        public void AddExtensionMethod(MethodInfo method) {
            var name = method.Name;
            var userdataMethod = GetMethod(name);
            if (userdataMethod == null) {
                name = string.Intern(name);
                userdataMethod = (m_Functions[name] = new UserdataMethodReflect(m_Type, name));
            }
            ((UserdataMethodReflect)userdataMethod).AddExtensionMethod(method);
        }
        /// <summary> 创建一个实例 </summary>
        public override ScriptUserdata CreateInstance(ScriptValue[] parameters, int length) {
            return new ScriptUserdataObject(m_Constructor.Call(false, null, parameters, length), this);
        }
        //获得一个变量的类型
        public override Type GetVariableType(string name) {
            var variable = GetVariable(name);
            return variable != null ? variable.FieldType : null;
        }
        /// <summary> 获得函数 </summary>
        public override UserdataMethod GetMethod(string name) {
            if (m_Functions.TryGetValue(name, out var userdataMethod))
                return userdataMethod;
            return GetFunction(name);
        }
        /// <summary> 获得一个类变量 </summary>
        public override object GetValue(object obj, string name) {
            if (m_Functions.TryGetValue(name, out var userdataMethod)) 
                return userdataMethod;
            if (m_NestedTypes.TryGetValue(name, out var nestedType)) 
                return nestedType;
            var variable = GetVariable(name);
            if (variable != null) return variable.GetValue(obj);
            userdataMethod = GetFunction(name);
            if (userdataMethod != null) return userdataMethod;
            nestedType = GetNestedType(name);
            if (nestedType.valueType != ScriptValue.nullValueType) return nestedType;
            throw new ExecutionException($"GetValue Type:[{m_Type.FullName}] 变量:[{name}]不存在");
        }
        /// <summary> 设置一个类变量 </summary>
        public override void SetValue(object obj, string name, ScriptValue value) {
            var variable = GetVariable(name);
            Util.Assert(variable != null, $"SetValue Type:[{m_Type.FullName}] 变量:[{name}]不存在");
            try {
                variable.SetValue(obj, Util.ChangeType(value, variable.FieldType));
            } catch (System.Exception e) {
                throw new ExecutionException($"SetValue 出错 源类型:{value.ValueTypeName}  目标类型:{variable?.FieldType?.Name}: {e}");
            }
        }
    }
}
