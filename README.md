# Scorpio-CSharp #
* author : while
* QQ群 : 245199668 [加群](http://shang.qq.com/wpa/qunwpa?idkey=8ef904955c52f7b3764403ab81602b9c08b856f040d284f7e2c1d05ed3428de8)

**此脚本是用作Unity游戏热更新使用的脚本,纯c#实现 基于.net2.0 兼容所有c#平台 语法类似 javascript**
* **脚本示例** 放在 [ScorpioDemo/Scripts](https://github.com/qingfeng346/Scorpio-CSharp/tree/master/ScorpioDemo/Scripts) 目录
* **语法测试** 可以先在此路径体验 http://sina.fengyuezhu.com/demo/ (必须安装UnityWebPlayer)
* **语法测试** 直接运行 **ScorpioDemo/bin/Debug/ScorpioDemo.exe**  左侧选中要测试的脚本,点击 **Run Script** 按钮即可
* **java版的Scorpio脚本** Scorpio-Java https://github.com/qingfeng346/Scorpio-Java
* **网络协议生成工具** ScorpioConversion https://github.com/qingfeng346/ScorpioConversion
* **性能测试** (C#light,ulua,Scorpio-CSharp) https://github.com/qingfeng346/ScriptTestor

## Unity3d发布平台支持(亲测):
- [x] Web Player
- [x] PC, Mac & Linux Standalone
- [x] iOS(包括IL2CPP)
- [x] Android
- [x] BlackBerry
- [x] Windows Phone 8
- [x] Windows 10 (Universal Windows Platform)
- [x] WebGL

## Unity导入Scorpio-CSharp:
* 第一种方法(建议) : 把trunk目录下的 Scorpio 文件夹复制到项目 然后删除 文件夹下的 Properties 文件夹和 Scorpio.csproj 文件即可
* 第二种方法 : 用VS打开Scorpio.sln编译一下项目 生成Scorpio.dll文件 然后复制到Unity项目Plugins目录下

## 上线游戏下载地址 http://pan.baidu.com/s/1sjQiCGH 欢迎下载试玩
* Sweet Sweeper 一款扫雷单机游戏 上线地址:
    * Google Play https://play.google.com/store/apps/details?id=com.Toydog.Minesweeper
    * AppStore https://itunes.apple.com/us/app/sweet-sweeper/id1049666778?l=zh&ls=1&mt=8
* 挂机游戏Demo  个人业余开发的一款挂机游戏的Demo示例

## 脚本内使用的宏定义说明:
* **SCORPIO_UWP**  UWP(Universal Windows Platform)专用 阉割掉的功能 : 
    * 不能脚本调用c#类中的 Delegate 变量
    * 不支持 ScorpioFunction 类型函数
    * 不能使用 Script.LoadFile 函数
    * require函数不可用
* **SCORPIO_DYNAMIC_DELEGATE** 动态创建Delegate对象 不适用的请自行实现一个继承DelegateTypeFactory的类

## master版本已知问题 ##

## master版本更新和修改内容 ##
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
```c#
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
```c#
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
```c#
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
```c#
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
```c#
    for (i=0,10000) {  
    }  
    for (i=0,10000,2) {  
    }
```
* 统一scriptobject产出函数 方便以后加入对象池
* 增加Unity例子 (亲测支持pc web android ios wp(需要修改一些基础函数调用,应该不影响功能使用,稍后会发布一个版本支持wp))