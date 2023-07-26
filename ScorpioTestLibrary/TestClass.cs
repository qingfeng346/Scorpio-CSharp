using Scorpio;
using System.Text;

public interface TestInterface {
    void Func1();
    void Func2();
    int Func3();
    string Func4();
}
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
public struct TestStruct {
    public int TestFunc1() {
        return 100;
    }
    public int TestFunc3(string arg1, int arg2 = 2, int arg3 = 3, string arg4 = "123123") {
        return 100;
    }
}
public class TestClass {
    public class TestClassInternal {

    }
    public static TestClass Instance { get; } = new TestClass();
    public int num;
    public static TestClass operator+(TestClass a, TestClass b) {
        return new TestClass() { num = a.num + b.num };
    }
    public int TestFunc1() {
        string s = null;
        Console.WriteLine(s.Length);
        return 100;
    }
    public int TestFunc2(string arg1, int arg2, params object[] args) {
        return arg2;
    }
    public int TestFunc3(string arg1, int arg2=2, int arg3=3, string arg4="123123") {
        return 100;
    }
}
public static class TestExtend {
    public static void TestExtend1(this TestClass testClass, string arg1 = "arg1", int arg2 = 2) {

    }
}
public static class TestStaticClass {
    public static float TestFloat = 1000;
    public static Action action1;
    public static Func<string, string> func1;
    private static LinkedList<(DateTime, Action)> timer = new LinkedList<(DateTime, Action)> ();
    public static string TestDelegate(string str) {
        action1?.Invoke();
        return func1(str);
    }
    public static void TestFunc3() {
        AddTimer(0.1f, () => {
            func1("fewa");
            TestFunc3();
        });
    }
    public static void AddTimer(float seconds, Action action) {
        timer.AddLast((DateTime.Now.AddSeconds(seconds), action));
    }
    public static TestClass www = new TestClass();
    public static bool Update() {
        if (timer.Count > 0) {
            var now = DateTime.Now;
            for (var it = timer.First; it != null; it = it.Next) {
                if (now > it.Value.Item1) {
                    timer.Remove(it);
                    it.Value.Item2();
                    break;
                }
            }
            return true;
        }
        return false;
    }
    public static TestClass Get() {
        return new TestClass();
    }
}