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
        private bool m_InitializeMethods;                               //是否初始化过所有函数
        private UserdataMethod m_Constructor;                           //所有构造函数
        private List<MethodInfo> m_Methods;                             //所有函数 包含扩展函数
        private Dictionary<string, UserdataVariable> m_Variables;       //所有的变量 FieldInfo,PropertyInfo,EventInfo
        private Dictionary<string, ScriptValue> m_NestedTypes;          //所有的内部类
        private Dictionary<string, UserdataMethod> m_Functions;         //所有的函数
        public UserdataTypeReflect(Type type) : base(type) {
            m_InitializeConstructor = false;
            m_InitializeMethods = false;
            m_Methods = new List<MethodInfo>();
            m_Variables = new Dictionary<string, UserdataVariable>();
            m_NestedTypes = new Dictionary<string, ScriptValue>();
            m_Functions = new Dictionary<string, UserdataMethod>();
            InitializeConstructor();
            InitializeMethods();
        }
        //初始化构造函数
        private void InitializeConstructor() {
            if (m_InitializeConstructor == true) return;
            m_InitializeConstructor = true;
            m_Constructor = new UserdataMethodReflect(m_Type, m_Type.ToString(), m_Type.GetTypeInfo().GetConstructors(Script.BindingFlag));
        }
        //初始化所有函数
        private void InitializeMethods() {
            if (m_InitializeMethods == true) return;
            m_InitializeMethods = true;
            m_Methods.AddRange(m_Type.GetTypeInfo().GetMethods(Script.BindingFlag));
        }
        //获取一个函数，名字相同返回值相同
        private UserdataMethod GetMethod(string name) {
            var methods = new List<MethodInfo>();
            foreach (var method in m_Methods) {
                if (method.Name.Equals(name)) {
                    methods.Add(method);
                }
            }
            if (methods.Count > 0)
                return m_Functions[name] = new UserdataMethodReflect(m_Type, name, methods.ToArray());
            return null;
        }
        //获取一个变量
        private UserdataVariable GetVariable(string name) {
            if (m_Variables.ContainsKey(name))
                return m_Variables[name];
            FieldInfo fInfo = m_Type.GetTypeInfo().GetField(name, Script.BindingFlag);
            if (fInfo != null) return m_Variables[name] = new UserdataField(fInfo);
            PropertyInfo pInfo = m_Type.GetTypeInfo().GetProperty(name, Script.BindingFlag);
            if (pInfo != null) return m_Variables[name] = new UserdataProperty(pInfo);
            //EventInfo eInfo = m_Type.GetTypeInfo().GetEvent(name, Script.BindingFlag);
            //if (eInfo != null) return m_Variables[name] = new UserdataEvent(m_Script, eInfo);
            return null;
        }
        //获取一个内部类
        private ScriptValue GetNestedType(string name) {
            Type nestedType = m_Type.GetTypeInfo().GetNestedType(name, Script.BindingFlag);
            if (nestedType != null) {
                return m_NestedTypes[name] = TypeManager.GetUserdataType(nestedType);
            }
            return ScriptValue.Null;
        }
        /// <summary> 创建一个实例 </summary>
        public override ScriptUserdata CreateInstance(ScriptValue[] parameters, int length) {
            return new ScriptUserdataObject(m_Constructor.Call(false, null, parameters, length), this);
        }
        //获得一个变量的类型
        protected override Type GetVariableType_impl(string name) {
            var variable = GetVariable(name);
            return variable != null ? variable.FieldType : null;
        }
        /// <summary> 获得一个类变量 </summary>
        protected override object GetValue_impl(object obj, string name) {
            if (m_Functions.ContainsKey(name)) return m_Functions[name];
            if (m_NestedTypes.ContainsKey(name)) return m_NestedTypes[name];
            UserdataVariable variable = GetVariable(name);
            if (variable != null) return variable.GetValue(obj);
            var nestedType = GetNestedType(name);
            if (nestedType.valueType != ScriptValue.nullValueType) return nestedType;
            UserdataMethod func = GetMethod(name);
            if (func != null) return func;
            throw new ExecutionException("GetValue 类[" + m_Type.ToString() + "] 变量 [" + name + "] 不存在");
        }
        /// <summary> 设置一个类变量 </summary>
        protected override void SetValue_impl(object obj, string name, ScriptValue value) {
            UserdataVariable variable = GetVariable(name);
            if (variable == null) throw new ExecutionException("SetValue 类[" + m_Type + "] 变量 [" + name + "] 不存在");
            try {
                variable.SetValue(obj, Util.ChangeType(value, variable.FieldType));
            } catch (System.Exception e) {
                throw new ExecutionException("SetValue 出错 源类型:" + value.ValueTypeName + " 目标类型:" + variable.FieldType.Name + " : " + e.ToString());
            }
        }
        //需要保证调用相同名字函数前 载入扩展函数，后续修改为任何时候都可以载入
        public override void AddExtensionMethod(MethodInfo method) {
            if (!m_Methods.Contains(method))
                m_Methods.Add(method);
        }
    }
}
