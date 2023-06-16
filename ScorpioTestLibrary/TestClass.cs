using Scorpio;
using System.Text;

public enum TestEnum {
    Test1,
    Test2,
}
public enum TestEnum1 {
    Test1,
    Test2,
}
public class TestClass1 {
    public class TestClass2 {

    }
    public enum TestEnum {

    }
}
public class TestClass {
    public int TestFunc1() {
        return 0;
    }
    public int TestFunc2(string arg1, int arg2) {
        return 0;
    }
}
public static class TestStaticClass {
    public static void TestFunc3(this TestClass testClass) {

    }
    //static ScriptValue wwww;
    public static TestClass www = new TestClass();
    //public static ScriptInstance AddComponent(this ScriptInstance instance) {
    //    wwww = new ScriptValue(instance);
    //    wwww.Free();
    //    return instance;
    //}
    static DateTime start;
    static Action action1;
    public static void Update() {
        if (action1 != null && DateTime.Now > start) {
            action1();
            action1 = null;
        }
        //wwww.GetValue("Update").call(wwww);
    }
    public static TestClass Get() {
        return new TestClass();
    }
    public static void Timer(float s, Action action) {
        start = DateTime.Now.AddSeconds(s);
        action1 = action;
    }
}