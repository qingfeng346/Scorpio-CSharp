var a = StringMap()
a["1111"] = 111
a["2222"] = 222
foreach (var pair in pairs(a)) {
    print(pair.key + "  "  + pair.value)
}
// print(a.keys())
// var str = "var functions = new (string, ScorpioHandle)[] {\n"
// var count = 0
// foreach (var pair in pairs(Array)) {
//     str += '("${pair.key}", new ${pair.key}()),\n'
//     count += 1
// }
// print(count)
// str += "}"
// print(str)
// var a = {}
// var b = {}
// var c = {}
// a.b = b
// b.c = c
// c.a = a
// class test {

// }
// var t = new test()
// t.a = t
// var a = {}
// a.t = t
// t.www = a
// function func() {
//     // print(a)
// }
// TestStaticClass.action1 = function() {
//     // print(a)
// }
// a.a = a
// b.c = c
// c.a = a
// var b = {}
// var c = {
//     d : {}
// }
// a.b = b
// a.c = b
// a.d = b
// b.c = c
// c.a = a
// async function timer(s, func) {
//     await sleep(1)
//     d = func
//     func()
// }
// TestStaticClass = importType("TestStaticClass")
// class Test {
//     test() {
//         var _this = this
//         TestStaticClass.Timer(1, function() {
//             print(_this)
//         })
//     }
// }
// var t = new Test()
// t.test()
// class FunplusSdk {
//     init() {
//         this.aaa = TestStaticClass.www
//     }
//     get() {
//         print(this.aaa.test1)
//     }
// }
// function main() {
//     FunplusSdk.init()
//     // var a = json.decode(io.toString(.Bytes))
//     // print(a)
//     main1()
// }
// async function main1() {
//     await sleep(1)
//     FunplusSdk.get()
// }
// main()
// TestStaticClass.
// TestEnum = importType("TestEnum")
// TestEnum1 = importType("TestEnum1")
// print(toEnumString(TestEnum.Test1, TestEnum1))
// print(TestEnum1.Test1)
// CSharpInt = importType("System.Int32")
// Dictionary = importType("System.Collections.Generic.Dictionary`2")
// print(Dictionary)
// DicInt = genericType(Dictionary, CSharpInt, CSharpInt)
// var a = new DicInt()
// print(a, typeOf(a))
// var a = ["1", "2", "3"]
// a.insert(0, "4")
// print(a)
// function main() {
//     for (var i = 0, 2) {
//         switch (i) {
//             case 1:
//                 continue;
//             case 2:
//                 break;
//         }
//         print("11111")
//     }
// }
// main()
// var a = []
// // for (var i = 1, 10) {
// //     a.add(toString(i))
// // }
// foreach (var pair in pairs(Array)) {
//     var args = []
//     switch (pair.key) {
//         // case "add":
//         // case "push":
//         // case "addUnique":
//         //     args = ["add1", "add2", "add3"]
//         //     break
//         // case "insert":
//         //     args = [0, "insert1", "insert2", "insert3"]
//         //     break 
//         // case "remove":
//         //     args = ["8","9","10"]
//         //     break;
//         // case "removeAt":
//         //     args = [0]
//         //     break
//         case "clear":
//             continue;
//         // case "sort":
//         //     args = [function(a, b) { return a.compareTo(b) }]
//         //     break
//         // case "indexOf":
//         // case "lastIndexOf":
//         //     args = ["1"]
//         //     break;
//         // case "find":
//         // case "findIndex":
//         // case "findLast":
//         // case "findLastIndex":
//         // case "findAll":
//         // case "findAllIndex":
//         // case "findAllLast":
//         // case "findAllLastIndex":
//         //     args = [ function(a) { return a == "5" } ]
//         //     break
//         // case "forEach":
//         // case "forEachLast":
//         //     args = [ function(a) { print(a) } ]
//         //     break;
//         // case "convertAll":
//         // case "map":
//         //     args = [ function(a) { return a } ]
//         //     break;
//         // case "+":
//         // case "-":
//         // case "toArray":
//         //     break
//     }
//     // print("调用函数 : " + pair.key)
//     // print(a[pair.key](args...))
// }
// a.clear()
// a += b
// b.clear()
// print(a.popFirst())
// a.forEach((v) => {
//     print(v)
//     var index = a.find((_) => { return _ == v})
//     print(index)
// })
// print(a)

