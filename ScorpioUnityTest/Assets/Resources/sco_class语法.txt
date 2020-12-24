class BaseClass {
    //构造函数
    constructor() {
        this.num1 = 100
    }
    hello() {
        print(this.num2)
    }
}
//申请一个class 可以继承一个类，只支持单继承
class TestClass : BaseClass {
    //构造函数
    constructor() {
        base.constructor()  //只支持单层base, 不支持 base.base.constructor()
        this.num2 = 200
    }
    hello() {
        base.hello()
        print(this.num1)
    }
}
var t = new TestClass()
t.hello()