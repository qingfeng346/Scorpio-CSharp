using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using Scorpio;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
namespace ScorpioExec
{
    public class Vector3
    {
        //
        // Fields
        //
        public float z;

        public float y;

        public float x;

        //
        // Constructors
        //
        public Vector3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0f;
        }

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public override bool Equals(object other)
        {
            if (!(other is Vector3))
            {
                return false;
            }
            Vector3 vector = (Vector3)other;
            return this.x.Equals(vector.x) && this.y.Equals(vector.y) && this.z.Equals(vector.z);
        }
        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1}, {2:F1})", new object[]
            {
                this.x,
                this.y,
                this.z
            });
        }
        //
        // Operators
        //
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator /(Vector3 a, float d)
        {
            return new Vector3(a.x / d, a.y / d, a.z / d);
        }
        public static Vector3 operator *(Vector3 a, float d)
        {
            return new Vector3(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3 operator *(float d, Vector3 a)
        {
            return new Vector3(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(-a.x, -a.y, -a.z);
        }
    }
    public class Program
    {
        public static string CurrentDirectory { get { return AppDomain.CurrentDomain.BaseDirectory; } }
        public static Assembly CompilerFile(string path)
        {
            CSharpCodeProvider Provider = new CSharpCodeProvider();
            CompilerParameters Parameters = new CompilerParameters();
            Parameters.ReferencedAssemblies.Add("System.dll");
            Parameters.GenerateExecutable = false;
            Parameters.GenerateInMemory = true;
            string[] fileNames = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
            CompilerResults cr = Provider.CompileAssemblyFromFile(Parameters, fileNames);
            if (cr.Errors.HasErrors) {
                string str = "cs文件编译错误: \n";
                foreach (CompilerError err in cr.Errors) {
                    str += (err.ToString() + "\n");
                }
                throw new Exception(str);
            }
            return cr.CompiledAssembly;
        }
        static void Main(string[] args)
        {
            Script script = new Script();
            Console.WriteLine("开始执行，当前版本:" + Script.Version);
            script.LoadLibrary();
            script.PushAssembly(typeof(Program).Assembly);
            if (Directory.Exists(CurrentDirectory + "/Library"))
            {
                string[] files = Directory.GetFiles(CurrentDirectory + "/Library", "*.dll", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    try {
                        script.PushAssembly(Assembly.LoadFile(file));
                        Console.WriteLine("导入文件[" + file + "]成功");
                    } catch (System.Exception ex) {
                        Console.WriteLine("导入文件[" + file + "]失败 " + ex.ToString());
                    }
                }
            }
            if (Directory.Exists(CurrentDirectory + "/Program")) {
                try {
                    script.PushAssembly(CompilerFile(CurrentDirectory + "/Program"));
                } catch (System.Exception ex) {
                    Console.WriteLine("编译文件失败 " + ex.ToString());
                }
            }
            if (args.Length >= 1) {
                try {
                    Stopwatch watch = Stopwatch.StartNew();
                    if (!script.HasValue("searchpath"))
                        script.SetObject("searchpath", Path.GetDirectoryName(args[0]));
                    Console.WriteLine("返回值为:" + script.LoadFile(args[0]));
                    Console.WriteLine("运行时间:" + watch.ElapsedMilliseconds + " ms");
                } catch (System.Exception ex) {
                    Console.WriteLine(script.GetStackInfo());
                    Console.WriteLine(ex.ToString());
                }
                Console.ReadKey();
            } else {
                while (true)
                {
                    try {
                        string str = Console.ReadLine();
                        if (str == "exit")  { 
                            break;
                        } else if (str == "clear") {
                            Console.Clear();
                        } else if (str == "version") {
                            Console.WriteLine(Script.Version);
                        } else {
                            script.LoadString(str);
                        }
                    } catch (System.Exception ex) {
                        Console.WriteLine(script.GetStackInfo());
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }
    }
}
