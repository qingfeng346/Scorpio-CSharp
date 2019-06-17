//普通函数  函数里的this 取决于获取函数的对象，规则可以参考 javascript
function hello() {
    print("hello " + this)
}
hello()         //hello 函数内 this 为 null
var tab = {}
tab.hello = hello
tab.hello()     //hello 函数内 this 为 tab


//不定参函数
function test(a,...b) {
    print(a)
    print("length : " + b.length())
    foreach (pair in pairs(b)) {
        print(pair.value)
    }
}
test(100,200,300,400)
