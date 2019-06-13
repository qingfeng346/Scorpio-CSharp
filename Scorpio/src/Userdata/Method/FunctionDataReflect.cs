using System;
using System.Reflection;
using Scorpio.Tools;
using Scorpio.Exception;
namespace Scorpio.Userdata {
    //反射函数数据
    public abstract class FunctionDataReflect : FunctionData {
        public MethodInfo Method;                   //普通函数对象(只有普通函数和扩展函数有用)
        public FunctionDataReflect(MethodInfo method, Type[] parameterType, object[] defaultParameter, bool[] refOut, int requiredNumber, Type paramType) :
            base(parameterType, defaultParameter, refOut, requiredNumber, paramType) {
            this.Method = method;
        }
        public override bool IsStatic => Method.IsStatic;
        public override bool IsGeneric => Util.IsGenericMethod(Method);
    }
    //普通反射函数
    public class FunctionDataReflectMethod : FunctionDataReflect {
        public FunctionDataReflectMethod(MethodInfo method, Type[] parameterType, object[] defaultParameter, bool[] refOut, int requiredNumber, Type paramType) :
            base(method, parameterType, defaultParameter, refOut, requiredNumber, paramType) {
        }
        public override object Invoke(object obj, ScriptValue[] parameters) {
            return Method.Invoke(obj, Args);
        }
    }
    //扩展反射函数
    public class FunctionDataReflectExtension : FunctionDataReflect {
        protected object[] FinishArgs;
        public FunctionDataReflectExtension(MethodInfo method, Type[] parameterType, object[] defaultParameter, bool[] refOut, int requiredNumber, Type paramType) :
            base(method, parameterType, defaultParameter, refOut, requiredNumber, paramType) {
            this.FinishArgs = new object[Args.Length + 1];
        }
        public override bool IsStatic => false;
        public override object Invoke(object obj, ScriptValue[] parameters) {
            FinishArgs[0] = obj;
            Array.Copy(Args, 0, FinishArgs, 1, Args.Length);
            return Method.Invoke(null, FinishArgs);
        }
    }
    //带 ref out 参数的 普通反射函数
    public class FunctionDataReflectMethodWithRefOut : FunctionDataReflectMethod {
        public FunctionDataReflectMethodWithRefOut(MethodInfo method, Type[] parameterType, object[] defaultParameter, bool[] refOut, int requiredNumber, Type paramType) :
            base(method, parameterType, defaultParameter, refOut, requiredNumber, paramType) {
        }
        public override object Invoke(object obj, ScriptValue[] parameters) {
            var ret = Method.Invoke(obj, Args);
            for (var i = 0; i < RequiredNumber; ++i) {
                if (RefOuts[i]) {
                    var instance = parameters[i].Get<ScriptInstance>();
                    if (instance == null) throw new ExecutionException($"带 ref out 标识的字段,必须传入 map, Index : {i}");
                    instance.SetValue("value", ScriptValue.CreateObject(Args[i]));
                }
            }
            return ret;
        }
    }
    //带 ref out 参数的 扩展反射函数
    public class FunctionDataReflectExtensionWithRefOut : FunctionDataReflectExtension {
        public FunctionDataReflectExtensionWithRefOut(MethodInfo method, Type[] parameterType, object[] defaultParameter, bool[] refOut, int requiredNumber, Type paramType) :
            base(method, parameterType, defaultParameter, refOut, requiredNumber, paramType) {
        }
        public override object Invoke(object obj, ScriptValue[] parameters) {
            FinishArgs[0] = obj;
            Array.Copy(Args, 0, FinishArgs, 1, Args.Length);
            var ret = Method.Invoke(null, FinishArgs);
            for (var i = 0; i < RequiredNumber; ++i) {
                if (RefOuts[i]) {
                    var instance = parameters[i].Get<ScriptInstance>();
                    if (instance == null) throw new ExecutionException($"带 ref out 标识的字段,必须传入 map, Index : {i}");
                    instance.SetValue("value", ScriptValue.CreateObject(FinishArgs[i + 1]));
                }
            }
            return ret;
        }
    }
}
