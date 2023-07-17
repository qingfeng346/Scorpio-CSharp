using System;
using System.Collections.Generic;
using Scorpio.Exception;
using Scorpio.Tools;

namespace Scorpio.Userdata
{
    /// <summary> 一个c#类的所有数据 </summary>
    public abstract class UserdataType {
        protected Type m_Type;                                                                          //Type
        protected UserdataMethod[] m_Operators = new UserdataMethod[UserdataOperator.OperatorCount];    //所有重载函数
        protected bool[] m_InitOperators = new bool[UserdataOperator.OperatorCount];                    //是否初始化过重载函数
        internal Dictionary<string, ScriptValue> m_StaticMethods;                                       //所有静态函数,类型 获取返回,不一定是真的静态函数
        internal Dictionary<string, ScriptValue> m_InstanceMethods;                                     //所有实例函数,实例 获取返回,不一定是真的实例函数
        internal Dictionary<string, ScriptValue> m_Values;                                              //所有的内部数据,内部类,脚本扩展函数
        public UserdataType(Type type) {
            m_Type = type;
            m_Values = new Dictionary<string, ScriptValue>();
            m_StaticMethods = new Dictionary<string, ScriptValue>();
            m_InstanceMethods = new Dictionary<string, ScriptValue>();
        }
        public Type Type => m_Type;
        public void Free() {
            m_Values.Free();
            m_StaticMethods.Free();
            m_InstanceMethods.Free();
        }
        //创建一个模板类
        public ScriptValue MakeGenericType(Script script, Type[] parameters) {
            if (m_Type.IsGenericType && m_Type.IsGenericTypeDefinition) {
                var types = m_Type.GetGenericArguments();
                var length = types.Length;
                if (length != parameters.Length)
                    throw new ExecutionException($"{m_Type.FullName} 传入的泛型参数个数错误 需要:{types.Length} 传入:{parameters.Length}");
                for (int i = 0; i < length; ++i) {
                    if (!types[i].BaseType.IsAssignableFrom(parameters[i]))
                        throw new ExecutionException($"{m_Type.FullName} 泛型类第{i+1}个参数不符合传入规则 需要:{types[i].BaseType.FullName} 传入:{parameters[i].FullName}");
                }
                return script.GetUserdataTypeValue(m_Type.MakeGenericType(parameters));
            }
            throw new ExecutionException($"类 {m_Type.FullName} 不是未定义的泛型类");
        }
        //获取一个内部类
        protected bool TryGetNestedType(Script script, string name, out ScriptValue value) {
            var nestedType = m_Type.GetNestedType(name, Script.BindingFlag);
            if (nestedType == null) {
                value = default;
                return false;
            }
            m_Values[string.Intern(name)] = value = script.GetUserdataTypeValue(nestedType).Reference();
            return true;
        }
        //获得操作符重载函数
        public UserdataMethod GetOperator(int operate) {
            if (m_InitOperators[operate]) return m_Operators[operate];
            m_InitOperators[operate] = true;
            string operatorName = "";
            switch (operate) {
                case UserdataOperator.PlusIndex: operatorName = UserdataOperator.Plus; break;
                case UserdataOperator.MinusIndex: operatorName = UserdataOperator.Minus; break;
                case UserdataOperator.MultiplyIndex: operatorName = UserdataOperator.Multiply; break;
                case UserdataOperator.DivideIndex: operatorName = UserdataOperator.Divide; break;
                case UserdataOperator.ModuloIndex: operatorName = UserdataOperator.Modulo; break;
                case UserdataOperator.InclusiveOrIndex: operatorName = UserdataOperator.InclusiveOr; break;
                case UserdataOperator.CombineIndex: operatorName = UserdataOperator.Combine; break;
                case UserdataOperator.XORIndex: operatorName = UserdataOperator.XOR; break;
                case UserdataOperator.ShiIndex: operatorName = UserdataOperator.Shi; break;
                case UserdataOperator.ShrIndex: operatorName = UserdataOperator.Shr; break;
                case UserdataOperator.GreaterIndex: operatorName = UserdataOperator.Greater; break;
                case UserdataOperator.GreaterOrEqualIndex: operatorName = UserdataOperator.GreaterOrEqual; break;
                case UserdataOperator.LessIndex: operatorName = UserdataOperator.Less; break;
                case UserdataOperator.LessOrEqualIndex: operatorName = UserdataOperator.LessOrEqual; break;
                case UserdataOperator.EqualIndex: operatorName = UserdataOperator.Equal; break;
                case UserdataOperator.GetItemIndex: operatorName = UserdataOperator.GetItem; break;
                case UserdataOperator.SetItemIndex: operatorName = UserdataOperator.SetItem; break;
            }
            return m_Operators[operate] = GetMethod(operatorName);
        }
        //设置变量,一般是脚本扩展函数
        public void SetValue(string name, ScriptValue value) {
            if (m_Values.TryGetValue(name, out var result)) {
                result.Free();
            }
            //正常引用计数
            m_Values[name] = value.Reference();
        }
        //获取静态变量
        public object GetStaticValue(Script script, string name) {
            if (m_StaticMethods.TryGetValue(name, out var value)) return value;
            if (m_Values.TryGetValue(name, out value)) return value;
            if (TryGetValue(null, name, out var val)) return val;
            var userdataMethod = GetMethod(name);
            if (userdataMethod != null) return m_StaticMethods[name] = new ScriptValue(script.NewStaticMethod().Set(name, userdataMethod));
            if (TryGetNestedType(script, name, out value)) return value;
            throw new ExecutionException($"GetValue Type:[{m_Type.FullName}] 静态变量:[{name}]不存在");
        }
        //获取实例变量
        public object GetInstanceValue(Script script, string name, object obj) {
            if (m_InstanceMethods.TryGetValue(name, out var value)) return value;
            if (m_Values.TryGetValue(name, out value)) return value;
            if (TryGetValue(obj, name, out var val)) return val;
            var userdataMethod = GetMethod(name);
            if (userdataMethod != null) return m_InstanceMethods[name] = new ScriptValue(script.NewInstanceMethod().Set(name, userdataMethod));
            throw new ExecutionException($"GetValue Type:[{m_Type.FullName}] 实例变量:[{name}]不存在");
        }


        /// <summary> 获取函数 </summary>
        protected abstract UserdataMethod GetMethod(string name);
        /// <summary> 创建一个实例 </summary>
        public abstract ScriptUserdata CreateInstance(Script script, ScriptValue[] parameters, int length);
        /// <summary> 获取一个变量的类型,只能获取 Field Property Event </summary>
        public abstract Type GetVariableType(string name);
        protected abstract bool TryGetValue(object obj, string name, out object value);
        /// <summary> 设置一个类变量 </summary>
        public abstract void SetValue(object obj, string name, ScriptValue value);
    }
}
