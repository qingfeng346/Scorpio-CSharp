using System;
using System.Reflection;
namespace Scorpio.Userdata {
    //反射构造函数
    public class FunctionDataConstructor : FunctionData {
        public ConstructorInfo Constructor;         //构造函数对象
        public FunctionDataConstructor(ConstructorInfo constructor, Type[] parameterType, object[] defaultParameter, bool[] refOut, int requiredNumber, Type paramType) :
            base(parameterType, defaultParameter, refOut, requiredNumber, paramType) {
            this.Constructor = constructor;
        }
        public override object Invoke(object obj) {
            return Constructor.Invoke(Args);
        }
    }
    //无参结构体构造函数
    public class FunctionDataStructConstructor : FunctionData {
        private readonly static Type[] EmptyTypes = new Type[0];
        private Type m_Type;
        public FunctionDataStructConstructor(Type type) : base(EmptyTypes, null, null, 0, null) {
            m_Type = type;
        }
        public override object Invoke(object obj) {
            return Activator.CreateInstance(m_Type);
        }
    }
}
