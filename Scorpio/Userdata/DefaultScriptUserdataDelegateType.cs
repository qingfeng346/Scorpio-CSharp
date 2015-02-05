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
        private class DynamicDelegate
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
                return Util.ChangeType((ScriptObject)m_Function.call(args), m_ReturnType);
            }
        }

        private Type m_DelegateType;            //委托类型
        private Type m_ReturnType;              //返回值类型
        private DynamicMethod MethodFactory;    //动态函数
        public DefaultScriptUserdataDelegateType(Script script, Type value) : base(script)
        {
            this.Value = value;
            this.ValueType = value;

            var InvokeMethod = value.GetMethod("Invoke");
            m_DelegateType = value;
            m_ReturnType = InvokeMethod.ReturnType;
            List<Type> argTypes = new List<Type>();
            argTypes.Add(DynamicDelegateType);
            foreach (var p in InvokeMethod.GetParameters()) {
                argTypes.Add(p.ParameterType);
            }
            MethodFactory = new DynamicMethod(Script.DynamicDelegateName, m_ReturnType, argTypes.ToArray(), DynamicDelegateType);
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
        public Delegate CreateDelegate(ScriptFunction func)
        {
            return MethodFactory.CreateDelegate(m_DelegateType, new DynamicDelegate(func, m_ReturnType));
        }
        public override object Call(ScriptObject[] parameters)
        {
            return CreateDelegate(parameters[0] as ScriptFunction);
        }
    }
}
