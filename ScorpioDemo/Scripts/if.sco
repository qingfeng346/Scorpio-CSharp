var a = 100
if (a == 100) {
    print("true")
}
if (a != 100) {
    print("false")
}
print(a == 100)
//除了null和false  其他所有值判断都是true
if (null || 100 || "") {
	print("null")
}