using System;
using System.Reflection;
namespace Scorpio.Userdata {
    //单个函数的所有数据
    public abstract class FunctionData {
        public Type[] ParameterType;                //所有参数类型
        public object[] DefaultParameter;           //默认参数
        public object[] Args;                       //参数数组（预创建）
        public int RequiredNumber;                  //必须的参数个数
        public int ParameterCount;                  //除去变长参数的参数个数
        public Type ParamType;                      //变长参数类型

        public bool IsNormal;                       //是否是普通函数，没有默认参数，没有变长参数
        public bool IsDefault;                      //是否有默认参数函数，没有变长参数
        public bool IsParams;                       //是否是变长参数

        public FunctionData(Type[] parameterType, object[] defaultParameter, int requiredNumber, Type paramType) {
            this.ParameterType = parameterType;
            this.DefaultParameter = defaultParameter;
            this.RequiredNumber = requiredNumber;
            this.ParamType = paramType;
            this.ParameterCount = parameterType.Length;
            this.Args = new object[parameterType.Length];
            this.IsParams = paramType != null;

            //必须参数和总共参数不同证明有默认参数
            var hadDefault = requiredNumber != parameterType.Length;
            this.IsNormal = !hadDefault && !IsParams;
            this.IsDefault = hadDefault && !IsParams;

            if (IsParams) {
                --RequiredNumber;
                --ParameterCount;
            }
        }
        //是否是静态函数
        public virtual bool IsStatic => false;
        //是否是没有实例的模板函数
        public virtual bool IsGeneric => false;
        public abstract object Invoke(object obj);
    }
}
