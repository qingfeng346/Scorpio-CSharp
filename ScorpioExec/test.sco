class Test1 {
    constructor() {
        this.a = 100
        print("11111 : " + this)
    }
    hello() {
        print("hello1")
    }
}
class Test2 : Test1 {
    constructor() {
        base.constructor()
        this.b = 200
        print("22222 : " + this)
        print(this.a, this.b)
    }
    hello() {
        print("hello2")
    }
}
class Test3 : Test1 {
    constructor() {
        this.a = 500
        base.constructor()
        this.b = 300
        print("33333 : " + this)
        print(this.a, this.b)
        print(base.a)
    }
    hello() {
        print("hello3")
    }
}
var t2 = new Test2()
var t3 = new Test3()
t2.hello()
t3.hello()
// aaa = {}
// b = 100
// print(aaa, b)
// var a = aaa[b] ?? (aaa[b] = [])
// a.add(100)
// var a = createStringBuilder()
// for (var i = 0, 100) {
//     a.append('0')
// }
// a.setLength(100)
// a[10]= 97
// print(a)
// a[50] = 98
// print(a)
// a = []
// (b ?? a).add(100)
// print(a)

// var TestStruct = import_type("ScorpioExec.TestStruct")
// var t = TestStruct()
// t.value1 = 200
// t.value2 = 300
// t.staticNumber = 500
// TestStruct.staticNumber = 1000
// print(t.value1, t.value2, t.staticNumber)
// var a = [100,200]
// foreach (var pair in pairs(a)) {
//     try {
//         b()
//     } catch (e) {
//         print(pair)
//     }
// }
// tab = {
//     test() {
//         this.testfun?.()
//         print("fewafawefawefaewf")
//     }
//     testfun() {
//         print("tetwt")
//     }
// }
// tab.test()
// function test() {
//     try {
//         throw "fewafwaef"
//     } catch (e) {
//         print("fewafawefwea")
//     }
// }
// test()
// CSharpSingle = {
//     PositiveInfinity : 1
// }
// class Color {
// }
// class Vector2 {

// }
// Screen = {
//     width = 1,
//     height = 1,
// }
// LayerMask = {
//     NameToLayer() {
//         return 1
//     }
// }
// FileUtil = {
//     GetMD5FromString() {
//         return "123123"
//     }
// }

// CONFIG_CUSTOM_ACCOUNT = "CONFIG_CUSTOM_ACCOUNT"
// CONFIG_CUSTOM_ENTRY_URL = "CONFIG_CUSTOM_ENTRY_URL"

// SinglePositiveInfinity = CSharpSingle.PositiveInfinity      //float 正无穷

// ColorRed = Color(1, 0, 0, 1)        //红色
// ColorWhite = Color.white            //白色
// ColorGreen = Color(0, 1, 0, 1)      //绿色
// ColorGray = Color(0.65, 0.65, 0.65) //灰色

// TileEdgeColor = Color(1, 1, 1, 0.5)             //格子在云边缘颜色
// TileNormalColor = Color.white                   //正常格子颜色
// TileActiveColor = Color(0.7, 0.7, 0.7, 1)       //格子被选中后闪烁颜色
// TileFieldActiveClour = Color(0.7, 0.7, 0.7, 1)  //田地被选择后的颜色

// ScreenWidth = Screen.width                           //屏幕宽度
// ScreenHeight = Screen.height                         //屏幕高度
// ScreenCenter = new Vector2(ScreenWidth / 2, ScreenHeight / 2)   //屏幕中心点

// UIHeight = 750                                       //UI虚拟屏幕高度
// UIWidth = ScreenWidth / ScreenHeight * UIHeight      //UI虚拟屏幕宽度
// UIVirtualHeight = 750                                //UI虚拟高度
// UIVirtualWidth = 1334                                //UI虚拟宽度

// LayerTerrain = 1L << LayerMask.NameToLayer("Terrain")       // Terrain 层

// ITEM_DIAMOND = 100000       //钻石ID
// ITEM_EXP = 101111           //经验
// ITEM_COIN = 102222          //金币
// ITEM_HEART = 103333         //爱心
// ITEM_ENERGY = 104444        //体力

