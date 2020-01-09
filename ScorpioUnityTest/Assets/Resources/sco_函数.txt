//普通函数  函数里的this 取决于获取函数的对象，规则可以参考 javascript
function hello(arg) {
    print("hello this : " + this)
    var lam = () => {
        print("lambada this : " + this)     //lambada表达式申请的this 继承父级 this 为 tab
    }
    lam()
}
hello()         //hello 函数内 this 为 null
var tab = { name : "tab" }
tab.hello = hello
tab.hello(100)     //hello 函数内 this 为 tab


//不定参函数
function test(a,...b) {
    print(a)
    print("length : " + b.length())
    foreach (pair in pairs(b)) {
        print(pair.value)
    }
}
test(100,200,300,400)

//展开参数传递
function testFunc(a1, a2, a3, a4, a5) {
    print(a1, a2, a3, a4, a5)
}

//参数后面加 ... , 调用时 会把 array 内的参数 展开传递, testFunc 的输出为  100    200    300    400    500
testFunc([100,200]..., 300, [400, 500]...)