using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Scorpio;
using System.IO;
namespace CancerDemo
{
    public partial class Form1 : Form
    {
        private readonly string PATH = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/a.txt";
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //try {
            //    textBox1.Text = File.ReadAllText(PATH);
            //} catch (System.Exception ex) {
            //    BuildOutPut("Load is error : " + ex.ToString());
            //}
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
            try {
                m_txtBuildOutput.Text = "";
                m_txtScriptOutput.Text = "";
                Script script = new Script();
                script.LoadLibrary();
                script.RegisterFunction("print", print);
                script.RegisterFunction("time", time);
                BuildOutPut("返回值为 " + script.LoadString("", textBox1.Text));
            } catch (System.Exception ex) {
                BuildOutPut(ex.ToString());
            }
        }
        private object time(object[] Parameters)
        {
            return Environment.TickCount;
        }
        private object print(object[] Parameters)
        {
            for (int i = 0; i < Parameters.Length;++i ) {
                ScriptOutPut(Parameters[i].ToString());
            }
            return 100;
        }
        private void BuildOutPut(string message)
        {
            m_txtBuildOutput.Text += (message + "\r\n");
        }
        private void ScriptOutPut(string message)
        {
            m_txtScriptOutput.Text += (message + "\r\n");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText(PATH, textBox1.Text, Encoding.UTF8);
        }
    }
}
