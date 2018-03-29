//创建一个泛型类 先载入泛型类
List = import_type("System.Collections.Generic.List`1")
//声明泛型类
ListInt = generic_type(List, import_type("System.Int32"))
var a = ListInt()
a.Add(200)
a.Add(300)
print(a.Count)