namespace Scorpio {
    //去反射函数
    public interface IScorpioFastReflectMethod {
        object Call(object obj, int methodIndex, object[] args);
    }
}
