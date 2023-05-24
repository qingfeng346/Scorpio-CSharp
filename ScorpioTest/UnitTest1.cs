using Scorpio.Commons;

namespace Scorpio {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1() {
            foreach (var file in FileUtil.GetFiles("../../../../ExampleScripts", new[] { "*.sco" }, SearchOption.TopDirectoryOnly)) {
                Console.WriteLine($"============================��ʼ�����ļ�{file}============================");
                var script = new Script();
                script.LoadLibraryV1();
                script.LoadFile(file);
            }
        }
        [TestMethod]
        public unsafe void TestMemory() {
            Console.WriteLine(sizeof(ScriptValue));
            //var script = new Script();
            //foreach (var value in script.Global) {
            //    var map = value.Value.Get<ScriptMapString>();
            //    if (map != null) {
            //        var a = 0;
            //    }
            //}
        }
    }
}