using Scorpio.Exception;
using System;

namespace Scorpio.Userdata {
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
        private IScorpioFastReflectClass m_FastReflectClass;
        private UserdataMethodFastReflect m_Constructor;
        public UserdataTypeFastReflect(Type type, IScorpioFastReflectClass value) : base(type) {
            m_FastReflectClass = value;
            m_Constructor = value.GetConstructor();
        }
        public IScorpioFastReflectClass FastReflectClass => m_FastReflectClass;
        public override ScriptUserdata CreateInstance(Script script, ScriptValue[] parameters, int length) {
            return script.NewUserdataObject().Set(this, m_Constructor.Call(script, false, null, parameters, length));
        }
        public override Type GetVariableType(string name) {
            return m_FastReflectClass.GetVariableType(name);
        }
        protected override UserdataMethod GetMethod(string name) {
            return m_FastReflectClass.GetMethod(name);
        }
        protected override bool TryGetValue(object obj, string name, out object value) {
            return m_FastReflectClass.TryGetValue(obj, name, out value);
        }
        public override void SetValue(object obj, string name, ScriptValue value) {
            m_FastReflectClass.SetValue(obj, name, value);
        }

    }
}
