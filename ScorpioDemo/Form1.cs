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
    public partial class Form1 : Form
    {
        private readonly string PATH = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/a.sco";
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try {
                textBox1.Text = File.ReadAllText(PATH);
            } catch (System.Exception ex) {
                BuildOutPut("Load is error : " + ex.ToString());
            }
        }
        private void Form1_Activated(object sender, EventArgs e)
        {
            //try {
            //    textBox1.Text = File.ReadAllText(PATH);
            //} catch (System.Exception ex) {
            //    BuildOutPut("Load is error : " + ex.ToString());
            //}
        }
        private void Run_Click(object sender, EventArgs e)
        {
            Script script = new Script();
            try {
                m_txtBuildOutput.Text = "";
                m_txtScriptOutput.Text = "";
                double start = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds;
                script.LoadLibrary();
                script.PushAssembly(typeof(System.Environment).Assembly);
                script.SetObject("print", new ScorpioFunction(print));
                BuildOutPut("返回值为 " + script.LoadString("", textBox1.Text));
                BuildOutPut("运行时间:" + (Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds - start) + " ms");
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
    }
}
