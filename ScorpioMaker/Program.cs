using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Compiler;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace Scorpio
{
    class Program
    {
        static void Main(string[] args)
        {
            string type = "";       //类型 1解析 0转换
            string source = "";     //源文件
            string target = "";     //目标文件
            try {
                for (int i = 0; i < args.Length; ++i) {
                    if (args[i] == "-t") {
                        type = args[i + 1];
                    } else if (args[i] == "-s") {
                        source = args[i + 1];
                    } else if (args[i] == "-o") {
                        target = args[i + 1];
                    }
                }
            } catch (System.Exception ex) {
                Console.WriteLine("参数出错 -t [类型 1解析 0转换] -s [源文件] -o [输出文件] error : " + ex.ToString());
                goto exit;
            }
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target)) {
                Console.WriteLine("参数出错 -t [类型 1解析 0转换] -s [源文件] -o [输出文件] ");
                goto exit;
            }
            source = Path.Combine(Environment.CurrentDirectory, source);
            if (string.IsNullOrEmpty(source)) target = source + ".sco";
            target = Path.Combine(Environment.CurrentDirectory, target);
            try {
                if (type.Equals("1"))
                    File.WriteAllBytes(target, Encoding.UTF8.GetBytes(ScorpioMaker.DeserializeToString(Util.GetFileBuffer(source))));
                else
                    File.WriteAllBytes(target, ScorpioMaker.Serialize(Util.GetFileString(source, Encoding.UTF8)));
            } catch (System.Exception ex) {
                Console.WriteLine("转换出错 error : " + ex.ToString());	
            }
        exit:
            Console.WriteLine("转换结束");
        }
    }
}
