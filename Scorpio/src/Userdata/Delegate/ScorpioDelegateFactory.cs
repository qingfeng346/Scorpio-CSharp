using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Scorpio.Commons {
    public class ScorpioDelegateFactory {
        class DelegateData {
            public bool ret;
            public int length;
            public Type[] types;
        }
        private static MethodInfo[] m_VoidMethods = new MethodInfo[32];     //无返回值的数组 有效值只有 0 - 16
        private static MethodInfo[] m_Methods = new MethodInfo[32];         //有返回值的数组 有效值只有 0 - 16
        private static Dictionary<Type, DelegateData> m_DelegateDatas = new Dictionary<Type, DelegateData>();
        static ScorpioDelegateFactory() {
            var methods = typeof(ScriptDelegate).GetMethods(Script.BindingFlag);
            foreach (var method in methods) {
                if (method.Name == "Action") {
                    m_VoidMethods[method.GetParameters().Length] = method;
                } else if (method.Name == "Func") {
                    m_Methods[method.GetParameters().Length] = method;
                }
            }
        }
        static DelegateData GetDelegateData(Type delegateType) {
            if (m_DelegateDatas.ContainsKey(delegateType)) { return m_DelegateDatas[delegateType]; }
            var data = new DelegateData();
            var method = delegateType.GetMethod("Invoke");
            var parameters = method.GetParameters();
            var length = parameters.Length;
            var isVoid = method.ReturnType == Util.TYPE_VOID;
            var types = new Type[isVoid ? length : length + 1];
            for (var i = 0; i < length; ++i) {
                types[i] = parameters[i].ParameterType;
            }
            data.length = length;
            if (isVoid) {
                data.types = types;
                data.ret = false;
            } else {
                types[length] = method.ReturnType;
                data.types = types;
                data.ret = true;
            }
            return m_DelegateDatas[delegateType] = data;
        }
        public static Delegate CreateDelegate(Script script, Type delegateType, ScriptObject scriptObject) {
            var data = GetDelegateData(delegateType);
            var func = new ScriptDelegate(script, scriptObject);
            if (data.ret) {
                return Delegate.CreateDelegate(delegateType, func, m_Methods[data.length].MakeGenericMethod(data.types));
            } else if (data.length == 0) {
                return Delegate.CreateDelegate(delegateType, func, m_VoidMethods[0]);
            } else {
                return Delegate.CreateDelegate(delegateType, func, m_VoidMethods[data.length].MakeGenericMethod(data.types));
            }
        }
    }
    public class ScriptDelegate {
        private Script m_Script;
        private ScriptObject m_Func;
        public ScriptDelegate(Script script, ScriptObject func) {
            m_Script = script;
            m_Func = func;
        }
        public void Action() {
            m_Func.call(ScriptValue.Null);
        }
        public void Action<T1>(T1 t1) {
            m_Func.call(ScriptValue.Null, t1);
        }
        public void Action<T1, T2>(T1 t1, T2 t2) {
            m_Func.call(ScriptValue.Null, t1, t2);
        }
        public void Action<T1, T2, T3>(T1 t1, T2 t2, T3 t3) {
            m_Func.call(ScriptValue.Null, t1, t2, t3);
        }
        public void Action<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4) {
            m_Func.call(ScriptValue.Null, t1, t2, t3, t4);
        }
        public void Action<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) {
            m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5);
        }
        public void Action<T1, T2, T3, T4, T5, T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) {
            m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6);
        }
        public void Action<T1, T2, T3, T4, T5, T6, T7>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) {
            m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7);
        }
        public void Action<T1, T2, T3, T4, T5, T6, T7, T8>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) {
            m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7, t8);
        }
        public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) {
            m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7, t8, t9);
        }
        public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10) {
            m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
        }
        public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11) {
            m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
        }
        public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12) {
            m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
        }
        public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13) {
            m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
        }
        public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14) {
            m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
        }
        public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15) {
            m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
        }
        public void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16) {
            m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
        }

        public TResult Func<TResult>() {
            return (TResult)Util.ChangeType(m_Script, m_Func.call(ScriptValue.Null), typeof(TResult));
        }
        public TResult Func<T1, TResult>(T1 t1) {
            return (TResult)Util.ChangeType(m_Script, m_Func.call(ScriptValue.Null, t1), typeof(TResult));
        }
        public TResult Func<T1, T2, TResult>(T1 t1, T2 t2) {
            return (TResult)Util.ChangeType(m_Script, m_Func.call(ScriptValue.Null, t1, t2), typeof(TResult));
        }
        public TResult Func<T1, T2, T3, TResult>(T1 t1, T2 t2, T3 t3) {
            return (TResult)Util.ChangeType(m_Script, m_Func.call(ScriptValue.Null, t1, t2, t3), typeof(TResult));
        }
        public TResult Func<T1, T2, T3, T4, TResult>(T1 t1, T2 t2, T3 t3, T4 t4) {
            return (TResult)Util.ChangeType(m_Script, m_Func.call(ScriptValue.Null, t1, t2, t3, t4), typeof(TResult));
        }
        public TResult Func<T1, T2, T3, T4, T5, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) {
            return (TResult)Util.ChangeType(m_Script, m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5), typeof(TResult));
        }
        public TResult Func<T1, T2, T3, T4, T5, T6, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) {
            return (TResult)Util.ChangeType(m_Script, m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6), typeof(TResult));
        }
        public TResult Func<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) {
            return (TResult)Util.ChangeType(m_Script, m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7), typeof(TResult));
        }
        public TResult Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) {
            return (TResult)Util.ChangeType(m_Script, m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7, t8), typeof(TResult));
        }
        public TResult Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) {
            return (TResult)Util.ChangeType(m_Script, m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7, t8, t9), typeof(TResult));
        }
        public TResult Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10) {
            return (TResult)Util.ChangeType(m_Script, m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10), typeof(TResult));
        }
        public TResult Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11) {
            return (TResult)Util.ChangeType(m_Script, m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11), typeof(TResult));
        }
        public TResult Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12) {
            return (TResult)Util.ChangeType(m_Script, m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12), typeof(TResult));
        }
        public TResult Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13) {
            return (TResult)Util.ChangeType(m_Script, m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13), typeof(TResult));
        }
        public TResult Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14) {
            return (TResult)Util.ChangeType(m_Script, m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14), typeof(TResult));
        }
        public TResult Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15) {
            return (TResult)Util.ChangeType(m_Script, m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15), typeof(TResult));
        }
        public TResult Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16) {
            return (TResult)Util.ChangeType(m_Script, m_Func.call(ScriptValue.Null, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16), typeof(TResult));
        }
    }
}
