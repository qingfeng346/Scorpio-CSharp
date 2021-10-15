using System;
using System.Collections.Generic;
using System.Text;

namespace ScorpioExec {
    public delegate void TestDelegate1();
    public delegate int TestDelegate2(int a1);
    public delegate string TestDelegate3(int a1, int a2);
    public delegate TestClass TestDelegate4(int a1, int a2);
    //public delegate string TestDelegate3(int a1, int a2);
    public static class TestClassEx {
        public static void TestFuncEx(this BaseClass cl, int a) {
            cl.TestNumber = a;
        }
    }
    public class BaseClass {
        public int TestNumber { get; set; }
    }
    public class TestClass : BaseClass {
        public static Scorpio.Script script;
        public static void TestStaticFunc(string name) {
            script.call(name);
        }
        public int num;
        public TestClass(int num, ref int tNum) {
            tNum = 0;
            this.num = num;
        }
        public TestClass(int num) {
            this.num = num;
        }
        public TestClass() {

        }
        public void TestOut(int[] results, ref int num, out string str) {
            str = "test";
        }
        public static int TestOut(ref int num, out string str) {
            str = "test";
            return 100;
        }
        public static void TestStaticFunc() {
            string a = null;
            Console.WriteLine(a.ToString());
        }
        public void TestArgs(int a, params object[] args) {
            foreach (var arg in args) {
                Console.WriteLine(arg);
            }
        }
        public void TestFunc() {

        }
        public void TestTemplate<T>(T arg) {

        }
        public static implicit operator TestClass(int value) {
            int refOut = 0;
            return new TestClass(value + 100, ref refOut);
        }
    }
    public struct TestStruct {
        public static int staticNumber { get; set; }
        public int value1;
        public int value2 { get; set; }
    }
    public interface TestInterface {
        void Func1(int a, int b);
        void Func2();
        int Func3();
        string Func4(int aaa, int bbb);
    }
}
