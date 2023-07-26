using System;
using Scorpio.Userdata;
namespace Scorpio {
    //去反射类
    public interface IScorpioFastReflectClass {
        UserdataMethodFastReflect GetConstructor();                     //获取构造函数
        Type GetVariableType(string name);                              //获取变量类型
        UserdataMethod GetMethod(string name);                          //获取类函数
        bool TryGetValue(object obj, string name, out object value);    //获取变量
        void SetValue(object obj, string name, ScriptValue value);      //设置变量
    }
}
