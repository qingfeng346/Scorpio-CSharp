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
        return TestClass() { num = this.num - num}
    }
    "()" {
        print("call : " + this.num)
    }
}
var t1 = TestClass(100)
var t2 = t1 + 100   //TestClass 重载 + 运算符
var t3 = t1 - 50    //TestClass 重载 - 运算符
t1()                //TestClass 重载 () 运算符
print(t2.num)
print(t3.num)
