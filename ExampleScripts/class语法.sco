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
        var inNum = 100
        var test = class {
            hello() {
                print("临时class : " + inNum)
            }
        }
        inNum = 200
        return test
    }
}
var t = TestClass()
t.baseFunc()
t.hello().hello()


//原生对象支持的 prototype 类请查看 Proto 文件夹下所有类
//可以修改原生对象的 prototype 例如 
Number.testFunc = function () {
    print("testFunc " + this)
}
100.testFunc()

String.end = function (str) {
    return this.endsWith(str)
}
print("str".end("r"))

/* 原表名称 
数字类型    Number
字符串      Number
Bool        Bool
函数        Function
数组        Array
Map         Map
*/
//数字类型 Number