//fewafawefawetwaetawet
//ttttttttttttttttttttwerwerwerwer
//FEWTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT
//4657777777777777777777777777777777
//88888888888888888888888888888888888888
Test = {
    function test()
    {
    this.a = -10
    }
}
Test.test()
print(Test.a)
Test.test()
print(Test.a)
Test.test()
print(Test.a)
Test.test()
print(Test.a)
return;

Test = 
{
	m_a = null,
	m_Btn = null,
	function OnClick(){
		this.m_a = -1
		print(this.m_a)
	}
}
Test.OnClick()
Test.OnClick()
Test.OnClick()
Test.OnClick()
Test.OnClick()


return;
List = import_type("System.Collections.Generic.List`1")
ListInt = generic_type(List, import_type("System.Int32"))
var Add = ListInt.Add
var list = ListInt()
Add(list, 400)
Add(list, 500)

list.set_Item(0, 200)
print(list.get_Item(0))

return;
print(0x01)
a = 200
a = tolong(a)
a = a << 2
print(a)
print(type(a))

return;
for (i=0,10000)
{
List = import_type("System.Collections.Generic.List`1")
ListInt = generic_type(List, import_type("System.Int32"))
var Add = ListInt.Add
var list = ListInt()
Add(list, 400)
Add(list, 500)
foreach (pair in pairs(list))
{
//print(pair)
}
}


return;
print("hello world")
print(123123)
var count = 0
var a = 3
var b = 3434
var c = 232323 
for (i=0,100000)
//for (var i=0;i<100000;++i)
{
count = a*4+c*444+b/3
}
String = import_type("System.String")
if (true)
{
//var a = String.Format("{0:f2}-{1}-{2}-{3}-{5}",100,200,300, 400,500,600)
print(a)
}
function aaa(a,...b)
{
    foreach (pair in vpairs(b))
    {
        print(pair)
    }
}
aaa(10,200,300)
try
{
    throw 1
    throw "11111111111111111111111111"
}
catch (e)
{
    print("throw" + e)
}

var b = [100,200,300,400]
foreach (pair in kpairs(b))
{
    print(pair)
}
var a = 100
switch (a)
{
case 200:
    print("hello world")
    break;
default:
    print("default")
    break;
}