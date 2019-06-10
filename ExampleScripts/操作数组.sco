a = [100,200,300,400]	//申请数组
print(a[0])				//获取数组
print(array.count(a))	//获取数组长度
a[10] = 5				//如果索引大于数组长度 数组长度会自动扩充
print(array.count(a))
print(a[20])			//获取数组索引大于数组长度 数组长度不会自动扩充
//下面是循环数组的几种方法
print("=================")
foreach ( var pair in pairs(a)) {
    print("${pair.key} = ${pair.value}")
}
print("=================")
for (var i = 0, array.count(a) - 1) {
	print("${i} = ${a[i]}")
}
print("=================")
for (var i = 0; i < array.count(a); i += 1) {
	print("${i} = ${a[i]}")
}
//更多操作数组的函数可以查看 LibraryArray.cs