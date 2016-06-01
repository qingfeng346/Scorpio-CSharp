using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scorpio;
using System.Threading;

namespace ScorpioReflect
{
    public class Test {
        public Test(int a ) {

        }
        public Test(params object[] args) {

        }
        public event Action<int> testEvent;
        public int a;
        public int b;
        public int GetA() { return a; }
        public void Func() { }
        public int get_Item(int a) {
            return 100;
        }
        public int op_Addition(int a, int b) {
            return 100;
        }
        public void TestParam1(string a, int b, params object[] args) {

        }
        public void TestParam2(string a, int b, params string[] args) {

        }
        public void TestTemp<T>(T a) {

        }
        public void aaaa(List<int> b ) {

        }
        public void TestTemp(int a) {

        }
        public int AAAA { set { a = value; } }
        public static int c;
        public string dddd { get; set; }
        public static int eee { get { return c; } }
        public void TestFunc(int a, int b)
        {

        }
        public void TestFunc(string b, int c)
        {

        }
        public static void TestFunc1() {

        }
        public static Test operator + (Test a, Test b) {
            return a;
        }
        public static Test operator + (Test a, int b) {
            return a;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            //            Thread.Sleep(1000);
            //            {
            //                try {
            //                    Script script = new Script();
            //                    script.LoadLibrary();
            //                    script.PushFastReflectClass(typeof(Test), new ScorpioClass_ScorpioReflect_Test(script));
            //                    script.SetObject("Test", script.CreateObject(new Test(100)));
            //                    var time = Environment.TickCount;
            //                    for (int i = 0; i < 1000000; ++i) {
            //                        script.LoadString(@"
            //Test.Func()
            //");
            //                    }
            //                    Console.WriteLine(Environment.TickCount - time);
            //                } catch (System.Exception ex) {
            //                    Console.WriteLine(ex.ToString());
            //                }

            //            }
            //            {
            //                Script script = new Script();
            //                script.LoadLibrary();
            //                script.SetObject("Test", script.CreateObject(new Test(100)));
            //                var time = Environment.TickCount;
            //                for (int i=0;i<1000000;++i)
            //                {
            //                    script.LoadString(@"
            //Test.Func()
            //");
            //                }

            //                Console.WriteLine(Environment.TickCount - time);
            //            }
            //            Console.ReadKey();
            var g = new Scorpio.ScorpioReflect.GenerateScorpioClass(typeof(Test));
            //g.AddExclude("aaaa");
            var str = g.Generate();
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + g.ScorpioClassName + ".cs", str, Encoding.UTF8);
        }
    }
}
