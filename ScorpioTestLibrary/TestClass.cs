using Scorpio;
using System.Text;

public interface TestInterface {
    void Func1();
    void Func2();
    int Func3();
    string Func4(string a,string b,string c);
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
public class TestClass {
    public int num;
    public static TestClass operator+(TestClass a, TestClass b) {
        return new TestClass() { num = a.num + b.num };
    }
    public int TestFunc1() {
        return 100;
    }
    public int TestFunc2(string arg1, int arg2) {
        return arg2;
    }
}
public static class TestStaticClass {
    public static Action action1;
    public static Func<string, string> func1;
    public static TestInterface testInterface;
    private static LinkedList<(DateTime, Action)> timer = new LinkedList<(DateTime, Action)> ();
    public static string TestDelegate(string str) {
        action1?.Invoke();
        return func1(str);
    }
    public static string TestI(string a, string b, string c) {
        return testInterface.Func4(a, b, c);
    }
    public static void TestFunc3(this TestClass testClass) {

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