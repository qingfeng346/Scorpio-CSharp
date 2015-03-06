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
namespace CancerDemo
{
    public delegate void HttpProcessListener(string error, byte[] bytes);
    public class Test
    {
        public static void httpPost(String uri, string body, HttpProcessListener listener)
        {
            Console.WriteLine("111111111111111111");
            //httpPost(uri, body, "utf8", listener);
        }
        public static void httpPost(String uri, string body, string encoding, HttpProcessListener listener)
        {
            Console.WriteLine("222222222222222222");
            //httpPost(uri, Encoding.GetEncoding(encoding).GetBytes(body), listener);
        }
        public static void httpPost(String uri, byte[] body, HttpProcessListener listener)
        {
            Console.WriteLine("33333333333333333333");
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
        private object print(object[] Parameters)
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
            textBox1.Text = File.ReadAllText(m_Path + "/" + listBox1.Text, Encoding.UTF8);
        }
    }
}
