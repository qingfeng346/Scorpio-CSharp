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