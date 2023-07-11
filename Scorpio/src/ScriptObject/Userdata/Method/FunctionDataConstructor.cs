using System;
using System.Reflection;
using Scorpio.Exception;
using Scorpio.Tools;

namespace Scorpio.Userdata {
    //反射构造函数
    public class FunctionDataConstructor : FunctionData {
        public ConstructorInfo Constructor;         //构造函数对象
        public FunctionDataConstructor(ConstructorInfo constructor, Type[] parameterType, object[] defaultParameter, bool[] refOut, int requiredNumber, Type paramType) :
            base(parameterType, defaultParameter, refOut, requiredNumber, paramType) {
            this.Constructor = constructor;
        }
        public override object Invoke(Script script, object obj, ScriptValue[] parameters) {
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
        public override object Invoke(Script script, object obj, ScriptValue[] parameters) {
            var ret = Constructor.Invoke(Args);
            for (var i = 0; i < RequiredNumber; ++i) {
                if (RefOuts[i]) {
                    var instance = parameters[i].Get<ScriptInstance>();
                    if (instance == null) throw new ExecutionException($"带 ref out 标识的字段,必须传入 map, 索引 : {i}");
                    instance.SetValueNoReference(RefOutValue, ScriptValue.CreateValue(script, Args[i]));
                }
            }
            return ret;
        }
    }
    //无参结构体构造函数
    public class FunctionDataStructConstructor : FunctionData {
        private Type m_Type;
        public FunctionDataStructConstructor(Type type) : base(ScorpioUtil.TYPE_EMPTY, null, ScorpioUtil.BOOL_EMPTY, 0, null) {
            m_Type = type;
        }
        public override object Invoke(Script script, object obj, ScriptValue[] parameters) {
            return Activator.CreateInstance(m_Type);
        }
    }
}
