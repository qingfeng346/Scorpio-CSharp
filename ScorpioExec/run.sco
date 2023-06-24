TestStaticClass = importType("TestStaticClass")
ScorpioInterface_TestInterface = importType("ScorpioInterface_TestInterface")
class Test {
    Func4(a, b, c) {
        print(a, b, c)
    }
}
t = new Test()
// t.fun()
function main() {
    // TestStaticClass.TestDelegate(json.encode({a:"fawefwaefawe",b:"fweafweaf"}))
    TestStaticClass.testInterface = new ScorpioInterface_TestInterface() { Value: t }
    TestStaticClass.TestI("a", "b", "c")
}
main()
// // async function get1() {
// //     return "Fewafawefaewf"
// // }
// // async function get() {
// //     var a = "123213"
// //     var b = "Fewfawf"
// //     var c = "fewfawfaw"
// //     var d = await get1()
// //     return d
// // }
// async function main() {
//     var a = "faewfawfe"
//     // await get()
//     await sleep(1)
//     // await get()
// }
// main()
// TestStaticClass.AddTimer(0.1, () => {
//     coroutine.stopAll()
// })