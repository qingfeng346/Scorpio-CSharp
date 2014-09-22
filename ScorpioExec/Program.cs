using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scorpio;
namespace ScorpioExec
{
    class test
    {
        public static string str1 = "fewfawef";
        public string str = "fffffffffff";
        public string GetStr()
        {
            return "weeeeeeeeeeeeee";
        }
        public static void test1()
        {
            Console.WriteLine("===== " + str1);
        }
        public static void test1(int b)
        {
            Console.WriteLine("int " + b);
        }
        public static void test1(string a)
        {
            Console.WriteLine("string " + a);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            try {
                Script script = new Script();
                script.LoadLibrary();
                test t = new test();
                script.SetObject("test", new ScriptUserdata(typeof(test)));
//                script.LoadString("", @"
//print(test.str)
//print(test.str1)
//test.str = ""111111111111111""
//test.str1 = ""22222222222222222""
//local b = test.GetStr
//print(b())
//");
                script.LoadString("", @"
print(test.str1)
test.str1 = 1651616
test.test1()
test.test1(100)
test.test1(""feawfweafwe"")
");
                //Console.WriteLine(t.str);
                Console.WriteLine(test.str1);
                //if (args.Length >= 1) {
                //    string str = File.ReadAllText(args[0]);
                //    Script script = new Script();
                //    script.LoadLibrary();
                //    script.RegisterFunction("time", time);
                //    Console.WriteLine("返回值为:" + script.LoadString("", str));
                //}
            } catch (System.Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            Console.ReadKey();
        }
        private static object time(object[] Parameters)
        {
            return Environment.TickCount;
        }
    }
}