// async function timer(s, f) {
//     await sleep(s)
//     f()
// }
// var a = { a : 100 }
// function getdata() {
//     return a
// }
// class FunctionBuildingInfo {
//     constructor() {
//         this.data = getdata()
//     }
//     test() {
//         print(this.data.a)
//     }
// }
// var info = new FunctionBuildingInfo()
// timer(1, () => {
//     info.test()
// })

// async function main1() {
//     await sleep(10)
//     return 100
// }
// async function main() {
//     // var a = await main1()
//     print("feawfeawfaewfwef")
//     var a = {
        
//     }
//     // print("-------------------------- ")
//     print(TestClass.AddComponent(a))
//     while (true) {
//         await sleep(1000)
//     }
// }
// main()
// return ""
// a.a = 100
// a.b = []
// c# 重载 [] 的类 可以直接使用 [] 操作
// Dictionary = importType("System.Collections.Generic.Dictionary`2")
// DicIntStr = genericType(Dictionary, Int32, SystemString)
// var d = DicIntStr()
// d[100] = "feawfaew"
// print(d[100])

// 如果重载 [] 的 key 为 string, 则必须使用 get_Item set_Item 设置获取值
// DicStrInt = genericType(Dictionary, SystemString, Int32)
// var d2 = DicStrInt()
// d2.set_Item("123", 100)
// print(d2.get_Item("123"))

// "str".testFunc().objFunc()
// print(true.testFunc(false))
// 100.testFunc().objFunc()
// function sleep1(seconds) {
//     var end = io.unixNow() + seconds * 1000
//     return coroutine.poll(function() {
//         return io.unixNow() >= end
//     }, function() {
//         return "poll123123123123"
//     })
// }
// function sleep2(seconds) {
//     //coroutine.epoll 回调, 调用 coroutine.done 时跳出
//     var ret = coroutine.epoll()
//     done(ret, seconds, "epoll")
//     return ret 
// }
// async function done(ret, seconds, result) {
//     await sleep1(seconds)
//     coroutine.done(ret, result)
// }
// function main1() {
//     // var a = "12321313"
//     // return a
// }
// function main() {
//     switch (a) {
//         case 1: 
//             break
//     }
// }


// #if !DA_GLOBAL

// #if UNITY_EDITOR
// #else
// #endif

// #else

// #if UNITY_EDITOR
// #else
// #endif

// #endif
// class Cl {
//     constructor() {
//         this.id = "id"
//         this.dataId = "dataId"
//         this.zoneId = "zoneId"
//         this.mapId = "mapId"
//     }
//     toString() {
//         return "TileInfo(${this.id}) ${this.dataId} zone:${this.zoneId} map:${this.mapId}"
//     }
//     async ttt() {
//         print("tttt - 1 " + io.unixNow())
//         this.ttt1()
//         await this.sl()
//         print("tttt - 2 " + io.unixNow())
//         await sleep(5)
//         print("tttt - 3  " +  io.unixNow())
//     }
//     async ttt1() {
//         print("tttt1 - 1  " +  io.unixNow())
//         await sleep(2)
//         print("tttt1 - 2  " +  io.unixNow())
//     }
//     async sl() {
//         await sleep(1)
//     }
//     get get() {
//         return "1111,2222"
//     }
// }
// Object.addGetProperty(Cl, "get2", function() {
//     return this.get
// })
// Test = importType("Test")
// // async function sssss() {
// //     print("ssss1")
// //     await sleep(2)
// //     print("ssss2")
// // }
// async function main1(num, a, b, c) {
//     print("main1-1", num, a, b, c)
//     var s = sleep(num)
//     await s
//     print("main1-2", num, a, b, c)
// }
// function main() {
//     coroutine.start(main1(1.1, "a3", "b3", "c3"))
//     coroutine.start(main1(1.2, "a4", "b4", "c4"))
//     coroutine.start(main1(2.15, "a1", "b1", "c1"))
//     coroutine.start(main1(2, "a2", "b2", "c2"))
// }
// function testCall(a, b, c) {
//     print("testCall ", a, b, c)
// }
// main()
// async function main2(a, b, c) {
//     print("main2-1", a, b, c)
//     await sleep(1)
//     print("main2-2", a, b, c)
//     coroutine.start(main3("3", "3", "3"))
// }
// async function main3(a, b, c) {
//     print("main3-1", a, b, c)
//     await sleep(0.1)
//     print("main3-2", a, b, c)
// }
// function testCall() {
// }
// coroutine.start(main1("1", "1", "1"))
// coroutine.start(main2("2", "2", "2"))
// main("1111", "2222")
// test("3333", "44444")


