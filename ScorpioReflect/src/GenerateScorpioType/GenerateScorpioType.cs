using System;
using System.Text;
using System.Collections.Generic;
namespace Scorpio.ScorpioReflect {
    public partial class GenerateScorpioType {
        private const string Template = @"using System;
using Scorpio;
namespace __Namespace {
    public class __ClassName {
        public static void Initialize(Script script) {__CreateObject
        }
    }
}";
        public string Namespace = "ScorpioType";
        public string ClassName = "ScorpioTypeFactory";
        private List<Type> m_Types = new List<Type>();
        public void AddType(Type type) {
            if (!m_Types.Contains(type)) m_Types.Add(type);
        }
        public string Generate() {
            m_Types.SortType();
            string str = Template;
            str = str.Replace("__Namespace", string.IsNullOrEmpty(Namespace) ? "ScorpioType" : Namespace);
            str = str.Replace("__ClassName", string.IsNullOrEmpty(ClassName) ? "ScorpioTypeFactory" : ClassName);
            str = str.Replace("__CreateObject", CreateObject());
            return str;
        }
        public string CreateObject() {
            var builder = new StringBuilder();
            foreach (var type in m_Types) {
                builder.AppendFormat(@"
            script.SetGlobal(""{0}"", ScriptValue.CreateObject(typeof({0})));", ScorpioReflectUtil.GetFullName(type));
            }
            return builder.ToString();
        }
    }
}
