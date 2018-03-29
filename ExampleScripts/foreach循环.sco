var arr = [100,200,300,400]
print(array.count(arr))
foreach ( var p in pairs(arr)) {
    print(p.key + " = " + p.value)
}
print("=================")
var tab = {a = 100, b = 200, c = 300, d = 400}
print(table.count(tab))
foreach ( var p in pairs(tab)) {
    print(p.key + " = " + p.value)
}
print("=================")
foreach (var p in kpairs(tab)) {
    print(p)
}
print("=================")
foreach (var p in vpairs(tab)) {
    print(p)
}
//上面是基础foreach使用 循环数组和table
//foreach 可以自己重载 
print("=================")
function mypair(a) {
	var index = 0
	var count = array.count(a)
	//直到不返回任何值 跳出foreach循环
	return function() {
		if (index < count) {
			return a[index++]
		}
	}
}
foreach (var p in mypair(arr)) {
	print(p)
}