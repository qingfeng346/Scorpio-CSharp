using Xunit.Abstractions;
using Scorpio.Commons;
using Scorpio;
using System.Text;
using Scorpio.Tools;
using System.Reflection;
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
                script.SetGlobal("UnitTest", ScriptValue.CreateValue(script, typeof(UnitTest)));
                script.SetGlobal("print", script.CreateFunction(new print(script)));
                script.LoadFile(file);
                script.Shutdown();
                script.ReleaseAll();
                StringReference.Check((i, entity) => {
                    WriteLine($"当前未释放String变量 索引:{i}  {entity}");
                });
                ScriptObjectReference.Check((i, entity) => {
                    WriteLine($"当前未释放Script变量 索引:{i}  {entity}");
                });
                StringReference.Clear();
                ScriptObjectReference.Clear();
            }
        }
        [Fact]
        public void CreateDelegate() {
            var generate = new GenerateScorpioDelegate();
            generate.option = new GenerateScorpioDelegate.Option() { buildType = 0, className = "DelegateFactory" };
            generate.AddType(typeof(Action<int, int, string>));
            generate.AddType(typeof(Func<int, int, string>));
            var output = "../../../DelegateFactory.cs";
            FileUtil.CreateFile(output, generate.Generate());
            WriteLine($"生成Delegate仓库 {Path.GetFullPath(output)}");
        }
    }
}