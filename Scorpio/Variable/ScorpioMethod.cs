using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Userdata;

namespace Scorpio.Variable
{
    public class ScorpioMethod
    {
        private UserdataMethod m_Method;
        private object m_Object;
        public string MethodName { get; private set; }
        public ScorpioMethod(object obj, string name, UserdataMethod method)
        {
            m_Object = obj;
            m_Method = method;
            MethodName = name;
        }
        public object Call(ScriptObject[] parameters)
        {
            return m_Method.Call(m_Object, parameters);
        }
    }
}
