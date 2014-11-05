namespace CancerDemo
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.Run = new System.Windows.Forms.Button();
            this.m_txtScriptOutput = new System.Windows.Forms.TextBox();
            this.m_txtBuildOutput = new System.Windows.Forms.TextBox();
            this.m_lblScriptOutput = new System.Windows.Forms.Label();
            this.m_lblBuildOutput = new System.Windows.Forms.Label();
            this.m_lblSource = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(265, 25);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(329, 469);
            this.textBox1.TabIndex = 0;
            // 
            // Run
            // 
            this.Run.Location = new System.Drawing.Point(652, 471);
            this.Run.Name = "Run";
            this.Run.Size = new System.Drawing.Size(75, 23);
            this.Run.TabIndex = 1;
            this.Run.Text = "Run Script";
            this.Run.UseVisualStyleBackColor = true;
            this.Run.Click += new System.EventHandler(this.Run_Click);
            // 
            // m_txtScriptOutput
            // 
            this.m_txtScriptOutput.Location = new System.Drawing.Point(602, 194);
            this.m_txtScriptOutput.Multiline = true;
            this.m_txtScriptOutput.Name = "m_txtScriptOutput";
            this.m_txtScriptOutput.ReadOnly = true;
            this.m_txtScriptOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.m_txtScriptOutput.Size = new System.Drawing.Size(275, 271);
            this.m_txtScriptOutput.TabIndex = 6;
            this.m_txtScriptOutput.WordWrap = false;
            // 
            // m_txtBuildOutput
            // 
            this.m_txtBuildOutput.Location = new System.Drawing.Point(602, 25);
            this.m_txtBuildOutput.Multiline = true;
            this.m_txtBuildOutput.Name = "m_txtBuildOutput";
            this.m_txtBuildOutput.ReadOnly = true;
            this.m_txtBuildOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.m_txtBuildOutput.Size = new System.Drawing.Size(275, 139);
            this.m_txtBuildOutput.TabIndex = 7;
            this.m_txtBuildOutput.WordWrap = false;
            // 
            // m_lblScriptOutput
            // 
            this.m_lblScriptOutput.AutoSize = true;
            this.m_lblScriptOutput.Location = new System.Drawing.Point(600, 167);
            this.m_lblScriptOutput.Name = "m_lblScriptOutput";
            this.m_lblScriptOutput.Size = new System.Drawing.Size(83, 12);
            this.m_lblScriptOutput.TabIndex = 8;
            this.m_lblScriptOutput.Text = "Script Output";
            // 
            // m_lblBuildOutput
            // 
            this.m_lblBuildOutput.AutoSize = true;
            this.m_lblBuildOutput.Location = new System.Drawing.Point(600, 5);
            this.m_lblBuildOutput.Name = "m_lblBuildOutput";
            this.m_lblBuildOutput.Size = new System.Drawing.Size(77, 12);
            this.m_lblBuildOutput.TabIndex = 9;
            this.m_lblBuildOutput.Text = "Build Output";
            // 
            // m_lblSource
            // 
            this.m_lblSource.AutoSize = true;
            this.m_lblSource.Location = new System.Drawing.Point(263, 5);
            this.m_lblSource.Name = "m_lblSource";
            this.m_lblSource.Size = new System.Drawing.Size(29, 12);
            this.m_lblSource.TabIndex = 10;
            this.m_lblSource.Text = "源码";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(13, 25);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(244, 472);
            this.listBox1.TabIndex = 11;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "文件列表";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 506);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.m_lblSource);
            this.Controls.Add(this.m_lblBuildOutput);
            this.Controls.Add(this.m_lblScriptOutput);
            this.Controls.Add(this.m_txtBuildOutput);
            this.Controls.Add(this.m_txtScriptOutput);
            this.Controls.Add(this.Run);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Scorpio";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button Run;
        private System.Windows.Forms.TextBox m_txtScriptOutput;
        private System.Windows.Forms.TextBox m_txtBuildOutput;
        private System.Windows.Forms.Label m_lblScriptOutput;
        private System.Windows.Forms.Label m_lblBuildOutput;
        private System.Windows.Forms.Label m_lblSource;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label1;
    }
}

