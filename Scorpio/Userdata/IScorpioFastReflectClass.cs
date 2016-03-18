using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Userdata {
    public interface IScorpioFastReflectClass {
        object CreateInstance(string type, object[] args);
        object GetValue(object obj, string name);
        void SetValue(object obj, string name, object value);
    }
}
