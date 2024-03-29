using Xunit.Abstractions;
using Scorpio.Commons;
using Scorpio;
using System.Text;
using Scorpio.FastReflect;

namespace ScorpioTest {
    public class UnitTest {
        private static ITestOutputHelper? _output;
        static void WriteLine(string message) {
            _output?.WriteLine(message);
        }
        class print : ScorpioHandle {
            private Script script;
            internal print(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var builder = new StringBuilder();
                var stack = script.GetStackInfo();
                builder.Append($"{stack.Breviary}:{stack.Line} ");
                for (var i = 0; i < length; ++i) {
                    if (i != 0) { builder.Append("    "); }
                    builder.Append(args[i]);
                }
                WriteLine(builder.ToString());
                return ScriptValue.Null;
            }
        }
        public UnitTest(ITestOutputHelper output) {
            _output = output;
        }
        [Fact]
        public void Test1() {
            foreach (var file in FileUtil.GetFiles("../../../../ExampleScripts", new[] { "*.sco" }, SearchOption.TopDirectoryOnly)) {
                WriteLine($"============================开始运行文件{file}============================");
                var script = new Script();
                script.LoadLibraryV1();
                script.PushAssembly(typeof(TestClass));
                script.SetGlobal("print", script.CreateFunction(new print(script)));
                script.LoadFile(file);
                script.Shutdown();
            }
        }
        [Fact]
        public void CreateDelegate() {
            var generate = new GenerateScorpioDelegate();
            generate.option = new GenerateScorpioDelegate.Option() { buildType = 0, className = "TestDelegateFactory" };
            generate.AddType(typeof(Action));
            generate.AddType(typeof(Action<int, int, string>));
            generate.AddType(typeof(Func<int, int, string>));
            generate.AddType(typeof(Func<string, string>));
            var output = "../../../../ScorpioTestLibrary/TestDelegateFactory.cs";
            FileUtil.CreateFile(output, generate.Generate());
            WriteLine($"生成Delegate仓库 {Path.GetFullPath(output)}");
        }
        [Fact]
        public void CreateFast() {
            var builder = new StringBuilder();
            builder.Append(@"using Scorpio;
public class TestFastClassManager {
    public static void Initialize(Script script) {
");
            var types = new Type[] { typeof(TestClass), typeof(TestStruct) };
            foreach (var type in types) {
                var generate = new GenerateScorpioClass(type);
                generate.AddExtensionType(typeof(TestStaticClass));
                generate.AddExtensionType(typeof(TestExtend));
                var output = $"../../../../ScorpioTestLibrary/{generate.ScorpioClassName}.cs";
                FileUtil.CreateFile(output, generate.Generate());
                WriteLine($"生成快速反射 {Path.GetFullPath(output)}");
                builder.AppendFormat("        script.SetFastReflectClass(typeof({0}), new {1}());\n", ScorpioFastReflectUtil.GetFullName(type), generate.ScorpioClassName);
            }
            builder.Append(@"   }
}
");
            FileUtil.CreateFile($"../../../../ScorpioTestLibrary/TestFastClassManager.cs", builder.ToString(), Encoding.UTF8);
        }
        [Fact]
        public void CreateInterface() {
            var generate = new GenerateScorpioInterface(typeof(TestInterface));
            var output = $"../../../../ScorpioTestLibrary/{generate.ScorpioClassName}.cs";
            FileUtil.CreateFile(output, generate.Generate());
            WriteLine($"生成interface {Path.GetFullPath(output)}");
        }
    }
}