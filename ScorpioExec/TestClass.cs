using System;

namespace Scorpio {
    public class BaseClass {
    }
    public class TestClass : BaseClass {
        public TestClass() { }
        public TestClass(int a) {
            Console.WriteLine("one constructor " + a);
        }
        public TestClass(string a, int b = 100) {
            Console.WriteLine("default constructor " + a + " b = " + b);
        }
        public TestClass(int a, int b, params object[] args) {
            Console.WriteLine("params constructor " + a + " b = " + b + "  argLength = " + args.Length);
        }
        public void TestFunc(int a) {
            Console.WriteLine("TestFunc - " + a);
        }
        public void TestDefaultFunc(int a, int b = 100) {
            Console.WriteLine("TestDefaultFunc - " + a + "¡¡b = " + b);
        }
        public void TestArgsFunc(int a, int b = 100, params object[] args) {
            Console.WriteLine("TestArgsFunc - " + a + "¡¡b = " + b + "  argLength = " + args.Length);
        }
        public void TestRefFunc(ref int a, out int b) {
            a += 100;
            b = 100;
        }
        public void TestRefDefaultFunc(ref int a, out int b, int c = 200) {
            a += 100;
            b = 100;
        }
        public void TestRefArgsFunc(ref int a, out int b, params object[] args) {
            a += 100;
            b = 100;
        }
    }
    public static class ClassEx {
        public static void TestFuncEx(this BaseClass cl, int a) {
            Console.WriteLine(a);
        }
        public static void TestDefaultFuncEx(this BaseClass cl, string a, int b = 100) {
            Console.WriteLine(a + "   " + b);
        }
        public static void TestArgsFuncEx(this BaseClass cl, params object[] args) {
            Console.WriteLine(args.Length);
        }
        public static void TestRefFuncEx(this BaseClass cl, ref int a, out int b) {
            a += 100;
            b = 100;
        }
        public static void TestRefDefaultFuncEx(this BaseClass cl, ref int a, out int b, int c = 200) {
            a += 100;
            b = 100;
        }
        public static void TestRefArgsFuncEx(this BaseClass cl, ref int a, out int b, params object[] args) {
            a += 100;
            b = 100;
        }
    }
}
