(2018-07-02)
-----------
* 修复struct无参构造函数调用失败
* 增加Unity生成去反射函数的示例


## <font color=#00ff00>v1.0.0 (2018-03-29)</font>
***
(2018-02-07) 
-----------
* 修改解决方案为 .net core, 支持在 windows 和 mac 上直接运行 sco 命令行

(2017-11-02)
-----------
* 修改table设置一个key为null 则table的map会删除此key

(2017-10-24)
-----------
* 支持 **finally** 关键字

(2017-08-22)
-----------
* 增加字符串内部变量功能，例如：

```javascript
var a = 1
var b = 2
print("a${a}a${b+1}a")
//输出 a1a3a
//字符串内添加 ${变量} 就可以使用
```

(2017-07-05)
-----------
* 修复一个运算符重载的BUG

(2017-07-04)
-----------
* 修复一个table function 中 赋值 this 的 bug

(2017-06-21)
-----------
* 添加userdata库，只有一个 rename 函数，可以重命名 c# 函数名称，例如：

```javascript
ListType = import_type("System.Collections.Generic.List`1")
StringType = import_type("System.String")
ListString = generic_type( ListType ,  StringType)      //载入List<String>
var lt = ListString();                                  //创建一个对象
//rename 函数第一个参数可以传入c# 类对象，也可以传入 实例对象
//rename 函数只需调用一次，所有实例类就都可以使用
userdata.rename(lt, "Add", "testAdd1")                  //重命名Add函数为testAdd1，Add还可以继续使用
userdata.rename(ListString, "Add", "testAdd2")          //重命名Add函数为testAdd2，
userdata.rename(ListString, "Capacity", "testCapacity")
lt.testAdd1("111")
lt.testAdd2("222")
print(lt.testCapacity)
lt.testCapacity = 15000
print(lt.testCapacity)
```

(2017-06-20)
-----------
* 提升调用c#函数性能
* 修复一个调用c#函数BUG

(2017-06-19)
-----------
* table的function变量全部支持this关键字，例如

```javascript
tab = {
    a : 100,
    b : function() {
        //此处可以使用this
        print(this.a)
    }
    function d () {
        //此处可以使用this
        print(this.a)
    }
}
tab.c = function() {
    //此处不可以使用 this ，只有在table内部声明才可以使用this
    print(this.a)
}
```

(2017-05-27)
-----------
* **string**库添加**char2ascii**函数
* **string**库添加**ascii2char**函数
* **print**支持直接输出**array**和**table**变量

(2017-05-11)
-----------
* 修复去反射生成工具，过滤器不能过滤 property( get,set ) 函数的bug

(2017-04-20)
-----------
* 修复array初始化时会传入对象引用的问题，例如  

```javascript
var num = 0
var arr = [num]     //修改前传入的num是引用，num本身改变会应用arr[0]的值
num += 1
print(arr[0])       //修改前会输出1，修改后会输出0
```

(2017-03-24)
-----------
* string库增加join函数

(2017-02-14)
-----------
* 增加 **long** 和 **int** 类型的 **[~]取反操作** ,多谢 **[福灵心至]** 同学的建议

(2016-12-30)
-----------
* 初始化 **table** 时，后申请的变量可以使用先申请的变量，例如

```javascript
a = {
    a1 = 100,
    a2 = a1 + 100,
    a3 = func1(),
    function func1() {      //函数优先初始化
        return 100
    }
}
```
(2016-12-27)
-----------
* 修改 **if** 简洁写法最后加 **;** 会导致后面的 **else** 和 **else if** 解析错误的问题 (感谢 **[小卒 北京]** 同学的反馈)

```javascript
if (true)
    return;             //修改前此行就解析出错
else
    print("false")
