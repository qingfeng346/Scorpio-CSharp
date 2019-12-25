var arr = [100,200,300,400]
print(array.count(arr))			//1.0 语法
print(arr.length())				//2.0 语法
foreach (var p in pairs(arr)) {
    print("array : " + p.key + " = " + p.value)
}
print("=================")
var tab = {a = 100, b = 200, c = 300, d = 400}
print(table.count(tab))			//1.0 语法
print(tab.length())				//2.0 语法
foreach ( var p in pairs(tab)) {
    print("table : " + p.key + " = " + p.value)
}
print("=================")
var keys = tab.keys()
foreach (var pair in pairs(keys)) {
	print("table : " + pair.value + " = " + tab[pair.value])
}
//上面是基础foreach使用 循环数组和table
//foreach 可以自己重载 
print("=================")
function mypair(a) {
	var index = 0
	var count = a.length()
	return {
		//必须实现一个 next 函数
		next() {
			if (index < count) {
				this.key = index
				this.value = a[index]
				index += 1
				return true
			}
			return false
		}
	}
}
foreach (var p in mypair(arr)) {
	print(p.value)
}