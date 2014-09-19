using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Runtime
{
    //指令类型
    public enum Opcode
    {
        /// <summary> 复制操作 </summary>
        MOV,
        /// <summary> 申请一个局部变量 </summary>
        VAR,
        /// <summary> 执行普通代码块 </summary>
        CALL_BLOCK,
        /// <summary> 执行If语句 </summary>
        CALL_IF,
        /// <summary> 执行For语句 </summary>
        CALL_FOR,
        /// <summary> 执行Foreach语句 </summary>
        CALL_FOREACH,
        /// <summary> 执行While语句 </summary>
        CALL_WHILE,
        /// <summary> 调用一个函数 </summary>
        CALL_FUNCTION,
        /// <summary> 递增递减变量 ++或-- </summary>
        CALC,
        /// <summary> 返回值 </summary>
        RET,
        /// <summary> break跳出 for foreach while </summary>
        BREAK,
        /// <summary> continue跳出本次 for foreach while </summary>
        CONTINUE,
    }
}
