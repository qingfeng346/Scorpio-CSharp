using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.Runtime;
using Scorpio.Compiler;
using Scorpio.CodeDom;
using Scorpio.CodeDom.Temp;
using Scorpio.Exception;
namespace Scorpio.Runtime
{
    //执行命令
    public class ScriptContext
    {
        private Script m_script;                                                        //脚本类
        private ScriptContext m_parent;                                                 //父级执行命令
        private ScriptExecutable m_scriptExecutable;                                    //执行命令堆栈
        private ScriptInstruction m_scriptInstruction;                                  //当前执行
        private Dictionary<String, ScriptObject> m_variableDictionary = new Dictionary<String, ScriptObject>();     //当前作用域所有变量
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
        public void Initialize(ScriptContext parent, Dictionary<String, ScriptObject> variable)
        {
            m_parent = parent;
            m_variableDictionary = variable;
        }
        private void Initialize(ScriptContext parent, string name, ScriptObject obj)
        {
            m_parent = parent;
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
            if (m_variableDictionary.ContainsKey(name)) {
                Util.SetObject(m_variableDictionary, name, obj);
                return true;
            }
            if (m_parent != null) {
                return m_parent.SetVariableObject(name, obj);
            }
            return false;
        }
        private object GetMember(CodeMember member)
        {
            if (member.Type == MEMBER_TYPE.STRING) {
                return member.MemberString;
            } else if (member.Type == MEMBER_TYPE.INDEX) {
                return member.MemberIndex;
            } else if (member.Type == MEMBER_TYPE.NUMBER) {
                return member.MemberNumber;
            } else {
                return ResolveOperand(member.MemberObject).ObjectValue ;
            }
        }
        private ScriptObject GetVariable(CodeMember member)
        {
            ScriptObject ret = null;
            if (member.Parent == null) {
                string name = member.MemberString;
                ScriptObject obj = GetVariableObject(name);
                ret = (obj == null ? m_script.GetValue(name) : obj);
            } else {
                ret = ResolveOperand(member.Parent).GetValueInternal(GetMember(member));
            }
            if (ret == null) throw new ExecutionException("GetVariable member is error");
            if (member.Calc != CALC.NONE) {
                ScriptNumber num = ret as ScriptNumber;
                if (num == null) throw new ExecutionException("++或者--只能应用于Number类型");
                return num.Calc(member.Calc);
            }
            return ret;
        }
        private void SetVariable(CodeMember member, ScriptObject variable)
        {
            if (member.Parent == null) {
                string name = member.MemberString;
                if (!SetVariableObject(name, variable))
                    m_script.SetObjectInternal(name, variable);
            } else {
                ResolveOperand(member.Parent).SetValueInternal(GetMember(member), variable);
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
                case Opcode.RESOLVE: ProcessResolve(); break;
                case Opcode.CONTINUE: ProcessContinue(); break;
                case Opcode.BREAK: ProcessBreak(); break;
                case Opcode.CALL_BLOCK: ProcessCallBlock(); break;
                case Opcode.CALL_FUNCTION: ProcessCallFunction(); break;
                case Opcode.CALL_IF: ProcessCallIf(); break;
                case Opcode.CALL_FOR: ProcessCallFor(); break;
                case Opcode.CALL_FORSIMPLE: ProcessCallForSimple(); break;
                case Opcode.CALL_FOREACH: ProcessCallForeach(); break;
                case Opcode.CALL_WHILE: ProcessCallWhile(); break;
                case Opcode.CALL_SWITCH: ProcessCallSwitch(); break;
                case Opcode.CALL_TRY: ProcessTry(); break;
                case Opcode.THROW: ProcessThrow(); break;
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
            SetVariable(m_scriptInstruction.Operand0 as CodeMember, ResolveOperand(m_scriptInstruction.Operand1));
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
        void ProcessCallForSimple()
        {
            CodeForSimple code = (CodeForSimple)m_scriptInstruction.Operand0;
            ScriptNumber beginNumber = ResolveOperand(code.Begin) as ScriptNumber;
            if (beginNumber == null) throw new ExecutionException("forsimple 初始值必须是number");
            ScriptNumber finishedNumber = ResolveOperand(code.Finished) as ScriptNumber;
            if (finishedNumber == null) throw new ExecutionException("forsimple 最大值必须是number");
            int begin = beginNumber.ToInt32();
            int finished = finishedNumber.ToInt32();
            int step;
            if (code.Step != null) {
                ScriptNumber stepNumber = ResolveOperand(code.Step) as ScriptNumber;
                if (stepNumber == null) throw new ExecutionException("forsimple Step必须是number");
                step = stepNumber.ToInt32();
            } else {
                step = 1;
            }
            var variables = code.variables;
            for (int i = begin; i <= finished; i += step)
            {
                variables[code.Identifier] = m_script.CreateNumber(i);
                code.BlockContext.Initialize(this, variables);
                code.BlockContext.Execute();
                if (code.BlockContext.IsBreak) break;
            }
        }
        void ProcessCallForeach()
        {
            CodeForeach code = (CodeForeach)m_scriptInstruction.Operand0;
            ScriptObject loop = ResolveOperand(code.LoopObject);
            if (!loop.IsFunction) throw new ExecutionException("foreach函数必须返回一个ScriptFunction");
            ScriptObject obj;
            for ( ; ; )
            {
                obj = m_script.CreateObject(((ScriptFunction)loop).Call());
                if (obj == null || obj is ScriptNull) return;
                code.Context.Initialize(this, code.Identifier, obj);
                code.Context.Execute();
                if (code.Context.IsBreak) break;
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
        void ProcessCallSwitch()
        {
            CodeSwitch code = (CodeSwitch)m_scriptInstruction.Operand0;
            ScriptObject obj = ResolveOperand(code.Condition);
            bool exec = false;
            foreach (TempCase c in code.Cases)
            {
                foreach (object a in c.Allow)
                {
                    if (a.Equals(obj.ObjectValue))
                    {
                        exec = true;
                        c.Context.Initialize(this);
                        c.Context.Execute();
                        break;
                    }
                }
            }
            if (exec == false && code.Default != null)
            {
                code.Default.Context.Initialize(this);
                code.Default.Context.Execute();
            }
        }
        void ProcessTry()
        {
            CodeTry code = (CodeTry)m_scriptInstruction.Operand0;
            try {
                code.TryContext.Initialize(this);
                code.TryContext.Execute();
            } catch (InteriorException ex) {
                code.CatchContext.Initialize(this, code.Identifier, ex.obj);
                code.CatchContext.Execute();
            } catch (System.Exception ex) {
                code.CatchContext.Initialize(this, code.Identifier, m_script.CreateObject(ex));
                code.CatchContext.Execute();
            }
        }
        void ProcessThrow()
        {
            CodeThrow code = (CodeThrow)m_scriptInstruction.Operand0;
            throw new InteriorException(ResolveOperand(code.obj));
        }
        void ProcessRet()
        {
            InvokeReturnValue(ResolveOperand(m_scriptInstruction.Operand0));
        }
        void ProcessResolve()
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
            ParseCall((CodeCallFunction)m_scriptInstruction.Operand0, false);
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
                return ParseCall(value as CodeCallFunction, true);
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
            } else if (value is CodeAssign) {
                return ParseAssign(value as CodeAssign);
            } else if (value is CodeEval) {
                return ParseEval(value as CodeEval);
            }
            return ScriptNull.Instance;
        }
        private ScriptObject ResolveOperand(CodeObject value)
        {
            m_script.SetStackInfo(value.StackInfo);
            ScriptObject ret = ResolveOperand_impl(value);
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
        ScriptObject ParseCall(CodeCallFunction scriptFunction, bool needRet)
        {
            ScriptObject obj = ResolveOperand(scriptFunction.Member);
            int num = scriptFunction.Parameters.Count;
            ScriptObject[] parameters = new ScriptObject[num];
            for (int i = 0; i < num; ++i) {
                parameters[i] = ResolveOperand(scriptFunction.Parameters[i]);
            }
            m_script.PushStackInfo();
            object ret = obj.Call(parameters);
            return needRet ? m_script.CreateObject(ret) : null;
        }
        ScriptArray ParseArray(CodeArray array)
        {
            ScriptArray ret = m_script.CreateArray();
            int num = array.Elements.Count;
            for (int i = 0; i < num; ++i) {
                ret.Add(ResolveOperand(array.Elements[i]));
            }
            return ret;
        }
        ScriptTable ParseTable(CodeTable table)
        {
            ScriptTable ret = m_script.CreateTable();
            foreach (TableVariable variable in table.Variables) {
                ret.SetValue(variable.key, ResolveOperand(variable.value));
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
                if (left is ScriptString || right is ScriptString) {
                    return m_script.CreateString(left.ToString() + right.ToString());
                } else if (left is ScriptNumber && right is ScriptNumber){
                    return (left as ScriptNumber).Compute(TokenType.Plus, right as ScriptNumber);
                } else {
                    throw new ExecutionException("operate [+] left right is not same type");
                }
            } else if (type == TokenType.Minus || type == TokenType.Multiply || type == TokenType.Divide || type == TokenType.Modulo ||
                type == TokenType.InclusiveOr || type == TokenType.Combine || type == TokenType.XOR || type == TokenType.Shr || type == TokenType.Shi) {
                ScriptNumber leftNumber = left as ScriptNumber;
                if (leftNumber == null) throw new ExecutionException("运算符[左边]必须是number类型");
                ScriptNumber rightNumber = ResolveOperand(operate.Right) as ScriptNumber;
                if (rightNumber == null) throw new ExecutionException("运算符[右边]必须是number类型");
                return leftNumber.Compute(type, rightNumber);
            } else {
                if (left is ScriptBoolean) {
                    bool b1 = ((ScriptBoolean)left).Value;
                    if (type == TokenType.And) {
                        if (b1 == false) return ScriptBoolean.False;
                        ScriptBoolean right = ResolveOperand(operate.Right) as ScriptBoolean;
                        if (right == null) throw new ExecutionException("operate [&&] right is not a bool");
                        return right.Value ? ScriptBoolean.True : ScriptBoolean.False;
                    } else if (type == TokenType.Or) {
                        if (b1 == true) return ScriptBoolean.True;
                        ScriptBoolean right = ResolveOperand(operate.Right) as ScriptBoolean;
                        if (right == null) throw new ExecutionException("operate [||] right is not a bool");
                        return right.Value ? ScriptBoolean.True : ScriptBoolean.False;
                    } else {
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
                    if (left is ScriptNull || right is ScriptNull) {
                        bool ret = false;
                        if (type == TokenType.Equal)
                            ret = (left == right);
                        else if (type == TokenType.NotEqual)
                            ret = (left != right);
                        else
                            throw new ExecutionException("nonsupport operate [" + type + "] with null");
                        return ret ? ScriptBoolean.True : ScriptBoolean.False;
                    }
                    if (type == TokenType.Equal) {
                        return left.ObjectValue.Equals(right.ObjectValue) ? ScriptBoolean.True : ScriptBoolean.False;
                    } else if (type == TokenType.NotEqual) {
                        return !left.ObjectValue.Equals(right.ObjectValue) ? ScriptBoolean.True : ScriptBoolean.False;
                    }
                    if (left.Type != right.Type)
                        throw new ExecutionException("[operate] left right is not same type");
                    if (left is ScriptString) {
                        return ((ScriptString)left).Compare(type, (ScriptString)right) ? ScriptBoolean.True : ScriptBoolean.False;
                    } else if (left is ScriptNumber) {
                        return ((ScriptNumber)left).Compare(type, (ScriptNumber)right) ? ScriptBoolean.True : ScriptBoolean.False;
                    } else {
                        throw new ExecutionException("nonsupport operate [" + type + "] with " + left.Type);
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
        ScriptObject ParseAssign(CodeAssign assign)
        {
            if (assign.AssignType == TokenType.Assign) {
                var ret = ResolveOperand(assign.value);
                SetVariable(assign.member, ret);
                return ret;
            } else {
                ScriptObject obj = GetVariable(assign.member);
                ScriptString str = obj as ScriptString;
                if (str != null)
                {
                    if (assign.AssignType == TokenType.AssignPlus)
                        return str.AssignPlus(ResolveOperand(assign.value));
                    else
                        throw new ExecutionException("string类型只支持[+=]赋值操作");
                }
                ScriptNumber num = obj as ScriptNumber;
                if (num != null)
                {
                    ScriptNumber right = ResolveOperand(assign.value) as ScriptNumber;
                    if (right == null)
                        throw new ExecutionException("[+= -=...]值只能为 number类型");
                    return num.AssignCompute(assign.AssignType, right);
                }
                throw new ExecutionException("[+= -=...]左边值只能为number或者string");
            }
        }
        ScriptObject ParseEval(CodeEval eval)
        {
            ScriptString obj = ResolveOperand(eval.EvalObject) as ScriptString;
            if (obj == null)
                throw new ExecutionException("Eval参数必须是一个字符串");
            return m_script.LoadString("", obj.Value, this);
        }
    }
}
