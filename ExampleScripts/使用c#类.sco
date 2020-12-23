//import_type 可以导入一个类  传入的值 必须是完整类名 
//也可以使用  push_assembly 压入一个 程序集 或者使用 load_assembly 加载一个程序集
//或者使用 Script.PushAssembly 在程序中手动压入一个程序集
Int32 = import_type("System.Int32")
SystemString = import_type("System.String")
print(Int32.MaxValue)
var int = Int32
print(int.MaxValue)

//调用import_type是要传入类的全名
Math = import_type("System.Math")
print(Math.Abs(-100))

// c# 数组可以直接使用 [] 操作
SystemArray = import_type("System.Array")
var intArray = SystemArray.CreateInstance(Int32, 10)
intArray[0] = 98989
print(intArray[0])

// c# 重载 [] 的类 可以直接使用 [] 操作
Dictionary = import_type("System.Collections.Generic.Dictionary`2")
DicIntStr = generic_type(Dictionary, Int32, SystemString)
var d = DicIntStr()
d[100] = "feawfaew"
print(d[100])

// 如果重载 [] 的 key 为 string, 则必须使用 get_Item set_Item 设置获取值
DicStrInt = generic_type(Dictionary, SystemString, Int32)
var d2 = DicStrInt()
d2.set_Item("123", 100)
print(d2.get_Item("123"))