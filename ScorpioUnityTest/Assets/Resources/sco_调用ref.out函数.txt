// c# 代码
/*
namespace ScorpioExec {
	public class TestClass {
		public void TestFunc(ref int refNum, out string outNum) {
			if (refNum == 100) {
				outNum = "out1";
			} else {
				outNum = "out2";
			}
			refNum = refNum + 500;
		}
	}
}
*/
//sco 代码
TestClass = import_type("ScorpioExec.TestClass")
var t = new TestClass()
var refNum = {value : 100}
var outNum = {}
t.TestFunc(refNum, outNum)  //ref out 的参数 必须传入map值，然后 ref out 返回的值会设置为 value
print(refNum.value, outNum.value)