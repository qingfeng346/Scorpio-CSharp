function test() {
    print("Hello world " + arg)
}
//函数对象可以制定变量 在函数内部可以直接使用
test.arg = 100
test()


//不定参函数
function test(a,...b) {
    print(a)
    print(array.count(b))
    foreach (pair in pairs(b))
    {
        print(pair.value)
    }
}
test(100,200,300,400)