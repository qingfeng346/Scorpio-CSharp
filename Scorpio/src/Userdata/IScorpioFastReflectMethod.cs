using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Userdata {
    public interface IScorpioFastReflectMethod {
        object Call(object obj, string type, object[] args);
    }
}
