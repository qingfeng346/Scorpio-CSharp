// c# 代码
/*
namespace ScorpioExec {
    public class TestClass {
        public int num = 0;
        public void TestFunc(ref int refNum, out string outNum) {
			if (refNum == 100) {
				outNum = "out1";
			} else {
				outNum = "out2";
			}
			refNum = refNum + 500;
		}
	}
    public static class ClassEx {
		public static void TestFunc1(this TestClass t, int num1) {
            t.num += num1;
        }
	}
}
*/
//sco 代码
TestClass = import_type("ScorpioExec.TestClass")
importExtension("ScorpioExec.ClassEx")
var t = new TestClass()
t.TestFunc1(100)
print(t.num)