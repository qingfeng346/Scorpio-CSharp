using System;
using Scorpio.Tools;
using Scorpio.Exception;
using System.Collections.Generic;
using System.Reflection;
namespace Scorpio.Userdata {
    public abstract class UserdataMethod {
        protected Type m_Type;                                  //所在类

        protected FunctionData[] m_Methods;                     //所有函数
        protected FunctionData[] m_StaticMethods;               //所有静态函数
        protected FunctionData[] m_GenericMethods;              //所有模板函数
        protected int m_GenericMethodCount = 0;                 //模板函数数量

        public string MethodName { get; protected set; }        //函数名字
        public bool IsStructConstructor { get; protected set; } //是否是结构体构造函数,结构体无参构造函数不能获取到

        public UserdataMethod(Type type, string methodName) {
            m_Type = type;
            MethodName = type.FullName + "." + methodName;
        }
        //创建一个模板函数
        public UserdataMethodGeneric MakeGenericMethod(Type[] parameters) {
            for (var i = 0; i < m_GenericMethodCount; ++i) {
                var method = m_GenericMethods[i];
                var types = method.Method.GetGenericArguments();
                var length = types.Length;
                if (length == parameters.Length) {
                    var accord = true;
                    for (var j = 0; j < length; ++j) {
                        if (!types[j].GetTypeInfo().BaseType.GetTypeInfo().IsAssignableFrom(parameters[j])) {
                            accord = false;
                            break;
                        }
                    }
                    if (accord) {
                        return new UserdataMethodGeneric(m_Type, MethodName, method.Method.MakeGenericMethod(parameters));
                    }
                }
            }
            throw new ExecutionException("没有找到合适的泛型函数 " + MethodName);
        }
        //优先检查无默认值，非不定参的函数
        bool CheckNormalType(ScriptValue[] parameters, int length, Type[] parameterTypes, int count) {
            for (var i = 0; i < count; ++i) {
                if (!Util.CanChangeType(parameters[i], parameterTypes[i]))
                    return false;
            }
            return true;
        }
        //检查有默认参数的函数
        bool CheckDefaultType(ScriptValue[] parameters, int length, Type[] parameterTypes, int requiredNumber, int count) {
            if (length < requiredNumber || length > count)
                return false;
            for (int i = 0; i < length; ++i) {
                if (!Util.CanChangeType(parameters[i], parameterTypes[i]))
                    return false;
            }
            return true;
        }
        //检查不定参函数
        bool CheckArgsType(ScriptValue[] parameters, int length, Type[] parameterTypes, Type paramType, int requiredNumber, int count) {
            if (length < requiredNumber) {
                return false;
            }
            for (int i = 0; i < length; ++i) {
                if (!Util.CanChangeType(parameters[i], i >= count ? paramType : parameterTypes[i])) {
                    return false;
                }
            }
            return true;
        }
        public object Call(bool isStatic,object obj, ScriptValue[] parameters, int length) {
            try {
                //无参结构体构造函数
                if (IsStructConstructor && length == 0) { return Activator.CreateInstance(m_Type); }
                //如果obj为null 则调用静态函数
                var methods = isStatic ? m_StaticMethods : m_Methods;
                var methodLength = methods.Length;
                FunctionData methodInfo = null;
                for (var i = 0; i < methodLength; ++i) {
                    var method = methods[i];
                    if (method.IsParams || method.HasDefault || length != method.ParameterCount) { continue; }
                    if (CheckNormalType(parameters, length, method.ParameterType, method.ParameterCount)) {
                        methodInfo = method;
                        goto callMethod;
                    }
                }
                for (int i = 0; i < methodLength; ++i) {
                    var method = methods[i];
                    if (method.IsParams || !method.HasDefault) { continue; }
                    if (CheckDefaultType(parameters, length, method.ParameterType, method.RequiredNumber, method.ParameterCount)) {
                        methodInfo = method;
                        goto callMethod;
                    }
                }
                for (int i = 0; i < methodLength; ++i) {
                    var method = methods[i];
                    if (!method.IsParams) { continue; }
                    if (CheckArgsType(parameters, length, method.ParameterType, method.ParamType, method.RequiredNumber, method.ParameterCount)) {
                        methodInfo = method;
                        goto callArgMethod;
                    }
                }
                throw new ExecutionException($"类[{m_Type.ToString()}]找不到合适的函数[{MethodName}]");
            callMethod:
                {
                    var requiredNumber = methodInfo.RequiredNumber;       //函数必须传入的参数个数
                    var parameterCount = methodInfo.ParameterCount;       //参数个数
                    var args = methodInfo.Args;                           //参数数组
                    for (int i = 0; i < parameterCount; ++i)
                        args[i] = i >= length ? methodInfo.DefaultParameter[i - requiredNumber] : Util.ChangeType(parameters[i], methodInfo.ParameterType[i]);
                    return methodInfo.Invoke(obj);
                }
            callArgMethod:
                {
                    var requiredNumber = methodInfo.RequiredNumber;       //函数必须传入的参数个数
                    var parameterCount = methodInfo.ParameterCount;       //参数个数
                    var args = methodInfo.Args;                           //参数数组
                    for (int i = 0; i < parameterCount; ++i)
                        args[i] = i >= length ? methodInfo.DefaultParameter[i - requiredNumber] : Util.ChangeType(parameters[i], methodInfo.ParameterType[i]);
                    if (length > parameterCount) {
                        Array array = Array.CreateInstance(methodInfo.ParamType, length - parameterCount);
                        for (int i = parameterCount; i < length; ++i)
                            array.SetValue(Util.ChangeType(parameters[i], methodInfo.ParamType), i - parameterCount);
                        args[parameterCount] = array;
                    } else {
                        args[parameterCount] = Array.CreateInstance(methodInfo.ParamType, 0);
                    }
                    return methodInfo.Invoke(obj);
                }
            } catch (System.Exception e) {
                throw new ExecutionException($"类[{m_Type.ToString()}] 调用函数出错 [{MethodName}] : " + e.ToString());
            }
        }
    }
}
