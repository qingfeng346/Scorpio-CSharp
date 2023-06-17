//测试调用 c# delegate
TestStaticClass = importType("TestStaticClass")
TestStaticClass.action1 = function() {
    print("Action")
}
TestStaticClass.func1 = function(arg) {
    print("Func : " + arg)
    return "haha"
}
print(TestStaticClass.TestDelegate("test"))