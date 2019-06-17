//申请一个类， 所有类都继承自 Object
class BaseClass {
    baseFunc() {
        print("baseFunc")
    }
    hello() {
        print("base hello : " + this.num)
    }
}
//申请一个class 可以继承一个类，只支持单继承
class TestClass : BaseClass {
    //构造函数
    constructor() {
        this.num = 100
        print("构造函数")
    }
    hello() {
        this[base(this).hello]()    //调用父级同名函数等用于c#  base.hello()
        print("hello : " + this.num)
    }
}
var t = TestClass()
t.hello()
t.baseFunc()