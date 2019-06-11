var base = {
    value = 100,
    hello1() {
        print("hello1 " + this.value) 
    }
    //等同于 hello2()
    "hello2"() { 
        print("hello2") 
    }
    //重载 () 
    "()"(arg1, arg2) {
        print("call () ", arg1, arg2)
    }
    //重载 +
    "+"(value) {
        this.value += value
        return this
    }
    //key也可以使用 number
    1 : 11111
}
//数字只能使用 [] 访问
print(base[1])
var c = "hello1"
//如果是变量可以使用 [] 访问
base[c]()

base.hello1()
base.hello2()
base(100, 200)
base += 300
print(base.value)

var base1 = clone(base)	    //完全复制一个table 里面的数据不会共享
var base2 = clone(base)
base1.value = 111
base2.value = 222
print("base1 : " + base1.value)
print("base2 : " + base2.value)