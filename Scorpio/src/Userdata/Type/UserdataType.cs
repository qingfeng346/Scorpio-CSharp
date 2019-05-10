using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Scorpio;
using Scorpio.Exception;
using Scorpio.Variable;
using Scorpio.Compiler;
namespace Scorpio.Userdata {
    /// <summary> 一个c#类的所有数据 </summary>
    public abstract class UserdataType {
        protected Script m_Script;                                                              //脚本系统
        protected Type m_Type;                                                                  //c#类型
        public UserdataType(Script script, Type type) {
            m_Script = script;
            m_Type = type;
        }
        /// <summary> 获取一个变量的类型,只能获取 Field Property Event </summary>
        public Type GetVariableType(string name) { return GetVariableType_impl(name); }
        /// <summary> 获得一个类变量 </summary>
        public object GetValue(object obj, string name) { return GetValue_impl(obj, name); }
        /// <summary> 设置一个类变量 </summary>
        public void SetValue(object obj, string name, ScriptValue value) { SetValue_impl(obj, name, value); }
        //创建一个模板类
        public ScriptValue MakeGenericType(Type[] parameters) {
            if (m_Type.IsGenericType && m_Type.IsGenericTypeDefinition) {
                var types = m_Type.GetTypeInfo().GetGenericArguments();
                var length = types.Length;
                if (length != parameters.Length)
                    throw new ExecutionException($"{m_Type.FullName} 传入的泛型参数个数错误 需要:{types.Length} 传入:{parameters.Length}");
                for (int i = 0; i < length; ++i) {
                    if (!types[i].GetTypeInfo().BaseType.GetTypeInfo().IsAssignableFrom(parameters[i]))
                        throw new ExecutionException($"{m_Type.FullName} 泛型类第{i+1}个参数不符合传入规则 需要:{types[i].GetTypeInfo().BaseType.FullName} 传入:{parameters[i].FullName}");
                }
                return m_Script.GetUserdataType(m_Type.MakeGenericType(parameters));
            }
            throw new ExecutionException($"类 {m_Type.FullName} 不是未定义的泛型类");
        }
        /// <summary> 创建一个实例 </summary>
        public abstract ScriptUserdata CreateInstance(ScriptValue[] parameters, int length);
        /// <summary> 获取一个变量的类型,只能获取 Field Property Event </summary>
        protected abstract Type GetVariableType_impl(string name);
        /// <summary> 获得一个类变量 </summary>
        protected abstract object GetValue_impl(object obj, string name);
        /// <summary> 设置一个类变量 </summary>
        protected abstract void SetValue_impl(object obj, string name, ScriptValue value);
        /// <summary> 添加一个扩展函数 </summary>
        public abstract void AddExtensionMethod(MethodInfo method);
    }
}
