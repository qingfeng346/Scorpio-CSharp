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

//参数后面加 ... , 调用时 会把 array 内的参数 展开传递
//testFunc 的输出为  100    200    300    400    500
testFunc([100,200]..., 300, [400, 500]...)