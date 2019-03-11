### v1.0.6
*2019-03-11*

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
* 增加 **userdata.typeof** 函数,获取c#类变量的类型, 示例 :
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

