using System;
using System.Collections.Generic;
using System.Text;

public enum TestEnum {
    Test1,
    Test,
}
public class TestClass {
    public Action testAction1;
    public Action<int> testAction2;
    public Func<int> testAction3;
    public Func<int,int, int, int> testAction4;
    public void Test() {
        if (testAction1 != null) {
            testAction1();
        }
        if (testAction2 != null) {
            testAction2(100);
        }
        if (testAction3 != null) {
            Console.WriteLine("test3 : " + testAction3());
        }
        if (testAction4 != null) {
            Console.WriteLine("test4 : " + testAction4(100,200,300));
        }
    }
    public static TestClass operator + (TestClass a, TestClass b) {
        return a;
    }
    public static TestClass operator +(TestClass a, int b) {
        return a;
    }
}