```

(2016-12-06)
-----------
* 修复生成去反射工具某些情况下过滤条件判断错误的问题

(2016-11-25)
-----------
* 申请table变量可以不赋值,默认为null

```javascript
var t = {
    a,      //修改前此处会报解析错误，修改后a会默认为null，相当于 a = null,
}
```

(2016-10-20)
-----------
* 增加支持c#的扩展函数方法 具体方法请查看注意事项

(2016-8-15)
-----------
* 支持c#函数默认参数

(2016-8-4)
-----------
* ScorpioExec 项目的生成改为 sco.exe ， 把 ScorpioExec\bin\Debug 加入环境变量可以使用 [sco 文件路径] 命令直接运行一个脚本文件

(2016-8-2)
-----------
* 增加 tosbyte tobyte toshort toushort touint toulong tofloat 函数, 可以传入制定类型的数字参数

(2016-6-20)
-----------
* 优化堆栈输出内容

(2016-6-17)
-----------
* 增加报错字段名输出
    * 修改前错误输出格式 : 文件名:行数:错误信息
    * 修改后错误输出格式 : 文件名:行数[字段名]:错误信息

(2016-6-13)
-----------
* **ScorpioReflect**项目增加**GenerateScorpioDelegate**类，可以自动生成**DelegateTypeFactory**类,具体方法请查看**注意事项**

(2016-6-6)
-----------
* 修改源码代码最低支持.net3.5,之前的.net版本不再兼容
* 支持 .net core
* 优化脚本执行性能
* UWP平台宏定义改为 SCORPIO_NET_CORE

(2016-6-1)
-----------
* 去反射类过滤不生成的函数改为 实现一个继承自 Scorpio.ScorpioReflect.ClassFilter 的类, 然后调用 GenerateScorpioClass 的 SetClassFilter 设置
* 去反射生成的变量,属性,事件,函数加入名字排序,不会导致每次生成有可能导致文件发生改变,也方便查找

(2016-5-31)
-----------
* 提升脚本执行性能 5% - 10%

(2016-5-31)
-----------
* 类型null支持当作table的key值
* 类型bool可以直接传入类型当作table的key值,修改前只可以传入变量

```javascript
var a = {
    true = 100,
    null = 100,
}
print(a[true])
print(a[null])
a[true] = 200
a[null] = 200
print(a[true])
print(a[null])
```

(2016-5-20)
-----------
* 增加func库(只对脚本函数有效)
	* func.count() 返回函数参数个数
	* func.isparams() 返回函数是否是不定参函数
	* func.isstatic() 返回函数是否是静态函数(不是table内部函数)
	* func.getparams() 返回函数参数名字数组
    
(2016-4-20)
-----------
* 优化去反射工具生成代码
	* 所有函数信息直接生成代码,不会运行时获取反射,il2cpp下获取反射函数会获取不全
	* 可以使用反射函数名调用运算符重载
	* 可是使用反射函数名操作property和event
	* GenerateScorpioClass 增加 AddExclude 函数,可以在生成时去除不想生成的函数

(2016-4-12)
-----------
* table 支持 + += 操作,用此操作可以实现伪继承 示例:

```javascript
var base = {
	value1 = 100,
	function hello1() { print("hello1 " + this.value1) }
	function hello2() { print("default hello2") }
}
var a = base + {
	value2 = 200,
	function hello2() { print("hello2 " + this.value2) }
}
var b = base + {
}
a.value1 = 300
a.hello1()		//输出 hello1 300
a.hello2()		//输出 hello2 200
b.hello1()		//输出 hello1 100
b.hello2()		//输出 default hello2
//用 + += 可以实现伪继承 相加的数据都是clone的,所以相互的数据不会共享
```
(2016-4-9)
-----------
* 增加宏定义判断,用法同c#
	* 支持关键字 **#define #if #ifndef #elseif #elif #endif**
	* **#elseif**和**#elif**功能一样

```javascript
#define TEST
#if TEST
	print("1")
	#if TEST1
		print("6")
	#endif
	print("2")
#elseif TEST2
	print("3")
	#if TEST
		print("4")
		#if TEST
			print("6")
		#endif
	#endif
	print("5")
#endif
```
* 增加函数 push_search 增加一个 require 的目录
* 增加函数 push_define 增加一个宏定义
	* 注:此函数是运行时添加 #define 函数是编译时增加

(2016-4-7)
-----------
* 优化脚本array类型操作,示例 

```javascript
var a = []
a[10] = 10
var c = a[20]
////////////////赋值操作/////////////////
//修改前 a[10] 会直接报错越界 
//修改后 a 长度会自动扩充成 11 长度

