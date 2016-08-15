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
namespace ScorpioDemo {
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