// TestClass = import_type("Scorpio.TestClass")
// importExtension("Scorpio.ClassEx")
// TestClass()
// TestClass(1)
// TestClass("")
// TestClass(100, 200, 1,2,3,4,5)
// var t = new TestClass()
// t.TestFunc(1)
// t.TestDefaultFunc(1)
// t.TestArgsFunc(1)
// var refValue = { value : 100 }
// var outValue = {}
// t.TestRefFunc(refValue, outValue)
// print("ref = " + refValue.value + "  out = " + outValue.value)
// t.TestRefDefaultFunc(refValue, outValue)
// print("ref = " + refValue.value + "  out = " + outValue.value)
// t.TestRefArgsFunc(refValue, outValue)
// print("ref = " + refValue.value + "  out = " + outValue.value)
// t.TestFuncEx(100)
// t.TestDefaultFuncEx("111")
// t.TestArgsFuncEx(100,200)
// t.TestRefFuncEx(refValue, outValue)
// print("ref = " + refValue.value + "  out = " + outValue.value)
// t.TestRefDefaultFuncEx(refValue, outValue)
// print("ref = " + refValue.value + "  out = " + outValue.value)
// t.TestRefArgsFuncEx(refValue, outValue)
// print("ref = " + refValue.value + "  out = " + outValue.value)
// // var refNum = {value : 100}
// // var outNum = {}
// // t.TestFunc(refNum, outNum)  //ref out �Ĳ��� ���봫��mapֵ��Ȼ�� ref out ���ص�ֵ������Ϊ value
// // print(refNum.value, outNum.value)
// // constructor
// // var testClass = importType("ScorpioExec.TestStruct")
// // userdata.extend(testClass, "ttt", function() {
// //     print(this.value1)
// // })
// // var t = new testClass()
// // t.value1 = 22222
// // // t.TestArgs(100, 200, 300)
// // t.ttt()
// //#import "test1.sco"
// // function main1() {
// //     return  { a : 100, b: 200}
// // }
// // function main() {
// //     var { a, b } = @{ a : 100, b: 200}
// //     print(a, b)
// //     // var b = function() {
// //     //     a = 200
// //     // }
// // }
// // main()
// //test1()
// //test2()
// // print(ConstString.A.B.C)
// // a = {
// // 	b : function() {

// // 	}
// // 	c : function() {

// // 	}
// // }
// // ConstString1 = {
// //     A = 22222
// // }
// // var now = io.unixNow()
// // for (var i = 0, 10000000) {
// // 	String.csFormat("aaaaaaa")
// // }
// // print(io.unixNow() - now)
// // var now = io.unixNow()
// // for (var i = 0, 10000000) {
// // 	var a = ConstString1.A
// // }
// // print(io.unixNow() - now)
// // a = {
// // 	b : 100
// // }
// // function main() {
// //     print(ConstString.A.B.C)
// // 	print(a.b)
// // }
// // main()
// // main()
// // function main() {
// //     a = { 
// //         b : {
// //             c : function() {

// //             }
// //     }}
// //     a.b.c()
// // //     // print(a.b.c)
// // }
// // main()
// // main()
// // var a = {
	
// // }
// // function testfunc(a, b) {
// //     print(a, b)
// // }
// // testfunc(100,200,300)
// // testfunc(200,300,400)
// // var a = "111"
// // var b = "111"
// // var c = "11" + toString(1)
// // print(a === c)
// // var now = io.unixNow()
// // for (var i = 0, 10000000) {
// //     testfunc()
// // }
// // print(io.unixNow() - now)
// // var now = io.unixNow()
// // for (var i = 0, 10000000) {
// //     var b = a === null
// // }
// // print(io.unixNow() - now)