////////////////取值操作/////////////////
//修改前 a[20] 会直接报错越界
//修改后 会返回null 但是 a 长度不会自动扩充
```
* 增加array库resize函数
* 修改c#类多次调用重载运算符 + - * / 会报错的BUG

(2016-3-24)
-----------
* 优化脚本执行性能,大约提升 10%-15%

(2016-3-21)
-----------
* 增加c#类去反射机制

(2016-3-9)
-----------
* c#类重载运算符,可以在脚本里直接使用 + - * / 

(2016-2-26)
-----------
* switch 语法 支持 case 变量 , case 暂时不支持return语法 只支持break

(2016-2-18)
-----------
* 修复UWP平台master配置下枚举类型会出错的BUG
* IScriptExtensions 改为 保存在 ScriptExtensions 类的 静态变量

(2016-1-27)
-----------
* string库增加split函数
* Delegate类型增加 + - 操作
* event类型增加 += -= 操作
* Delegate类型和event类型可是用 变量.Type 获取Delegate类型

(2016-1-6)
-----------
* 优化运算符,逻辑运算,赋值运算的逻辑
* 修改逻辑运算判断, 除了false和null,其他类型逻辑判断全部都是true

(2015-12-24)
-----------
* math库增加 sqrt 和 pow 函数
* 增加解析语法出错可以报出文件的功能

(2015-12-2)
-----------
* 修复foreach for循环堆栈内临时变量会变的BUG
* 优化部分错误提示

(2015-11-9)
-----------
* 实现UWP平台下阉割的功能
* Script添加IScriptExtensions(扩展函数)对象, 如果遇到某平台下不能使用的函数, 可以自行实现一个继承IScriptExtensions的对象

(2015-11-2)
-----------
* math库增加clamp函数

(2015-10-29)
-----------
* 修复table内基础(number string)变量当作函数参数传入后 然后通过自运算会影响 堆栈数据的BUG

(2015-10-28)
-----------
* 修复变长函数的参数 多次调用会覆盖参数的BUG
* 修复临时函数内不能修改父级堆栈变量的BUG

(2015-10-22)
-----------
* UWP(Universal Windows Platform) 应用去除Script.LoadFile函数,UWP平台下请自行实现

(2015-9-15)
-----------
* math库增加 floor 函数

(2015-9-14)
-----------
* 修复 functin 递归调用会导致数据错误的BUG

(2015-8-5)
-----------
* 增加ScriptNumberInt类 (可以通用 toint 函数转换成 int 类型 调用一些函数的时候 可以强制执行int类型时使用 默认传入int值还是会使用double表示  只有通用toint函数才能生成int类型)
* 基础库增加 toint is_int 函数
* 修复 long 类型 在脚本里面使用 [-]执行负值的时候 会转换成double类型的BUG

(2015-8-1)
-----------
* 适配Windows通用平台UWP(Universal Windows Platform)

(2015-7-14)
-----------
* 修复三目运算符条件判断优先级问题

(2015-7-10)
-----------
* table库增加 keys values 函数

(2015-7-7)
-----------
* array库增加 sort 函数
* 修复table array类型相等比较会报错的BUG

(2015-7-1)
-----------
* 修复某些特殊情况下 function(){} 这种类似 lambda 表达式的作用域以及值问题
* 修改bool类型 跟其他不是bool类型相等比较一律返回false

(2015-6-29)
-----------
* array库增加 popfirst safepopfirst poplast safepoplast 函数

(2015-6-27)
-----------
* ScorpioMaker转成二进制文件 第一个字节写入一个null 用来区别字符串文件
* Script类LoadFile函数支持直接载入二进制文件

(2015-6-9)
-----------
* 修复 continue 导致跳出循环的BUG

(2015-6-2)
-----------
* 修复 function(){} 这种类似 lambda 表达式的作用域以及值问题

(2015-5-26)
-----------
* ScorpioMaker工具修复 Deserialize 函数行数会多一行的问题
* 修复ScorpioMaker.Deserialize关键字null读取错误的问题
* 增加toenum函数

(2015-5-5)
-----------
* ScorpioMaker工具修复 普通字符串 "\n" 会当作回车处理的问题
* 修复三目运算符的优先级问题

(2015-4-10)
-----------
* ScorpioMaker工具修复 @"" @'' 字符串的支持

(2015-4-9)
-----------
* 修改解析文本[回车]判断 由原来的[\r\n]修改为[\n]
* 完善ScorpioMaker工具 可以由文本sco文件转换为二进制文件 也可以由二进制文件转换回文本sco文件

(2015-4-7)
-----------
* 修复返回function类型 父区域的值会变化的BUG 例如:

```javascript
function test(data) { 
	return function() {
		print(data)
	}
}
var b = test(100)
b()
test(200)
b()
/*
(修改前)上述代码会输出(b的data值会随test函数调用改变)
100
200
(修改后)上述代码会输出(b的data值不会随test函数调用改变)
100
100
*/
```
(2015-4-1)
-----------
* string库增加indexof lastindexof startswith endswith contains函数
* 修改运行时发生异常 错误输出会加上 文件行信息 例如:

```javascript
	print(null.a)
	//修改前报错会输出 类型[Null]不支持获取变量
	//修改后报错会输出 test.sco:1 : 类型[Null]不支持获取变量
