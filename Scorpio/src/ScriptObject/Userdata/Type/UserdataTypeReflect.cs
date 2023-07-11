using System;
using System.Collections.Generic;
using System.Reflection;
using Scorpio.Exception;
using Scorpio.Tools;
namespace Scorpio.Userdata {
    //反射类管理
    public class UserdataTypeReflect : UserdataType {
        private UserdataMethod m_Constructor;                           //所有构造函数
        private MethodInfo[] m_Methods;                                 //所有函数
        private Dictionary<string, UserdataVariable> m_Variables;       //所有的变量 FieldInfo,PropertyInfo,EventInfo
        private Dictionary<string, UserdataMethodReflect> m_Functions;  //所有的函数
        public UserdataTypeReflect(Type type) : base(type) {
            m_Variables = new Dictionary<string, UserdataVariable>();
            m_Functions = new Dictionary<string, UserdataMethodReflect>();
            //GetConstructors 去掉 NonPublic 标识, 否则取函数会取出一些错误的函数例如类 System.Diagnostics.Process
            m_Constructor = new UserdataMethodReflect(m_Type, string.Intern(m_Type.ToString()), m_Type.GetConstructors(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy));
            m_Methods = m_Type.GetMethods(Script.BindingFlag);
        }
        //获取一个函数，名字相同返回值相同
        private UserdataMethodReflect GetFunction(string name) {
            var methods = Array.FindAll(m_Methods, method => method.Name == name);
            if (methods.Length > 0) {
                name = string.Intern(name);
                return m_Functions[name] = new UserdataMethodReflect(m_Type, name, methods);
            }
            return null;
        }
        //获取一个内部类
        private ScriptValue GetNestedType(string name) {
            var nestedType = m_Type.GetNestedType(name, Script.BindingFlag);
            if (nestedType != null) {
                return m_Values[string.Intern(name)] = ScorpioTypeManager.GetUserdataType(nestedType);
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
            if (pInfo != null) return m_Variables[name] = new UserdataProperty(pInfo);
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
        /// <summary> 获得函数 </summary>
        protected override UserdataMethod GetMethod(string name) {
            if (m_Functions.TryGetValue(name, out var userdataMethod))
                return userdataMethod;
            return GetFunction(name);
        }
        //获得一个变量的类型
        public override Type GetVariableType(string name) {
            var variable = GetVariable(name);
            return variable != null ? variable.FieldType : null;
        }
        /// <summary> 获得一个类变量 </summary>
        public override object GetValue(object obj, string name) {
            if (m_Functions.TryGetValue(name, out var userdataMethod)) 
                return userdataMethod;
            if (m_Values.TryGetValue(name, out var value)) 
                return value;
            var variable = GetVariable(name);
            if (variable != null) return variable.GetValue(obj);
            userdataMethod = GetFunction(name);
            if (userdataMethod != null) return userdataMethod;
            value = GetNestedType(name);
            if (value.valueType != ScriptValue.nullValueType) return value;
            throw new ExecutionException($"GetValue Type:[{m_Type.FullName}] 变量:[{name}]不存在");
        }
        /// <summary> 设置一个类变量 </summary>
        public override void SetValue(object obj, string name, ScriptValue value) {
            var variable = GetVariable(name);
#if SCORPIO_ASSERT
            if (variable == null)
                throw new ExecutionException($"SetValue Type:[{m_Type.FullName}] 变量:[{name}]不存在");
#endif
            try {
                variable.SetValue(obj, value.ChangeType(variable.FieldType));
            } catch (System.Exception e) {
                throw new ExecutionException($"SetValue [{name}] 出错 源类型:{value.ValueTypeName}({value}) 目标类型:{variable?.FieldType?.Name}: {e}");
            }
        }
    }
}
