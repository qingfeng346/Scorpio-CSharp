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
* **脚本示例** 放在 [bin/ExampleScripts](https://github.com/qingfeng346/Scorpio-CSharp/tree/develop/ExampleScripts) 目录
* **语法测试** 注册环境变量后,直接运行命令行 [sco 文件名],可以运行一个脚本文件 
* **性能测试** (C#light,ulua,Scorpio-CSharp) https://github.com/qingfeng346/ScriptTestor

### 基础介绍
* VSCode 基础语法提示插件 https://marketplace.visualstudio.com/items?itemName=while.scorpio 或者 VSCode 直接搜索 scorpio, 快捷键 windows(alt+g) & mac (alt[option] + g) 可以运行正在编辑的脚本
    * 环境配置
        * [Release](https://github.com/qingfeng346/Scorpio-CSharp/releases) 下载相应系统的压缩包
        * 解压文件然后添加解压目录到系统变量
        * 或者解压文件,然后在解压文件运行命令 **./sco -type register** 自动添加到系统变量
* 脚本教程 http://www.fengyuezhu.com/readme/
* nuget地址 https://www.nuget.org/packages/Scorpio-CSharp/
* Scorpio-CSharp语法 体验地址 http://www.fengyuezhu.com/static/projects/Scorpio-CSharp/scriptconsole/
* 脚本实现Space Shooter 体验地址 http://www.fengyuezhu.com/static/projects/Scorpio-CSharp/unitysample/
* 脚本实现Space Shooter 源码地址 https://github.com/qingfeng346/ScorpioUnitySample
* 网络协议,Excel表数据转换工具 : https://github.com/qingfeng346/ScorpioConversion
* Sco脚本的Java实现 : https://github.com/qingfeng346/Scorpio-Java
* 码云地址 : http://git.oschina.net/qingfeng346/Scorpio-CSharp



### 项目宏定义说明:
* **SCORPIO_DYNAMIC_DELEGATE** 动态创建Delegate对象 不适用的请自行实现一个继承 DelegateTypeFactory 的类,目前亲测只有android和windows(exe)平台可用

### 源码目录说明
* Script 文件是脚本的引擎对象
* Util 文件是一些常用的函数集合
* ScriptXXX 所有Script开头的类都是脚本内保存的对象
    * ScriptObject 所有脚本类都继承自此类
    * ScriptNull 空类型 null
    * ScriptBoolean bool 类型
    * ScriptNumber 数字类型 衍生出 ScriptNumberDouble，ScriptNumberInt，ScriptNumberLong 三个类
    * ScriptString 字符串类型
    * ScriptFunction 函数类型
    * ScriptArray 数组([])类型，相当于c#的List< ScriptObject >
    * ScriptTable table类型，相当于c#的Dictionary<object,ScriptObject>
    * ScriptEnum 处理所有c#内的枚举
    * ScriptUserdata 处理所有c#内的对象，衍生出所有Userdata目录下的类
* 子文件夹
    * CodeDom 此目录下全部都是解析脚本后生成的中间代码
    * Compiler 此目录下是脚本解释器
    * Exception 脚本引擎抛出的已知异常，例如解析失败，未支持语法等
    * Function 所有的函数类型，脚本函数，扩展函数等
    * Library 脚本内使用的库的源码，例如**json**,**array**,**table**,**string**,**math**库等，方便使用，初始化脚本时请调用 LoadLibrary 函数后方可使用
    * Runtime 此目录是运行 CodeDom 目录下的所有中间代码
    * Serialize 序列化字节码使用，把文本文件解析成二进制数据以及把二进制数据反序列化成文本文件
    * Userdata   此目录是根据c#代码内object的类型分别处理的代码，例如DefaultScriptUserdataDelegate是处理Delegate类型的对象，DefaultScriptUserdataObject是处理普通的c#对象，DefaultScriptUserdataEnum是处理枚举对象等
    * Variable 脚本内对象的差异化处理，例如ScriptNumberDouble，ScriptNumberInt，ScriptNumberLong三个类都是处理number类型，但是不同类型的处理方式不同

### 注意事项 ##
> 如果要使用 **Script.LoadFile** 函数，文件编码要改成 **utf8 without bom (无签名的utf8格式)**, 否则bom三个字节会解析失败

> 使用 **import_type import_extension**  前要确认是否已经添加该类的程序集(Assembly),例如要使用 **UnityEngine.dll** 中的 **UnityEngine.GameObject** 类,要先再c#中调用 **script.PushAssembly(typeof(GameObject).GetTypeInfo().Assembly)** 压入程序集,然后**UnityEngine.dll** 中的类就都可以使用了,也就是 **每个dll文件的程序集** 都要添加一次

> 使用 **import_type** 引入内部类时使用 **+**, 例如 **import_type("[namespace].[clasname]+[internalclass]")**

> 脚本内所有c#变量(除了number,string等基础类型) **均为引用**,struct变量也一样

> c#重载 **[]** 运算符后脚本里不能直接使用[],请使用 **get_Item set_Item** 函数

> c#数组对象获取元素不能直接使用 **[]** ,请使用 **GetValue SetValue** 函数, 具体参数可以参考 **Array** 类的 **GetValue SetValue** 函数

> c#中event对象+= -=操作可以使用函数 **add_[event变量名] remove_[event变量名]** 代替

> 同类中静态函数和实例函数不要重名,否则会调用失败 例如 static void Test(object a); void Test(object a, object b); 两个函数不一定会当静态还是实例函数处理

> 同类中重载的函数相同参数不要是继承关系,否则可能调用失败,例如 void Test(object a); void Test(string a); 两个Test函数都可以传入string,但是调用时不一定会调用哪一个

> c#类的变量不能类似 **+= -= \*= /= 等赋值计算操作(只有event可以使用 += -=)** 请使用 **变量 = 变量 + XXX**

> 如果要使用c#扩展函数,请调用一次 **import_extension("实现扩展函数的类的全路径")** , 注意：这个函数一定要在调用扩展函数之前调用,否则会找不到

> IL2CPP生成后,好多Unity的类的函数反射回调用不到,遇到这种情况请自行包一层函数,自己写的c#代码不会有这种情况

> UWP平台master配置下 **generic_method** 函数会出问题,可能是因为UWP屏蔽了此函数 报错: PlatformNotSupported_NoTypeHandleForOpenTypes. For more information, visit http://go.microsoft.com/fwlink/?LinkId=623485

> UWP平台master配置下 **generic_type** 函数也会出问题

> 不能使用 **SCORPIO_DYNAMIC_DELEGATE** 的平台,要实现一个继承 **DelegateTypeFactory** 的类,然后调用 **ScriptUserdataDelegateType.SetFactory** 函数设置一下 例如:

```csharp
public class MyDelegateFactory : DelegateTypeFactory {
	public Delegate CreateDelegate(Script script, Type type, ScriptFunction func) {
		if (type == typeof(UnityAction))						//UnityAction委托类型
			return new UnityAction (() => { func.call (); });
		else if (type == typeof(Application.LogCallback)) 		//Application.LogCallback 委托类型
			return new Application.LogCallback((arg1, arg2, arg3) => { func.call(arg1, arg2, arg3); });
		else if (type == typeof(Comparison<Transform>))			//Comparison<Transform> List排序委托类型, 带返回值
			return new Comparison<Transform>((arg1, arg2) => { return Util.ToInt32(((ScriptObject)func.call(arg1, arg2)).ObjectValue); });
		//自己可能用到的委托类型请自行添加
		Debug.Log ("Delegate Type is not found : " + type + "  func : " + func);
        return null;
	}
}
///然后实现完以后 调用 ScriptUserdataDelegateType.SetFactory(new MyDelegateFactory()); 设置一下
```
> 也可以使用**ScorpioReflect**项目中的**GenerateScorpioDelegate**类可以自动生成一个 DelegateTypeFactory 类  

```csharp
var generate = new Scorpio.ScorpioReflect.GenerateScorpioDelegate();
generate.AddType(typeof(Action<bool>));     //使用AddType 添加所有需要用到的 Delegate , 模板函数 要单独添加
generate.AddType(typeof(Action<int>));
System.IO.File.WriteAllText(Application.dataPath + "/Scripts/ScorpioDelegate/" + gen.ClassName + ".cs", gen.Generate());        //调用Generate函数生成内容

/***********下面是生成文件示例***********************
using System;
using Scorpio;
using Scorpio.Userdata;
namespace ScorpioDelegate {
    public class ScorpioDelegateFactory : DelegateTypeFactory {
        public static void Initialize() {
            ScriptUserdataDelegateType.SetFactory(new ScorpioDelegateFactory());
        }
        public Delegate CreateDelegate(Script script, Type type, ScriptFunction func) {
            if (type == typeof(System.Action<System.Boolean>))
                return new System.Action<System.Boolean>((arg0) => { func.call(arg0); });
            if (type == typeof(System.Action<System.Int32>))
                return new System.Action<System.Int32>((arg0) => { func.call(arg0); });
            throw new Exception("Delegate Type is not found : " + type + "  func : " + func);
        }
    }
}
生成文件后 调用 Initialize 函数即可
*/
```

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
- [ ] 理论上可以支持所有平台

### 反射调用运算符重载函数
*	\+    op_Addition
*	\-    op_Subtraction
*	\*    op_Multiply
*	/    op_Division
*	%    op_Modulus
*	|    op_BitwiseOr
*	&    op_BitwiseAnd
*	^    op_ExclusiveOr
*	\>    op_GreaterThan
*	<    op_LessThan
*	==   op_Equality
*	!=   op_Inequality
*	[]   get_Item(获取变量)
*	[]   set_Item(设置变量)


### 源码目录说明
* **Scorpio** 脚本引擎项目,平常使用只需导入此目录即可
* **ScorpioExec** 跟lua.exe一样,命令行调用Scorpio脚本, 序列化和反序列化脚本
* **ScorpioReflect** Scorpio脚本去反射机制的实现
* **ScorpioTest** Unity内使用Scorpio脚本例子

### Unity导入Scorpio-CSharp:
* 复制文件夹 [Scorpio/src](https://github.com/qingfeng346/Scorpio-CSharp/tree/master/Scorpio/src) 到Unity项目即可

### Scorpio脚本Hello World函数 (c# console项目):
```csharp
using System;
using Scorpio;
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
    public class MyDelegateFactory : Scorpio.Userdata.DelegateTypeFactory {
        public Delegate CreateDelegate(Script script, Type type, ScriptFunction func) {
            if (type == typeof(TestDelegate1))                        //UnityAction委托类型
                return new TestDelegate1((arg1, arg2) => { func.call(arg1, arg2); });
            //自己可能用到的委托类型请自行添加
            return null;
        }
    }
    class MainClass {
        public static void Main(string[] args) {
            Scorpio.Userdata.ScriptUserdataDelegateType.SetFactory(new MyDelegateFactory());        //设置委托生成器
            Script script = new Script();                           //new一个Script对象
            script.LoadLibrary();                                   //加载所有Scorpio的库，源码在Library目录下
            script.PushAssembly(typeof(MainClass).Assembly);            //添加当前程序的程序集
            script.SetObject("CTest", script.CreateObject(new Test(300)));  //SetObject可以设置一个c#对象到脚本里
                                                                            //LoadString 解析一段字符串,LoadString传入的参数就是热更新的文本文件内容
            script.LoadString(@"
print(""hello world"")
");
            //Scorpio脚本调用c#函数
            script.LoadString(@"
MyTest = import_type('HelloWorld.Test')             //import_type 要写入类的全路径 要加上命名空间 否则找不到此类,然后赋值给 MyTest 对象
MyTest.StaticFunc()                                 //调用c#类的静态函数
var t = MyTest(200)                                 //new 一个Test对象, 括号里面是构造函数的参数
t.Func()                                            //调用c#的内部函数
CTest.Func()                                        //调用c#的内部函数 CTest是通过 script.SetObject 函数设置
MyTest.dele = function(a, b) {                      //设置委托类型
    print(a + '   ' + b)
}
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

## 版本日志
[版本日志](https://github.com/qingfeng346/Scorpio-CSharp/blob/master/ReleaseNotes.md)


## 更新日志 (**v1.0.0**版本以前)
[更新日志](https://github.com/qingfeng346/Scorpio-CSharp/blob/master/ChangeLog.md)

## 捐助作者
### 如果此项目对你有所帮助,可以请作者喝杯咖啡

![](https://github.com/qingfeng346/qingfeng346.github.io/raw/master/img/wx.jpg)
![](https://github.com/qingfeng346/qingfeng346.github.io/raw/master/img/zfb.jpg)