//自动捕捉  script 和 c# exception
try {
    var a = null
    a.err()
} catch (e) {
    print(e)
}
//主动抛出error，可以抛出任意类型
try {
    throw "exception"
} catch (e) {
    print(e)
}