async function get1() {
    return "Fewafawefaewf"
}
async function get() {
    var a = "123213"
    var b = "Fewfawf"
    var c = "fewfawfaw"
    var d = await get1()
    return d
}
async function main() {
    var eee = await get()
    print(eee)
    await get()
    await get()
}
main()