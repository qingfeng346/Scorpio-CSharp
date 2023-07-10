a = [100,200]	//申请数组
print(a[0])				//获取数组
a[2] = 300
print(a.length())		//获取数组长度
print(a[20])			//获取数组索引大于数组长度
//下面是循环数组的几种方法
print("=================")
foreach ( var pair in pairs(a)) {
    print("${pair.key} = ${pair.value}")
}
print("=================")
for (var i = 0, a.length() - 1) {
	print("${i} = ${a[i]}")
}
print("=================")
for (var i = 0; i < a.length(); i += 1) {
	print("${i} = ${a[i]}")
}
print("=================")
a.forEach((value, index) => {
	print("${index} = ${value}")
	//return false 可以打断循环
	if (index == 1) {
		return false
	}
})
print("=================")
a.forEachLast((value, index) => {
	print("${index} = ${value}")
	//return false 可以打断循环
	if (index == 1) {
		return false
	}
})
//更多操作数组的函数可以查看 LibraryArray.cs