// STORE_TAB_DIAMOND = 0       //商店 钻石 tab页
// STORE_TAB_HEART = 1         //商店 爱心 tab页
// STORE_TAB_COIN = 2          //商店 金币 tab页
// STORE_TAB_ENERGY = 3        //商店 体力 tab页

// SHOP_TAB_FACTORY = 0        //商店 工厂 tab页
// SHOP_TAB_BUILDING = 1       //商店 建筑 tab页
// SHOP_TAB_CROP = 2           //商店 作物 tab页
// SHOP_TAB_DECORATION = 3     //商店 装饰 tab页
// SHOP_TAB_RECOVERED = 4      //商店 回收 tab页

//GM 密钥
// GM_SECRET_KEY = FileUtil.GetMD5FromString("${Application.identifier}_${Application.platform}_${Application.version}_${SystemInfo.deviceModel}_${SystemInfo.deviceName}_${SystemInfo.systemMemorySize}_${math.floor(io.unixNow() / 1000 / 86400)}")

// PAUSE_DRAGON_AI = false

// CloudClickEffectID = 3      //点击云雾特效ID
// HillClickEffectID = 4       //点击石头特效ID


// bbb()
// function ccc(a,b,c) {
//     print(a,b,c)
// }
// ccc?.(aaa()...)
// print("over")
// print(io.unixNow() / 1000 / 86400)
// print(io.unixNow())
// TestClass = import_type("ScorpioExec.TestClass")
// function test() {
//     TestClass.TestStaticFunc("test1")
// }
// function test1() {
//     TestClass.TestStaticFunc("test2")
// }
// function test2() {
//     throw "fewafwaefwaf"
// }
// test()
// function test() {
//     throw "12312312312312213"
// }
// function test1() {
//     test()
// }
// test1()
// function test() {
//     try {
//         print("try1")
//         try {
//             print("try2")
//             throw "fewafwaefwa"
//         } catch (e) {
//             print("catch2 : " + e)
//         }
//         print("12312312")
//     } catch (e) {
//         print("catch1")
//     }
// }
// test()
// print("hello")

// function www() {

// }

// var tab = {}
// var tab1 = {}
// var a = print.bind(tab)
// var b = print.bind(tab)
// a("111111111")

// var arr = []
// arr.add(a)
// print(arr.length())
// arr.remove(print.bind(tab))
// print(arr.length())
// TestClass = import_type("ScorpioExec.TestClass")
// var t = new TestClass(111)
// function aaa() {
//     t.num = "1232131"
//     // TestClass.TestStaticFunc()
// }
// function bbb() {
//     aaa()
// }
// function ccc() {
//     bbb()
// }
// ccc()



// importExtension("ScorpioExec.TestClassEx")
// TestClass = import_type("ScorpioExec.TestClass")
// var a = TestClass(100);
// a.TestFuncEx(500)
// print(a.TestNumber)
// StoreTabType = {
//     Coin : 2,
//     Heart : 3,
//     Energy : 4,
//     Dragon : 21,
//     Building : 22,
//     Crop : 23,
//     Factory : 25,
//     Decoration : 40,
//     GetString : function(id) {
//         switch (id) {
//             case 2: return 'Coin'; 
//             case 3: return 'Heart'; 
//             case 4: return 'Energy'; 
//             case 21: return 'Dragon'; 
//             case 22: return 'Building'; 
//             case 23: return 'Crop'; 
//             case 25: return 'Factory'; 
//             case 40: return 'Decoration'; 
//         }
//     }
// }
// ReddotType = {

// }
// function hello(a) {
//     switch (a) {
//         case 0: return;
//         case 1: return;
//     }
// }
// hello(0)

// var a = "AAA"
// print(a.toOneLower())

// DownloadStatus = {
//     RequestAssets : 100
// }
// class DownloadAssets {
//     SetStatus(status) {
//         print("setstatus : ", this)
//     }
//     Exe() {
//         this.SetStatus(DownloadStatus.RequestAssets)
//     }
// }
// var d = DownloadAssets()
// d.Exe()
// b = {a = "222"}
// var a = b?.[test()]
// print(a)
// var a = false
// a |= true
// print(a)
// print(a)
// TestClass = import_type("ScorpioExec.TestClass")
// importExtension("ScorpioExec.TestClassEx")
// importExtension("ScorpioExec.TestClassEx")
// var a = TestClass(200)
// // a.TestFuncEx(1111, 2222, 1, 2, 3)
// var ref = {value = 11111}
// a.TestFuncEx(ref)
// print(ref)
// a.TestArgs(1111, 1, 2, 3)
// class Cl {

