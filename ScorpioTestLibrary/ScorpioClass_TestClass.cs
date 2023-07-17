using System;
using System.Collections.Generic;
using System.Reflection;
using Scorpio;
using Scorpio.Userdata;
using Scorpio.Tools;
using Scorpio.Exception;
public class ScorpioClass_TestClass : IScorpioFastReflectClass {
    public UserdataMethodFastReflect GetConstructor() {
        return ScorpioClass_TestClass_Constructor.GetInstance();
    }
    public Type GetVariableType(string name) {
        switch (name) {
            case "num": return typeof(System.Int32);
            case "Instance": return typeof(TestClass);
            case "Equals": return typeof(System.Boolean);
            case "get_Instance": return typeof(TestClass);
            case "GetHashCode": return typeof(System.Int32);
            case "GetType": return typeof(System.Type);
            case "op_Addition": return typeof(TestClass);
            case "ReferenceEquals": return typeof(System.Boolean);
            case "TestFunc1": return typeof(System.Int32);
            case "TestFunc2": return typeof(System.Int32);
            case "TestFunc3": return typeof(System.Int32);
            case "ToString": return typeof(System.String);
            default: throw new ExecutionException("TestClass [GetVariableType] 找不到变量 : " + name);
        }
    }
    public UserdataMethod GetMethod(string name) {
        switch (name) {
            case "Equals": return ScorpioClass_TestClass_Equals.GetInstance();
            case "get_Instance": return ScorpioClass_TestClass_get_Instance.GetInstance();
            case "GetHashCode": return ScorpioClass_TestClass_GetHashCode.GetInstance();
            case "GetType": return ScorpioClass_TestClass_GetType.GetInstance();
            case "op_Addition": return ScorpioClass_TestClass_op_Addition.GetInstance();
            case "ReferenceEquals": return ScorpioClass_TestClass_ReferenceEquals.GetInstance();
            case "TestExtend1": return ScorpioClass_TestClass_TestExtend1.GetInstance();
            case "TestFunc1": return ScorpioClass_TestClass_TestFunc1.GetInstance();
            case "TestFunc2": return ScorpioClass_TestClass_TestFunc2.GetInstance();
            case "TestFunc3": return ScorpioClass_TestClass_TestFunc3.GetInstance();
            case "ToString": return ScorpioClass_TestClass_ToString.GetInstance();
            default: return null;
        }
    }
    public bool TryGetValue(object obj, string name, out object value) {
        switch (name) {
            case "num": value = ((TestClass)obj).num; return true;
            case "Instance": value = TestClass.Instance; return true;
            default: value = null; return false;
        }
    }
    public void SetValue(object obj, string name, ScriptValue value) {
        switch (name) {
            case "num": { ((TestClass)obj).num = (System.Int32)(value.ChangeType(typeof(System.Int32))); return; }
            default: throw new ExecutionException("TestClass [SetValue] 找不到变量 : " + name);
        }
    }

