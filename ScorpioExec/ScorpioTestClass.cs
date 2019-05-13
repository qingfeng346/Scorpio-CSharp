using System;
using System.Collections.Generic;
using System.Reflection;
using Scorpio;
using Scorpio.Userdata;
//using Scorpio.Variable;
using Scorpio.Commons;
public class ScorpioClass_TestClass : ScorpioFastReflectClass {
    private Script m_Script;
    public ScorpioClass_TestClass(Script script) {
        m_Script = script;
    }
    public UserdataMethodFastReflect GetConstructor() {
        return ScorpioClass_TestClass_Constructor.GetMethod(m_Script);
    }
    public Type GetVariableType(string name) {
        if (name == "testAction1") return typeof(System.Action);
        if (name == "testAction2") return typeof(System.Action<System.Int32>);
        if (name == "testAction3") return typeof(System.Func<System.Int32>);
        if (name == "testAction4") return typeof(System.Func<System.Int32,System.Int32,System.Int32,System.Int32>);
        if (name == "tttt") return typeof(System.Int32);
        if (name == "TestPro") return typeof(System.Int32);
        if (name == "Equals") return typeof(System.Boolean);
        if (name == "get_TestPro") return typeof(System.Int32);
        if (name == "GetHashCode") return typeof(System.Int32);
        if (name == "GetType") return typeof(System.Type);
        if (name == "op_Addition") return typeof(TestClass);
        if (name == "ReferenceEquals") return typeof(System.Boolean);
        if (name == "ToString") return typeof(System.String);
        throw new Exception("TestClass [GetVariableType] 找不到变量 : " + name);
    }
    public object GetValue(object obj, string name) {
        if (name == "testAction1") return ((TestClass)obj).testAction1;
        if (name == "testAction2") return ((TestClass)obj).testAction2;
        if (name == "testAction3") return ((TestClass)obj).testAction3;
        if (name == "testAction4") return ((TestClass)obj).testAction4;
        if (name == "tttt") return ((TestClass)obj).tttt;
        if (name == "TestPro") return ((TestClass)obj).TestPro;
        if (name == "Equals") return ScorpioClass_TestClass_Equals.GetMethod(m_Script);
        if (name == "get_TestPro") return ScorpioClass_TestClass_get_TestPro.GetMethod(m_Script);
        if (name == "GetHashCode") return ScorpioClass_TestClass_GetHashCode.GetMethod(m_Script);
        if (name == "GetType") return ScorpioClass_TestClass_GetType.GetMethod(m_Script);
        if (name == "op_Addition") return ScorpioClass_TestClass_op_Addition.GetMethod(m_Script);
        if (name == "ReferenceEquals") return ScorpioClass_TestClass_ReferenceEquals.GetMethod(m_Script);
        if (name == "set_TestPro") return ScorpioClass_TestClass_set_TestPro.GetMethod(m_Script);
        if (name == "Test") return ScorpioClass_TestClass_Test.GetMethod(m_Script);
        if (name == "ToString") return ScorpioClass_TestClass_ToString.GetMethod(m_Script);
        throw new Exception("TestClass [GetValue] 找不到变量 : " + name);
    }
    public void SetValue(object obj, string name, ScriptValue value) {
        if (name == "testAction1") { ((TestClass)obj).testAction1 = (System.Action)(Util.ChangeType(m_Script, value, typeof(System.Action))); return; }
        if (name == "testAction2") { ((TestClass)obj).testAction2 = (System.Action<System.Int32>)(Util.ChangeType(m_Script, value, typeof(System.Action<System.Int32>))); return; }
        if (name == "testAction3") { ((TestClass)obj).testAction3 = (System.Func<System.Int32>)(Util.ChangeType(m_Script, value, typeof(System.Func<System.Int32>))); return; }
        if (name == "testAction4") { ((TestClass)obj).testAction4 = (System.Func<System.Int32,System.Int32,System.Int32,System.Int32>)(Util.ChangeType(m_Script, value, typeof(System.Func<System.Int32,System.Int32,System.Int32,System.Int32>))); return; }
        if (name == "tttt") { ((TestClass)obj).tttt = (System.Int32)(Util.ChangeType(m_Script, value, typeof(System.Int32))); return; }
        if (name == "TestPro") { ((TestClass)obj).TestPro = (System.Int32)(Util.ChangeType(m_Script, value, typeof(System.Int32))); return; }
        throw new Exception("TestClass [SetValue] 找不到变量 : " + name);
    }