// }
// var a = Cl()
// for (var i = 0 ; i < 10000000; i++) {
// 	a.a = 100
// 	a.b = 100
// 	a.c = 200
// 	a.d = 2222
// 	a.e = "feawfaewf"
// 	a.f = "wefwae"
// 	a.h = "wweee"
// }
// var a = null
// var b = false
// var c = (a ?? b) ?? "fewa"
// print(c)
//print("hello world")
//print(String.format("aaaa{}wwww", 100, 200))
//var d = !w
//print (d)
// var a = json.decode(`{"aaaa" : 100L}`)
// print(a)
// var a = []
// a.addUnique(100)
// a.addUnique(100)
// print(a.popLast())
// print(a.popLast())
// function hello(a1,a2,a3,a4) {
// }
// for (var i = 0 ; i < 1000000; i ++) {
//     hello([100,200]...,[300,400]...)
// }
// for (var i = 0 ; i < 1000000; i ++) {
//     hello([100,200],[300,400])
// }
// hello([100,200]...,[300,400], ["feawf", "feawfafw"]...)
// // var a = b == 100 ? 100 : 200
// // print(a)
// UI_DOWNLOAD = {

// }
// UI = {
//     Objects = {},       		//所有UI的信息
//     function InitializeUI(index) {
//         // print("=================== " + index)
// 		// if (this.Objects.containsKey(index)) { return this.Objects[index]; }
// 		// print("22222222222222222222222222222 " + index)
//         // var str = "${index}_ATTR"
//         // print(str)
//         // var attribute = _G["${index}_ATTR"]
//         var com = UI_DOWNLOAD
//         // var com = isMap(value) ? clone(value) : value()		//如果是map就clone 否则就是 class 直接new一个对象
//         // var com = clone(value)
//         com.Hide = function(args) { UI.Hide(index, args) }
//         com.Hide()
//     }
// }
// UI.InitializeUI("feawfaewfaewf")
// // class www {

// // }
// // try {
// //     throw "1111"
// //     throw new www()
// // } catch (e) {
// //     if (getPrototype(e) == www) {

// //     }
// // }
// // var a = 100
// // var b = 111
// // switch (a) {
// //     case 50 + 50:
// //     case 300:
// //         for (var i = 0;i<100;i++) {
// //             if (i == 10) {
// //                 print("ffffffffffffff")
// //                 break
// //             }
// //         }
// //         print("111111111111")
// //     case 200:
// //         print(200)
        
// //     default:
// //         print("default")
// //         break
// // }

// // class Cl {

// // }
// // var a = new Cl()
// // setPropertys(a, {a : 100, b : 200})
// // print(json.encode(a))
// // var t = {
// //     function Test() {
// //         return t
// //     }
// // }   

// // t.Test() { a : 100}
// // var a = t.Test() { b : 200}
// // print(t)
// // Test().testFunc()
// // function test() {

// // }
// // var a = [100,200]
// // var b = [200,300]
// // test(a..., vvv, b... )
// // var eee = 0
// // for (var i = 0; i < 10000000; i += 1) {
// //     var a = i + 1
// //     var b = 2.3
// //     if(a < b) {
// //         a = a + 1
// //     } else {
// //         b = b + 1
// //     }
// //     if(a == b){
// //         b = b + 1
// //     }
// //     eee = eee + a * b + a / b
// // }
// // print(eee)
// // Math = import_type("System.Math")
// // for (var i = 0, 10000000) {
// //     Math.Min(10,20)
// // }
// // var a = 100L
// // var b = 100
// // print(isDouble(b + a))
// // var a = null
// // var a = {
// //     num = 100,
// //     func() {
// //         var b = () => {
// //             print(this)
// //         }
// //         b()
// //     }
// // }
// // a.func()