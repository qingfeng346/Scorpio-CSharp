var baseMap = {
    value = 100,
    hello1() {
        print("hello1 " + this.value) 
    }
    //等同于 hello2()
    "hello2"() { 
        print("hello2") 
    }
    //key也可以使用 number
    1 : 11111
}
//数字只能使用 [] 访问
print(baseMap[1])
var c = "hello1"
//如果是变量可以使用 [] 访问
baseMap[c]()

baseMap.hello1()
baseMap.hello2()
print(baseMap.value)

var base1 = clone(baseMap)	    //完全复制一个table 里面的数据不会共享
var base2 = clone(baseMap)
base1.value = 111
base2.value = 222
print("base1 : " + base1.value)
print("base2 : " + base2.value)