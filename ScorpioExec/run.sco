class tttt {
    func() {
        print("func")
    }
}
function main1() {
    var builder = new StringBuilder()
    builder.append("123123123")
    var t = new tttt()
    t.func()
    print(toIndex(t))
    print(toIndex(builder))
}
async function main() {
    main1()
    await sleep(1)
    var t = new tttt()
    t.func()
    print(toIndex(t))
    var builder = new StringBuilder()
    builder.append("123123123")
    print(toIndex(builder))
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