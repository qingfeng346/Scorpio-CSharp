using System;
using System.Collections.Generic;
using System.Text;

namespace ScorpioExec {
    public class TestClass {
        public int num;
        public TestClass(int num) {
            this.num = num;
        }
        public void TestOut(int[] results, ref int aaaa, out string bbbb) {
            bbbb = "feawfawe";
        }
        public static implicit operator TestClass(int value) {
            return new TestClass(value + 100);
        }
    }
}
