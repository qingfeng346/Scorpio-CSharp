using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
namespace Scorpio.Userdata
{
    /// <summary> 动态委托类型(声明) </summary>
    public class DefaultScriptUserdataDelegateType : ScriptUserdata
    {
        private static readonly MethodInfo DynamicDelegateMethod;
        private static readonly Type DynamicDelegateType = typeof(DynamicDelegate);
        static DefaultScriptUserdataDelegateType()
        {
            DynamicDelegateMethod = DynamicDelegateType.GetMethod("MyFunction");
        }
        public class DynamicDelegate
        {
            private Type m_ReturnType;
            private ScriptFunction m_Function;
            public DynamicDelegate(ScriptFunction function, Type returnType)
            {
                m_Function = function;
                m_ReturnType = returnType;
            }
            public object MyFunction(object[] args)
            {
                if (Util.IsVoid(m_ReturnType))
                    return m_Function.call(args);
                return Util.ChangeType(m_Function.call(args), m_ReturnType);
            }
        }
        private class DelegateInfo
        {
            public MethodInfo InvokeMethod;
            private Type m_DelegateType;
            private DynamicMethod MethodFactory;
            public DelegateInfo(Type type)
            {
                m_DelegateType = type;
                InvokeMethod = type.GetMethod("Invoke");
                Type returnType = InvokeMethod.ReturnType;
                List<Type> argTypes = new List<Type>();
                argTypes.Add(DynamicDelegateType);
                foreach (var p in InvokeMethod.GetParameters()) {
                    argTypes.Add(p.ParameterType);
                }
                MethodFactory = new DynamicMethod(Script.DynamicDelegateName, returnType, argTypes.ToArray(), DynamicDelegateType);
                ILGenerator generator = MethodFactory.GetILGenerator();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldc_I4, argTypes.Count - 1);
                generator.Emit(OpCodes.Newarr, typeof(object));
                for (int i = 1; i < argTypes.Count; ++i)
                {
                    generator.Emit(OpCodes.Dup);
                    generator.Emit(OpCodes.Ldc_I4, i - 1);
                    generator.Emit(OpCodes.Ldarg, i);
                    generator.Emit(OpCodes.Box, argTypes[i]);
                    generator.Emit(OpCodes.Stelem_Ref);
                }
                generator.Emit(OpCodes.Call, DynamicDelegateMethod);
                if (Util.IsVoid(InvokeMethod.ReturnType)) {
                    generator.Emit(OpCodes.Pop);
                    generator.Emit(OpCodes.Ret);
                } else {
                    generator.Emit(OpCodes.Unbox_Any, InvokeMethod.ReturnType);
                    generator.Emit(OpCodes.Ret);
                }
            }
            public Delegate CreateDelegate(DynamicDelegate dele)
            {
                return MethodFactory.CreateDelegate(m_DelegateType, dele);
            }
            public Type GetReturnType()
            {
                return InvokeMethod.ReturnType;
            }
        }
        private static Dictionary<Type, DelegateInfo> m_DynamicMethod = new Dictionary<Type, DelegateInfo>();
        private DelegateInfo m_Info;
        public DefaultScriptUserdataDelegateType(Script script, Type value) : base(script)
        {
            this.Value = value;
            this.ValueType = value;
            if (m_DynamicMethod.ContainsKey(value)) {
                m_Info = m_DynamicMethod[value];
            } else {
                m_Info = new DelegateInfo(value);
                m_DynamicMethod.Add(value, m_Info);
            }
        }
        public override ScriptObject Call(ScriptObject[] parameters)
        {
            return Script.CreateObject(m_Info.CreateDelegate(new DynamicDelegate((ScriptFunction)parameters[0], m_Info.GetReturnType())));
        }
    }
}
