## v2.2.1 (*2021-08-27*)
* `[新增]`支持 #import 预编译, 多文件可以编译成单文件
* `[新增]`支持 decimal 类型
* `[新增]`支持枚举为long类型转换
* `[新增]`新增原生HashSet类型
* `[优化]`整理Script Load相关函数
* `[优化]`加速Array，IList取值速度
* `[修复]`修复快速反射数组类型问题

## v2.2.0 (*2021-07-27*)
* `[新增]`支持多变量赋值 [`示例参考`](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/%E5%A4%9A%E8%BF%94%E5%9B%9E%E5%80%BC.sco)
    * 暂不支持全局变量
* `[新增]`Object 增加类函数 getHashCode
* `[新增]`Object 增加静态函数 referenceEquals

## v2.1.0 (*2021-06-23*)
* `[新增]`支持宏定义语法
* `[新增]`支持协程 [示例参考](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/%E5%8D%8F%E7%A8%8B.sco)
    * 需在主线程调用 script.UpdateCoroutine()
    * 扩展协程类型请重载 ICoroutineProcessor [示例参考](https://github.com/qingfeng346/Scorpio-CSharp/tree/v2.0/ScorpioUnityTest/Assets/Scripts/CoroutineProcessor.cs)
* `[修复]`修复 _G foreach 问题

## v2.0.14 (*2021-05-11*)
* `[新增]`增加Script.Shutdown函数,卸载脚本内存
* `[优化]`性能优化
* `[优化]`优化脚本占用内存
* `[优化]`增加`ScriptMapString`类型,key只能为string类型,通过 `@{}` 申请
* `[优化]`优化json.encode函数,增加递归引用的判断

## v2.0.13 (*2021-01-26*)
* `[优化]`优化调用c#函数效率
* `[优化]`优化堆栈储存效率
* `[优化]`json.decode 支持N解析为long类型

## v2.0.12 (*2020-11-26*)
* `[新增]`命令行增加 **os,net** 库

## v2.0.11 (*2020-10-28*)
* `[修复]`修复 forsimple 内 continue 问题


## v2.0.10 (*2020-09-07*)
* `[新增]`新增 gc 函数
* `[优化]`Array.Sort 函数支持返回 bool 类型
* `[优化]`修改 NewMap 指令, 初始化 Map 支持128个以上元素
* `[修复]`修复 try catch 在 IL2CPP 下会报错的问题(由于IL2CPP finally规则和.net平台不一致导致)

## v2.0.9 (*2020-07-11*)
* `[新增]`增加 StringBuilder 原型类
* `[新增]`String 支持 []
* `[新增]`String 函数 at 第二个参数支持返回 code 或 String
* `[新增]`String 增加 indexOfChar  lastIndexOfChar 函数
* `[新增]`Array Map StringBuilder 支持 new 语法
* `[新增]`支持 base 语法, 原 base 函数改为 getBase
* `[新增]`Array 增加 convertAll 函数
* `[新增]`Map 增加 find findValue 函数
* `[优化]`整理 GetHashCode
* `[优化]`Array forEach 返回值改为索引值
* `[优化]`Map forEach 返回值改为Key值

## v2.0.8 (*2020-06-10*)
* `[新增]`增加**isNull**函数
* `[新增]`增加**interface**命令,可以生成一个interface的继承中间类
* `[新增]`快速反射生成支持 struct set属性以及Field的赋值
* `[优化]`优化语法解析, **return [ (** 前方如果是回车则认为是新一句执行,彻底排除 **;** 的使用
* `[优化]`优化 **String.toCharCode** 函数, 增加第二个参数:是否解析为数组
* `[优化]`**String.at** 函数返回改成 **long**)
* `[修复]`修复 **Array.forEachLast** 报错的问题

## v2.0.7 (*2020-05-29*)
* `[新增]`增加**toChar**函数
* `[新增]`**String**库增加**charCodeAt padLeft padRight compareTo**函数
* `[新增]`增加**ScorpioUnGenerateAttribute**属性类,可以过滤快速反射生成
* `[新增]`支持解析\u字符串
* `[修复]`修复某些类的构造函数调用失败的问题
* `[修复]`修复扩展函数引入会失效的问题

## v2.0.6 (*2020-05-19*)
* `[新增]`**String** 库增加 **fromCharCode toCharCode** 函数
* `[优化]`优化**foreach,forsimple**运行效率
* `[修复]`修复**foreach,forsimple**内发生**catch**会报错的问题

## v2.0.5 (*2020-04-16*)
* `[优化]`优化函数调用逻辑
* `[修复]`修复 try catch 导致堆栈错误的BUG
* `[修复]`修复 ?. 操作会导致栈泄露

## v2.0.4 (*2020-02-28*)
* `[新增]`增加 try catch throw 语法 [示例参考](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/trycatch.sco)
* `[新增]`增加编译时排除某个全局函数的接口, 命令行参数 --ignore(-g)
* `[优化]`importExtension 引入扩展函数, **this**类型的的子类没有引入的问题
* `[优化]`优化脚本报错堆栈信息
* `[优化]`bind后的函数相等判断,只要函数主体和bind类型一致则相等

## v2.0.3 (*2019-12-23*)
* `[新增]`增加 ?. 语法, 自动判断前置变量是否为 null , 如果为 null 则直接返回 null
    * [] 语法 ?.[] 
    * () 语法 ?.()
* `[新增]`bool 类型支持 | 和 & 运算符
* `[新增]`添加 importExtension 函数, 参数为静态类的 type, 可以导入扩展函数
* `[新增]`String 类增加 toOneUpper(首字母大写) toOneLower(首字母小写) 函数
* `[修复]`修复 || 运算符前置值只有为true直接返回的问题,非空不会直接返回
* `[修复]`修复快速反射 get set 的一个bug
* `[修复]`修复调用c#不定参函数的问题

## v2.0.2 (*2019-11-01*)
* `[新增]`添加 **SCORPIO_DEBUG** 宏定义, 可以使用 Script.GetStackInfo GetStackInfos 获取脚本堆栈信息
* `[新增]`print printf 输出增加 文件:行 信息
* `[新增]`null 支持 ! 取反操作, 返回 true
* `[新增]`Array 类添加 toArray 函数，返回一个 c# 的数组
* `[新增]`快速反射支持导出扩展函数
* `[新增]`快速反射支持导出部分模板函数
* `[优化]`优化运算符调用GC问题
* `[修复]`修复 while 循环 break 和 continue 的问题
* `[修复]`修复 快速反射生成 new 覆盖的 Property 错误的问题

## v2.0.1 (*2019-10-09*)
* `[新增]`新增 **??** 表达式, 返回值为 **null** 则返回 **??** 后的值
* `[优化]`运行文件 修改为 **.net core 3.0** 生成, 提升启动速度, 优化文件大小

## v2.0.0 (*2019-09-06*)
* `[优化]`运行方式改为**IL**执行,执行效率大幅提升
* `[优化]`增加原表操作
* `[优化]`其他修改内容可以查看 **v2.0.0-preview** 版本更新日志

## v2.0.0_preview11 (*2019-08-08*)
* `[新增]`增加 赋值操作返回值(重要) [示例参考](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/%E5%9F%BA%E7%A1%80%E8%AF%AD%E6%B3%95.sco)
* `[新增]`增加 函数参数展开传递(重要) [示例参考](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/%E5%87%BD%E6%95%B0.sco)
* `[新增]`增加 同时申请多个局部变量 例如 : var a,b,c,d
* `[新增]`增加 createArray 函数, 快速创建一个 c# 数组
* `[新增]`Array 类增加 push 函数, 功能同 add
* `[优化]`修改定义class时父类为null,则自动继承Object
* `[修复]`修复 json 格式化字符串时特殊字符未格式化完全的问题

## v2.0.0-preview10 (*2019-07-09*)
* `[新增]`增加switch语法 [示例参考](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/switch.sco)
* `[新增]`支持 long 和 double 之间直接运算比较
* `[新增]`lambada 表达式申请的函数, this 继承父级 [示例参考](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/%E5%87%BD%E6%95%B0.sco)
* `[新增]`支持动态定义class [示例参考](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/class%E8%AF%AD%E6%B3%95.sco)
* `[新增]`foreach pairs 支持传入 type 实例 和 _G
* `[新增]`Array 原表增加 join 函数
* `[新增]`Map 原表增加 forEach forEachValue 函数
* `[新增]`Array Map forEach 函数返回 false 时停止循环
* `[新增]`math 库增加 三角函数
* `[新增]`快速反射支持 op_Implicit 函数
* `[新增]`命令行增加 version 命令
* `[优化]`优化 == 运行逻辑
* `[修复]`修复 + 运算符 右侧为 string 时某些情况下返回的不是 string 的问题
* `[修复]`修复 快速反射 构造函数含有 ref out 标识时生成出错

## v2.0.0-preview9 (*2019-06-26*)
* `[新增]`支持 **long** 和 **double** 类型之间直接运算比较
* `[新增]`**Array** 原表增加函数 **join** 
* `[新增]`**Map** 原表增加函数 **forEach forEachValue**)
* `[新增]`**Array Map** **forEach** 函数, 返回 **false** 则停止循环
* `[新增]`**math** 库增加三角函数 **sin sinh asin cos cosh acos tan tanh atan**)
* `[新增]`**lambada**表达式申请的funstion, this继承父级 [脚本示例](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/%E5%87%BD%E6%95%B0.sco)
* `[新增]`快速反射 支持 **op_Implicit** 函数
* `[新增]`快速反射 修复构造函数 有 **ref out** 时 生成出错
* `[新增]`修改 **class** 定义方式，支持动态定义 **class** [脚本示例](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/class%E8%AF%AD%E6%B3%95.sco)