    public class ScorpioClass_TestClass_Constructor : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(false, new Type[]{}, new bool[]{}, null, 0),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestClass), "Constructor", methodInfos, new ScorpioClass_TestClass_Constructor()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return new TestClass(); }
                default: throw new ExecutionException("TestClass 找不到合适的函数 : Constructor    type : " + methodIndex);
            }
        }
    }

    public class ScorpioClass_TestClass_Equals : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(false, new Type[]{typeof(System.Object)}, new bool[]{false}, null, 0),
                new ScorpioFastReflectMethodInfo(true, new Type[]{typeof(System.Object),typeof(System.Object)}, new bool[]{false,false}, null, 1),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestClass), "Equals", methodInfos, new ScorpioClass_TestClass_Equals()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return ((TestClass)obj).Equals((System.Object)args[0]); }
                case 1: { return TestClass.Equals((System.Object)args[0], (System.Object)args[1]); }
                default: throw new ExecutionException("TestClass 找不到合适的函数 : Equals    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_get_Instance : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(true, new Type[]{}, new bool[]{}, null, 0),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestClass), "get_Instance", methodInfos, new ScorpioClass_TestClass_get_Instance()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return TestClass.Instance; }
                default: throw new ExecutionException("TestClass 找不到合适的函数 : get_Instance    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_GetHashCode : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(false, new Type[]{}, new bool[]{}, null, 0),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestClass), "GetHashCode", methodInfos, new ScorpioClass_TestClass_GetHashCode()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return ((TestClass)obj).GetHashCode(); }
                default: throw new ExecutionException("TestClass 找不到合适的函数 : GetHashCode    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_GetType : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(false, new Type[]{}, new bool[]{}, null, 0),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestClass), "GetType", methodInfos, new ScorpioClass_TestClass_GetType()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return ((TestClass)obj).GetType(); }
                default: throw new ExecutionException("TestClass 找不到合适的函数 : GetType    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_op_Addition : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(true, new Type[]{typeof(TestClass),typeof(TestClass)}, new bool[]{false,false}, null, 0),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestClass), "op_Addition", methodInfos, new ScorpioClass_TestClass_op_Addition()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return (TestClass)args[0] + (TestClass)args[1]; }
                default: throw new ExecutionException("TestClass 找不到合适的函数 : op_Addition    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_ReferenceEquals : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(true, new Type[]{typeof(System.Object),typeof(System.Object)}, new bool[]{false,false}, null, 0),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestClass), "ReferenceEquals", methodInfos, new ScorpioClass_TestClass_ReferenceEquals()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return TestClass.ReferenceEquals((System.Object)args[0], (System.Object)args[1]); }
                default: throw new ExecutionException("TestClass 找不到合适的函数 : ReferenceEquals    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_TestExtend1 : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(false, new Type[]{}, new bool[]{}, null, 0),
                new ScorpioFastReflectMethodInfo(false, new Type[]{typeof(System.String)}, new bool[]{false}, null, 1),
                new ScorpioFastReflectMethodInfo(false, new Type[]{typeof(System.String),typeof(System.Int32)}, new bool[]{false,false}, null, 2),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestClass), "TestExtend1", methodInfos, new ScorpioClass_TestClass_TestExtend1()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { ((TestClass)obj).TestExtend1(); return null; }
                case 1: { ((TestClass)obj).TestExtend1((System.String)args[0]); return null; }
                case 2: { ((TestClass)obj).TestExtend1((System.String)args[0], (System.Int32)args[1]); return null; }
                default: throw new ExecutionException("TestClass 找不到合适的函数 : TestExtend1    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_TestFunc1 : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(false, new Type[]{}, new bool[]{}, null, 0),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestClass), "TestFunc1", methodInfos, new ScorpioClass_TestClass_TestFunc1()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return ((TestClass)obj).TestFunc1(); }
                default: throw new ExecutionException("TestClass 找不到合适的函数 : TestFunc1    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_TestFunc2 : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(false, new Type[]{typeof(System.String),typeof(System.Int32),typeof(System.Object[])}, new bool[]{false,false,false}, typeof(System.Object), 0),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestClass), "TestFunc2", methodInfos, new ScorpioClass_TestClass_TestFunc2()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return ((TestClass)obj).TestFunc2((System.String)args[0], (System.Int32)args[1], (System.Object[])args[2]); }
                default: throw new ExecutionException("TestClass 找不到合适的函数 : TestFunc2    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_TestFunc3 : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(false, new Type[]{typeof(System.String)}, new bool[]{false}, null, 0),
                new ScorpioFastReflectMethodInfo(false, new Type[]{typeof(System.String),typeof(System.Int32)}, new bool[]{false,false}, null, 1),
                new ScorpioFastReflectMethodInfo(false, new Type[]{typeof(System.String),typeof(System.Int32),typeof(System.Int32)}, new bool[]{false,false,false}, null, 2),
                new ScorpioFastReflectMethodInfo(false, new Type[]{typeof(System.String),typeof(System.Int32),typeof(System.Int32),typeof(System.String)}, new bool[]{false,false,false,false}, null, 3),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestClass), "TestFunc3", methodInfos, new ScorpioClass_TestClass_TestFunc3()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return ((TestClass)obj).TestFunc3((System.String)args[0]); }
                case 1: { return ((TestClass)obj).TestFunc3((System.String)args[0], (System.Int32)args[1]); }
                case 2: { return ((TestClass)obj).TestFunc3((System.String)args[0], (System.Int32)args[1], (System.Int32)args[2]); }
                case 3: { return ((TestClass)obj).TestFunc3((System.String)args[0], (System.Int32)args[1], (System.Int32)args[2], (System.String)args[3]); }
                default: throw new ExecutionException("TestClass 找不到合适的函数 : TestFunc3    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_ToString : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(false, new Type[]{}, new bool[]{}, null, 0),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestClass), "ToString", methodInfos, new ScorpioClass_TestClass_ToString()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return ((TestClass)obj).ToString(); }
                default: throw new ExecutionException("TestClass 找不到合适的函数 : ToString    type : " + methodIndex);
            }
        }
    }
}