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
    public TestClass1 test1 = new TestClass1();
    public byte[] Bytes {
        get {
            return Encoding.UTF8.GetBytes("{}");
        }
    }
}
public static class TestStaticClass {
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