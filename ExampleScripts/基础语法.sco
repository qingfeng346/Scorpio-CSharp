var num1 = 100		//申请一个number默认保存成double型
var num2 = 100L		//数字后面加L会保存成long型
var num3 = 0xFF		//16进制数字 保存成long型
//`` 、“”、 ‘’ 都可以申请String, ``内可以直接使用” ‘,不用转译, 其他同理
var str1 = "123‘12`3"
var str2 = '123"12`3'
var str3 = `123"12'3`
var str4 = @"1111
2222222
333333
4444444"
//字符串内部可以使用 ${} 替换成变量
var str5 = @'1111
${str1}
333333
4444444'
var str6 = @`111
222
333
444`

//数字比较 > >= < <=
print(num1 == 100)
print(num2 != 100)
//所有数字运算支持 + - * / %
print(num1 + 100)
//long类型支持位运算 | & ^ << >>
print(num2 | 2L)
//字符串比较 支持 == !=
print(str1 == str2)
//任何值跟字符串相加都会保存成string
print(str1 + 500)
print("str4 : " + str4)
//if 操作 支持 && || 以及取反操作
if ((num1 == 100 && num2 == 100) || !(str1 == "")) {
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
//申请一个Array
var arr = [100,200,300]
//申请一个Map
var tab = {a = 100, b = 200}
//赋值操作返回
var n1 = 100
var n2 = n1 += 200
var n3 = n1 = 500