async function main() {
    var a = "1"
    timer(1, () => {
        print(a)
        a = "3"
    })
    a = "2"
    print("main")
    await sleep(2)
    print(a)
}
async function timer(s, func) {
    //新协程会延迟一帧执行
    print("timer")
    await sleep(s)
    func()
}
main()