# Scorpio-CSharp #
* author : while
* QQ群 : 245199668

## 脚本示例放在 ScorpioDemo/Scripts 目录
## 直接运行 ScorpioDemo/bin/Debug/ScorpioDemo.exe  左侧选中要测试的脚本 点击 Run Script 按钮 即可
## ps: 此脚本是用作Unity游戏热更新使用的脚本,纯c#实现 基于.net2.0 兼容所有c#平台 语法类似 javascript 
## Unity3d发布平台支持:
- [x] WebPlayer
- [x] PC (Windows Mac Linux)
- [x] IOS(包括IL2CPP)
- [x] Android
- [x] BlackBerry
- [x] Windows Phone 8
- [x] WebGL(Unity5.0Beta)
- [ ] Windows Store Apps

## Scorpio-Java(java版的Scorpio脚本) https://github.com/qingfeng346/Scorpio-Java
## ScorpioConversion(网络协议生成工具) https://github.com/qingfeng346/ScorpioConversion/
## 性能比较用例(C#light,ulua,Scorpio-CSharp) https://github.com/qingfeng346/ScriptTestor

## v0.0.9beta (2015-2-11) ##
-----------
* 增加调用c#函数 找不到合适函数的错误输出
* 修复[%]运算解析错误的问题
* 修复 for while循环 return 后没有跳出循环的BUG
* 支持Unity IL2CPP (IL2CPP暂不支持 Emit 如需使用 delegate 请把DefaultScriptUserdataDelegateType.cs文件的 //#define SCORPIO_IL2CPP 改为 #define SCORPIO_IL2CPP) 然后自己实现一个DelegateTypeFactory类  示例:  
```c#
    public class DelegateFactory : DelegateTypeFactory  {
        public Delegate CreateDelegate(Type type, ScriptFunction func) {  
            if (type == typeof(UIEventListener.VoidDelegate))  
                return new UIEventListener.VoidDelegate((go) => { func.call(go); });  
            return null;  
        }  
    }  
    DefaultScriptUserdataDelegateType.SetFactory(new DelegateFactory());  
```

## v0.0.8beta (2014-12-17) ##
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

## v0.0.7beta (2014-11-25) ##
-----------
* 增加声明泛型类的函数 示例： ListInt 就相当于c#的List<int>
```javascript
    List = import_type("System.Collections.Generic.List`1")  
    ListInt = generic_type(List, import_type("System.Int32"))   
```
* 大幅优化与c#交互效率 具体测试结果请参考 (https://github.com/qingfeng346/ScriptTestor)

## v0.0.6beta (2014-11-14) ##
-----------
* 适配Unity3d WebGL (Unity5.0.0b1测试通过 WebGL示例地址 http://yunpan.cn/cAVkfYdGbgFug  提取码 2df5)
* 修复Unity下Delegate动态委托出错的BUG
* 修复赋值操作（如 = += -= 等）出错报不出堆栈的问题
* 优化数字和字符串的执行效率
* 同步发布Scorpio-Java 地址:https://github.com/qingfeng346/Scorpio-Java

## v0.0.5beta (2014-11-4) ##
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

## v0.0.4beta (2014-10-27) ##
-----------
* 增加赋值操作返回值  示例: 
```javascript
    if ((a = true) == true)
    var a = 100
    if ((a += 100) == 200) { }
```
* 修复对Unity3d Windows Phone 8 版本的兼容问题  （亲测支持wp版本）

## v0.0.3beta (2014-10-18) ##
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

## v0.0.2beta (2014-10-13) ##
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