## v2.0.0_preview8 (*2019-06-17*)
* `[新增]`增加原表操作, 基础原表目录 Scorpio/src/Proto [脚本示例](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/class%E8%AF%AD%E6%B3%95.sco)
* `[新增]`原表支持运算符重载 基础类型(number,string,bool,null)除外 [脚本示例](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/%E8%BF%90%E7%AE%97%E7%AC%A6%E9%87%8D%E8%BD%BD.sco)
* `[新增]`支持调用含有 ref out 标识的函数 [脚本示例](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/%E8%B0%83%E7%94%A8ref.out%E5%87%BD%E6%95%B0.sco)
* `[新增]`快速反射支持创建模板函数,支持 ref out 标识的函数
* `[新增]`c#数组支持直接 [] 获取元素 [脚本示例](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/%E4%BD%BF%E7%94%A8c%23%E7%B1%BB.sco)
* `[新增]`c#重载 **[]中括号(除去key为string的重载)**, 支持 [] 直接调用 [脚本示例](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/%E4%BD%BF%E7%94%A8c%23%E7%B1%BB.sco)
* `[新增]`c#元素支持 += -= 等 运算符赋值操作
* `[新增]`支持生成脚本 IL 文件，运行时可以省去解析编译的过程
* `[新增]`添加 ScriptNamespace 类， 可以使用 importNamespace 导入， 引用c#的命名空间
* `[新增]`增加 IO 库
* `[新增]`命令行增加生成快速反射文件和IL文件命令, 可以使用 sco -help 查看
* `[优化]`优化脚本性能, 修改为栈编译运行,部分运行测试,左侧为2.0版本,测试结果,基础运算大幅提升,其他操作也有较大的提升
    ![](https://raw.githubusercontent.com/qingfeng346/Scorpio-CSharp/v2.0/%E6%80%A7%E8%83%BD%E6%B5%8B%E8%AF%95/1.png)
    ![](https://raw.githubusercontent.com/qingfeng346/Scorpio-CSharp/v2.0/%E6%80%A7%E8%83%BD%E6%B5%8B%E8%AF%95/2.png)
    ![](https://raw.githubusercontent.com/qingfeng346/Scorpio-CSharp/v2.0/%E6%80%A7%E8%83%BD%E6%B5%8B%E8%AF%95/3.png)
    ![](https://raw.githubusercontent.com/qingfeng346/Scorpio-CSharp/v2.0/%E6%80%A7%E8%83%BD%E6%B5%8B%E8%AF%95/4.png)
* `[优化]`所有库函数名重写,命名规则为骆驼式命名法, 第一个单词以小写字母开始,从第二个单词开始以后的每个单词的首字母都采用大写字母, 不过可以调用Script.LoadLibraryV1, 兼容 1.0 命名规则库
* `[优化]`function内的this变量规则为前置变量,规则参考javascript [脚本示例](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/%E5%87%BD%E6%95%B0.sco)
* `[优化]`设置快速反射类的函数修改为 Scorpio.Userdata.TypeManager.SetFastReflectClass
* `[丢弃]`删除 kpairs vpairs 函数
* `[丢弃]`删除 try catch
* `[丢弃]`删除 switch 语法
* `[丢弃]`删除引用扩展函数,请使用静态函数方式调用

## v1.0.11 (*2019-05-28*)
* `[优化]`支持自动关联 c# delegate，省去 DelegateFactory 的设置
* `[修复]`修复一个去反射函数调用的BUG


## v1.0.10 (*2019-03-19*)
* `[新增]`添加 **io** 库
* `[新增]`**table** 库添加 **define_function** 函数, 可以动态申请内部函数,例如
```javascript
var tab = { num = 100 }
table.define_function(tab, "fun", function() {
    return this.num
})
print(tab.fun()) //输出100
```
* `[新增]`**#** 可以代替 **function** 关键字申请函数, 示例:
```javascript
#test() { }
//等同于
function test() { }
```

## v1.0.9 (*2019-03-15*)
* `[新增]`支持 **`** 符号申请字符串, 用法同 单引号 双引号一致, 支持 **@** 符号

## v1.0.8 (*2019-03-14*)
* `[新增]`申请**array**时分隔符支持 分号 **;**  示例;
```javascript
var a = [100;200;300]
//等同于
var a = [100,200,300]
```
* `[新增]`支持脚本直接调用c#运算符 

运算符号 | 反射名称                       | 脚本直接调用
-----   |  ----                         | ----
\+      |  op_Addition                  | 支持(+= 不支持)
\-      |  op_Subtraction               | 支持(-= 不支持)
\*      |  op_Multiply                  | 支持(*= 不支持)
/       |  op_Division                  | 支持(/= 不支持)
%       |  op_Modulus                   | 支持(%= 不支持)
\|      |  op_BitwiseOr                 | 支持(\|= 不支持)
&       |  op_BitwiseAnd                | 支持(&= 不支持)
^       |  op_ExclusiveOr               | 支持(^= 不支持)
\>      |  op_GreaterThan               | 支持
\>=     |  op_GreaterThanOrEqual        | 支持
<       |  op_LessThan                  | 支持
<=      |  op_LessThanOrEqual           | 支持
==      |  op_Equality                  | 不支持
!=      |  op_Inequality                | 不支持
[]      |  get_Item(获取变量)            | 不支持
[]      |  set_Item(设置变量)            | 不支持

## v1.0.7 (*2019-03-11*)
* `[新增]`可以访问 c# 类私有变量和函数（去反射不支持）
* `[新增]`增加 **let** 关键字,用法同 **var local**)
* `[新增]`增加 **string.cs_format** 函数,格式化c#字符串, 示例 : 
```javascript
//输出 00100,200
print(string.cs_format("{0:D5},{1}", toint(100), 200))
```
* `[新增]`增加 **array.toarray** 函数, sco array 转 c# array, 示例 :
```javascript
// arr 为 int[] {100,200,300}
var arr = array.toarray([100,200,300], import_type("System.Int32"))
```
* `[新增]`增加 **userdata.typeof** 函数,获取c#类变量的类型(支持去反射), 示例 :
```c#
//c# 代码
public class TestClass {
    public int a;
    public string b { get; set; }
}
```
```javascript
//sco 代码
var TestClass = import_type("TestClass")
//t1 的类型为 System.Int32 等同于 import_type("System.Int32")
var t1 = userdata.typeof(TestClass, "a")
//t1 的类型为 System.String
var t2 = userdata.typeof(TestClass, "b")
```

## v1.0.5 (*2019-02-15*)
* `[修复]`修复临时作用域变量自运算的BUG（多谢**avatarANDY**同学的反馈）

## v1.0.4 (*2018-12-20*)
* `[修复]`修复脚本长度小于10字符时, 解析出错的问题

## v1.0.3 (*2018-11-28*)
* `[修复]`修复新脚本解析格式化字符串的问题

## v1.0.2 (*2018-11-27*)
* `[优化]`全新的脚本解析,速度更快,增加语法更容易
* `[修复]`修复struct无参构造函数调用失败

## v1.0.0 (*2017-11-08*)
* `[新增]`第一个发行版
* `[新增]`linux运行需要先安装 libunwind 和 icu，可以使用 yum install libunwind 和 yum install icu 安装

