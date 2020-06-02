### Scorpio-CSharp
---
* author : while
* QQ群 : 245199668 [加入QQ群](http://shang.qq.com/wpa/qunwpa?idkey=8ef904955c52f7b3764403ab81602b9c08b856f040d284f7e2c1d05ed3428de8)

### <font color=yellow size=5>基础介绍</font>
---
```
sco是c#实现的解释型脚本,是一种高效,轻量,可嵌入的脚本语言,语法类似javascript,类型为弱类型,通过使用基于栈的虚拟机解释字节码来运行.
```
* **脚本示例** [ExampleScripts](https://github.com/qingfeng346/Scorpio-CSharp/tree/master/ExampleScripts)
* VSCode 基础语法提示插件 https://marketplace.visualstudio.com/items?itemName=while.scorpio
* nuget地址 https://www.nuget.org/packages/Scorpio-CSharp/
* 脚本实现Space Shooter 体验地址 http://www.fengyuezhu.com/static/projects/Scorpio-CSharp/unitysample/
* 脚本实现Space Shooter 源码地址 https://github.com/qingfeng346/ScorpioUnitySample
* 码云地址 : http://git.oschina.net/qingfeng346/Scorpio-CSharp


### <font color=yellow size=5>安装 **sco** 命令行</font>
---
#### 方式一 手动安装
* 下载[Release](https://github.com/qingfeng346/Scorpio-CSharp/releases)对应版本,解压添加到环境变量

#### 方式二 自动安装
* windows 7, windows 8, MacOS, Linux 需要预先安装 [PowerShell Core](https://github.com/PowerShell/PowerShell/releases), 然后运行命令行
```powershellcore
pwsh -Command "Invoke-Expression ((New-Object System.Net.WebClient).DownloadString('https://qingfeng346.gitee.io/installsco.ps1'))"
```

* windows 10 直接运行命令行, 也可以安装 **PowerShell Core** 使用上面的方法安装
```powershellcore
powershell.exe -Command "Invoke-Expression ((New-Object System.Net.WebClient).DownloadString('https://qingfeng346.gitee.io/installsco.ps1'))"
```

### <font color=yellow size=5>兼容的 **.net** 平台</font>
---
* unity2018及以上
* .net framework 4.0 及以上
* .net standard 2.0 及以上
* .net core 2.0 及以上
* asp.net
* asp.net core
* mono
* xamarin

<font color=yellow size=5>注意事项</font>
---
<font color=red size=3>

* 脚本文本文件编码要改成 **utf8 without bom (无签名的utf8格式)**

* 使用 **importType** 函数引入一个c#类, 参数字符串请参考 **Type.GetType**, 类似 
    * TopNamespace.SubNameSpace.ContainingClass+NestedClass,MyAssembly
    * TopNamespace.Sub\+Namespace.ContainingClass+NestedClass,MyAssembly

* 脚本内所有c#实例(除了**bool,number,string,enum**等基础类型) **均为引用**, **struct** 变量也一样

* **event** 对象 **+= -=** 操作可以使用函数 **add_[event变量名] remove_[event变量名]** 代替

* c# 扩展函数, 请先调用 **importExtension("类型")** 引用

* **Unity3d** 使用 **IL2CPP** 后, 部分**Unity3D**的类或函数不能反射获取,请配置**link.xml**或者使用**快速反射功能**

* **genericMethod, genericType** 函数在**IL2CPP**下生成未声明过的类型会报错

</font>

### <font color=yellow size=5>反射调用c#运算符重载函数</font>
---
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


### <font color=yellow size=5>快速反射</font>
---
* 快速反射类生成
    * 使用命令行可以生成快速反射类文件,例子
    ```
    sco fast -dll [dll文件路径] -class [class完整名] -output [输出目录]
    ```

* 快速反射类使用
    * 例如使用快速反射的类为 **UnityEngine.GameObject** , 生成的快速反射类则为**ScorpioClass_UnityEngine_GameObject**, 然后 **c#** 调用
    ```csharp
    Scorpio.Userdata.TypeManager.SetFastReflectClass(typeof(UnityEngine.GameObject), new ScorpioClass_UnityEngine_GameObject(script))
    ```

### <font color=yellow size=5>源码目录说明</font>
---
* **Scorpio** 脚本引擎源码
* **ScorpioExec** 命令行 **sco**
* **ScorpioReflect** 快速反射机制的实现
* **ScorpioTest** **Unity3D**内使用**sco**脚本示例

### <font color=yellow size=5>Unity相关</font>
---
* 支持的Unity版本
    * **Unity2018**及以上
    * 请设置 **PlayerSetting** 内 **Api Compatibility Level** 为 **.NET Standard 2.0**

* 支持的Unity平台:
    * PC, Mac & Linux Standalone(包括IL2CPP)
    * iOS(包括IL2CPP)
    * Android(包括IL2CPP)
    * UWP(仅支持IL2CPP)
    * WebGL

* Unity导入Scorpio-CSharp:
    * 复制源码目录 Scorpio/src 到Unity项目即可


### 捐助作者
---
![](https://github.com/qingfeng346/qingfeng346.github.io/raw/master/img/wx.jpg)
![](https://github.com/qingfeng346/qingfeng346.github.io/raw/master/img/zfb.jpg)