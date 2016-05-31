using System;

#if SCORPIO_DYNAMIC_DELEGATE
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using Scorpio.Exception;
#endif

namespace Scorpio.Userdata
{
    public interface DelegateTypeFactory {
        Delegate CreateDelegate(Script script, Type type, ScriptFunction func);
    }
#if !SCORPIO_DYNAMIC_DELEGATE
    public class DefaultScriptUserdataDelegateType : ScriptUserdata
    {
        private static DelegateTypeFactory m_Factory = null;
        public static void SetFactory(DelegateTypeFactory factory) { m_Factory = factory; }
        public DefaultScriptUserdataDelegateType(Script script, Type value) : base(script)
        {
            this.m_Value = value;
            this.m_ValueType = value;
        }
        public override object Call(ScriptObject[] parameters)
        {
            return m_Factory != null ? m_Factory.CreateDelegate(m_Script, m_ValueType, parameters[0] as ScriptFunction) : null;
        }
    }
#else
    /// <summary> 动态委托类型(声明) </summary>
    public class DefaultScriptUserdataDelegateType : ScriptUserdata
    {
        public static void SetFactory(DelegateTypeFactory factory) {  }
        private static readonly MethodInfo DynamicDelegateMethod;
        private static readonly Type DynamicDelegateType = typeof(DynamicDelegate);
        static DefaultScriptUserdataDelegateType()
        {
            DynamicDelegateMethod = DynamicDelegateType.GetMethod("MyFunction");
        }
        private class DynamicDelegate
        {
            private Script m_Script;
            private Type m_ReturnType;
            private ScriptFunction m_Function;
            public DynamicDelegate(Script script, ScriptFunction function, Type returnType)
            {
                m_Script = script;
                m_Function = function;
                m_ReturnType = returnType;
            }
            public object MyFunction(object[] args)
            {
                if (Util.IsVoid(m_ReturnType))
                    return m_Function.call(args);
                ScriptObject ret = (ScriptObject)m_Function.call(args) ??  m_Script.Null;
                if (Util.CanChangeType(ret, m_ReturnType))
                    return Util.ChangeType(m_Script, ret, m_ReturnType);
                throw new ExecutionException(m_Script, "委托返回值不能从源类型:" + (ret.IsNull ? "null" : ret.ObjectValue.GetType().Name) + " 转换成目标类型:" + m_ReturnType.Name);
            }
        }
        private Type m_DelegateType;            //委托类型
        private Type m_ReturnType;              //返回值类型
        private DynamicMethod MethodFactory;    //动态函数
        public DefaultScriptUserdataDelegateType(Script script, Type value) : base(script)
        {
            this.m_Value = value;
            this.m_ValueType = value;
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
            for (int i = 1; i < argTypes.Count; ++i) {
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
        public override object Call(ScriptObject[] parameters)
        {
            return MethodFactory.CreateDelegate(m_DelegateType, new DynamicDelegate(m_Script, parameters[0] as ScriptFunction, m_ReturnType));
        }
    }
#endif
}
