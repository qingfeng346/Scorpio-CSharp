class t0 {
    constructor() {
        print("t0")
    }
}
class t1 : t0 {
    constructor() {
        var c = "Feawfaewf"
        base.constructor()
        print("t1")
    }
}
class t2 : t1 {
    constructor() {
        var a = "123213213"
        var b = "Fewfaewfaewf"
        base.constructor()
        print("t2")
    }
}
var a = t2()
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