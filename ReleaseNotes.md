### v1.0.8
*2019-03-14*

* 申请**array**时分隔符支持 分号 **;**  示例;
```javascript
var a = [100;200;300]
//等同于
var a = [100,200,300]
```
* 支持脚本直接调用c#运算符 

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

### v1.0.7
*2019-03-11*

* 可以访问 c# 类私有变量和函数（去反射模块不支持）
* 增加 **let** 关键字,用法同 **var local**
* 增加 **string.cs_format** 函数,格式化c#字符串, 示例 : 
```javascript
//输出 00100,200
print(string.cs_format("{0:D5},{1}", toint(100), 200))
```
* 增加 **array.toarray** 函数, sco array 转 c# array, 示例 :
```javascript
// arr 为 int[] {100,200,300}
var arr = array.toarray([100,200,300], import_type("System.Int32"))
```
* 增加 **userdata.typeof** 函数,获取c#类变量的类型(支持去反射), 示例 :
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

### v1.0.5
*2019-02-15*

* 修复临时作用域变量自运算的BUG（多谢**avatarANDY**同学的反馈）

### v1.0.4
*2018-12-20*

* 修复脚本长度小于10字符时, 解析出错的问题

### v1.0.3
*2018-11-28*

* 修复新脚本解析格式化字符串的问题

### v1.0.2
*2018-11-27*

* 修复struct无参构造函数调用失败
* 全新的脚本解析,速度更快,增加语法更容易

### v1.0.0
*2017-11-08*

* 第一个发行版
* linux运行需要先安装 libunwind 和 icu，可以使用 yum install libunwind 和 yum install icu 安装

