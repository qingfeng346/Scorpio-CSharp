using Scorpio;
public class TestFastClassManager {
    public static void Initialize(Script script) {
        script.SetFastReflectClass(typeof(TestClass), new ScorpioClass_TestClass());
        script.SetFastReflectClass(typeof(TestStruct), new ScorpioClass_TestStruct());
   }
}
