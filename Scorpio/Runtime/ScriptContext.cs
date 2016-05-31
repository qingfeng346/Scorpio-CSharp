using System;
using System.Collections.Generic;
using Scorpio;
using Scorpio.Compiler;
using Scorpio.CodeDom;
using Scorpio.CodeDom.Temp;
using Scorpio.Exception;
using Scorpio.Function;
namespace Scorpio.Runtime
{
    //执行命令
    //注意事项:
    //所有调用另一个程序集的地方 都要new一个新的 否则递归调用会相互影响
    public class ScriptContext
    {
        private Script m_script;                                            //脚本类
        private ScriptContext m_parent;                                     //父级执行命令
        private ScriptInstruction[] m_scriptInstructions;                   //指令集
        private ScriptInstruction m_scriptInstruction;                      //当前执行的指令
        private int m_InstructionCount;                                     //指令数量
        private Executable_Block m_block;                                   //指令集类型
        private Dictionary<String, ScriptObject> m_variableDictionary;      //当前作用域所有变量
        private ScriptObject m_returnObject = null;                         //返回值
        private bool m_Break = false;                                       //break跳出
        private bool m_Continue = false;                                    //continue跳出
        private bool m_Over = false;                                        //函数是否已经结束
        
        public ScriptContext(Script script, ScriptExecutable scriptExecutable) : this(script, scriptExecutable, null, Executable_Block.None) { }
        public ScriptContext(Script script, ScriptExecutable scriptExecutable, ScriptContext parent) : this(script, scriptExecutable, parent, Executable_Block.None) { }
        public ScriptContext(Script script, ScriptExecutable scriptExecutable, ScriptContext parent, Executable_Block block) {
            m_script = script;
            m_parent = parent;
            m_block = block;
            m_variableDictionary = new Dictionary<String, ScriptObject>();
            if (scriptExecutable != null) {
                m_scriptInstructions = scriptExecutable.ScriptInstructions;
                m_InstructionCount = m_scriptInstructions.Length;
            }
        }
        private bool IsOver { get { return m_Break || m_Over; } }                       //break 或者 return  跳出循环
        private bool IsExecuted { get { return m_Break || m_Over || m_Continue; } }     //continue break return 当前模块是否执行完成
        public void Initialize(Dictionary<String, ScriptObject> variable) {
            foreach (KeyValuePair<String, ScriptObject> pair in variable)
                m_variableDictionary[pair.Key] = pair.Value;
        }
        private void Initialize(string name, ScriptObject obj) {
            m_variableDictionary.Add(name, obj);
        }
        //初始化所有数据 每次调用 Execute 调用
        private void Reset()
        {
            m_returnObject = null;
            m_Over = false;
            m_Break = false;
            m_Continue = false;
        }
        private void ApplyVariableObject(string name)
        {
            if (!m_variableDictionary.ContainsKey(name))
                m_variableDictionary.Add(name, m_script.Null);
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
            return member.Type == MEMBER_TYPE.VALUE ? member.MemberValue : ResolveOperand(member.MemberObject).KeyValue;
        }
        private ScriptObject GetVariable(CodeMember member)
        {
            ScriptObject ret = null;
            if (member.Parent == null) {
                string name = (string)member.MemberValue;
                ScriptObject obj = GetVariableObject(name);
                ret = (obj == null ? m_script.GetValue(name) : obj);
                ret.Name = name;
            } else {
                ret = ResolveOperand(member.Parent);
                /*此处设置一下堆栈位置 否则 函数返回值取值出错会报错位置 例如  
                    function Get() { 
                        return null 
                    } 
                    Get().a
                    
                上述代码报错会报道 return null 那一行 但实际出错 是 .a 的时候 下面这句话就是把堆栈设置回 .a 那一行
                */
                m_script.SetStackInfo(member.StackInfo);
                ret = ret.GetValue(GetMember(member));
            }
            if (ret == null) throw new ExecutionException(m_script, "GetVariable member is error");
            if (member.Calc != CALC.NONE) {
                ScriptNumber num = ret as ScriptNumber;
                if (num == null) throw new ExecutionException(m_script, "++或者--只能应用于Number类型");
                return num.Calc(member.Calc);
            }
            return ret;
        }
        private void SetVariable(CodeMember member, ScriptObject variable)
        {
            if (member.Parent == null) {
                string name = (string)member.MemberValue;
                if (!SetVariableObject(name, variable))
                    m_script.SetObjectInternal(name, variable);
            } else {
                ResolveOperand(member.Parent).SetValue(GetMember(member), variable);
            }
        }
        public ScriptObject Execute()
        {
            Reset();
            int iInstruction = 0;
            while (iInstruction < m_InstructionCount) {
                m_scriptInstruction = m_scriptInstructions[iInstruction++];
                ExecuteInstruction();
                if (IsExecuted) break;
            }
            return m_returnObject;
        }
        private ScriptObject Execute(ScriptExecutable executable)
        {
            if (executable == null) return null;
            Reset();
            ScriptInstruction[] scriptInstructions = executable.ScriptInstructions;
            int iInstruction = 0;
            int iInstructionCount = scriptInstructions.Length ;
            while (iInstruction < iInstructionCount) {
                m_scriptInstruction = scriptInstructions[iInstruction++];
                ExecuteInstruction();
                if (IsExecuted) break;
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
            ScriptContext context = new ScriptContext(m_script, null, this, Executable_Block.For);
            context.Execute(code.BeginExecutable);
            for ( ; ; ) {
                if (code.Condition != null) {
                    if (!context.ResolveOperand(code.Condition).LogicOperation()) break;
                }
                ScriptContext blockContext = new ScriptContext(m_script, code.BlockExecutable, context, Executable_Block.For);
                blockContext.Execute();
                if (blockContext.IsOver) break;
                context.Execute(code.LoopExecutable);
            }
        }
        void ProcessCallForSimple()
        {
            CodeForSimple code = (CodeForSimple)m_scriptInstruction.Operand0;
            ScriptNumber beginNumber = ResolveOperand(code.Begin) as ScriptNumber;
            if (beginNumber == null) throw new ExecutionException(m_script, "forsimple 初始值必须是number");
            ScriptNumber finishedNumber = ResolveOperand(code.Finished) as ScriptNumber;
            if (finishedNumber == null) throw new ExecutionException(m_script, "forsimple 最大值必须是number");
            int begin = beginNumber.ToInt32();
            int finished = finishedNumber.ToInt32();
            int step;
            if (code.Step != null) {
                ScriptNumber stepNumber = ResolveOperand(code.Step) as ScriptNumber;
                if (stepNumber == null) throw new ExecutionException(m_script, "forsimple Step必须是number");
                step = stepNumber.ToInt32();
            } else {
                step = 1;
            }
            ScriptContext context;
            for (int i = begin; i <= finished; i += step) {
                context = new ScriptContext(m_script, code.BlockExecutable, this, Executable_Block.For);
                context.Initialize(code.Identifier, m_script.CreateNumber(i));
                context.Execute();
                if (context.IsOver) break;
            }
        }
        void ProcessCallForeach()
        {
            CodeForeach code = (CodeForeach)m_scriptInstruction.Operand0;
            ScriptObject loop = ResolveOperand(code.LoopObject);
            if (!(loop is ScriptFunction)) throw new ExecutionException(m_script, "foreach函数必须返回一个ScriptFunction");
            object obj;
            ScriptFunction func = (ScriptFunction)loop;
            ScriptContext context;
            for ( ; ; ) {
                obj = func.Call();
                if (obj == null) return;
                context = new ScriptContext(m_script, code.BlockExecutable, this, Executable_Block.Foreach);
                context.Initialize(code.Identifier, m_script.CreateObject(obj));
                context.Execute();
                if (context.IsOver) break;
            }
        }
        void ProcessCallIf() {
            CodeIf code = (CodeIf)m_scriptInstruction.Operand0;
            if (ProcessAllow(code.If)) {
                ProcessCondition(code.If);
                return;
            }
            foreach (TempCondition ElseIf in code.ElseIf) {
                if (ProcessAllow(ElseIf)) {
                    ProcessCondition(ElseIf);
                    return;
                }
            }
            if (code.Else != null && ProcessAllow(code.Else)) {
                ProcessCondition(code.Else);
            }
        }
        bool ProcessAllow(TempCondition con) {
            if (con.Allow != null && !ResolveOperand(con.Allow).LogicOperation()) {
                return false;
            }
            return true;
        }
        void ProcessCondition(TempCondition condition) {
            new ScriptContext(m_script, condition.Executable, this, condition.Block).Execute();
        }
        void ProcessCallWhile() {
            CodeWhile code = (CodeWhile)m_scriptInstruction.Operand0;
            TempCondition condition = code.While;
            ScriptContext context;
            for ( ; ; ) {
                if (!ProcessAllow(condition)) {
                    break;
                }
                context = new ScriptContext(m_script, condition.Executable, this, Executable_Block.While);
                context.Execute();
                if (context.IsOver) {
                    break;
                }
            }
        }
        void ProcessCallSwitch()
        {
            CodeSwitch code = (CodeSwitch)m_scriptInstruction.Operand0;
            ScriptObject obj = ResolveOperand(code.Condition);
            bool exec = false;
            foreach (TempCase Case in code.Cases) {
                foreach (CodeObject allow in Case.Allow) {
                    if (ResolveOperand(allow).Equals(obj)) {
                        exec = true;
                        new ScriptContext(m_script, Case.Executable, this, Executable_Block.Switch).Execute();
                        break;
                    }
                }
                if (exec) { break; }
            }
            if (exec == false && code.Default != null) {
                new ScriptContext(m_script, code.Default.Executable, this, Executable_Block.Switch).Execute();
            }
        }
        void ProcessTry()
        {
            CodeTry code = (CodeTry)m_scriptInstruction.Operand0;
            try {
                new ScriptContext(m_script, code.TryExecutable, this).Execute();
            } catch (InteriorException ex) {
                ScriptContext context = new ScriptContext(m_script, code.CatchExecutable, this);
                context.Initialize(code.Identifier, ex.obj);
                context.Execute();
            } catch (System.Exception ex) {
                ScriptContext context = new ScriptContext(m_script, code.CatchExecutable, this);
                context.Initialize(code.Identifier, m_script.CreateObject(ex));
                context.Execute();
            }
        }
        void ProcessThrow()
        {
            throw new InteriorException(ResolveOperand(((CodeThrow)m_scriptInstruction.Operand0).obj));
        }
        void ProcessRet()
        {
            if (m_scriptInstruction.Operand0 == null)
                InvokeReturnValue(null);
            else
                InvokeReturnValue(ResolveOperand(m_scriptInstruction.Operand0));
        }
        void ProcessResolve()
        {
            ResolveOperand(m_scriptInstruction.Operand0);
        }
        void ProcessCallBlock()
        {
            ParseCallBlock((CodeCallBlock)m_scriptInstruction.Operand0);
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
            m_Continue = true;
            if (!SupportContinue()) {
                if (m_parent == null)
                    throw new ExecutionException(m_script, "当前模块不支持continue语法");
                m_parent.InvokeContinue(con);
            }
        }
        private void InvokeBreak(CodeObject bre)
        {
            m_Break = true;
            if (!SupportBreak()) {
                if (m_parent == null)
                    throw new ExecutionException(m_script, "当前模块不支持break语法");
                m_parent.InvokeBreak(bre);
            }
        }
        private ScriptObject ResolveOperand_impl(CodeObject value)
        {
            if (value is CodeScriptObject) {
                return ParseScriptObject((CodeScriptObject)value);
            } else if (value is CodeRegion) {
                return ParseRegion((CodeRegion)value);
            } else if (value is CodeFunction) {
                return ParseFunction((CodeFunction)value);
            } else if (value is CodeCallFunction) {
                return ParseCall((CodeCallFunction)value, true);
            } else if (value is CodeMember) {
                return GetVariable((CodeMember)value);
            } else if (value is CodeArray) {
                return ParseArray((CodeArray)value);
            } else if (value is CodeTable) {
                return ParseTable((CodeTable)value);
            } else if (value is CodeOperator) {
                return ParseOperate((CodeOperator)value);
            } else if (value is CodeTernary) {
                return ParseTernary((CodeTernary)value);
            } else if (value is CodeAssign) {
                return ParseAssign((CodeAssign)value);
            } else if (value is CodeEval) {
                return ParseEval((CodeEval)value);
            }
            return m_script.Null;
        }
        ScriptObject ResolveOperand(CodeObject value)
        {
            m_script.SetStackInfo(value.StackInfo);
            ScriptObject ret = ResolveOperand_impl(value);
            if (value.Not) {
                ScriptBoolean b = ret as ScriptBoolean;
                if (b == null) throw new ExecutionException(m_script, "Script Object Type [" + ret.Type + "] is cannot use [!] sign");
                ret = b.Inverse();
            }  else if (value.Negative) {
                ScriptNumber b = ret as ScriptNumber;
                if (b == null) throw new ExecutionException(m_script, "Script Object Type [" + ret.Type + "] is cannot use [-] sign");
                ret = b.Negative();
            }
            return ret;
        }
        ScriptObject ParseScriptObject(CodeScriptObject obj)
        {
            //此处原先使用Clone 是因为 number 和 string 有自运算的操作 会影响常量 但是现在设置变量会调用Assign() 基础类型会自动复制一次 所以去掉clone
            return obj.Object;
            //return obj.Object.Clone();
        }
        ScriptObject ParseRegion(CodeRegion region)
        {
            return ResolveOperand(region.Context);
        }
        ScriptFunction ParseFunction(CodeFunction func)
        {
            return func.Func.Create().SetParentContext(this);
        }
        void ParseCallBlock(CodeCallBlock block) {
            new ScriptContext(m_script, block.Executable, this).Execute();
        }
        ScriptObject ParseCall(CodeCallFunction scriptFunction, bool needRet)
        {
            ScriptObject obj = ResolveOperand(scriptFunction.Member);
            int num = scriptFunction.ParametersCount;
            ScriptObject[] parameters = new ScriptObject[num];
            for (int i = 0; i < num; ++i) {
                //此处要调用Assign 如果传入number string等基础类型  在函数内自运算的话 会影响 传入的值
                parameters[i] = ResolveOperand(scriptFunction.Parameters[i]).Assign();
            }
            m_script.PushStackInfo();
            object ret = obj.Call(parameters);
            return needRet ? m_script.CreateObject(ret) : null;
        }
        ScriptArray ParseArray(CodeArray array)
        {
            ScriptArray ret = m_script.CreateArray();
            foreach (CodeObject ele in array.Elements) {
                ret.Add(ResolveOperand(ele));
            }
            return ret;
        }
        ScriptTable ParseTable(CodeTable table)
        {
            ScriptTable ret = m_script.CreateTable();
            foreach (CodeTable.TableVariable variable in table.Variables) {
                ret.SetValue(variable.key, ResolveOperand(variable.value));
            }
            foreach (ScriptScriptFunction func in table.Functions) {
                func.SetTable(ret);
                ret.SetValue(func.Name, func);
            }
            return ret;
        }
        ScriptObject ParseOperate(CodeOperator operate)
        {
            TokenType type = operate.Operator;
            ScriptObject left = ResolveOperand(operate.Left);
            switch (type) {
            case TokenType.Plus:
                ScriptObject right = ResolveOperand(operate.Right);
                if (left is ScriptString || right is ScriptString) { return m_script.CreateString(left.ToString() + right.ToString()); }
                return left.Compute(type, right);
            case TokenType.Minus:
            case TokenType.Multiply:
            case TokenType.Divide:
            case TokenType.Modulo:
            case TokenType.InclusiveOr:
            case TokenType.Combine:
            case TokenType.XOR:
            case TokenType.Shr:
            case TokenType.Shi:
                return left.Compute(type, ResolveOperand(operate.Right));
            case TokenType.And:
                if (!left.LogicOperation()) return m_script.False;
                return m_script.GetBoolean(ResolveOperand(operate.Right).LogicOperation());
            case TokenType.Or:
                if (left.LogicOperation()) return m_script.True;
                return m_script.GetBoolean(ResolveOperand(operate.Right).LogicOperation());
            case TokenType.Equal:
                return m_script.GetBoolean(left.Equals(ResolveOperand(operate.Right)));
            case TokenType.NotEqual:
                return m_script.GetBoolean(!left.Equals(ResolveOperand(operate.Right)));
            case TokenType.Greater:
            case TokenType.GreaterOrEqual:
            case TokenType.Less:
            case TokenType.LessOrEqual:
                return m_script.GetBoolean(left.Compare(type, ResolveOperand(operate.Right)));
            default:
                throw new ExecutionException(m_script, "不支持的运算符 " + type);
            }
        }
        ScriptObject ParseTernary(CodeTernary ternary)
        {
            return ResolveOperand(ternary.Allow).LogicOperation() ? ResolveOperand(ternary.True) : ResolveOperand(ternary.False);
        }
        ScriptObject ParseAssign(CodeAssign assign)
        {
            if (assign.AssignType == TokenType.Assign) {
                var ret = ResolveOperand(assign.value);
                SetVariable(assign.member, ret);
                return ret;
            } else {
                return GetVariable(assign.member).AssignCompute(assign.AssignType, ResolveOperand(assign.value));
            }
        }
        ScriptObject ParseEval(CodeEval eval)
        {
            ScriptString obj = ResolveOperand(eval.EvalObject) as ScriptString;
            if (obj == null) throw new ExecutionException(m_script, "Eval参数必须是一个字符串");
            return m_script.LoadString("", obj.Value, this, false);
        }
    }
}