```
(2015-3-31)
-----------
* 增加function类型内部变量 例如:

```javascript
function test() { print(str) }
test.str = "hello world"
/*上述代码会输出(str就相当于 test函数的变量)
hello world 
*/
test = function() { print(str) }
test.str = "hello world"
/*上述代码会输出(str就相当于 test函数的变量)
hello world 
*/
```
(2015-3-7)
-----------
* 增加c#委托和脚本function类型无缝切换 例如:

```csharp
    public delegate void Action();
    public class Test {
        public static void Func(Action action);
    }
```

```javascript
    //修改前代码要写成这样:
    Test.Func(Action( function() { } ) )
    //修改后可以去掉Action 程序会自动检测并转换
    Test.Func(function() {} )
```
(2015-3-6)
-----------
* array库增加 safepop 函数(如果array长度小于0默认返回null)
* string库增加 isnullorempty 函数
* Script类增加 ClearStackInfo 函数
* 修复某些语法情况下出错报不出堆栈的问题
* 修复相同名字相同参数类型函数泛型和非泛型判断错误的问题 例如(修改前):

```csharp
    public class Test {
        public static void Func<T>(int args) {}
        public static void Func(int args) {}
    }
//如果在脚本里面调用 Test.Func(100) 按顺序查找会先找到泛型函数 从而调用Func函数失败
//注:泛型函数声明在非泛型函数之后不会有问题
```
(2015-3-5)
-----------
* array库增加 pop 函数
* 修复循环continue会导致跳出循环的BUG (多谢[**过期**,**丶守望灬稻田**]同学提供反馈)
* 修复相同常量自运算的问题 例如(修改前) (多谢[**过期**]同学提供反馈):

```javascript
    var a = []
    for (var i=0;i<2;++i)
        array.add(a, 0)
    a[0]++
    for (var i=0;i<array.count(a);++i)
        print(i + " , " + a[i])
    /*上面代码会输出
    0 , 1
    1 , 1
    */
```
(2015-3-4)
-----------
* array库增加 remove removeat contains indexof lastindexof 函数
* table库增加 remove containskey clear 函数
* 全局函数增加 is_null is_bool is_number is_double is_long is_string is_function is_array is_table is_enum is_userdata函数
* 全局函数type函数 返回值由枚举Scorpio.ObjectType改为int型
* 增加单句执行语法  例如(修改后):

```javascript
    if (true) { 
        print("hello world ")
    }
    //上面的代码可以写成
    if (true)
        print("hello world")
	/*注:如果是 没有返回值的return
		if(true) return
		请在return后面加上[;] 否则会解析失败
		if(true) return;
	*/
```
* 修复调用c#变长参数的函数 某些情况判断错误的问题
* 修复()内区域变量[!][-]修饰符会失效的BUG 例如(修改前)(多谢[**he110world**]同学提供反馈): 

```javascript
    print((-1)) 
    //上面代码会输出 1
