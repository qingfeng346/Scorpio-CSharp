var base = {
    value1 = 100,
    function hello1() { 
		print("hello1 " + this.value1) 
	}
    function hello2() { 
		print("default hello2") 
	}
}
var a = base + {
    value2 = 200,
    function hello2() { print("hello2 " + this.value2) }
}
var b = base + {
}
a.value1 = 300
a.hello1()      //输出 hello1 300
a.hello2()      //输出 hello2 200
b.hello1()      //输出 hello1 100
b.hello2()      //输出 default hello2
//用 + += 可以实现伪继承 相加的数据都是clone的,所以相互的数据不会共享

var base1 = clone(base)	//完全复制一个table 里面的数据不会共享
var base2 = clone(base)
base1.value1 = 300
print(base1.value1)
print(base2.value1)