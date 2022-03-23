class Cl {
    constructor() {
        this.id = "id"
        this.dataId = "dataId"
        this.zoneId = "zoneId"
        this.mapId = "mapId"
    }
    toString() {
        return "TileInfo(${this.id}) ${this.dataId} zone:${this.zoneId} map:${this.mapId}"
    }
}
var c = new Cl()
print("wwww " + c)
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