```

(2015-2-11)
-----------
* 增加调用c#函数 找不到合适函数的错误输出
* 修复[%]运算解析错误的问题
* 修复 for while循环 return 后没有跳出循环的BUG

(2014-12-17)
-----------
* 增加16进制表达式 16进制表达式会保存成long型 示例：print(0xffff)
* 增加位运算(| & ^ >> <<) 位运算只支持long型  示例：var a = 0L print(a |= 1)
* 增加单引号字符串声明 示例 print('hello world')
* 增加json库 json.encode  json.decode
* Script类增加LoadTokens函数
* 增加require函数 可以加载一个文件 搜索目录为 _G["searchpath"]
* 增加generic_method函数 可以声明泛型函数 示例： 

```csharp
    //c#代码
    public class Test {  
        public static T Func<T>() {  
            return default(T);  
        }  
    }  
```

```javascript
    //sco代码
    var func = generic_method(import_type("Test").Func, import_type("System.Int32"))  
    print(func())  
```
* 发布ScorpioMaker工具 可以把脚本编译成二进制数据
* 修改table类型Key值 可以为任意变量
* 修改string类型可以用 []表达式 获取指定位置的字符
* 修改 解析Array类型 最后一个值加逗号会解析失败的问题
* 修复 负值常量 多次运行 值会修改的BUG
* 增加新增功能的示例
* 发布ScorpioMessage项目 可以热更新网络协议 传送门 https://github.com/qingfeng346/ScorpioMessage

(2014-11-25)
-----------
* 增加声明泛型类的函数 示例： ListInt 就相当于c#的List<int>

```javascript
    List = import_type("System.Collections.Generic.List`1")  
    ListInt = generic_type(List, import_type("System.Int32"))   
```
* 大幅优化与c#交互效率 具体测试结果请参考 (https://github.com/qingfeng346/ScriptTestor)

(2014-11-14)
-----------
* 适配Unity3d WebGL (Unity5.0.0b1测试通过 WebGL示例地址 http://yunpan.cn/cAVkfYdGbgFug  提取码 2df5)
* 修复Unity下Delegate动态委托出错的BUG
* 修复赋值操作（如 = += -= 等）出错报不出堆栈的问题
* 优化数字和字符串的执行效率
* 同步发布Scorpio-Java 地址:https://github.com/qingfeng346/Scorpio-Java

(2014-11-4)
-----------
* 增加table声明语法  支持 Key 用 数字和字符串声明 示例：

```javascript
    var a = { 
        1 = 1, 
        "a" = a, 
        b = b
    }
```
* 增加elseif语法 现支持三种 elseif,elif,else if 都可以当作 else if 语法
* 修改 不同类型之间 做 ==  != 比较报错的问题  改成  不同类型之间==比较 直接返回false
* 增加switch语句 只支持 number和string 并且 case 只支持常量 不支持变量
* 支持try catch 语法 示例： 

```javascript
    try { 
        throw "error" 
    } catch (e) { 
        print(e) 
    }
```
* 支持脚本调用 c# 变长参数(params) 的函数
* 增加 switch trycatch import_type 示例

(2014-10-27)
-----------
* 增加赋值操作返回值  示例: 

```javascript
    if ((a = true) == true) { }
    var a = 100
    if ((a += 100) == 200) { }
```
* 修复对Unity3d Windows Phone 8 版本的兼容问题  （亲测支持wp版本）

(2014-10-18)
-----------
* 增加对Delegate动态委托的支持 示例：

```csharp
    //c#代码
    namespace Scropio {  
        public class Hello {  
            public delegate void Test(int a, int b);  
            public static Test t;  
        }  
    }  
```

```javascript
    //sco代码
    function test(a,b) {   
        print(a)  
        print(b)  
    }  
    Hello.t = Hello.Test(test)  
    Hello.t(100,200)
``` 

(2014-10-13)
-----------
* 修复已知BUG
* 增加对不定参的支持 示例：(args会传入一个Array)

```javascript
    function hello(a,...args) { }  
```
* 增加 eval函数 可以动态执行一段代码
* 删除对ulong类型的支持
* 增加基础for循环 for(i=begin,finished(包含此值),step) 示例：  

```csharp
    for (i=0,10000) {  
    }  
    for (i=0,10000,2) {  
    }
```
* 统一scriptobject产出函数 方便以后加入对象池
* 增加Unity例子 (亲测支持pc web android ios wp(需要修改一些基础函数调用,应该不影响功能使用,稍后会发布一个版本支持wp))