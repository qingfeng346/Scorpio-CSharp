TestStaticClass = importType("TestStaticClass")
// async function get1() {
//     return "Fewafawefaewf"
// }
// async function get() {
//     var a = "123213"
//     var b = "Fewfawf"
//     var c = "fewfawfaw"
//     var d = await get1()
//     return d
// }
async function main() {
    var a = "faewfawfe"
    // await get()
    await sleep(1)
    // await get()
}
main()
TestStaticClass.AddTimer(0.1, () => {
    coroutine.stopAll()
})