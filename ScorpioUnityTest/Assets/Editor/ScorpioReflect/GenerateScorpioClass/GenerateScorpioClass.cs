using System;
using System.Reflection;
using System.Collections.Generic;
using Scorpio.Tools;
using System.Text;
using System.Linq;

namespace Scorpio.ScorpioReflect {
    //过滤不生成的变量 函数 属性 和 事件 
    public interface ClassFilter {
        bool Check(GenerateScorpioClass generate, Type type, FieldInfo fieldInfo);
        bool Check(GenerateScorpioClass generate, Type type, EventInfo eventInfo);
        bool Check(GenerateScorpioClass generate, Type type, PropertyInfo propertyInfo);
        bool Check(GenerateScorpioClass generate, Type type, MethodInfo methodInfo);
    }
    public partial class GenerateScorpioClass {
        private readonly static Type ScorpioUnGenerate = typeof(ScorpioUnGenerateAttribute);
        private Type m_Type;                                                        //类型
        private ClassFilter m_ClassFilter;                                          //生成类过滤
        private List<Type> m_Extensions = new List<Type>();                         //所有扩展类
        private List<MethodInfo> m_ExtensionMethods = new List<MethodInfo>();       //类型的所有扩展函数
        private List<FieldInfo> m_Fields = new List<FieldInfo>();                   //过滤的公共变量
        private List<EventInfo> m_Events = new List<EventInfo>();                   //过滤的事件
        private List<PropertyInfo> m_Propertys = new List<PropertyInfo>();          //过滤的属性
        private List<MethodInfo> m_Methods = new List<MethodInfo>();                //过滤的函数
        private SortedSet<string> m_MethodNames = new SortedSet<string>();              //所有的函数名字
        private SortedSet<string> m_ExtensionUsing = new SortedSet<string>();           //扩展类所有的命名空间
        public List<FieldInfo> AllFields { get; } = new List<FieldInfo>();
        public List<EventInfo> AllEvents { get; } = new List<EventInfo>();
        public List<PropertyInfo> AllPropertys { get; } = new List<PropertyInfo>();
        public List<MethodInfo> AllMethods { get; } = new List<MethodInfo>();
        public string ScorpioClassName { get; }         //生成的最终类名字
        public string FullName { get; }                 //要生成的类名字
        public bool IsStruct { get; }                   //是否是结构体
        public GenerateScorpioClass(Type type) {
            m_Type = type;
            IsStruct = !type.IsClass;
            FullName = ScorpioReflectUtil.GetFullName(m_Type);
            ScorpioClassName = "ScorpioClass_" + ScorpioReflectUtil.GetGenerateClassName(type);
            AllFields.AddRange(m_Type.GetFields(ScorpioReflectUtil.BindingFlag));
            AllEvents.AddRange(m_Type.GetEvents(ScorpioReflectUtil.BindingFlag));
            var propertys = m_Type.GetProperties(ScorpioReflectUtil.BindingFlag);
            foreach (var property in propertys) {
                //如果是 get 则参数是0个  set 参数是1个  否则就可能是 [] 的重载
                if ((property.CanRead && property.GetGetMethod().GetParameters().Length == 0) ||
                    (property.CanWrite && property.GetSetMethod().GetParameters().Length == 1)) {
                    AllPropertys.Add(property);
                }
            }
            var methods = (m_Type.IsAbstract && m_Type.IsSealed) ? m_Type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) : m_Type.GetMethods(ScorpioReflectUtil.BindingFlag);
            foreach (var method in methods) {
                //屏蔽掉 模版函数 模板函数只能使用反射
                if (!ScorpioReflectUtil.CheckGenericMethod(method)) { continue; }
                AllMethods.Add(method);
            }
        }
        public void SetClassFilter(ClassFilter classFilter) {
            if (classFilter == null) { return; }
            m_ClassFilter = classFilter;
        }
        public void AddExtensionType(Type type) {
            if (type == null || !Util.IsExtensionType(type)) { return; }
            m_Extensions.Add(type);
        }
        bool Check(FieldInfo fieldInfo) {
            return !fieldInfo.IsDefined(ScorpioUnGenerate, true) && (m_ClassFilter == null || m_ClassFilter.Check(this, m_Type, fieldInfo));
        }
        bool Check(EventInfo eventInfo) {
            return !eventInfo.IsDefined(ScorpioUnGenerate, true) && (m_ClassFilter == null || m_ClassFilter.Check(this, m_Type, eventInfo));
        }
        bool Check(PropertyInfo propertyInfo) {
            return !propertyInfo.IsDefined(ScorpioUnGenerate, true) && (m_ClassFilter == null || m_ClassFilter.Check(this, m_Type, propertyInfo));
        }
        bool Check(MethodInfo methodInfo) {
            return !methodInfo.IsDefined(ScorpioUnGenerate, true) && (m_ClassFilter == null || m_ClassFilter.Check(this, m_Type, methodInfo));
        }
        void Init() {
            (m_Fields = new List<FieldInfo>(AllFields.Where(_ => Check(_)))).SortField();
            (m_Events = new List<EventInfo>(AllEvents.Where(_ => Check(_)))).SortEvent();
            (m_Propertys = new List<PropertyInfo>(AllPropertys.Where(_ => Check(_)))).SortProperty();
            m_Methods.Clear();
            foreach (var method in AllMethods) {
                if (!Check(method)) { continue; }
                var valid = true;
                var methodHandle = method.MethodHandle;
                //判断函数是不是 event add remove 函数
                foreach (var eventInfo in AllEvents) {
                    if (eventInfo.GetAddMethod().MethodHandle == methodHandle || eventInfo.GetRemoveMethod().MethodHandle == methodHandle) {
                        if (!Check(eventInfo)) { valid = false; }
                        break;
                    }
                }
                if (valid) {
                    //判断函数是不是 get set 函数
                    foreach (var propertyInfo in AllPropertys) {
                        //如果是struct 并且是 set 属性则屏蔽,强转结构体的set会报错
                        if (IsStruct && propertyInfo.GetSetMethod()?.MethodHandle == methodHandle) {
                            valid = false;
                            break;
                        } else if (propertyInfo.GetGetMethod()?.MethodHandle == methodHandle || propertyInfo.GetSetMethod()?.MethodHandle == methodHandle) {
                            if (!Check(propertyInfo)) { valid = false; }
                            break;
                        }
                    }
                }
                if (valid) m_Methods.Add(method);
            }
            m_Methods.SortMethod();
            m_ExtensionMethods.Clear();
            foreach (var extensionInfo in m_Extensions) {
                var nameSpace = extensionInfo.Namespace;
                var methods = extensionInfo.GetMethods(Script.BindingFlag);
                foreach (var methodInfo in methods) {
                    //非扩展函数
                    if (!Util.IsExtensionMethod(methodInfo) || !Check(methodInfo)) { continue; }
                    var paramterType = methodInfo.GetParameters()[0].ParameterType;
                    //判断是模板函数
                    if (paramterType.IsGenericParameter && paramterType.BaseType != null && paramterType.BaseType.IsAssignableFrom(m_Type)) {
                        m_ExtensionMethods.Add(methodInfo);
                    } else if (ScorpioReflectUtil.CheckGenericMethod(methodInfo) && paramterType.IsAssignableFrom(m_Type)) {
                        m_ExtensionMethods.Add(methodInfo);
                    } else {
                        continue;
                    }
                    if (!string.IsNullOrWhiteSpace(nameSpace)) m_ExtensionUsing.Add(nameSpace);
                }
            }
            m_ExtensionMethods.SortMethod();
            m_Methods.ForEach(_ => m_MethodNames.Add(_.Name));
            m_ExtensionMethods.ForEach(_ => m_MethodNames.Add(_.Name));
        }
        string GetExtensionsUsing() {
            var builder = new StringBuilder();
            foreach (var name in m_ExtensionUsing) {
                builder.Append($@"
using {name};");
            }
            return builder.ToString();
        }
        public string Generate() {
            Init();
            return ClassTemplate.Replace("__extensions_using", GetExtensionsUsing())
                    .Replace("__reflect_content", GenerateReflectList())
                    .Replace("__getvariabletype_content", GenerateGetVariableType())
                    .Replace("__method_content", GenerateGetMethod())
                    .Replace("__getvalue_content", GenerateGetValue())
                    .Replace("__setvalue_content", GenerateSetValue())
                    .Replace("__constructor_content", GenerateConstructor())
                    .Replace("__methods_content", GenerateMethod())
                    .Replace("__class", ScorpioClassName)
                    .Replace("__fullname", FullName);
        }
    }
}