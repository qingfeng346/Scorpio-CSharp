var t = {
    function Test() {
        return t
    }
}   

t.Test() { a : 100}
var a = t.Test() { b : 200}
print(t)
// Test().testFunc()
// function test() {

// }
// var a = [100,200]
// var b = [200,300]
// test(a..., vvv, b... )
// var eee = 0
// for (var i = 0; i < 10000000; i += 1) {
//     var a = i + 1
//     var b = 2.3
//     if(a < b) {
//         a = a + 1
//     } else {
//         b = b + 1
//     }
//     if(a == b){
//         b = b + 1
//     }
//     eee = eee + a * b + a / b
// }
// print(eee)
// Math = import_type("System.Math")
// for (var i = 0, 10000000) {
//     Math.Min(10,20)
// }
// var a = 100L
// var b = 100
// print(isDouble(b + a))
// var a = null
// var a = {
//     num = 100,
//     func() {
//         var b = () => {
//             print(this)
//         }
//         b()
//     }
// }
// a.func()