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
            case "Equals": return typeof(System.Boolean);
            case "GetHashCode": return typeof(System.Int32);
            case "GetType": return typeof(System.Type);
            case "ReferenceEquals": return typeof(System.Boolean);
            case "TestFunc1": return typeof(System.Int32);
            case "TestFunc2": return typeof(System.Int32);
            case "ToString": return typeof(System.String);
            default: throw new ExecutionException("TestClass [GetVariableType] 找不到变量 : " + name);
        }
    }
    public UserdataMethod GetMethod(string name) {
        switch (name) {
            case "Equals": return ScorpioClass_TestClass_Equals.GetInstance();
            case "GetHashCode": return ScorpioClass_TestClass_GetHashCode.GetInstance();
            case "GetType": return ScorpioClass_TestClass_GetType.GetInstance();
            case "ReferenceEquals": return ScorpioClass_TestClass_ReferenceEquals.GetInstance();
            case "TestFunc1": return ScorpioClass_TestClass_TestFunc1.GetInstance();
            case "TestFunc2": return ScorpioClass_TestClass_TestFunc2.GetInstance();
            case "TestFunc3": return ScorpioClass_TestClass_TestFunc3.GetInstance();
            case "ToString": return ScorpioClass_TestClass_ToString.GetInstance();
            default: return null;
        }
    }
    public bool GetValue(object obj, string name, out object value) {
        switch (name) {
            case "Equals": value = ScorpioClass_TestClass_Equals.GetInstance(); return true;
            case "GetHashCode": value = ScorpioClass_TestClass_GetHashCode.GetInstance(); return true;
            case "GetType": value = ScorpioClass_TestClass_GetType.GetInstance(); return true;
            case "ReferenceEquals": value = ScorpioClass_TestClass_ReferenceEquals.GetInstance(); return true;
            case "TestFunc1": value = ScorpioClass_TestClass_TestFunc1.GetInstance(); return true;
            case "TestFunc2": value = ScorpioClass_TestClass_TestFunc2.GetInstance(); return true;
            case "TestFunc3": value = ScorpioClass_TestClass_TestFunc3.GetInstance(); return true;
            case "ToString": value = ScorpioClass_TestClass_ToString.GetInstance(); return true;
            default: value = null; return false;
        }
    }
    public void SetValue(object obj, string name, ScriptValue value) {
        switch (name) {
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
                new ScorpioFastReflectMethodInfo(false, new Type[]{typeof(System.String),typeof(System.Int32)}, new bool[]{false,false}, null, 0),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestClass), "TestFunc2", methodInfos, new ScorpioClass_TestClass_TestFunc2()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return ((TestClass)obj).TestFunc2((System.String)args[0], (System.Int32)args[1]); }
                default: throw new ExecutionException("TestClass 找不到合适的函数 : TestFunc2    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestClass_TestFunc3 : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(false, new Type[]{}, new bool[]{}, null, 0),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestClass), "TestFunc3", methodInfos, new ScorpioClass_TestClass_TestFunc3()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { ((TestClass)obj).TestFunc3(); return null; }
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