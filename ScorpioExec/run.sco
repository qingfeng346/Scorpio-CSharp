TestClass = importType("TestClass")
var t = new TestClass()
t.TestFunc1()
// async function test(epoll, sec) {
//     await sleep(sec)
//     coroutine.done(epoll, {a:100})
// }
// function sleep1(sec) {
//     var epoll = coroutine.epoll()
//     test(epoll, sec)
//     return epoll
// }
// function sleep2(sec) {
//     return coroutine.poll(function() {
//         return true
//     }, function() {
//         return {a:13,b:21321}
//     })
// }
// async function ttt() {
//     await sleep1(1)
//     return "Fewafaewf"
// }
// async function main() {
//     var a = await sleep2(1)
//     print(a)
// }
// main()