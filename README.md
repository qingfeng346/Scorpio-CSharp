# Scorpio-CSharp #
* author : while
* QQ群 ：245199668

## 脚本示例放在  ScorpioDemo/Scripts 目录
## 直接运行 ScorpioDemo/bin/Debug/ScorpioDemo.exe  左侧选中要测试的脚本 点击 Run Script 按钮 即可
## ps: 此脚本是用作Unity游戏热更新使用的脚本,纯c#实现 基于.net2.0  兼容所有c#平台 (现Scorpio-Java已发布 https://github.com/qingfeng346/Scorpio-Java)
## 性能比较用例（C#light,ulua,Scorpio-CSharp比较）（https://github.com/qingfeng346/ScriptTestor）

## v0.0.7beta (2014-11-25) ##
-----------
* 增加声明泛型类的函数 示例： ListInt 就相当于c#的List<int>
    * List = import_type("System.Collections.Generic.List`1")  
      ListInt = generic_type(List, import_type("System.Int32"))   
* 大幅优化与c#交互效率 具体测试结果请参考 （https://github.com/qingfeng346/ScriptTestor）

## v0.0.6beta (2014-11-14) ##
-----------
* 适配Unity3d WebGL (Unity5.0.0b1测试通过 WebGL示例地址 http://yunpan.cn/cAVkfYdGbgFug  提取码 2df5)
* 修复Unity下Delegate动态委托出错的BUG
* 修复赋值操作（如 = += -= 等）出错报不出堆栈的问题
* 优化数字和字符串的执行效率
* 同步发布Scorpio-Java 地址:https://github.com/qingfeng346/Scorpio-Java


## v0.0.5beta (2014-11-4) ##
-----------
* 增加table声明语法  支持 Key 用 数字和字符串声明 示例：var a = { 1 = 1, "a" = a, b = b}
* 增加elseif语法 现支持三种 elseif,elif,else if 都可以当作 else if 语法
* 修改 不同类型之间 做 ==  != 比较报错的问题  改成  不同类型之间==比较 直接返回false
* 增加switch语句 只支持 number和string 并且 case 只支持常量 不支持变量
* 支持try catch 语法 示例： try { trhow "error" } catch (e) { print(e) }
* 支持脚本调用 c# 变长参数(params) 的函数
* 增加 switch trycatch import_type 示例


## v0.0.4beta (2014-10-27) ##
-----------
* 增加赋值操作返回值  例如: if ((a = true) == true)        a = 100   if ((a += 100) == 200)
* 修复对Unity3d Windows Phone 8 版本的兼容问题  （亲测支持wp版本）

## v0.0.3beta (2014-10-18) ##
-----------
* 增加对Delegate动态委托的支持  
        示例：  
    * c# :  
        namespace Scropio {  
            public class Hello {  
                public delegate void Test(int a, int b);  
                public static Test t;  
            }  
        }  
    * script:  
        function test(a,b) {   
            print(a)  
            print(b)  
        }  
        Hello.t = Hello.Test(test)  
        Hello.t(100,200)
        
## v0.0.2beta (2014-10-13) ##
-----------
* 修复已知BUG
* 增加对不定参的支持  
    示例：  
        function hello(a,...args) { }    
    args会传入一个Array
* 增加 eval函数 可以动态执行一段代码
* 删除对ulong类型的支持
* 增加基础for循环 for(i=begin,finished(包含此值),step)  
    示例：  
        for (i=0,10000)  
        {  
        }  
        for (i=0,10000,2)  
        {  
        }
* 统一scriptobject产出函数 方便以后加入对象池
* 增加Unity例子 (亲测支持pc web android ios wp(需要修改一些基础函数调用,应该不影响功能使用,稍后会发布一个版本支持wp))