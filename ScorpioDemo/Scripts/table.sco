a = 
{
    1 = 200,
    a = 100,
    b = function ()
    {
        //不能使用this 这种声明算静态函数
        print(this == null)
        print("b")
    },
    //可以使用this 算内部函数
    function c()
    {
        print("this " + this.a)
        print("self " + self.a)
    }
    //等同于 d = 300
    "d" = 300,
    //等同于 e = 300 
    'e' = 400,
}
a.b()
a.c()
//数字只能使用 [] 访问
print(a[1])
print(a.d)
a.d = 400
print(a.d)
a["d"] = 500
print(a.d)