// using System;
// using System.Collections.Generic;
// using System.Reflection;
// using Scorpio;
// using Scorpio.Userdata;
// using Scorpio.Variable;
// using Scorpio.Commons;
// public class ScorpioClass_TestClass : ScorpioFastReflectClass {
//     private Script m_Script;
//     public ScorpioClass_TestClass(Script script) {
//         m_Script = script;
//     }
//     public UserdataMethodFastReflect GetConstructor() {
//         return ScorpioClass_TestClass_Constructor.GetMethod(m_Script);
//     }
//     public Type GetVariableType(string name) {
//         if (name == "testAction1") return typeof(System.Action);
//         if (name == "testAction2") return typeof(System.Action<System.Int32>);
//         if (name == "testAction3") return typeof(System.Func<System.Int32>);
//         if (name == "testAction4") return typeof(System.Func<System.Int32,System.Int32,System.Int32,System.Int32>);
//         if (name == "Equals") return typeof(System.Boolean);
//         if (name == "GetHashCode") return typeof(System.Int32);
//         if (name == "GetType") return typeof(System.Type);
//         if (name == "ReferenceEquals") return typeof(System.Boolean);
//         if (name == "ToString") return typeof(System.String);
//         throw new Exception("TestClass [GetVariableType] 找不到变量 : " + name);
//     }
//     public object GetValue(object obj, string name) {
//         if (name == "testAction1") return ((TestClass)obj).testAction1;
//         if (name == "testAction2") return ((TestClass)obj).testAction2;
//         if (name == "testAction3") return ((TestClass)obj).testAction3;
//         if (name == "testAction4") return ((TestClass)obj).testAction4;
//         if (name == "Equals") return ScorpioClass_TestClass_Equals.GetInstance(m_Script, obj);
//         if (name == "GetHashCode") return ScorpioClass_TestClass_GetHashCode.GetInstance(m_Script, obj);
//         if (name == "GetType") return ScorpioClass_TestClass_GetType.GetInstance(m_Script, obj);
//         if (name == "ReferenceEquals") return ScorpioClass_TestClass_ReferenceEquals.GetInstance(m_Script, obj);
//         if (name == "Test") return ScorpioClass_TestClass_Test.GetInstance(m_Script, obj);
//         if (name == "ToString") return ScorpioClass_TestClass_ToString.GetInstance(m_Script, obj);
//         throw new Exception("TestClass [GetValue] 找不到变量 : " + name);
//     }
//     public void SetValue(object obj, string name, ScriptValue value) {
//         if (name == "testAction1") { ((TestClass)obj).testAction1 = (System.Action)(Util.ChangeType(m_Script, value, typeof(System.Action))); return; }
//         if (name == "testAction2") { ((TestClass)obj).testAction2 = (System.Action<System.Int32>)(Util.ChangeType(m_Script, value, typeof(System.Action<System.Int32>))); return; }
//         if (name == "testAction3") { ((TestClass)obj).testAction3 = (System.Func<System.Int32>)(Util.ChangeType(m_Script, value, typeof(System.Func<System.Int32>))); return; }
//         if (name == "testAction4") { ((TestClass)obj).testAction4 = (System.Func<System.Int32,System.Int32,System.Int32,System.Int32>)(Util.ChangeType(m_Script, value, typeof(System.Func<System.Int32,System.Int32,System.Int32,System.Int32>))); return; }
//         throw new Exception("TestClass [SetValue] 找不到变量 : " + name);
//     }

//     public class ScorpioClass_TestClass_Constructor : ScorpioFastReflectMethod {
//         private static ScorpioFastReflectMethodInfo[] _methods;
//         private static UserdataMethodFastReflect _method;
//         private static ScorpioMethod _instance;
//         static ScorpioClass_TestClass_Constructor() {
//             var methods = new List<ScorpioMethodInfo>();
//             methods.Add(new ScorpioFastReflectMethodInfo(false, new Type[] {}, null, 0));
//             _methods = methods.ToArray();
//         }
//         public static UserdataMethodFastReflect GetMethod(Script script) {
//             if (_method == null) {
//                 _method = new UserdataMethodFastReflect(script, typeof(TestClass), "Constructor", _methods, new ScorpioClass_TestClass_Constructor()); 
//             }
//             return _method;
//         }
//         public static ScorpioMethod GetInstance(Script script, object obj) {
//             GetMethod(script);
//             if (obj == null) {
//                 if (_instance == null) {
//                     _instance = new ScorpioTypeMethod(script, "Constructor", _method, typeof(TestClass));
//                 }
//                 return _instance;
//             }
//             return new ScorpioObjectMethod(obj, "Constructor", _method);
//         }
//         public object Call(object obj, int methodIndex, object[] args) {
//             switch (methodIndex) {
//             case 0: return new TestClass();
//             default: throw new Exception("TestClass 找不到合适的函数 : Constructor    type : " + type);
//             }
//         }
//     }

