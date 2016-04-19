function test() {
    print("Hello world " + arg)
}
//函数对象可以制定变量 在函数内部可以直接使用
test.arg = 100
test()