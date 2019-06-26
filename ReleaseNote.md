## v2.0.0_preview9

* 支持 **long** 和 **double** 类型之间直接运算比较
* **Array** 原表增加函数 **join** 
* **Map** 原表增加函数 **forEach forEachValue**
* **Array Map** **forEach** 函数, 返回 **false** 则停止循环
* **math** 库增加三角函数 **sin sinh asin cos cosh acos tan tanh atan**
* **lambada**表达式申请的funstion, this继承父级 [脚本示例](https://github.com/qingfeng346/Scorpio-CSharp/blob/master/ExampleScripts/%E5%87%BD%E6%95%B0.sco)
* 快速反射 支持 **op_Implicit** 函数
* 快速反射 修复构造函数 有 **ref out** 时 生成出错
* 修改 **class** 定义方式，支持动态定义 **class** [脚本示例](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/class%E8%AF%AD%E6%B3%95.sco)


## v2.0.0_preview8

### 新增功能
* 增加原表操作, 基础原表目录 Scorpio/src/Proto [脚本示例](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/class%E8%AF%AD%E6%B3%95.sco)
* 原表支持运算符重载 基础类型(number,string,bool,null)除外 [脚本示例](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/%E8%BF%90%E7%AE%97%E7%AC%A6%E9%87%8D%E8%BD%BD.sco)
* 支持调用含有 ref out 标识的函数 [脚本示例](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/%E8%B0%83%E7%94%A8ref.out%E5%87%BD%E6%95%B0.sco)
* 快速反射支持创建模板函数,支持 ref out 标识的函数
* c#数组支持直接 [] 获取元素 [脚本示例](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/%E4%BD%BF%E7%94%A8c%23%E7%B1%BB.sco)
* c#重载 **[]中括号(除去key为string的重载)**, 支持 [] 直接调用 [脚本示例](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/%E4%BD%BF%E7%94%A8c%23%E7%B1%BB.sco)
* c#元素支持 += -= 等 运算符赋值操作
* 支持生成脚本 IL 文件，运行时可以省去解析编译的过程
* 添加 ScriptNamespace 类， 可以使用 importNamespace 导入， 引用c#的命名空间
* 增加 IO 库
* 可以直接赋值c#的delegate, 不用再生成 DelegateFactory 类, (测试 UWP 不可用, Android IOS 正常)
* 命令行增加生成快速反射文件和IL文件命令, 可以使用 sco -help 查看
* 优化脚本性能, 修改为栈编译运行,部分运行测试,左侧为2.0版本,测试结果,基础运算大幅提升,其他操作也有较大的提升
    ![](https://raw.githubusercontent.com/qingfeng346/Scorpio-CSharp/v2.0/%E6%80%A7%E8%83%BD%E6%B5%8B%E8%AF%95/1.png)
    ![](https://raw.githubusercontent.com/qingfeng346/Scorpio-CSharp/v2.0/%E6%80%A7%E8%83%BD%E6%B5%8B%E8%AF%95/2.png)
    ![](https://raw.githubusercontent.com/qingfeng346/Scorpio-CSharp/v2.0/%E6%80%A7%E8%83%BD%E6%B5%8B%E8%AF%95/3.png)
    ![](https://raw.githubusercontent.com/qingfeng346/Scorpio-CSharp/v2.0/%E6%80%A7%E8%83%BD%E6%B5%8B%E8%AF%95/4.png)

### 修改功能
* 所有库函数名重写,命名规则为骆驼式命名法, 第一个单词以小写字母开始,从第二个单词开始以后的每个单词的首字母都采用大写字母, 不过可以调用Script.LoadLibraryV1, 兼容 1.0 命名规则库
* function内的this变量规则为前置变量,规则参考javascript [脚本示例](https://github.com/qingfeng346/Scorpio-CSharp/blob/v2.0/ExampleScripts/%E5%87%BD%E6%95%B0.sco)
* 设置快速反射类的函数修改为 Scorpio.Userdata.TypeManager.SetFastReflectClass

### 删除功能
* 删除 kpairs vpairs 函数
* 删除 try catch
* 删除 switch 语法
* 删除引用扩展函数,请使用静态函数方式调用