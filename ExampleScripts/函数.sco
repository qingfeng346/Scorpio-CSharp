//普通函数
function hello() {
    print("hello")
}
hello()

//不定参函数
function test(a,...b) {
    print(a)
    print("length : " + b.length())
    foreach (pair in pairs(b)) {
        print(pair.value)
    }
}
test(100,200,300,400)