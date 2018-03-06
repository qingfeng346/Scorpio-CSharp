var num1 = 100		//申请一个number默认保存成double型
var num2 = 100L		//数字后面加L会保存成long型
var num3 = 0xFF		//16进制数字 保存成long型
//数字比较 > >= < <=
print(num1 == 100)
print(num2 != 100)
//所有数字运算支持 + - * / %
print(num1 + 100)
//long类型支持位运算 | & ^ << >>
print(num2 | 2)

//申请一个string 单引号和双引号都可以使用 双引号内可以直接使用单引号 反之同理
//字符串前面加@申请一段不转义字符串 用法参考c#的@字符串
var str1 = "123‘123"
var str2 = '123"123'
var str3 = @"1111
2222222
333333
4444444"
var str4 = @'1111
2222222
333333
4444444'
//字符串比较 支持 == != > >= < <=
print(str1 == str2)
print(str1 > str2)
//任何值跟字符串相加都会保存成string
print(str1 + 500)

//if 操作 支持 && || 以及取反操作
if (num1 == 100 && num2 == 100 || !(str1 == "")) {
    print("true")
}
if (num1 != 100) {
    print("false")
}
print(a == 100)
//除了null和false  其他所有值判断都是true
if (null || 100 || "") {
	print("null")
}
//申请一个数组
var arr = [100,200,300]
//申请一个table
var tab = {a = 100, b = 200}