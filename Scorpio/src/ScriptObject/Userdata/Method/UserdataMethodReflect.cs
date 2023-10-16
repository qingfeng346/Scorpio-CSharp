using System;
using System.Collections.Generic;
using System.Reflection;
using Scorpio.Tools;
namespace Scorpio.Userdata {
    public class UserdataMethodReflect : UserdataMethod {
        //构造函数
        public UserdataMethodReflect(Type type, string methodName, ConstructorInfo[] methods) :
            base(type, methodName) {
            Initialize(new List<MethodBase>(methods), true);
        }
        //普通函数
        public UserdataMethodReflect(Type type, string methodName, IEnumerable<MethodInfo> methods) :
            base(type, methodName) {
            Initialize(new List<MethodBase>(methods), false);
        }
        //空白扩展函数
        public UserdataMethodReflect(Type type, string methodName) : base(type, methodName) {
            m_Methods = new FunctionData[0];
        }
        protected void Initialize(List<MethodBase> methods, bool isConstructor) {
            var functionMethod = new List<FunctionData>();          //实例函数
            var functionStaticMethod = new List<FunctionData>();    //静态函数
            var genericMethods = new List<FunctionDataGeneric>();   //模板函数

            var parameterTypes = new List<Type>();                  //参数类型
            var defaultParameter = new List<object>();              //默认参数
            var refOut = new List<bool>();                          //是否是 ref out

            var length = methods.Count;                             //函数数量
            var requiredNumber = 0;                                 //必须的参数个数
            var hasRefOut = false;                                  //是否包含 ref out 参数
            Type paramType = null;                                  //变长参数类型
            FunctionData functionData;
            Type[] parType = null;
            object[] defPar = null;
            bool[] refO = null;
            for (int i = 0; i < length; ++i) {
                var method = methods[i];
                if (method.IsGenericMethod()) {
                    genericMethods.Add(new FunctionDataGeneric(method as MethodInfo));
                } else {
                    var parameters = method.GetParameters();            //所有参数
                    ParseParameters(parameters, 0, parameterTypes, defaultParameter, refOut, ref hasRefOut, ref requiredNumber, ref paramType);
                    parType = parameterTypes.Count > 0 ? parameterTypes.ToArray() : ScorpioUtil.TYPE_EMPTY;
                    defPar = defaultParameter.Count > 0 ? defaultParameter.ToArray() : ScorpioUtil.OBJECT_EMPTY;
                    refO = refOut.Count > 0 ? refOut.ToArray() : ScorpioUtil.BOOL_EMPTY;
                    if (method is ConstructorInfo) {
                        if (hasRefOut) {
                            functionData = new FunctionDataConstructorWithRefOut(method as ConstructorInfo, parType, defPar, refO, requiredNumber, paramType);
                        } else {
                            functionData = new FunctionDataConstructor(method as ConstructorInfo, parType, defPar, refO, requiredNumber, paramType);
                        }
                    } else {
                        if (hasRefOut) {
                            functionData = new FunctionDataReflectMethodWithRefOut(method as MethodInfo, parType, defPar, refO, requiredNumber, paramType);
                        } else {
                            functionData = new FunctionDataReflectMethod(method as MethodInfo, parType, defPar, refO, requiredNumber, paramType);
                        }
                    }
                    if (functionData.IsStatic)
                        functionStaticMethod.Add(functionData);
                    else
                        functionMethod.Add(functionData);
                }
            }
            if (isConstructor && m_Type.IsValueType) {
                functionMethod.Add(new FunctionDataStructConstructor(m_Type));
            }
            functionMethod.Sort((a, b) => { return a.Priority - b.Priority; });
            functionStaticMethod.Sort((a, b) => { return a.Priority - b.Priority; });
            m_Methods = functionMethod.ToArray();
            m_StaticMethods = functionStaticMethod.ToArray();
            m_GenericMethods = genericMethods.ToArray();
        }
        //添加一个扩展函数
        public void AddExtensionMethod(MethodInfo method) {
            var length = m_Methods.Length;
            for (var i = 0; i < length; ++i) {
                var methodInfo = m_Methods[i] as FunctionDataExtensionMethod;
                if (methodInfo != null && methodInfo.Method == method) { return; }
            }
            Array.Resize(ref m_Methods, length + 1);
            var parameterTypes = new List<Type>();                  //参数类型
            var defaultParameter = new List<object>();              //默认参数
            var refOut = new List<bool>();                          //是否是 ref out
            var requiredNumber = 0;                                 //必须的参数个数
            var hasRefOut = false;                                  //是否包含 ref out 参数
            Type paramType = null;                                  //变长参数类型
            ParseParameters(method.GetParameters(), 0, parameterTypes, defaultParameter, refOut, ref hasRefOut, ref requiredNumber, ref paramType);
            var parType = parameterTypes.Count > 0 ? parameterTypes.ToArray() : ScorpioUtil.TYPE_EMPTY;
            var defPar = defaultParameter.Count > 0 ? defaultParameter.ToArray() : ScorpioUtil.OBJECT_EMPTY;
            var refO = refOut.Count > 0 ? refOut.ToArray() : ScorpioUtil.BOOL_EMPTY;
            if (hasRefOut) {
                m_Methods[length] = new FunctionDataExtensionMethodWithRefOut(method, parType, defPar, refO, requiredNumber, paramType);
            } else {
                m_Methods[length] = new FunctionDataExtensionMethod(method, parType, defPar, refO, requiredNumber, paramType);
            }
        }
        void ParseParameters(ParameterInfo[] parameters, int begin, List<Type> parameterTypes, List<object> defaultParameter, List<bool> refOut, ref bool hasRefOut,  ref int requiredNumber, ref Type paramType) {
            var hadDefault = false;
            hasRefOut = false;
            paramType = null;
            parameterTypes.Clear();
            defaultParameter.Clear();
            refOut.Clear();
            for (int i = begin; i < parameters.Length; ++i) {
                var parameter = parameters[i];
                if (parameter.IsRetvalOrOut()) {
                    hasRefOut = true;
                    refOut.Add(true);
                    parameterTypes.Add(parameter.ParameterType.GetElementType());
                } else {
                    refOut.Add(false);
                    parameterTypes.Add(parameter.ParameterType);
                }
                if (parameter.DefaultValue != DBNull.Value) {
                    if (!hadDefault) {
                        hadDefault = true;
                        requiredNumber = i - begin;
                    }
                    defaultParameter.Add(parameter.DefaultValue);
                } else {
                    defaultParameter.Add(null);
                }
                if (parameter.IsParams()) {
                    paramType = parameter.ParameterType.GetElementType();
                    break;
                }
            }
            if (!hadDefault) { requiredNumber = parameterTypes.Count; }
        }
    }
}
