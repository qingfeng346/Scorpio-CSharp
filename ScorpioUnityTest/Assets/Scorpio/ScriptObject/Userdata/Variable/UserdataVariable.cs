using System;
namespace Scorpio.Userdata {
    //一个类的变量
    public abstract class UserdataVariable {
        public Type FieldType { get; protected set; }
        public abstract object GetValue(object obj);
        public abstract void SetValue(object obj, object value);
    }
}