    public class ScorpioClass_TestClass_Constructor : ScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _method;
        public static UserdataMethodFastReflect GetMethod(Script script) {
            if (_method == null) {
                var methods = new List<ScorpioFastReflectMethodInfo>();
                methods.Add(new ScorpioFastReflectMethodInfo(false, new Type[] {}, null, 0));
                _method = new UserdataMethodFastReflect(script, typeof(TestClass), "Constructor", methods.ToArray(), new ScorpioClass_TestClass_Constructor()); 
            }
            return _method;
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
            case 0: return new TestClass();
            default: throw new Exception("TestClass 找不到合适的函数 : Constructor    type : " + methodIndex);
            }
        }
    }

    public class ScorpioClass_TestClass_Equals : ScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _method;
        public static UserdataMethodFastReflect GetMethod(Script script) {
            if (_method == null) {
                var methods = new List<ScorpioFastReflectMethodInfo>();
                methods.Add(new ScorpioFastReflectMethodInfo(false, new Type[] {typeof(System.Object)}, null, 0));
                methods.Add(new ScorpioFastReflectMethodInfo(true, new Type[] {typeof(System.Object),typeof(System.Object)}, null, 1));
                _method = new UserdataMethodFastReflect(script, typeof(TestClass), "Equals", methods.ToArray(), new ScorpioClass_TestClass_Equals()); 
            }
            return _method;
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
            case 0: { return ((TestClass)obj).Equals((System.Object)args[0]); }
            case 1: { return TestClass.Equals((System.Object)args[0],(System.Object)args[1]); }
            default: throw new Exception("TestClass 找不到合适的函数 : Equals    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_get_TestPro : ScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _method;
        public static UserdataMethodFastReflect GetMethod(Script script) {
            if (_method == null) {
                var methods = new List<ScorpioFastReflectMethodInfo>();
                methods.Add(new ScorpioFastReflectMethodInfo(false, new Type[] {}, null, 0));
                _method = new UserdataMethodFastReflect(script, typeof(TestClass), "get_TestPro", methods.ToArray(), new ScorpioClass_TestClass_get_TestPro()); 
            }
            return _method;
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
            case 0: { return ((TestClass)obj).TestPro; }
            default: throw new Exception("TestClass 找不到合适的函数 : get_TestPro    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_GetHashCode : ScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _method;
        public static UserdataMethodFastReflect GetMethod(Script script) {
            if (_method == null) {
                var methods = new List<ScorpioFastReflectMethodInfo>();
                methods.Add(new ScorpioFastReflectMethodInfo(false, new Type[] {}, null, 0));
                _method = new UserdataMethodFastReflect(script, typeof(TestClass), "GetHashCode", methods.ToArray(), new ScorpioClass_TestClass_GetHashCode()); 
            }
            return _method;
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
            case 0: { return ((TestClass)obj).GetHashCode(); }
            default: throw new Exception("TestClass 找不到合适的函数 : GetHashCode    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_GetType : ScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _method;
        public static UserdataMethodFastReflect GetMethod(Script script) {
            if (_method == null) {
                var methods = new List<ScorpioFastReflectMethodInfo>();
                methods.Add(new ScorpioFastReflectMethodInfo(false, new Type[] {}, null, 0));
                _method = new UserdataMethodFastReflect(script, typeof(TestClass), "GetType", methods.ToArray(), new ScorpioClass_TestClass_GetType()); 
            }
            return _method;
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
            case 0: { return ((TestClass)obj).GetType(); }
            default: throw new Exception("TestClass 找不到合适的函数 : GetType    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_op_Addition : ScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _method;
        public static UserdataMethodFastReflect GetMethod(Script script) {
            if (_method == null) {
                var methods = new List<ScorpioFastReflectMethodInfo>();
                methods.Add(new ScorpioFastReflectMethodInfo(true, new Type[] {typeof(TestClass),typeof(TestClass)}, null, 0));
                methods.Add(new ScorpioFastReflectMethodInfo(true, new Type[] {typeof(TestClass),typeof(System.Int32)}, null, 1));
                _method = new UserdataMethodFastReflect(script, typeof(TestClass), "op_Addition", methods.ToArray(), new ScorpioClass_TestClass_op_Addition()); 
            }
            return _method;
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
            case 0: { return (TestClass)args[0] + (TestClass)args[1]; }
            case 1: { return (TestClass)args[0] + (System.Int32)args[1]; }
            default: throw new Exception("TestClass 找不到合适的函数 : op_Addition    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_ReferenceEquals : ScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _method;
        public static UserdataMethodFastReflect GetMethod(Script script) {
            if (_method == null) {
                var methods = new List<ScorpioFastReflectMethodInfo>();
                methods.Add(new ScorpioFastReflectMethodInfo(true, new Type[] {typeof(System.Object),typeof(System.Object)}, null, 0));
                _method = new UserdataMethodFastReflect(script, typeof(TestClass), "ReferenceEquals", methods.ToArray(), new ScorpioClass_TestClass_ReferenceEquals()); 
            }
            return _method;
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
            case 0: { return TestClass.ReferenceEquals((System.Object)args[0],(System.Object)args[1]); }
            default: throw new Exception("TestClass 找不到合适的函数 : ReferenceEquals    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_set_TestPro : ScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _method;
        public static UserdataMethodFastReflect GetMethod(Script script) {
            if (_method == null) {
                var methods = new List<ScorpioFastReflectMethodInfo>();
                methods.Add(new ScorpioFastReflectMethodInfo(false, new Type[] {typeof(System.Int32)}, null, 0));
                _method = new UserdataMethodFastReflect(script, typeof(TestClass), "set_TestPro", methods.ToArray(), new ScorpioClass_TestClass_set_TestPro()); 
            }
            return _method;
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
            case 0: { ((TestClass)obj).TestPro = (System.Int32)args[0]; return null; }
            default: throw new Exception("TestClass 找不到合适的函数 : set_TestPro    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_Test : ScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _method;
        public static UserdataMethodFastReflect GetMethod(Script script) {
            if (_method == null) {
                var methods = new List<ScorpioFastReflectMethodInfo>();
                methods.Add(new ScorpioFastReflectMethodInfo(false, new Type[] {}, null, 0));
                _method = new UserdataMethodFastReflect(script, typeof(TestClass), "Test", methods.ToArray(), new ScorpioClass_TestClass_Test()); 
            }
            return _method;
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
            case 0: { ((TestClass)obj).Test(); return null; }
            default: throw new Exception("TestClass 找不到合适的函数 : Test    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_ToString : ScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _method;
        public static UserdataMethodFastReflect GetMethod(Script script) {
            if (_method == null) {
                var methods = new List<ScorpioFastReflectMethodInfo>();
                methods.Add(new ScorpioFastReflectMethodInfo(false, new Type[] {}, null, 0));
                _method = new UserdataMethodFastReflect(script, typeof(TestClass), "ToString", methods.ToArray(), new ScorpioClass_TestClass_ToString()); 
            }
            return _method;
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
            case 0: { return ((TestClass)obj).ToString(); }
            default: throw new Exception("TestClass 找不到合适的函数 : ToString    type : " + methodIndex);
            }
        }
    }
}