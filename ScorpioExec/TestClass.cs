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
        public static void TestFuncEx(this TestClass cl) {

        }
    }
    public class BaseClass {
        public int TestNumber { get; set; }
    }
    public class TestClass : BaseClass {
        public int num;
        public TestClass(int num, ref int tNum) {
            tNum = 0;
            this.num = num;
        }
        public TestClass(int num) {
            this.num = num;
        }
        public void TestOut(int[] results, ref int num, out string str) {
            str = "test";
        }
        public static int TestOut(ref int num, out string str) {
            str = "test";
            return 100;
        }
        public int TestNumber { get; set; }
        public void TestFunc() {

        }
        public void TestTemplate<T>(T arg) {

        }
        public static implicit operator TestClass(int value) {
            int refOut = 0;
            return new TestClass(value + 100, ref refOut);
        }
    }
}
