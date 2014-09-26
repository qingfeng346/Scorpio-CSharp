using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.Runtime;
using Scorpio.Compiler;
using Scorpio.CodeDom;
using Scorpio.CodeDom.Temp;
using Scorpio.Exception;
using Scorpio.Collections;
namespace Scorpio.Runtime
{
    //执行命令
    public class ScriptContext
    {
        private Script m_script;                                                        //脚本类
        private ScriptContext m_parent;                                                 //父级执行命令
        private ScriptExecutable m_scriptExecutable;                                    //执行命令堆栈
        private ScriptInstruction m_scriptInstruction;                                  //当前执行
        private VariableDictionary m_variableDictionary = new VariableDictionary();     //当前作用域所有变量
        private ScriptObject m_returnObject = null;                                     //返回值
        private Executable_Block m_block;                                               //堆栈类型
        private bool m_Break = false;                                                   //break跳出
        private bool m_Over = false;                                                    //函数是否已经结束
        private int m_InstructionCount = 0;                                             //指令数量
        public ScriptContext(Script script, ScriptExecutable scriptExecutable) : this(script, scriptExecutable, null, Executable_Block.None) { }
        public ScriptContext(Script script, ScriptExecutable scriptExecutable, ScriptContext parent, Executable_Block block)
        {
            m_script = script;
            m_parent = parent;
            m_scriptExecutable = scriptExecutable;
            m_variableDictionary.Clear();
            m_block = block;
            m_InstructionCount = m_scriptExecutable != null ? m_scriptExecutable.Count : 0;
        }
        private bool IsBreak { get { return m_Break; } }                 //是否已经Break
        private bool IsOver { get { return m_Break || m_Over; } }        //此逻辑块是否已经执行完成
        public void Initialize(ScriptContext parent, VariableDictionary variable)
        {
            m_parent = parent;
            m_variableDictionary = variable;
        }
        private void Initialize(string name, ScriptObject obj)
        {
            m_variableDictionary.Clear();
            m_variableDictionary.Add(name, obj);
        }
        private void Initialize(ScriptContext parent)
        {
            m_parent = parent;
            m_variableDictionary.Clear();
        }
        private void ApplyVariableObject(string name)
        {
            if (!m_variableDictionary.ContainsKey(name))
                m_variableDictionary.Add(name, ScriptNull.Instance);
        }
        private ScriptObject GetVariableObject(string name)
        {
            if (m_variableDictionary.ContainsKey(name))
                return m_variableDictionary[name];
            if (m_parent != null)
                return m_parent.GetVariableObject(name);
            return null;
        }
        private bool SetVariableObject(string name, ScriptObject obj)
        {
            if (Util.SetObject(m_variableDictionary, name, obj)) {
                return true;
            }
            if (m_parent != null) {
                return m_parent.SetVariableObject(name, obj);
            }
            return false;
        }
        private bool ContainsVariable(string name)
        {
            if (m_variableDictionary.ContainsKey(name))
                return true;
            if (m_parent != null)
                return m_parent.ContainsVariable(name);
            return false;
        }
        private ScriptObject GetVariable(CodeMember member)
        {
            ScriptObject ret = null;
            if (member.Parent == null) {
                string name = member.MemberString;
                ScriptObject obj = GetVariableObject(name);
                ret = (obj == null ? m_script.GetValue(name) : obj);
            } else {
                ScriptObject parent = ResolveOperand(member.Parent);
                if (parent == null) throw new ExecutionException("GetVariable parent is null");
                if (parent is ScriptArray) {
                    if (member.Type == MEMBER_TYPE.NUMBER) {
                        ScriptArray array = (ScriptArray)parent;
                        ret = array.GetValue(member.MemberNumber);
                    } else if (member.Type == MEMBER_TYPE.OBJECT) {
                        ScriptNumber mem = ResolveOperand(member.Member) as ScriptNumber;
                        if (mem == null) throw new ExecutionException("GetVariable Array Element is must a number");
                        ScriptArray array = (ScriptArray)parent;
                        ret = array.GetValue(mem.ToInt32());
                    } else {
                        throw new ExecutionException("GetVariable Array Element is must a number");
                    }
                } else if (parent is ScriptTable) {
                    if (member.Type == MEMBER_TYPE.NUMBER) {
                        ScriptTable table = (ScriptTable)parent;
                        return table.GetValue(member.MemberNumberObject);
                    } else if (member.Type == MEMBER_TYPE.STRING) {
                        ScriptTable table = (ScriptTable)parent;
                        return table.GetValue(member.MemberString);
                    } else if (member.Type == MEMBER_TYPE.OBJECT) {
                        ScriptObject mem = ResolveOperand(member.Member);
                        if (!(mem is ScriptString || mem is ScriptNumber)) throw new ExecutionException("GetVariable Table Element is must a string or number");
                        ScriptTable table = (ScriptTable)parent;
                        ret = table.GetValue(mem.ObjectValue);
                    }
                } else if (parent is ScriptUserdata) {
                    if (member.Type == MEMBER_TYPE.STRING) {
                        ScriptUserdata data = (ScriptUserdata)parent;
                        return data.GetValue(member.MemberString);
                    } else if (member.Type == MEMBER_TYPE.OBJECT) {
                        ScriptString mem = ResolveOperand(member.Member) as ScriptString;
                        if (mem == null) throw new ExecutionException("GetVariable Table Element is must a string");
                        ScriptUserdata data = (ScriptUserdata)parent;
                        ret = data.GetValue(mem.Value);
                    } else {
                        throw new ExecutionException("GetVariable Table Element is must a string");
                    }
                } else {
                    throw new ExecutionException("GetVariable member parent is not table or array or userdata");
                }
            }
            if (ret == null) throw new ExecutionException("GetVariable member is error");
            if (member.Calc != CALC.NONE) {
                ScriptNumber num = ret as ScriptNumber;
                if (num == null) throw new ExecutionException("++或者--只能应用于Number类型");
                return num.Calc(member.Calc);
            }
            return ret;
        }
        private void SetVariable(CodeMember member, CodeObject obj)
        {
            if (member.Parent == null) {
                string name = member.MemberString;
                ScriptObject variable = ResolveOperand(obj);
                if (!SetVariableObject(name, variable))
                    m_script.SetObjectInternal(name, variable);
            } else {
                ScriptObject parent = ResolveOperand(member.Parent);
                if (parent == null) throw new ExecutionException("SetVariable parent is null");
                if (parent is ScriptArray) {
                    if (member.Type == MEMBER_TYPE.NUMBER) {
                        ScriptArray array = (ScriptArray)parent;
                        array.SetValue(member.MemberNumber, ResolveOperand(obj));
                    } else if (member.Type == MEMBER_TYPE.OBJECT) {
                        ScriptNumber mem = ResolveOperand(member.Member) as ScriptNumber;
                        if (mem == null) throw new ExecutionException("SetVariable Array Element is must a number");
                        ScriptArray array = (ScriptArray)parent;
                        array.SetValue(mem.ToInt32(), ResolveOperand(obj));
                    } else {
                        throw new ExecutionException("SetVariable Array Element is must a number");
                    }
                } else if (parent is ScriptTable) {
                    if (member.Type == MEMBER_TYPE.NUMBER) {
                        ScriptTable table = (ScriptTable)parent;
                        table.SetValue(member.MemberNumberObject, ResolveOperand(obj));
                    } else if (member.Type == MEMBER_TYPE.STRING) {
                        ScriptTable table = (ScriptTable)parent;
                        table.SetValue(member.MemberString, ResolveOperand(obj));
                    } else if (member.Type == MEMBER_TYPE.OBJECT) {
                        ScriptString mem = ResolveOperand(member.Member) as ScriptString;
                        if (mem == null) throw new ExecutionException("GetVariable Table Element is must a string or number");
                        ScriptTable table = (ScriptTable)parent;
                        table.SetValue(mem.Value, ResolveOperand(obj));
                    }
                } else if (parent is ScriptUserdata) {
                    if (member.Type == MEMBER_TYPE.STRING) {
                        ScriptUserdata data = (ScriptUserdata)parent;
                        data.SetValue(member.MemberString, ResolveOperand(obj));
                    } else if (member.Type == MEMBER_TYPE.OBJECT) {
                        ScriptString mem = ResolveOperand(member.Member) as ScriptString;
                        if (mem == null) throw new ExecutionException("GetVariable Table Element is must a string");
                        ScriptUserdata data = (ScriptUserdata)parent;
                        data.SetValue(mem.Value, ResolveOperand(obj));
                    } else {
                        throw new ExecutionException("GetVariable Table Element is must a string");
                    }
                } else {
                    throw new ExecutionException("SetVariable member parent is not table or array");
                }
            }
        }
        private void Reset()
        {
            m_returnObject = null;
            m_Over = false;
            m_Break = false;
        }
        public ScriptObject Execute()
        {
            Reset();
            int iInstruction = 0;
            while (iInstruction < m_InstructionCount)
            {
                m_scriptInstruction = m_scriptExecutable[iInstruction++];
                ExecuteInstruction();
                if (IsOver) break;
            }
            return m_returnObject;
        }
        private ScriptObject Execute(ScriptExecutable executable)
        {
            if (executable == null) return null;
            Reset();
            int iInstruction = 0;
            int iInstructionCount = executable.Count;
            while (iInstruction < iInstructionCount)
            {
                m_scriptInstruction = executable[iInstruction++];
                ExecuteInstruction();
                if (IsOver) break;
            }
            return m_returnObject;
        }
        private void ExecuteInstruction()
        {
            switch (m_scriptInstruction.Opcode)
            {
                case Opcode.VAR: ProcessVar(); break;
                case Opcode.MOV: ProcessMov(); break;
                case Opcode.RET: ProcessRet(); break;
                case Opcode.CALC: ProcessCalc(); break;
                case Opcode.CONTINUE: ProcessContinue(); break;
                case Opcode.BREAK: ProcessBreak(); break;
                case Opcode.CALL_BLOCK: ProcessCallBlock(); break;
                case Opcode.CALL_FUNCTION: ProcessCallFunction(); break;
                case Opcode.CALL_IF: ProcessCallIf(); break;
                case Opcode.CALL_FOR: ProcessCallFor(); break;
                case Opcode.CALL_FOREACH: ProcessCallForeach(); break;
                case Opcode.CALL_WHILE: ProcessCallWhile(); break;
            }
        }
        private bool SupportReturnValue()
        {
            return m_block == Executable_Block.Function || m_block == Executable_Block.Context;
        }
        private bool SupportContinue()
        {
            return m_block == Executable_Block.For || m_block == Executable_Block.Foreach || m_block == Executable_Block.While;
        }
        private bool SupportBreak()
        {
            return m_block == Executable_Block.For || m_block == Executable_Block.Foreach || m_block == Executable_Block.While;
        }
        void ProcessVar()
        {
            ApplyVariableObject((string)m_scriptInstruction.Value);
        }
        void ProcessMov()
        {
            SetVariable((CodeMember)m_scriptInstruction.Operand0, m_scriptInstruction.Operand1);
        }
        void ProcessContinue()
        {
            InvokeContinue(m_scriptInstruction.Operand0);
        }
        void ProcessBreak()
        {
            InvokeBreak(m_scriptInstruction.Operand0);
        }
        void ProcessCallFor()
        {
            CodeFor code = (CodeFor)m_scriptInstruction.Operand0;
            ScriptContext context = code.Context;
            ScriptContext blockContext = code.BlockContext;
            context.Initialize(this);
            context.Execute(code.BeginExecutable);
            ScriptBoolean Condition;
            for ( ; ; )
            {
                if (code.Condition != null) {
                    Condition = context.ResolveOperand(code.Condition) as ScriptBoolean;
                    if (Condition == null) throw new ExecutionException("for 跳出条件必须是一个bool型");
                    if (!Condition.Value) break;
                }
                blockContext.Initialize(context);
                blockContext.Execute();
                if (blockContext.IsBreak) break;
                context.Execute(code.LoopExecutable);
            }
        }
        void ProcessCallForeach()
        {
            CodeForeach code = (CodeForeach)m_scriptInstruction.Operand0;
            ScriptObject loop = ResolveOperand(code.LoopObject);
            if (!loop.IsFunction) throw new ExecutionException("foreach函数必须返回一个ScriptFunction");
            ScriptContext context = new ScriptContext(m_script, code.Executable, this, Executable_Block.Foreach);
            for ( ; ; )
            {
                ScriptObject obj = ((ScriptFunction)loop).Call();
                if (obj == null || obj.IsNull) return;
                context.Initialize(code.Identifier, obj);
                context.Execute();
                if (context.IsBreak) break;
            }
        }
        void ProcessCallIf()
        {
            CodeIf code = (CodeIf)m_scriptInstruction.Operand0;
            if (ProcessCondition(code.If, Executable_Block.If))
                return;
            int length = code.ElseIf.Count;
            for (int i = 0; i < length; ++i) {
                if (ProcessCondition(code.ElseIf[i], Executable_Block.If))
                    return;
            }
            ProcessCondition(code.Else, Executable_Block.If);
        }
        bool ProcessCondition(TempCondition con, Executable_Block block)
        {
            if (con == null) return false;
            if (con.Allow != null)
            {
                ScriptBoolean b = ResolveOperand(con.Allow) as ScriptBoolean;
                if (b == null) throw new ExecutionException("if 条件必须是一个bool型");
                if (b.Value == false) return false;
            }
            con.Context.Initialize(this);
            con.Context.Execute();
            return true;
        }
        void ProcessCallWhile()
        {
            CodeWhile code = (CodeWhile)m_scriptInstruction.Operand0;
            TempCondition condition = code.While;
            for ( ; ; )
            {
                if (!ProcessCondition(condition, Executable_Block.While)) break;
                if (condition.Context.IsBreak) break;
            }
        }
        void ProcessRet()
        {
            InvokeReturnValue(ResolveOperand(m_scriptInstruction.Operand0));
        }
        void ProcessCalc()
        {
            ResolveOperand(m_scriptInstruction.Operand0);
        }
        void ProcessCallBlock()
        {
            ScriptContext context = (ScriptContext)m_scriptInstruction.Value;
            context.Initialize(this);
            context.Execute();
        }
        void ProcessCallFunction()
        {
            ParseCall((CodeCallFunction)m_scriptInstruction.Operand0);
        }
        private void InvokeReturnValue(ScriptObject value)
        {
            m_Over = true;
            if (SupportReturnValue()) {
                m_returnObject = value;
            } else {
                m_parent.InvokeReturnValue(value);
            }
        }
        private void InvokeContinue(CodeObject con)
        {
            m_Over = true;
            if (!SupportContinue()) {
                if (m_parent == null)
                    throw new ExecutionException("this block is not support continue");
                m_parent.InvokeContinue(con);
            }
        }
        private void InvokeBreak(CodeObject bre)
        {
            m_Break = true;
            if (!SupportBreak()) {
                if (m_parent == null)
                    throw new ExecutionException("this block is not support break");
                m_parent.InvokeBreak(bre);
            }
        }
        private ScriptObject ResolveOperand_impl(CodeObject value)
        {
            if (value is CodeScriptObject) {
                return ParseScriptObject((CodeScriptObject)value);
            } else if (value is CodeFunction) {
                return ParseFunction((CodeFunction)value);
            } else if (value is CodeCallFunction) {
                return ParseCall(value as CodeCallFunction);
            } else if (value is CodeMember) {
                return GetVariable(value as CodeMember);
            } else if (value is CodeArray) {
                return ParseArray(value as CodeArray);
            } else if (value is CodeTable) {
                return ParseTable(value as CodeTable);
            } else if (value is CodeOperator) {
                return ParseOperate(value as CodeOperator);
            } else if (value is CodeTernary) {
                return ParseTernary(value as CodeTernary);
            }
            return ScriptNull.Instance;
        }
        private ScriptObject ResolveOperand(CodeObject value)
        {
            m_script.SetStackInfo(value.StackInfo);
            var ret = ResolveOperand_impl(value);
            if (value.Not) {
                ScriptBoolean b = ret as ScriptBoolean;
                if (b == null) throw new ExecutionException("Script Object Type [" + ret.Type + "] is cannot use [!] sign");
                ret = b.Inverse();
            }  else if (value.Negative) {
                ScriptNumber b = ret as ScriptNumber;
                if (b == null) throw new ExecutionException("Script Object Type [" + ret.Type + "] is cannot use [-] sign");
                ret = b.Negative();
            }
            return ret;
        }
        ScriptObject ParseScriptObject(CodeScriptObject obj)
        {
            return obj.Object;
        }
        ScriptFunction ParseFunction(CodeFunction func)
        {
            func.Func.SetParentContext(this);
            return func.Func;
        }
        ScriptObject ParseCall(CodeCallFunction scriptFunction)
        {
            ScriptObject obj = ResolveOperand(scriptFunction.Member);
            int num = scriptFunction.Parameters.Count;
            ScriptObject[] parameters = new ScriptObject[num];
            for (int i = 0; i < num; ++i) {
                parameters[i] = ResolveOperand(scriptFunction.Parameters[i]);
            }
            m_script.PushStackInfo();
            return obj.Call(parameters);
        }
        ScriptArray ParseArray(CodeArray array)
        {
            ScriptArray ret = new ScriptArray();
            int num = array.Elements.Count;
            for (int i = 0; i < num; ++i) {
                ret.Add(ResolveOperand(array.Elements[i]));
            }
            return ret;
        }
        ScriptTable ParseTable(CodeTable table)
        {
            ScriptTable ret = new ScriptTable();
            foreach (TableVariable variable in table.Variables) {
                ret.SetValue(variable.Key, ResolveOperand(variable.Value));
            }
            foreach (ScriptFunction func in table.Functions) {
                func.SetTable(ret);
                ret.SetValue(func.Name, func);
            }
            return ret;
        }
        ScriptObject ParseOperate(CodeOperator operate)
        {
            TokenType type = operate.Operator;
            ScriptObject left = ResolveOperand(operate.Left);
            if (type == TokenType.Plus) {
                ScriptObject right = ResolveOperand(operate.Right);
                if (left is ScriptString || right is ScriptString || (left is ScriptString && right is ScriptString)) {
                    return left.Plus(right);
                } else {
                    throw new ExecutionException("operate [+] left right is not same type");
                }
            } else if (type == TokenType.Minus || type == TokenType.Multiply || type == TokenType.Divide || type == TokenType.Modulo) {
                if (!(left is ScriptNumber)) throw new ExecutionException("operate [+ - * /] left is not number");
                ScriptObject right = ResolveOperand(operate.Right);
                if (!(right is ScriptNumber)) throw new ExecutionException("operate [+ - * /] right is not number");
                if (operate.Operator == TokenType.Minus)
                    return left.Minus(right);
                else if (operate.Operator == TokenType.Multiply)
                    return left.Multiply(right);
                else if (operate.Operator == TokenType.Divide)
                    return left.Divide(right);
                else if (operate.Operator == TokenType.Modulo)
                    return left.Modulo(right);
            } else {
                if (left is ScriptBoolean) {
                    if (type == TokenType.And) {
                        bool b1 = ((ScriptBoolean)left).Value;
                        if (b1 == false) return ScriptBoolean.False;
                        ScriptBoolean right = ResolveOperand(operate.Right) as ScriptBoolean;
                        if (right == null) throw new ExecutionException("operate [&&] right is not a bool");
                        return right.Value ? ScriptBoolean.True : ScriptBoolean.False;
                    } else if (type == TokenType.Or) {
                        bool b1 = ((ScriptBoolean)left).Value;
                        if (b1 == true) return ScriptBoolean.True;
                        ScriptBoolean right = ResolveOperand(operate.Right) as ScriptBoolean;
                        if (right == null) throw new ExecutionException("operate [||] right is not a bool");
                        return right.Value ? ScriptBoolean.True : ScriptBoolean.False;
                    } else {
                        bool b1 = ((ScriptBoolean)left).Value;
                        ScriptBoolean right = ResolveOperand(operate.Right) as ScriptBoolean;
                        if (right == null) throw new ExecutionException("operate [==] [!=] right is not a bool");
                        bool b2 = right.Value;
                        if (type == TokenType.Equal)
                            return b1 == b2 ? ScriptBoolean.True : ScriptBoolean.False;
                        else if (type == TokenType.NotEqual)
                            return b1 != b2 ? ScriptBoolean.True : ScriptBoolean.False;
                        else
                            throw new ExecutionException("nonsupport operate [" + type + "]  with bool");
                    }
                } else {
                    ScriptObject right = ResolveOperand(operate.Right);
                    if (left is ScriptNull || right is ScriptNull)
                    {
                        bool ret = false;
                        if (type == TokenType.Equal)
                            ret = (left == right);
                        else if (type == TokenType.NotEqual)
                            ret = (left != right);
                        else
                            throw new ExecutionException("nonsupport operate [" + type + "] with null");
                        return ret ? ScriptBoolean.True : ScriptBoolean.False;
                    }
                    if (left.Type != right.Type)
                        throw new ExecutionException("[operate] left right is not same type");
                    if (left is ScriptString) {
                        string str1 = ((ScriptString)left).Value;
                        string str2 = ((ScriptString)right).Value;
                        bool ret = false;
                        if (type == TokenType.Equal)
                            ret = str1 == str2;
                        else if (type == TokenType.NotEqual)
                            ret = str1 != str2;
                        else if (type == TokenType.Greater)
                            ret = string.Compare(str1, str2) < 0;
                        else if (type == TokenType.GreaterOrEqual)
                            ret = string.Compare(str1, str2) <= 0;
                        else if (type == TokenType.Less)
                            ret = string.Compare(str1, str2) > 0;
                        else if (type == TokenType.LessOrEqual)
                            ret = string.Compare(str1, str2) >= 0;
                        else
                            throw new ExecutionException("nonsupport operate [" + type + "] with string");
                        return ret ? ScriptBoolean.True : ScriptBoolean.False;
                    } else if (left is ScriptNumber) {
                        return ((ScriptNumber)left).Compare(type, operate, (ScriptNumber)right) ? ScriptBoolean.True : ScriptBoolean.False;
                    } else if (left is ScriptEnum) {
                        bool ret = false;
                        if (type == TokenType.Equal)
                            ret = (left.ObjectValue == right.ObjectValue);
                        else if (type == TokenType.NotEqual)
                            ret = (left.ObjectValue != right.ObjectValue);
                        else
                            throw new ExecutionException("nonsupport operate [" + type + "] with enum");
                        return ret ? ScriptBoolean.True : ScriptBoolean.False;
                    }
                }
            }
            throw new ExecutionException("错误的操作符号 " + operate.Operator);
        }
        ScriptObject ParseTernary(CodeTernary ternary)
        {
            ScriptBoolean b = ResolveOperand(ternary.Allow) as ScriptBoolean;
            if (b == null) throw new ExecutionException("三目运算符 条件必须是一个bool型");
            return b.Value ? ResolveOperand(ternary.True) : ResolveOperand(ternary.False);
        }
    }
}
