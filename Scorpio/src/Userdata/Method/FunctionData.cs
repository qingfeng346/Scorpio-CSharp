using System;
using System.Reflection;
namespace Scorpio.Userdata {
    //单个函数的所有数据
    public abstract class FunctionData {
        public Type[] ParameterType;                //所有参数类型
        public int RequiredNumber;                  //必须的参数个数
        public int ParameterCount;                  //除去变长参数的参数个数
        public object[] Args;                       //参数数组（预创建 可以共用）

        public bool IsParams;                       //是否是变长参数
        public Type ParamType;                      //变长参数类型
        public bool IsGeneric;                      //是否是模板函数
        public bool HasDefault;                     //是否有默认参数
        public bool IsStatic;                       //是否是静态函数
        public object[] DefaultParameter;           //默认参数

        public MethodInfo Method;                   //普通函数对象(只有普通函数和扩展函数有用)
        public FunctionData(Type[] parameterType, object[] defaultParameter, Type paramType) {
            this.ParameterType = parameterType;
            this.RequiredNumber = parameterType.Length;
            this.ParameterCount = parameterType.Length;
            this.Args = new object[parameterType.Length];

            this.DefaultParameter = defaultParameter;
            this.ParamType = paramType;
            this.IsParams = paramType != null;
            this.HasDefault = false;
            if (defaultParameter != null && defaultParameter.Length > 0) {
                HasDefault = true;
                RequiredNumber -= defaultParameter.Length;
            }
            if (IsParams) {
                --RequiredNumber;
                --ParameterCount;
            }
        }
        public abstract object Invoke(object obj);
    }
}
