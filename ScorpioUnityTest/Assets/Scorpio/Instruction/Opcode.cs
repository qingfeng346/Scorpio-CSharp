namespace Scorpio.Instruction {
    //指令集大类型
    public enum OpcodeType : byte {
        None,       //无效类型
        Load,       //压栈
        New,        //new
        Store,      //取栈
        Compute,    //运算
        Compare,    //比较
        Jump,       //跳转
    }
    //指令类型, 枚举使用byte 类型 switch 会比int 稍快一丢丢
    public enum Opcode : byte {
        None,

        //压栈操作
        LoadBegin = 1,
        LoadConstNull,          //push null
        LoadConstDouble,        //push double
        LoadConstLong,          //push long
        LoadConstString,        //push string
        LoadConstFalse,         //push false
        LoadConstTrue,          //push true
        LoadLocal,              //push a local value by index
        LoadInternal,           //push a internal value
        LoadGlobal,             //push a global value by index
        LoadGlobalString,       //push a global value by string
        LoadFunction,           //push a function
        LoadValue,              //push a value by index
        LoadValueString,        //push a value by string
        LoadValueObject,        //push a value by object
        LoadValueObjectDup,     //push a value by object
        LoadBase,               //push base value
        CopyStackTop,           //复制栈顶的数据
        CopyStackTopIndex,      //复制栈顶的数据
        LoadEnd,


        //New操作
        NewBegin = LoadBegin + 40,
        NewFunction,            //new function
        NewLambdaFunction,      //new lambda function
        NewArray,               //new array
        NewMap,                 //new map
        NewMapObject,           //new map with key contain object
        NewType,                //new class
        NewTypeParent,          //new class with parent, 已弃用deprecated,兼容旧版本,暂时不能删除
        NewMapString,           //new map only string key
        NewAsyncFunction,       //new async function
        NewAsyncLambdaFunction, //new async lambda function
        NewAsyncType,           //new async type
        NewEnd,

        //取栈操作
        StoreBegin = NewBegin + 20,
        StoreLocalAssign,       //store local value and assign
        StoreInternalAssign,    //store internal value and assign
        StoreValueStringAssign, //store a value by string and assign
        StoreValueObjectAssign, //store a value by object and assign
        StoreGlobalAssign,      //store global value by index and assign
        StoreGlobalStringAssign,//store global value by string and assign
        StoreValueAssign,       //store a value by index and assign

        StoreLocal,             //store local value
        StoreInternal,          //store internal value
        StoreGlobal,            //store global value by index
        StoreGlobalString,      //store global value by string
        StoreValueString,       //store a value by string
        StoreValueObject,       //store a value by object
        StoreEnd,

        //运算指令
        ComputeBegin = StoreBegin + 30,
        Plus,
        Minus,
        Multiply,
        Divide,
        Modulo,
        InclusiveOr,
        Combine,
        XOR,
        Shr,
        Shi,
        FlagNot,                //取反操作
        FlagMinus,              //取负操作
        FlagNegative,           //取非操作
        ComputeEnd,

        //比较指令
        CompareBegin = ComputeBegin + 20,
        Greater,
        GreaterOrEqual,
        Less,
        LessOrEqual,
        Equal,
        NotEqual,
        And,
        Or,
        CompareEnd,

        //跳转指令
        JumpBegin = CompareBegin + 20,
        Jump,                   //跳转到执行索引
        Pop,                    //弹出栈顶的值
        PopNumber,              //弹出一定数量的栈顶的值
        FalseTo,                //栈顶如果是false则跳转
        TrueTo,                 //栈顶如果是true则跳转
        FalseLoadFalse,         //如果是false则压入一个false,并跳转
        TrueLoadTrue,           //如果是true则压入一个true,并跳转
        CallEmpty,              //调用内部函数,没有参数
        Call,                   //调用一个函数
        CallVi,                 //调用内部函数
        CallUnfold,             //调用一个函数 有参数需要展开
        CallViUnfold,           //调用内部函数 有参数需要展开
        CallBase,               //调用父级函数
        CallBaseUnfold,         //调用父级函数 有参数需要展开
        RetNone,                //return
        Ret,                    //return a value
        NotNullTo,              //如果栈顶不为null则跳转
        NullTo,                 //栈顶如果是null则跳转并且不取出栈顶
        TryTo,                  //异常跳转
        TryEnd,                 //try结束
        Throw,                  //throw
        Await,                  //await
        JumpEnd,
    }
}
