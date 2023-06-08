using Xunit.Abstractions;
using Scorpio.Commons;
using Scorpio;
using System.Text;
using Scorpio.Tools;

namespace ScorpioTest {
    public class UnitTest {
        private static ITestOutputHelper? _output;
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
                _output?.WriteLine(builder.ToString());
                return ScriptValue.Null;
            }
        }
        public UnitTest(ITestOutputHelper output) {
            _output = output;
        }
        public static ScriptInstance AddComponent(ScriptInstance instance) {
            return instance;
        }
        [Fact]
        public void Test1() {
            foreach (var file in FileUtil.GetFiles("../../../../ExampleScripts", new[] { "*.sco" }, SearchOption.TopDirectoryOnly)) {
                _output?.WriteLine($"============================开始运行文件{file}============================");
                var script = new Script();
                script.LoadLibraryV1();
                script.SetGlobal("UnitTest", ScriptValue.CreateValue(script, typeof(UnitTest)));
                script.SetGlobal("print", script.CreateFunction(new print(script)));
                script.LoadFile(file);
                script.Shutdown();
                script.ReleaseAll();
                StringReference.Check((i, entity) => {
                    _output?.WriteLine($"当前未释放String变量 索引:{i}  {entity}");
                });
                ScriptObjectReference.Check((i, entity) => {
                    _output?.WriteLine($"当前未释放Script变量 索引:{i}  {entity}");
                });
                StringReference.Clear();
                ScriptObjectReference.Clear();
            }
        }
        [Fact]
        public void Test2() {
            var script = new Script();
            script.LoadLibraryV1();
            script.SetGlobal("UnitTest", ScriptValue.CreateValue(script, typeof(UnitTest)));
            script.SetGlobal("print", script.CreateFunction(new print(script)));
            script.LoadFile("../../../../ScorpioExec/run.sco");
            script.Shutdown();
            script.ReleaseAll();
            StringReference.Check((i, entity) => {
                _output?.WriteLine($"当前未释放String变量 索引:{i}  {entity}");
            });
            ScriptObjectReference.Check((i, entity) => {
                _output?.WriteLine($"当前未释放Script变量 索引:{i}  {entity}");
            });
            StringReference.Clear();
            ScriptObjectReference.Clear();
        }
    }
}