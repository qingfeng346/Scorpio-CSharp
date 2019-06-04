namespace Scorpio.Compiler {
    public enum OpcodeType : byte {
        None,       //无效类型
        Load,       //压栈
        New,        //new
        Store,      //取栈
        Compute,    //运算
        Compare,    //比较
        Jump,       //跳转
    }
    //指令类型
    public enum Opcode : byte {
        None,

        //压栈操作
        LoadBegin,
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
        CopyStackTop,           //复制栈顶的数据
        CopyStackTopIndex,      //复制栈顶的数据
        LoadEnd,


        //New操作
        NewBegin,
        NewFunction,            //load a new function
        NewArray,               //new array
        NewMap,                 //new map
        NewType,                //new class
        NewTypeParent,          //new class with parent
        NewEnd,

        //取栈操作
        StoreBegin,
        StoreLocal,             //store local value
        StoreInternal,          //store internal value
        StoreGlobal,            //store global value by index
        StoreGlobalString,      //store global value by string
        StoreValue,             //store a value by index
        StoreValueString,       //store a value by string
        StoreValueObject,       //store a value by object
        StoreEnd,

        //运算指令
        ComputeBegin,
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
        CompareBegin,
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
        JumpBegin,
        Jump,                   //跳转到执行索引
        Pop,                    //弹出栈顶的值
        PopNumber,              //弹出一定数量的栈顶的值
        FalseTo,                //栈顶如果是false则跳转
        TrueTo,                 //栈顶如果是true则跳转
        FalseLoadFalse,         //如果是false则压入一个false
        TrueLoadTrue,           //如果是true则压入一个true
        Call,                   //call a function
        CallRet,                //call a function need ret
        CallEach,               //call a function when in foreach
        CallVi,                 //调用内部函数
        CallViRet,              //调用内部函数带返回值
        RetNone,                //return
        Ret,                    //return a value
        JumpEnd,
    }
    //单条指令
    public class ScriptInstruction {
        public OpcodeType optype;   //指令类型
        public Opcode opcode;       //指令类型
        public int opvalue;         //指令值
        public int line;            //代码在多少行
        public ScriptInstruction(Opcode opcode, int opvalue, int line) {
            this.optype = OpcodeType.None;
            this.opcode = opcode;
            this.opvalue = opvalue;
            this.line = line;
            SetOpcode(opcode, opvalue);
        }
        public void SetOpcode(Opcode opcode) {
            SetOpcode(opcode, opvalue);
        }
        public void SetOpcode(Opcode opcode, int opvalue) {
            this.opcode = opcode;
            this.opvalue = opvalue;
            if (opcode > Opcode.LoadBegin && opcode < Opcode.LoadEnd) {
                this.optype = OpcodeType.Load;
            } else if (opcode > Opcode.NewBegin && opcode < Opcode.NewEnd) {
                this.optype = OpcodeType.New;
            } else if (opcode > Opcode.StoreBegin && opcode < Opcode.StoreEnd) {
                this.optype = OpcodeType.Store;
            } else if (opcode > Opcode.ComputeBegin && opcode < Opcode.ComputeEnd) {
                this.optype = OpcodeType.Compute;
            } else if (opcode > Opcode.CompareBegin && opcode < Opcode.CompareEnd) {
                this.optype = OpcodeType.Compare;
            } else if (opcode > Opcode.JumpBegin && opcode < Opcode.JumpEnd) {
                this.optype = OpcodeType.Jump;
            }
        }
    }
}
