using System;
using System.Collections.Generic;
using System.Reflection;
using Scorpio;
using Scorpio.Userdata;
using Scorpio.Tools;
using Scorpio.Exception;
public class ScorpioClass_TestStruct : IScorpioFastReflectClass {
    public UserdataMethodFastReflect GetConstructor() {
        return ScorpioClass_TestStruct_Constructor.GetInstance();
    }
    public Type GetVariableType(string name) {
        switch (name) {
            case "Equals": return typeof(System.Boolean);
            case "GetHashCode": return typeof(System.Int32);
            case "GetType": return typeof(System.Type);
            case "ReferenceEquals": return typeof(System.Boolean);
            case "TestFunc1": return typeof(System.Int32);
            case "TestFunc3": return typeof(System.Int32);
            case "ToString": return typeof(System.String);
            default: throw new ExecutionException("TestStruct [GetVariableType] 找不到变量 : " + name);
        }
    }
    public UserdataMethod GetMethod(string name) {
        switch (name) {
            case "Equals": return ScorpioClass_TestStruct_Equals.GetInstance();
            case "GetHashCode": return ScorpioClass_TestStruct_GetHashCode.GetInstance();
            case "GetType": return ScorpioClass_TestStruct_GetType.GetInstance();
            case "ReferenceEquals": return ScorpioClass_TestStruct_ReferenceEquals.GetInstance();
            case "TestFunc1": return ScorpioClass_TestStruct_TestFunc1.GetInstance();
            case "TestFunc3": return ScorpioClass_TestStruct_TestFunc3.GetInstance();
            case "ToString": return ScorpioClass_TestStruct_ToString.GetInstance();
            default: return null;
        }
    }
    public bool TryGetValue(object obj, string name, out object value) {
        switch (name) {
            default: value = null; return false;
        }
    }
    public void SetValue(object obj, string name, ScriptValue value) {
        switch (name) {
            default: throw new ExecutionException("TestStruct [SetValue] 找不到变量 : " + name);
        }
    }

    public class ScorpioClass_TestStruct_Constructor : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(false, new Type[]{}, new bool[]{}, null, 0),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestStruct), "Constructor", methodInfos, new ScorpioClass_TestStruct_Constructor()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return new TestStruct(); }
                default: throw new ExecutionException("TestStruct 找不到合适的函数 : Constructor    type : " + methodIndex);
            }
        }
    }

    public class ScorpioClass_TestStruct_Equals : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(false, new Type[]{typeof(System.Object)}, new bool[]{false}, null, 0),
                new ScorpioFastReflectMethodInfo(true, new Type[]{typeof(System.Object),typeof(System.Object)}, new bool[]{false,false}, null, 1),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestStruct), "Equals", methodInfos, new ScorpioClass_TestStruct_Equals()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return ((TestStruct)obj).Equals((System.Object)args[0]); }
                case 1: { return TestStruct.Equals((System.Object)args[0], (System.Object)args[1]); }
                default: throw new ExecutionException("TestStruct 找不到合适的函数 : Equals    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestStruct_GetHashCode : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(false, new Type[]{}, new bool[]{}, null, 0),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestStruct), "GetHashCode", methodInfos, new ScorpioClass_TestStruct_GetHashCode()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return ((TestStruct)obj).GetHashCode(); }
                default: throw new ExecutionException("TestStruct 找不到合适的函数 : GetHashCode    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestStruct_GetType : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(false, new Type[]{}, new bool[]{}, null, 0),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestStruct), "GetType", methodInfos, new ScorpioClass_TestStruct_GetType()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return ((TestStruct)obj).GetType(); }
                default: throw new ExecutionException("TestStruct 找不到合适的函数 : GetType    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestStruct_ReferenceEquals : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(true, new Type[]{typeof(System.Object),typeof(System.Object)}, new bool[]{false,false}, null, 0),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestStruct), "ReferenceEquals", methodInfos, new ScorpioClass_TestStruct_ReferenceEquals()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return TestStruct.ReferenceEquals((System.Object)args[0], (System.Object)args[1]); }
                default: throw new ExecutionException("TestStruct 找不到合适的函数 : ReferenceEquals    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestStruct_TestFunc1 : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(false, new Type[]{}, new bool[]{}, null, 0),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestStruct), "TestFunc1", methodInfos, new ScorpioClass_TestStruct_TestFunc1()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return ((TestStruct)obj).TestFunc1(); }
                default: throw new ExecutionException("TestStruct 找不到合适的函数 : TestFunc1    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestStruct_TestFunc3 : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(false, new Type[]{typeof(System.String)}, new bool[]{false}, null, 0),
                new ScorpioFastReflectMethodInfo(false, new Type[]{typeof(System.String),typeof(System.Int32)}, new bool[]{false,false}, null, 1),
                new ScorpioFastReflectMethodInfo(false, new Type[]{typeof(System.String),typeof(System.Int32),typeof(System.Int32)}, new bool[]{false,false,false}, null, 2),
                new ScorpioFastReflectMethodInfo(false, new Type[]{typeof(System.String),typeof(System.Int32),typeof(System.Int32),typeof(System.String)}, new bool[]{false,false,false,false}, null, 3),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestStruct), "TestFunc3", methodInfos, new ScorpioClass_TestStruct_TestFunc3()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return ((TestStruct)obj).TestFunc3((System.String)args[0]); }
                case 1: { return ((TestStruct)obj).TestFunc3((System.String)args[0], (System.Int32)args[1]); }
                case 2: { return ((TestStruct)obj).TestFunc3((System.String)args[0], (System.Int32)args[1], (System.Int32)args[2]); }
                case 3: { return ((TestStruct)obj).TestFunc3((System.String)args[0], (System.Int32)args[1], (System.Int32)args[2], (System.String)args[3]); }
                default: throw new ExecutionException("TestStruct 找不到合适的函数 : TestFunc3    type : " + methodIndex);
            }
        }
    }
    public class ScorpioClass_TestStruct_ToString : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {
                new ScorpioFastReflectMethodInfo(false, new Type[]{}, new bool[]{}, null, 0),
            };
            return _instance = new UserdataMethodFastReflect(typeof(TestStruct), "ToString", methodInfos, new ScorpioClass_TestStruct_ToString()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {
                case 0: { return ((TestStruct)obj).ToString(); }
                default: throw new ExecutionException("TestStruct 找不到合适的函数 : ToString    type : " + methodIndex);
            }
        }
    }
}