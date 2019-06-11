class BaseClass {
    baseFunc() {
        print("baseFunc")
    }
}
//申请一个class 可以继承一个类，只支持单继承
class TestClass : BaseClass {
    //构造函数
    constructor() {
        print("构造函数")
    }
    hello() {
        print("hello")
    }
    //可以重载运算符 key 值为 运算符 字符串 例如 + - * / % ^ & | << >>
    "()"() {

    }
    "+"() {

    }
}
var t = TestClass()
t.hello()
t.baseFunc()