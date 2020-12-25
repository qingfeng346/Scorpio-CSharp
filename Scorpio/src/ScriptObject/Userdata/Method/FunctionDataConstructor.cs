using System;
using System.Reflection;
using Scorpio.Exception;
namespace Scorpio.Userdata {
    //反射构造函数
    public class FunctionDataConstructor : FunctionData {
        public ConstructorInfo Constructor;         //构造函数对象
        public FunctionDataConstructor(ConstructorInfo constructor, Type[] parameterType, object[] defaultParameter, bool[] refOut, int requiredNumber, Type paramType) :
            base(parameterType, defaultParameter, refOut, requiredNumber, paramType) {
            this.Constructor = constructor;
        }
        public override object Invoke(object obj, ScriptValue[] parameters) {
            return Constructor.Invoke(Args);
        }
    }
    //包含 ref out 参数的构造函数
    public class FunctionDataConstructorWithRefOut : FunctionDataWithRefOut {
        public ConstructorInfo Constructor;         //构造函数对象
        public FunctionDataConstructorWithRefOut(ConstructorInfo constructor, Type[] parameterType, object[] defaultParameter, bool[] refOut, int requiredNumber, Type paramType) :
            base(parameterType, defaultParameter, refOut, requiredNumber, paramType) {
            this.Constructor = constructor;
        }
        public override object Invoke(object obj, ScriptValue[] parameters) {
            var ret = Constructor.Invoke(Args);
            for (var i = 0; i < RequiredNumber; ++i) {
                if (RefOuts[i]) {
                    var instance = parameters[i].Get<ScriptInstance>();
                    if (instance == null) throw new ExecutionException($"带 ref out 标识的字段,必须传入 map, Index : {i}");
                    instance.SetValue(RefOutValue, ScriptValue.CreateValue(Args[i]));
                }
            }
            return ret;
        }
    }
    //无参结构体构造函数
    public class FunctionDataStructConstructor : FunctionData {
        private Type m_Type;
        public FunctionDataStructConstructor(Type type) : base(EmptyTypes, null, EmptyBool, 0, null) {
            m_Type = type;
        }
        public override object Invoke(object obj, ScriptValue[] parameters) {
            return Activator.CreateInstance(m_Type);
        }
    }
}
