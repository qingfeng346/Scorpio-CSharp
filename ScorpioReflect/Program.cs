using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ScorpioReflect
{
    public class Test
    {
        private int wwwww;
        public int a;
        public int b;
        public int GetA()
        {
            return a;
        }
        public void Func()
        {

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
    }
    class Program
    {
        static void Main(string[] args)
        {
            var g = new GenerateScorpioClass(typeof(Test));
            var str = g.Generate();
            File.WriteAllText(@"C:\Users\qingf\Desktop\ConsoleApplication1\ConsoleApplication1\" + g.ScorpioClassName + ".cs", str, Encoding.UTF8);
        }
    }
}
