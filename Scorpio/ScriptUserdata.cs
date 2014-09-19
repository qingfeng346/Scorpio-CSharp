using System;
using System.Collections.Generic;
using System.Text;
namespace Scorpio
{
    //语言数据
    public class ScriptUserdata : ScriptObject
    {
        public object Value { get; set; }
        public ScriptUserdata(object value)
        {
            Type = ObjectType.UserData;
            Value = value;
        }
        public override string ToString() { return "Userdata"; }
    }
}