//     public class ScorpioClass_TestClass_Equals : ScorpioFastReflectMethod {
//         private static ScorpioFastReflectMethodInfo[] _methods;
//         private static UserdataMethodFastReflect _method;
//         private static ScorpioMethod _instance;
//         static ScorpioClass_TestClass_Equals() {
//             var methods = new List<ScorpioMethodInfo>();
//             methods.Add(new ScorpioFastReflectMethodInfo(false, new Type[] {typeof(System.Object)}, null, 0));
//             methods.Add(new ScorpioFastReflectMethodInfo(true, new Type[] {typeof(System.Object),typeof(System.Object)}, null, 1));
//             _methods = methods.ToArray();
//         }
//         public static UserdataMethodFastReflect GetMethod(Script script) {
//             if (_method == null) {
//                 _method = new UserdataMethodFastReflect(script, typeof(TestClass), "Equals", _methods, new ScorpioClass_TestClass_Equals()); 
//             }
//             return _method;
//         }
//         public static ScorpioMethod GetInstance(Script script, object obj) {
//             GetMethod(script);
//             if (_instance == null) {
//                 _instance = new ScorpioStaticMethod("Equals", _method);
//             }
//             return _instance;
//         }
//         public object Call(object obj, int methodIndex, object[] args) {
//             switch (methodIndex) {
//             case 0: { return ((TestClass)obj).Equals((System.Object)args[0]); }

