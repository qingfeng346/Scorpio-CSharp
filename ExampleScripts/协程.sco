function sleep1(seconds) {
    var end = io.unixNow() + seconds * 1000
    //coroutine.poll 轮询,返回true时跳出
    return coroutine.poll(function() {
        return io.unixNow() > end
    })
}
function sleep2(seconds) {
    //coroutine.epoll 回调, 调用 coroutine.done 时跳出
    var ret = coroutine.epoll()
    done(ret, seconds)
    return ret 
}
async function done(ret, seconds) {
    await sleep1(seconds)
    coroutine.done(ret)
}
async function main() {
    var now = io.unixNow()
    print("start")
    await sleep1(2)
    print(io.unixNow() - now)
    await sleep2(2)
    print(io.unixNow() - now)
}
main()
