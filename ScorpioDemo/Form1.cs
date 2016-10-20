using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Scorpio;
using System.IO;
using System.Reflection;
namespace ScorpioDemo {
    public class TestClass {
        public int a = 100;
        public void Test1() {
            Console.WriteLine("test1");
        }
    }
    public static class TestEx {
        public static void Test3() {

        }
        public static void Test2(this TestClass t) {
            Console.WriteLine("test2 " + t.a);
        }

    }
    public partial class Form1 : Form
    {
        private string m_Path = "";
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            TestClass c = new TestClass();
            //c.Test2();
            Console.WriteLine(typeof(TestClass).IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false));
            {
                MethodInfo method = typeof(TestClass).GetMethod("Test2", BindingFlags.Static | BindingFlags.Public);
                Console.WriteLine(method == null ? "null" : method.ToString());
            }
            {
                MethodInfo[] methods = typeof(TestClass).GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                foreach (var method in methods) {
                    Console.WriteLine(method.ToString());
                }
            }
            Console.WriteLine("=================================");
            {
                Console.WriteLine(typeof(TestEx).IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false));
                MethodInfo[] methods = typeof(TestEx).GetMethods(Script.BindingFlag);
                foreach (var method in methods) {
                    if (method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)) {
                        method.Invoke(null, new object[] { c });
                    }
                    //Console.WriteLine(method.ToString() + "  " + method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false));
                }
            }
            
            m_Path = Path.GetDirectoryName(Path.GetDirectoryName(Environment.CurrentDirectory)) + "/Scripts";
            LoadFileList();
        }
        private void Form1_Activated(object sender, EventArgs e)
        {
            LoadFileList();
        }
        private void LoadFileList()
        {
            listBox1.Items.Clear();
            string[] files = Directory.GetFiles(m_Path);
            for (int i = 0; i < files.Length;++i ) {
                listBox1.Items.Add(Path.GetFileName(files[i]));
            }
        }
        private void Run_Click(object sender, EventArgs e)
        {
            Script script = new Script();
            try {
                m_txtBuildOutput.Text = "";
                m_txtScriptOutput.Text = "";
                script.LoadLibrary();
                script.PushAssembly(GetType().Assembly);
                Stopwatch watch = Stopwatch.StartNew();
                script.SetObject("print", new ScorpioFunction(print));
                BuildOutPut("返回值为 " + script.LoadString(textBox1.Text));
                BuildOutPut("运行时间:" + watch.ElapsedMilliseconds + " ms");
            } catch (System.Exception ex) {
                BuildOutPut("堆栈数据为 " + script.GetStackInfo());
                BuildOutPut(ex.ToString());
            }
        }
        private object print(Script script, object[] Parameters)
        {
            for (int i = 0; i < Parameters.Length; ++i) {
                ScriptOutPut(Parameters[i].ToString());
            }
            return null;
        }
        private void BuildOutPut(string message)
        {
            m_txtBuildOutput.Text += (message + "\r\n");
        }
        private void ScriptOutPut(string message)
        {
            m_txtScriptOutput.Text += (message + "\r\n");
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(listBox1.Text)) {
                textBox1.Text = File.ReadAllText(m_Path + "/" + listBox1.Text, Encoding.UTF8);
            }
        }
    }
}
