# Scorpio-CSharp #
* author : while
* QQ群 : 245199668 [加入QQ群](http://shang.qq.com/wpa/qunwpa?idkey=8ef904955c52f7b3764403ab81602b9c08b856f040d284f7e2c1d05ed3428de8)


### 此脚本为纯c#实现的脚本系统,最低支持 .net framework 3.5和 .net standard 2.0, 语法类似 javascript
* 兼容各个平台
    * .net framework 3.5 以上
    * .net standard 2.0 以上
    * .net core
    * unity
    * asp.net
    * asp.net core
    * mono
    * xamarin
* **脚本示例** 放在 [ExampleScripts](https://github.com/qingfeng346/Scorpio-CSharp/tree/v2.0/ExampleScripts) 目录
* **脚本教程** http://www.fengyuezhu.com/readme/
* **语法测试** 注册环境变量后,直接运行命令行 [sco 文件名],可以运行一个脚本文件, 语法测试地址
    * http://www.fengyuezhu.com/static/projects/Scorpio-CSharp/scriptconsole/


### 基础介绍
* VSCode 基础语法提示插件 https://marketplace.visualstudio.com/items?itemName=while.scorpio 或者 VSCode 直接搜索 scorpio, 快捷键 windows(alt+g) & mac (alt[option] + g) 可以运行正在编辑的脚本
    * 环境配置
        * [Release](https://github.com/qingfeng346/Scorpio-CSharp/releases) 下载相应系统的压缩包
        * 解压文件然后添加解压目录到系统变量
        * 或者解压文件,然后在解压文件运行命令 **./sco -type register** 自动添加到系统变量
* nuget地址 https://www.nuget.org/packages/Scorpio-CSharp/
* 脚本实现Space Shooter 体验地址 http://www.fengyuezhu.com/static/projects/Scorpio-CSharp/unitysample/
* 脚本实现Space Shooter 源码地址 https://github.com/qingfeng346/ScorpioUnitySample
* 码云地址 : http://git.oschina.net/qingfeng346/Scorpio-CSharp


### 注意事项 ##
> 如果要使用 **Script.LoadFile** 函数，文件编码要改成 **utf8 without bom (无签名的utf8格式)**, 否则bom三个字节会解析失败

> 使用 **import_type import_extension**  前要确认是否已经添加该类的程序集(Assembly),例如要使用 **UnityEngine.dll** 中的 **UnityEngine.GameObject** 类,要先再c#中调用 **script.PushAssembly(typeof(GameObject).GetTypeInfo().Assembly)** 压入程序集,然后**UnityEngine.dll** 中的类就都可以使用了,也就是 **每个dll文件的程序集** 都要添加一次

> 使用 **import_type** 引入内部类时使用 **+**, 例如 **import_type("[namespace].[clasname]+[internalclass]")**

> 脚本内所有c#变量(除了**number,string**等基础类型) **均为引用**,struct变量也一样

> **c# event** 对象+= -=操作可以使用函数 **add_[event变量名] remove_[event变量名]** 代替

> 如果要使用c#扩展函数,请调用一次 **import_extension("实现扩展函数的类的全路径")** , 注意：这个函数一定要在调用扩展函数之前调用,否则会找不到

> IL2CPP生成后,好多Unity的类的函数反射回调用不到,遇到这种情况请自行包一层函数,自己写的c#代码不会有这种情况

> UWP平台master配置下 **generic_method, generic_type** 函数会出问题,其他平台均无问题 **(android&il2cpp ios&il2cpp webgl等)**, 报错: PlatformNotSupported_NoTypeHandleForOpenTypes. For more information, visit http://go.microsoft.com/fwlink/?LinkId=623485


### 使用去反射功能注意事项
* 不能调用模板函数
* 不能调用含有ref和out参数的函数

### Unity3d发布平台支持(亲测):
- [x] PC, Mac & Linux Standalone(包括IL2CPP)
- [x] iOS(包括IL2CPP)
- [x] Android(包括IL2CPP)
- [x] Universal Windows Platform(包括IL2CPP)
- [x] WebGL
- [x] Tizen
- [x] 理论上可以支持所有平台

### 反射调用c#运算符重载函数

运算符号 | 反射名称                       | 脚本是否支持直接调用
-----   |  ----                         | ----
\+      |  op_Addition                  | 支持
\-      |  op_Subtraction               | 支持
\*      |  op_Multiply                  | 支持
/       |  op_Division                  | 支持
%       |  op_Modulus                   | 支持
\|      |  op_BitwiseOr                 | 支持
&       |  op_BitwiseAnd                | 支持
^       |  op_ExclusiveOr               | 支持
\>      |  op_GreaterThan               | 支持
\>=     |  op_GreaterThanOrEqual        | 支持
<       |  op_LessThan                  | 支持
<=      |  op_LessThanOrEqual           | 支持
==      |  op_Equality                  | 支持
!=      |  op_Inequality                | 不支持, 脚本 != 会直接取反 ==
[]      |  get_Item(获取变量)            | 支持 **key** 不为**string**的情况
[]      |  set_Item(设置变量)            | 支持 **key** 不为**string**的情况


### 源码目录说明
* **Scorpio** 脚本引擎源码
* **ScorpioExec** 生成命令行**sco**,命令行调用脚本,序列化和反序列化脚本
* **ScorpioReflect** 去反射机制的实现
* **ScorpioTest** Unity内使用Scorpio脚本示例

### Unity导入Scorpio-CSharp:
* 复制文件夹 [Scorpio/src](https://github.com/qingfeng346/Scorpio-CSharp/tree/v2.0/Scorpio/src) 到Unity项目即可

### Scorpio脚本Hello World函数 (c# console项目):
```csharp
using System;
using Scorpio;
using Scorpio.Userdata;
namespace HelloWorld {
    public delegate void TestDelegate1(int a, int b);
    public class Test {
        public static TestDelegate1 dele;
        private int a = 100;
        public Test(int a) {
            this.a = a;
        }
        public void Func() {
            Console.WriteLine("Func " + a);
        }
        public static void StaticFunc() {
            Console.WriteLine("StaticFunc");
        }
        public static void Call() {
            if (dele != null) dele(100, 200);
        }
    }
    public enum TestEnum {
        Test1,
        Test2,
        Test3,
    }
    class MainClass {
        public static void Main(string[] args) {
            TypeManager.PushAssembly(typeof(MainClass).Assembly);            //添加当前程序的程序集
            Script script = new Script();                           //new一个Script对象
            script.LoadLibrary();                                   //加载所有Scorpio的库，源码在Library目录下
            script.SetGlobal("CTest", ScriptValue.CreateObject(new Test(300)));  //SetObject可以设置一个c#对象到脚本里
                                                                            //LoadString 解析一段字符串,LoadString传入的参数就是热更新的文本文件内容
            script.LoadString(@"
print(""hello world"")
");
            //Scorpio脚本调用c#函数
            script.LoadString(@"
MyTest = import_type('HelloWorld.Test')                     //import_type 要写入类的全路径 要加上命名空间 否则找不到此类,然后赋值给 MyTest 对象
TestDelegate1 = import_type('HelloWorld.TestDelegate1')     //委托类型
MyTest.StaticFunc()                                         //调用c#类的静态函数
var t = MyTest(200)                                         //new 一个Test对象, 括号里面是构造函数的参数
t.Func()                                                    //调用c#的内部函数
CTest.Func()                                                //调用c#的内部函数 CTest是通过 script.SetObject 函数设置
MyTest.dele = function(a, b) {                              //设置委托类型, c# 委托类型可以直接传入 脚本 function
    print('script function : ' + a + '   ' + b)
}
var func = TestDelegate1(function(a, b) {                  //也可以直接申请一个委托类型
    print('delegate : ' + a + '   ' + b)
})
MyTest.dele += func                                         //只有使用 委托构造函数申请的 function 才可以 -=
MyTest.dele(1111,2222)
MyTest.dele -= func
MyTest.dele(11111,22222)

MyTest.Call()
TestEnum = import_type('HelloWorld.TestEnum')       //引入枚举
print(TestEnum.Test1)                               //直接使用枚举
");
            Console.ReadKey();
        }
    }
}
```

### c#去反射类使用
* 把**ScorpioReflect**项目中的**GenerateScorpioClass**文件夹下的所有cs文件复制到项目工程,放到**Editor**目录即可,此类只用作生成中间代码,后期不会使用,使用示例: 

```csharp
//就拿UnityEngine.GameObject类为例
//先生成中间代码
//使用GenerateScorpioClass生成代码,ScorpioClassName变量是中间类名称,Generate()返回生成后的代码
//下面两句代码是生成UnityEngine.GameObject的中间类到工程目录
var gen = new GenerateScorpioClass(typeof(UnityEngine.GameObject));
System.IO.File.WriteAllText(Application.dataPath + "/Scripts/ScorpioClass/" + gen.ScorpioClassName + ".cs", gen.Generate());


//然后调用Script类的PushFastReflectClass函数把typeof(UnityEngine.GameObject)和生成后的类ScorpioClass_UnityEngine_GameObject关联上
Script script = new Script();
script.LoadLibrary();
script.PushFastReflectClass(typeof(UnityEngine.GameObject), new ScorpioClass_UnityEngine_GameObject(script));
```

## 捐助作者
### 如果此项目对你有所帮助,可以请作者喝杯咖啡

![](https://github.com/qingfeng346/qingfeng346.github.io/raw/master/img/wx.jpg)
![](https://github.com/qingfeng346/qingfeng346.github.io/raw/master/img/zfb.jpg)