//             case 1: { return TestClass.Equals((System.Object)args[0],(System.Object)args[1]); }
//             default: throw new Exception("TestClass 找不到合适的函数 : Equals    type : " + type);
//             }
//         }
//     }
//     public class ScorpioClass_TestClass_GetHashCode : ScorpioFastReflectMethod {
//         private static ScorpioFastReflectMethodInfo[] _methods;
//         private static UserdataMethodFastReflect _method;
//         private static ScorpioMethod _instance;
//         static ScorpioClass_TestClass_GetHashCode() {
//             var methods = new List<ScorpioMethodInfo>();
//             methods.Add(new ScorpioFastReflectMethodInfo(false, new Type[] {}, null, 0));
//             _methods = methods.ToArray();
//         }
//         public static UserdataMethodFastReflect GetMethod(Script script) {
//             if (_method == null) {
//                 _method = new UserdataMethodFastReflect(script, typeof(TestClass), "GetHashCode", _methods, new ScorpioClass_TestClass_GetHashCode()); 
//             }
//             return _method;
//         }
//         public static ScorpioMethod GetInstance(Script script, object obj) {
//             GetMethod(script);
//             if (obj == null) {
//                 if (_instance == null) {
//                     _instance = new ScorpioTypeMethod(script, "GetHashCode", _method, typeof(TestClass));
//                 }
//                 return _instance;
//             }
//             return new ScorpioObjectMethod(obj, "GetHashCode", _method);
//         }
//         public object Call(object obj, int methodIndex, object[] args) {
//             switch (methodIndex) {
//             case 0: { return ((TestClass)obj).GetHashCode(); }
//             default: throw new Exception("TestClass 找不到合适的函数 : GetHashCode    type : " + type);
//             }
//         }
//     }
//     public class ScorpioClass_TestClass_GetType : ScorpioFastReflectMethod {
//         private static ScorpioFastReflectMethodInfo[] _methods;
//         private static UserdataMethodFastReflect _method;
//         private static ScorpioMethod _instance;
//         static ScorpioClass_TestClass_GetType() {
//             var methods = new List<ScorpioMethodInfo>();
//             methods.Add(new ScorpioFastReflectMethodInfo(false, new Type[] {}, null, 0));
//             _methods = methods.ToArray();
//         }
//         public static UserdataMethodFastReflect GetMethod(Script script) {
//             if (_method == null) {
//                 _method = new UserdataMethodFastReflect(script, typeof(TestClass), "GetType", _methods, new ScorpioClass_TestClass_GetType()); 
//             }
//             return _method;
//         }
//         public static ScorpioMethod GetInstance(Script script, object obj) {
//             GetMethod(script);
//             if (obj == null) {
//                 if (_instance == null) {
//                     _instance = new ScorpioTypeMethod(script, "GetType", _method, typeof(TestClass));
//                 }
//                 return _instance;
//             }
//             return new ScorpioObjectMethod(obj, "GetType", _method);
//         }
//         public object Call(object obj, int methodIndex, object[] args) {
//             switch (methodIndex) {
//             case 0: { return ((TestClass)obj).GetType(); }
//             default: throw new Exception("TestClass 找不到合适的函数 : GetType    type : " + type);
//             }
//         }
//     }
//     public class ScorpioClass_TestClass_ReferenceEquals : ScorpioFastReflectMethod {
//         private static ScorpioFastReflectMethodInfo[] _methods;
//         private static UserdataMethodFastReflect _method;
//         private static ScorpioMethod _instance;
//         static ScorpioClass_TestClass_ReferenceEquals() {
//             var methods = new List<ScorpioMethodInfo>();
//             methods.Add(new ScorpioFastReflectMethodInfo(true, new Type[] {typeof(System.Object),typeof(System.Object)}, null, 0));
//             _methods = methods.ToArray();
//         }
//         public static UserdataMethodFastReflect GetMethod(Script script) {
//             if (_method == null) {
//                 _method = new UserdataMethodFastReflect(script, typeof(TestClass), "ReferenceEquals", _methods, new ScorpioClass_TestClass_ReferenceEquals()); 
//             }
//             return _method;
//         }
//         public static ScorpioMethod GetInstance(Script script, object obj) {
//             GetMethod(script);
//             if (_instance == null) {
//                 _instance = new ScorpioStaticMethod("ReferenceEquals", _method);
//             }
//             return _instance;
//         }
//         public object Call(object obj, int methodIndex, object[] args) {
//             switch (methodIndex) {
//             case 0: { return TestClass.ReferenceEquals((System.Object)args[0],(System.Object)args[1]); }
//             default: throw new Exception("TestClass 找不到合适的函数 : ReferenceEquals    type : " + type);
//             }
//         }
//     }
//     public class ScorpioClass_TestClass_Test : ScorpioFastReflectMethod {
//         private static ScorpioFastReflectMethodInfo[] _methods;
//         private static UserdataMethodFastReflect _method;
//         private static ScorpioMethod _instance;
//         static ScorpioClass_TestClass_Test() {
//             var methods = new List<ScorpioMethodInfo>();
//             methods.Add(new ScorpioFastReflectMethodInfo(false, new Type[] {}, null, 0));
//             _methods = methods.ToArray();
//         }
//         public static UserdataMethodFastReflect GetMethod(Script script) {
//             if (_method == null) {
//                 _method = new UserdataMethodFastReflect(script, typeof(TestClass), "Test", _methods, new ScorpioClass_TestClass_Test()); 
//             }
//             return _method;
//         }
//         public static ScorpioMethod GetInstance(Script script, object obj) {
//             GetMethod(script);
//             if (obj == null) {
//                 if (_instance == null) {
//                     _instance = new ScorpioTypeMethod(script, "Test", _method, typeof(TestClass));
//                 }
//                 return _instance;
//             }
//             return new ScorpioObjectMethod(obj, "Test", _method);
//         }
//         public object Call(object obj, int methodIndex, object[] args) {
//             switch (methodIndex) {
//             case 0: { ((TestClass)obj).Test(); return null; }
//             default: throw new Exception("TestClass 找不到合适的函数 : Test    type : " + type);
//             }
//         }
//     }
//     public class ScorpioClass_TestClass_ToString : ScorpioFastReflectMethod {
//         private static ScorpioFastReflectMethodInfo[] _methods;
//         private static UserdataMethodFastReflect _method;
//         private static ScorpioMethod _instance;
//         static ScorpioClass_TestClass_ToString() {
//             var methods = new List<ScorpioMethodInfo>();
//             methods.Add(new ScorpioFastReflectMethodInfo(false, new Type[] {}, null, 0));
//             _methods = methods.ToArray();
//         }
//         public static UserdataMethodFastReflect GetMethod(Script script) {
//             if (_method == null) {
//                 _method = new UserdataMethodFastReflect(script, typeof(TestClass), "ToString", _methods, new ScorpioClass_TestClass_ToString()); 
//             }
//             return _method;
//         }
//         public static ScorpioMethod GetInstance(Script script, object obj) {
//             GetMethod(script);
//             if (obj == null) {
//                 if (_instance == null) {
//                     _instance = new ScorpioTypeMethod(script, "ToString", _method, typeof(TestClass));
//                 }
//                 return _instance;
//             }
//             return new ScorpioObjectMethod(obj, "ToString", _method);
//         }
//         public object Call(object obj, int methodIndex, object[] args) {
//             switch (methodIndex) {
//             case 0: { return ((TestClass)obj).ToString(); }
//             default: throw new Exception("TestClass 找不到合适的函数 : ToString    type : " + type);
//             }
//         }
//     }
// }