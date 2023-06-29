var a = {
    activity: {
        eventData: [
            {
                groupId: 7206
            },
            {
                groupId: 7206
            },
            {
                groupId: 7206
            },
            {
                groupId: 7206
            },
            {
                groupId: 7206
            },
            {
                groupId: 7206
            }
        ]
    }
}
print(json.encode(a))
var b = json.decode(json.encode(a), false)
var eventDatas = b?.activity?.eventData
eventDatas.forEach((value) => {
    if (value.groupId != null) {
        print("true")
    } else {
        print("false")
    }
})
eventDatas.forEach((value) => {
    if (value.groupId != null) {
        print("true")
    } else {
        print("false")
    }
})
// function main() {
//     TestStaticClass.TestDelegate(json.encode({a:"fawefwaefawe",b:"fweafweaf"}))
// }
// main()
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