//创建一个泛型类 先载入泛型类
SystemString = import_type("System.String")
Int32 = import_type("System.Int32")
List = import_type("System.Collections.Generic.List`1")
//声明泛型类
ListInt = generic_type(List, import_type("System.Int32"))
var a = ListInt()
a.Add(200)
a.Add(300)
print("Count : " + a.Count)
print(a[0], a[1])

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