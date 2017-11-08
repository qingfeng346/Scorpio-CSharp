a = {
    1 = 200,	//Key可以直接写数字
    a = 100,	//Key也可以写名字
	//等同于 d = 300
    "d" = 300,	//或者字符串
    //等同于 e = 300 
    'e' = 400,
    func1 = function () {
        print("func1 : " + this.a)
    },
    function func2() {
        print("func2 : " + this.a)
    }
}
//调用函数
a.func1()
a.func2()
//数字只能使用 [] 访问
print(a[1])
var c = "d"
//如果是变量可以使用 [] 访问
print(a[c])