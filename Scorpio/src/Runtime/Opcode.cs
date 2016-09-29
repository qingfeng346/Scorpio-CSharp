using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Runtime
{
    //指令类型
    public enum Opcode
    {
        /// <summary> 赋值操作 </summary>
        MOV,
        /// <summary> 申请一个局部变量 </summary>
        VAR,
        /// <summary> 执行普通代码块 </summary>
        CALL_BLOCK,
        /// <summary> 执行If语句 </summary>
        CALL_IF,
        /// <summary> 执行For语句 </summary>
        CALL_FOR,
        /// <summary> 执行For语句 </summary>
        CALL_FORSIMPLE,
        /// <summary> 执行Foreach语句 </summary>
        CALL_FOREACH,
        /// <summary> 执行While语句 </summary>
        CALL_WHILE,
        /// <summary> 执行switch语句 </summary>
        CALL_SWITCH,
        /// <summary> 执行try catch语句 </summary>
        CALL_TRY,
        /// <summary> 调用一个函数 </summary>
        CALL_FUNCTION,
        /// <summary> throw </summary>
        THROW,
        /// <summary> 解析一个变量 </summary>
        RESOLVE,
        /// <summary> 返回值 </summary>
        RET,
        /// <summary> break跳出 for foreach while </summary>
        BREAK,
        /// <summary> continue跳出本次 for foreach while </summary>
        CONTINUE,
    }
}
