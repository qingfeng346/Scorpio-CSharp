//import_type 可以导入一个类  传入的值 必须是完整类名 
//也可以使用  push_assembly 压入一个 程序集 或者使用 load_assembly 加载一个程序集
//或者使用 Script.PushAssembly 在程序中手动压入一个程序集
String = import_type("System.String")
print(String.Format("{0}-{1}-{2}-{3}-{4}-{5}", "0", "1", "2", "3" ,"4", "5"))
Int32 = import_type("System.Int32")
print(Int32.MaxValue)
var int = Int32
print(int.MaxValue)

//调用import_type是要传入类的全名
Math = import_type("System.Math")
print(Math.Abs(-100))