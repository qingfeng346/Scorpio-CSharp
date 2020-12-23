//所有的class 都支持 运算符重载   number string bool null 除外
class TestClass {
    constructor(num) {
        this.num = num
    }
    //支持的运算符重载有 + - * / % ^ & | << >> ()
    "+"(num) {
        return TestClass() { num = this.num + num}
    }
    "-"(num) {
        this.num -= num
        return this
    }
    "=="(num) {
        return this.num == num
    }
    "()" {
        print("()call : " + this.num)
    }
}
var t1 = TestClass(100)
t1 -= 10            //TestClass - 运算符操作本身数据
var t2 = t1 + 100   //TestClass + 运算符
t1()                //TestClass () 运算符
print(t1.num)
print(t2.num)
print(t1 == 90)
