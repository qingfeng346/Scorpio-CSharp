using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.Userdata;
namespace Scorpio.Variable
{
    public abstract class ScorpioMethod
    {
        public UserdataMethod Method { get; protected set; }        //函数引用
        public string MethodName { get; protected set; }            //函数名字
        public abstract object Call(ScriptObject[] parameters);     //调用函数
    }
    //实例函数
    public class ScorpioObjectMethod : ScorpioMethod
    {
        private object m_Object;
        public ScorpioObjectMethod(object obj, string name, UserdataMethod method)
        {
            m_Object = obj;
            Method = method;
            MethodName = name;
        }
        public override object Call(ScriptObject[] parameters)
        {
            return Method.Call(m_Object, parameters);
        }
    }
    //类函数
    public class ScorpioTypeMethod : ScorpioMethod
    {
        public ScorpioTypeMethod(string name, UserdataMethod method)
        {
            Method = method;
            MethodName = name;
        }
        public override object Call(ScriptObject[] parameters)
        {
            int length = parameters.Length;
            Util.Assert(length > 0);
            Util.Assert(parameters[0] is ScriptUserdata);
            if (length > 1) {
                ScriptObject[] pars = new ScriptObject[parameters.Length - 1];
                Array.Copy(parameters, 1, pars, 0, pars.Length);
                return Method.Call(parameters[0].ObjectValue, pars);
            } else {
                return Method.Call(parameters[0].ObjectValue, new ScriptObject[0]);
            }
        }
    }
    //静态函数
    public class ScorpioStaticMethod : ScorpioMethod
    {
        public ScorpioStaticMethod(string name, UserdataMethod method)
        {
            Method = method;
            MethodName = name;
        }
        public override object Call(ScriptObject[] parameters)
        {
            return Method.Call(null, parameters);
        }
    }

}
