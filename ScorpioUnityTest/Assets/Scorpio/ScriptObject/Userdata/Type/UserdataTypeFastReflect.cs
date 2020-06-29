using System;

namespace Scorpio.Userdata {
    //去反射类
    public interface ScorpioFastReflectClass {
        UserdataMethodFastReflect GetConstructor();                     //获取构造函数
        Type GetVariableType(string name);                              //获取变量类型
        UserdataMethod GetMethod(string name);                          //获取类函数
        object GetValue(object obj, string name);                       //获取变量
        void SetValue(object obj, string name, ScriptValue value);      //设置变量
    }
    //去反射函数
    public interface ScorpioFastReflectMethod {
        object Call(object obj, int methodIndex, object[] args);
    }
    //单个去反射函数的信息
    public class ScorpioFastReflectMethodInfo {
        public bool IsStatic;           //是否是静态函数
        public Type[] ParameterType;    //参数类型列表
        public bool[] RefOut;           //ref out
        public Type ParamType;          //不定参类型
        public int MethodIndex;         //函数索引
        public ScorpioFastReflectMethodInfo(bool isStatic, Type[] parameterType, bool[] refOut, Type paramType, int methodIndex) {
            this.IsStatic = isStatic;
            this.ParameterType = parameterType;
            this.RefOut = refOut;
            this.ParamType = paramType;
            this.MethodIndex = methodIndex;
        }
    }
    //快速反射类管理
    public class UserdataTypeFastReflect : UserdataType {
        private ScorpioFastReflectClass m_Value;
        private UserdataMethodFastReflect m_Constructor;
        public UserdataTypeFastReflect(Type type, ScorpioFastReflectClass value) : base(type) {
            m_Value = value;
            m_Constructor = value.GetConstructor();
        }
        public override ScriptUserdata CreateInstance(ScriptValue[] parameters, int length) {
            return new ScriptUserdataObject(m_Constructor.Call(false, null, parameters, length), this);
        }
        public override Type GetVariableType(string name) {
            return m_Value.GetVariableType(name);
        }
        public override UserdataMethod GetMethod(string name) {
            return m_Value.GetMethod(name);
        }
        public override object GetValue(object obj, string name) {
            return m_Value.GetValue(obj, name);
        }
        public override void SetValue(object obj, string name, ScriptValue value) {
            m_Value.SetValue(obj, name, value);
        }
        public ScorpioFastReflectClass FastReflectClass { get { return m_Value; } }
    }
}
