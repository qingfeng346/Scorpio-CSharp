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
