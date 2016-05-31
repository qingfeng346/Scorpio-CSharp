using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.Exception;
using Scorpio.Runtime;
using Scorpio.CodeDom;
using Scorpio.CodeDom.Temp;
using Scorpio.Variable;
using Scorpio.Function;
namespace Scorpio.Compiler
{
    //上下文解析
    public partial class ScriptParser
    {
        private enum DefineType {
            None,       //没有进入#if
            Already,    //已经进入条件了
            Being,      //还没找到合适的 正在处理
            Break,      //跳过
        }
        private class DefineState {
            public DefineType State;
            public DefineState(DefineType state) {
                this.State = state;
            }
        }
        private class DefineObject {
            public bool Not;
        }
        private class DefineString : DefineObject {
            public string Define;
            public DefineString(string define) {
                this.Define = define;
            }
        }
        private class DefineOperate : DefineObject {
            public DefineObject Left;       //左边值
            public DefineObject Right;      //右边值
            public bool and;                //是否是并且操作
            public DefineOperate(DefineObject left, DefineObject right, bool and) {
                this.Left = left;
                this.Right = right;
                this.and = and;
            }
        }
        private Script m_script;                                                        //脚本类
        private string m_strBreviary;                                                   //当前解析的脚本摘要
        private int m_iNextToken;                                                       //当前读到token
        private List<Token> m_listTokens;                                               //token列表
        private Stack<ScriptExecutable> m_Executables = new Stack<ScriptExecutable>();  //指令栈
        private ScriptExecutable m_scriptExecutable;                                    //当前指令栈
        private Stack<DefineState> m_Defines = new Stack<DefineState>();                //define状态
        private DefineState m_define;                                                   //define当前状态
        public ScriptParser(Script script, List<Token> listTokens, string strBreviary)
        {
            m_script = script;
            m_strBreviary = strBreviary;
            m_iNextToken = 0;
            m_listTokens = new List<Token>(listTokens);
        }
        public void BeginExecutable(Executable_Block block)
        {
            m_scriptExecutable = new ScriptExecutable(block);
            m_Executables.Push(m_scriptExecutable);
        }
        public void EndExecutable()
        {
            m_Executables.Pop();
            m_scriptExecutable = (m_Executables.Count > 0) ? m_Executables.Peek() : null;
        }
        private int GetSourceLine()
        {
            return PeekToken().SourceLine;
        }
        //解析脚本
        public ScriptExecutable Parse()
        {
            m_iNextToken = 0;
            return ParseStatementBlock(Executable_Block.Context, false, TokenType.Finished);
        }
        //解析区域代码内容( {} 之间的内容)
        private ScriptExecutable ParseStatementBlock(Executable_Block block)
        {
            return ParseStatementBlock(block, true, TokenType.RightBrace);
        }
        //解析区域代码内容( {} 之间的内容)
        private ScriptExecutable ParseStatementBlock(Executable_Block block, bool readLeftBrace, TokenType finished)
        {
            BeginExecutable(block);
            if (readLeftBrace && PeekToken().Type != TokenType.LeftBrace) {
                ParseStatement();
            } else {
                if (readLeftBrace) ReadLeftBrace();
                TokenType tokenType;
                while (HasMoreTokens()) {
                    tokenType = ReadToken().Type;
                    if (tokenType == finished)
                        break;
                    UndoToken();
                    ParseStatement();
                }
            }
            ScriptExecutable ret = m_scriptExecutable;
            ret.EndScriptInstruction();
            EndExecutable();
            return ret;
        }
        //解析区域代码内容 ({} 之间的内容)
        private void ParseStatement()
        {
            Token token = ReadToken();
            switch (token.Type)
            {
                case TokenType.Var:
                    ParseVar();
                    break;
                case TokenType.LeftBrace:
                    ParseBlock();
                    break;
                case TokenType.If:
                    ParseIf();
                    break;
                case TokenType.For:
                    ParseFor();
                    break;
                case TokenType.Foreach:
                    ParseForeach();
                    break;
                case TokenType.While:
                    ParseWhile();
                    break;
                case TokenType.Switch:
                    ParseSwtich();
                    break;
                case TokenType.Try:
                    ParseTry();
                    break;
                case TokenType.Throw:
                    ParseThrow();
                    break;
                case TokenType.Return:
                    ParseReturn();
                    break;
                case TokenType.Sharp:
                    ParseSharp();
                    break;
                case TokenType.Identifier:
                case TokenType.Increment:
                case TokenType.Decrement:
                case TokenType.Eval:
                    ParseExpression();
                    break;
                case TokenType.Break:
                    m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.BREAK, new CodeObject(m_strBreviary, token.SourceLine)));
                    break;
                case TokenType.Continue:
                    m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CONTINUE, new CodeObject(m_strBreviary, token.SourceLine)));
                    break;
                case TokenType.Function:
                    ParseFunction();
                    break;
                case TokenType.SemiColon:
                    break;
                default:
                    throw new ParserException("不支持的语法 ", token);
            }
        }
        //解析函数（全局函数或类函数）
        private void ParseFunction()
        {
            Token token = PeekToken();
            UndoToken();
            ScriptScriptFunction func = ParseFunctionDeclaration(true);
            m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.MOV, new CodeMember(func.Name), new CodeFunction(func, m_strBreviary, token.SourceLine)));
        }
        //解析函数（返回一个函数）
        private ScriptScriptFunction ParseFunctionDeclaration(bool needName)
        {
            Token token = ReadToken();
            if (token.Type != TokenType.Function)
                throw new ParserException("Function declaration must start with the 'function' keyword.", token);
            String strFunctionName = needName ? ReadIdentifier() : (PeekToken().Type == TokenType.Identifier ? ReadIdentifier() : "");
            List<String> listParameters = new List<String>();
            bool bParams = false;
            Token peek = ReadToken();
            if (peek.Type == TokenType.LeftPar) {
                if (PeekToken().Type != TokenType.RightPar) {
                    while (true) {
                        token = ReadToken();
                        if (token.Type == TokenType.Params) {
                            token = ReadToken();
                            bParams = true;
                        }
                        if (token.Type != TokenType.Identifier) {
                            throw new ParserException("Unexpected token '" + token.Lexeme + "' in function declaration.", token);
                        }
                        String strParameterName = token.Lexeme.ToString();
                        listParameters.Add(strParameterName);
                        token = PeekToken();
                        if (token.Type == TokenType.Comma && !bParams)
                            ReadComma();
                        else if (token.Type == TokenType.RightPar)
                            break;
                        else
                            throw new ParserException("Comma ',' or right parenthesis ')' expected in function declararion.", token);
                    }
                }
                ReadRightParenthesis();
                peek = ReadToken();
            }
            if (peek.Type == TokenType.LeftBrace) {
                UndoToken();
            }
            ScriptExecutable executable = ParseStatementBlock(Executable_Block.Function);
            return m_script.CreateFunction(strFunctionName, new ScorpioScriptFunction(m_script, listParameters, executable, bParams));
        }
        //解析Var关键字
        private void ParseVar()
        {
            for (; ; ) {
                Token peek = PeekToken();
                if (peek.Type == TokenType.Function) {
                    ScriptScriptFunction func = ParseFunctionDeclaration(true);
                    m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.VAR, func.Name));
                    m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.MOV, new CodeMember(func.Name), new CodeFunction(func, m_strBreviary, peek.SourceLine)));
                } else {
                    m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.VAR, ReadIdentifier()));
                    peek = PeekToken();
                    if (peek.Type == TokenType.Assign) {
                        UndoToken();
                        ParseStatement();
                    }
                }
                peek = ReadToken();
                if (peek.Type != TokenType.Comma) {
                    UndoToken();
                    break;
                }
                peek = PeekToken();
                if (peek.Type == TokenType.Var) {
                    ReadToken();
                }
            }
        }
        //解析普通代码块 {}
        private void ParseBlock()
        {
            UndoToken();
            m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_BLOCK, new CodeCallBlock(ParseStatementBlock(Executable_Block.Block))));
        }
        //解析if(判断语句)
        private void ParseIf()
        {
            CodeIf ret = new CodeIf();
            ret.If = ParseCondition(true, Executable_Block.If);
            List<TempCondition> ElseIf = new List<TempCondition>();
            for (; ; )
            {
                Token token = ReadToken();
                if (token.Type == TokenType.ElseIf) {
                    ElseIf.Add(ParseCondition(true, Executable_Block.If));
                } else if (token.Type == TokenType.Else) {
                    if (PeekToken().Type == TokenType.If) {
                        ReadToken();
                        ElseIf.Add(ParseCondition(true, Executable_Block.If));
                    } else {
                        UndoToken();
                        break;
                    }
                } else {
                    UndoToken();
                    break;
                }
            }
            if (PeekToken().Type == TokenType.Else)
            {
                ReadToken();
                ret.Else = ParseCondition(false, Executable_Block.If);
            }
            ret.Init(ElseIf);
            m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_IF, ret));
        }
        //解析判断内容
        private TempCondition ParseCondition(bool condition, Executable_Block block)
        {
            CodeObject con = null;
            if (condition) {
                ReadLeftParenthesis();
                con = GetObject();
                ReadRightParenthesis();
            }
            return new TempCondition(con, ParseStatementBlock(block), block);
        }
        //解析for语句
        private void ParseFor()
        {
            ReadLeftParenthesis();
            int partIndex = m_iNextToken;
			if (PeekToken ().Type == TokenType.Var) ReadToken ();
			Token identifier = ReadToken();
			if (identifier.Type == TokenType.Identifier) {
                Token assign = ReadToken();
                if (assign.Type == TokenType.Assign) {
                    CodeObject obj = GetObject();
                    Token comma = ReadToken();
                    if (comma.Type == TokenType.Comma) {
						ParseFor_Simple((string)identifier.Lexeme, obj);
                        return;
                    }
                }
            }
            m_iNextToken = partIndex;
            ParseFor_impl();
        }
        //解析单纯for循环
        private void ParseFor_Simple(string Identifier, CodeObject obj)
        {
            CodeForSimple ret = new CodeForSimple();
            ret.Identifier = Identifier;
            ret.Begin = obj;
            ret.Finished = GetObject();
            if (PeekToken().Type == TokenType.Comma)
            {
                ReadToken();
                ret.Step = GetObject();
            }
            ReadRightParenthesis();
            ret.BlockExecutable = ParseStatementBlock(Executable_Block.For);
            m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_FORSIMPLE, ret));
        }
        //解析正规for循环
        private void ParseFor_impl()
        {
            CodeFor ret = new CodeFor();
            Token token = ReadToken();
            if (token.Type != TokenType.SemiColon)
            {
                UndoToken();
                ret.BeginExecutable = ParseStatementBlock(Executable_Block.ForBegin, false, TokenType.SemiColon);
            }
            token = ReadToken();
            if (token.Type != TokenType.SemiColon)
            {
                UndoToken();
                ret.Condition = GetObject();
                ReadSemiColon();
            }
            token = ReadToken();
            if (token.Type != TokenType.RightPar)
            {
                UndoToken();
                ret.LoopExecutable = ParseStatementBlock(Executable_Block.ForLoop, false, TokenType.RightPar);
            }
            ret.BlockExecutable = ParseStatementBlock(Executable_Block.For);
            m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_FOR, ret));
        }
        //解析foreach语句
        private void ParseForeach()
        {
            CodeForeach ret = new CodeForeach();
            ReadLeftParenthesis();
            if (PeekToken().Type == TokenType.Var) ReadToken();
            ret.Identifier = ReadIdentifier();
            ReadIn();
            ret.LoopObject = GetObject();
            ReadRightParenthesis();
            ret.BlockExecutable = ParseStatementBlock(Executable_Block.Foreach);
            m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_FOREACH, ret));
        }
        //解析while（循环语句）
        private void ParseWhile()
        {
            CodeWhile ret = new CodeWhile();
            ret.While = ParseCondition(true, Executable_Block.While);
            m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_WHILE, ret));
        }
        //解析swtich语句
        private void ParseSwtich()
        {
            CodeSwitch ret = new CodeSwitch();
            ReadLeftParenthesis();
            ret.Condition = GetObject();
            ReadRightParenthesis();
            ReadLeftBrace();
            List<TempCase> Cases = new List<TempCase>();
            for (; ; ) {
                Token token = ReadToken();
                if (token.Type == TokenType.Case) {
                    List<CodeObject> allow = new List<CodeObject>();
                    ParseCase(allow);
                    Cases.Add(new TempCase(m_script, allow, ParseStatementBlock(Executable_Block.Switch, false, TokenType.Break)));
                } else if (token.Type == TokenType.Default) {
                    ReadColon();
                    ret.Default = new TempCase(m_script, null, ParseStatementBlock(Executable_Block.Switch, false, TokenType.Break));
                } else if (token.Type != TokenType.SemiColon) {
                    UndoToken();
                    break;
                }
            }
            ReadRightBrace();
            ret.SetCases(Cases);
            m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_SWITCH, ret));
        }
        //解析case
        private void ParseCase(List<CodeObject> allow)
        {
            allow.Add(GetObject());
            ReadColon();
            if (ReadToken().Type == TokenType.Case) {
                ParseCase(allow);
            } else {
                UndoToken();
            }
        }
        //解析try catch
        private void ParseTry()
        {
            CodeTry ret = new CodeTry();
            ret.TryExecutable = ParseStatementBlock(Executable_Block.Context);
            ReadCatch();
            ReadLeftParenthesis();
            ret.Identifier = ReadIdentifier();
            ReadRightParenthesis();
            ret.CatchExecutable = ParseStatementBlock(Executable_Block.Context);
            m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_TRY, ret));
        }
        //解析throw
        private void ParseThrow()
        {
            CodeThrow ret = new CodeThrow();
            ret.obj = GetObject();
            m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.THROW, ret));
        }
        //解析return
        private void ParseReturn()
        {
            Token peek = PeekToken();
            if (peek.Type == TokenType.RightBrace ||
                peek.Type == TokenType.SemiColon ||
                peek.Type == TokenType.Finished)
                m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.RET, null));
            else
                m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.RET, GetObject()));
        }
        //解析 #
        private void ParseSharp() {
            Token token = ReadToken();
            if (token.Type == TokenType.Define) {
                if (m_scriptExecutable.m_Block != Executable_Block.Context)
                    throw new ParserException("#define只能使用在上下文", token);
                m_script.PushDefine(ReadIdentifier());
            } else if (token.Type == TokenType.If) {
                if (m_define == null) {
                    if (IsDefine()) {
                        m_define = new DefineState(DefineType.Already);
                    } else {
                        m_define = new DefineState(DefineType.Being);
                        PopSharp();
                    }
                } else if (m_define.State == DefineType.Already) {
                    if (IsDefine()) {
                        m_Defines.Push(m_define);
                        m_define = new DefineState(DefineType.Already);
                    } else {
                        m_Defines.Push(m_define);
                        m_define = new DefineState(DefineType.Being);
                        PopSharp();
                    }
                } else if (m_define.State == DefineType.Being) {
                    m_Defines.Push(m_define);
                    m_define = new DefineState(DefineType.Break);
                    PopSharp();
                } else if (m_define.State == DefineType.Break) {
                    m_Defines.Push(m_define);
                    m_define = new DefineState(DefineType.Break);
                    PopSharp();
                }
            } else if (token.Type == TokenType.Ifndef) {
                if (m_define == null) {
                    if (!IsDefine()) {
                        m_define = new DefineState(DefineType.Already);
                    } else {
                        m_define = new DefineState(DefineType.Being);
                        PopSharp();
                    }
                } else if (m_define.State == DefineType.Already) {
                    if (!IsDefine()) {
                        m_Defines.Push(m_define);
                        m_define = new DefineState(DefineType.Already);
                    } else {
                        m_Defines.Push(m_define);
                        m_define = new DefineState(DefineType.Being);
                        PopSharp();
                    }
                } else if (m_define.State == DefineType.Being) {
                    m_Defines.Push(m_define);
                    m_define = new DefineState(DefineType.Break);
                    PopSharp();
                }
            } else if (token.Type == TokenType.ElseIf) {
                if (m_define == null) {
                    throw new ParserException("未找到#if或#ifndef", token);
                } else if (m_define.State == DefineType.Already || m_define.State == DefineType.Break) {
                    m_define.State = DefineType.Break;
                    PopSharp();
                } else if (IsDefine()) {
                    m_define.State = DefineType.Already;
                } else {
                    m_define.State = DefineType.Being;
                    PopSharp();
                }
            } else if (token.Type == TokenType.Else) {
                if (m_define == null) {
                    throw new ParserException("未找到#if或#ifndef", token);
                } else if (m_define.State == DefineType.Already || m_define.State == DefineType.Break) {
                    m_define.State = DefineType.Break;
                    PopSharp();
                } else {
                    m_define.State = DefineType.Already;
                }
            } else if (token.Type == TokenType.Endif) {
                if (m_define == null) {
                    throw new ParserException("未找到#if或#ifndef", token);
                } else if (m_Defines.Count > 0) {
                    m_define = m_Defines.Pop();
                    if (m_define.State == DefineType.Break)
                        PopSharp();
                } else {
                    m_define = null;
                }
            } else {
                throw new ParserException("#后缀不支持" + token.Type, token);
            }
        }
        private bool IsDefine() {
            return IsDefine_impl(ReadDefine());
        }
        private bool IsDefine_impl(DefineObject define) {
            bool ret = false;
            if (define is DefineString) {
                ret = m_script.ContainDefine(((DefineString)define).Define);
            } else {
                DefineOperate oper = (DefineOperate)define;
                bool left = IsDefine_impl(oper.Left);
                if (left && !oper.and) {
                    ret = true;
                } else if (!left && oper.and) {
                    ret = false;
                } else if (oper.and) {
                    ret = left && IsDefine_impl(oper.Right);
                } else {
                    ret = left || IsDefine_impl(oper.Right);
                }
            }
            if (define.Not) { ret = !ret; }
            return ret;
        }
        private DefineObject ReadDefine() {
            Stack<bool> operateStack = new Stack<bool>();
            Stack<DefineObject> objectStack = new Stack<DefineObject>();
            while (true) {
                objectStack.Push(GetOneDefine());
                if (!OperatorDefine(operateStack, objectStack))
                    break;
            }
            while (operateStack.Count > 0) {
                objectStack.Push(new DefineOperate(objectStack.Pop(), objectStack.Pop(), operateStack.Pop()));
            }
            return objectStack.Pop();
        }
        private bool OperatorDefine(Stack<bool> operateStack, Stack<DefineObject> objectStack) {
            Token peek = PeekToken();
            if (peek.Type != TokenType.And && peek.Type != TokenType.Or) { return false; }
            ReadToken();
            while (operateStack.Count > 0) {
                objectStack.Push(new DefineOperate(objectStack.Pop(), objectStack.Pop(), operateStack.Pop()));
            }
            operateStack.Push(peek.Type == TokenType.And);
            return true;
        }
        private DefineObject GetOneDefine() {
            Token token = ReadToken();
            bool not = false;
            if (token.Type == TokenType.Not) {
                not = true;
                token = ReadToken();
            }
            if (token.Type == TokenType.LeftPar) {
                var ret = ReadDefine();
                ReadRightParenthesis();
                ret.Not = not;
                return ret;
            } else if (token.Type == TokenType.Identifier) {
                return new DefineString(token.Lexeme.ToString()) { Not = not };
            } else {
                throw new ParserException("宏定义判断只支持 字符串", token);
            }
        }
        //跳过未解析宏定义 
        private void PopSharp() {
            for (;;) {
                if (ReadToken().Type == TokenType.Sharp) {
                    if (PeekToken().Type == TokenType.Define) {
                        ReadToken();
                    } else {
                        break;
                    }
                }
            }
            ParseSharp();
        }
        //解析表达式
        private void ParseExpression()
        {
            UndoToken();
            Token peek = PeekToken();
            CodeObject member = GetObject();
            if (member is CodeCallFunction) {
                m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_FUNCTION, member));
            } else if (member is CodeMember) {
                if ((member as CodeMember).Calc != CALC.NONE)
                    m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.RESOLVE, member));
                else
                    throw new ParserException("变量后缀不支持此操作符  " + PeekToken().Type, peek);
            } else if (member is CodeAssign || member is CodeEval) {
                m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.RESOLVE, member));
            } else {
                throw new ParserException("语法不支持起始符号为 " + member.GetType(), peek);
            }
        }
        //获取一个Object
        private CodeObject GetObject()
        {
            Stack<TempOperator> operateStack = new Stack<TempOperator>();
            Stack<CodeObject> objectStack = new Stack<CodeObject>();
            while (true) {
                objectStack.Push(GetOneObject());
                if (!P_Operator(operateStack, objectStack))
                    break;
            }
            while (operateStack.Count > 0) {
                objectStack.Push(new CodeOperator(objectStack.Pop(), objectStack.Pop(), operateStack.Pop().Operator, m_strBreviary, GetSourceLine()));
            }
            CodeObject ret = objectStack.Pop();
            if (ret is CodeMember) {
                CodeMember member = ret as CodeMember;
                if (member.Calc == CALC.NONE) {
                    Token token = ReadToken();
                    switch (token.Type) {
                        case TokenType.Assign:
                        case TokenType.AssignPlus:
                        case TokenType.AssignMinus:
                        case TokenType.AssignMultiply:
                        case TokenType.AssignDivide:
                        case TokenType.AssignModulo:
                        case TokenType.AssignCombine:
                        case TokenType.AssignInclusiveOr:
                        case TokenType.AssignXOR:
                        case TokenType.AssignShr:
                        case TokenType.AssignShi:
                            ret = new CodeAssign(member, GetObject(), token.Type, m_strBreviary, token.SourceLine);
                            break;
                        default:
                            UndoToken();
                            break;
                    }
                }
            }
			if (PeekToken ().Type == TokenType.QuestionMark) {
				ReadToken();
				CodeTernary ternary = new CodeTernary();
				ternary.Allow = ret;
				ternary.True = GetObject();
				ReadColon();
				ternary.False = GetObject();
				return ternary;
			}
            return ret;
        }
        //解析操作符
        private bool P_Operator(Stack<TempOperator> operateStack, Stack<CodeObject> objectStack)
        {
            TempOperator curr = TempOperator.GetOper(PeekToken().Type);
            if (curr == null) return false;
            ReadToken();
            while (operateStack.Count > 0) {
                if (operateStack.Peek().Level >= curr.Level) {
                    objectStack.Push(new CodeOperator(objectStack.Pop(), objectStack.Pop(), operateStack.Pop().Operator, m_strBreviary, GetSourceLine()));
                } else {
                    break;
                }
            }
            operateStack.Push(curr);
            return true;
        }
        //获得单一变量
        private CodeObject GetOneObject()
        {
            CodeObject ret = null;
            Token token = ReadToken();
            bool not = false;
            bool negative = false;
            CALC calc = CALC.NONE;
            if (token.Type == TokenType.Not) {
                not = true;
                token = ReadToken();
            } else if (token.Type == TokenType.Minus) {
                negative = true;
                token = ReadToken();
            } else if (token.Type == TokenType.Increment) {
                calc = CALC.PRE_INCREMENT;
                token = ReadToken();
            } else if (token.Type == TokenType.Decrement) {
                calc = CALC.PRE_DECREMENT;
                token = ReadToken();
            }
            switch (token.Type)
            {
                case TokenType.Identifier:
                    ret = new CodeMember((string)token.Lexeme);
                    break;
                case TokenType.Function:
                    UndoToken();
                    ret = new CodeFunction(ParseFunctionDeclaration(false));
                    break;
                case TokenType.LeftPar:
                    ret = new CodeRegion(GetObject());
                    ReadRightParenthesis();
                    break;
                case TokenType.LeftBracket:
                    UndoToken();
                    ret = GetArray();
                    break;
                case TokenType.LeftBrace:
                    UndoToken();
                    ret = GetTable();
                    break;
                case TokenType.Eval:
                    ret = GetEval();
                    break;
                case TokenType.Null:
                    ret = new CodeScriptObject(m_script, null);
                    break;
                case TokenType.Boolean:
                case TokenType.Number:
                case TokenType.String:
                case TokenType.SimpleString:
                    ret = new CodeScriptObject(m_script, token.Lexeme);
                    break;
                default:
                    throw new ParserException("Object起始关键字错误 ", token);
            }
            ret.StackInfo = new StackInfo(m_strBreviary, token.SourceLine);
            ret = GetVariable(ret);
            ret.Not = not;
            ret.Negative = negative;
            if (ret is CodeMember) {
                if (calc != CALC.NONE) {
                    ((CodeMember)ret).Calc = calc;
                } else {
                    Token peek = ReadToken();
                    if (peek.Type == TokenType.Increment) {
                        calc = CALC.POST_INCREMENT;
                    } else if (peek.Type == TokenType.Decrement) {
                        calc = CALC.POST_DECREMENT;
                    } else {
                        UndoToken();
                    }
                    if (calc != CALC.NONE) {
                        ((CodeMember)ret).Calc = calc;
                    }
                }
            } else if (calc != CALC.NONE) {
                throw new ParserException("++ 或者 -- 只支持变量的操作", token);
            }
            return ret;
        }
        //返回变量数据
        private CodeObject GetVariable(CodeObject parent)
        {
            CodeObject ret = parent;
            for ( ; ; ) {
                Token m = ReadToken();
                if (m.Type == TokenType.Period) {
                    string identifier = ReadIdentifier();
                    ret = new CodeMember(identifier, ret);
                } else if (m.Type == TokenType.LeftBracket) {
                    CodeObject member = GetObject();
                    ReadRightBracket();
                    if (member is CodeScriptObject) {
                        ret = new CodeMember(((CodeScriptObject)member).Object.KeyValue, ret);
                    } else {
                        ret = new CodeMember(member, ret);
                    }
                } else if (m.Type == TokenType.LeftPar) {
                    UndoToken();
                    ret = GetFunction(ret);
                } else {
                    UndoToken();
                    break;
                }
                ret.StackInfo = new StackInfo(m_strBreviary, m.SourceLine);
            }
            return ret;
        }
        //返回一个调用函数 Object
        private CodeCallFunction GetFunction(CodeObject member)
        {
            ReadLeftParenthesis();
            List<CodeObject> pars = new List<CodeObject>();
            Token token = PeekToken();
            while (token.Type != TokenType.RightPar)
            {
                pars.Add(GetObject());
                token = PeekToken();
                if (token.Type == TokenType.Comma)
                    ReadComma();
                else if (token.Type == TokenType.RightPar)
                    break;
                else
                    throw new ParserException("Comma ',' or right parenthesis ')' expected in function declararion.", token);
            }
            ReadRightParenthesis();
            return new CodeCallFunction(member, pars);
        }
        //返回数组
        private CodeArray GetArray()
        {
            ReadLeftBracket();
            Token token = PeekToken();
            CodeArray ret = new CodeArray();
            while (token.Type != TokenType.RightBracket)
            {
                if (PeekToken().Type == TokenType.RightBracket)
                    break;
                ret._Elements.Add(GetObject());
                token = PeekToken();
                if (token.Type == TokenType.Comma) {
                    ReadComma();
                } else if (token.Type == TokenType.RightBracket) {
                    break;
                } else
                    throw new ParserException("Comma ',' or right parenthesis ']' expected in array object.", token);
            }
            ReadRightBracket();
            ret.Init();
            return ret;
        }
        //返回Table数据
        private CodeTable GetTable()
        {
            CodeTable ret = new CodeTable();
            ReadLeftBrace();
            while (PeekToken().Type != TokenType.RightBrace) {
                Token token = ReadToken();
                if (token.Type == TokenType.Identifier || token.Type == TokenType.String || token.Type == TokenType.SimpleString || token.Type == TokenType.Number || 
                    token.Type == TokenType.Boolean || token.Type == TokenType.Null) {
                    Token next = ReadToken();
                    if (next.Type == TokenType.Assign || next.Type == TokenType.Colon) {
                        if (token.Type == TokenType.Null) {
                            ret._Variables.Add(new CodeTable.TableVariable(m_script.Null.KeyValue, GetObject()));
                        } else {
                            ret._Variables.Add(new CodeTable.TableVariable(token.Lexeme, GetObject()));
                        }
                        Token peek = PeekToken();
                        if (peek.Type == TokenType.Comma || peek.Type == TokenType.SemiColon) {
                            ReadToken();
                        }
                    } else {
                        throw new ParserException("Table变量赋值符号为[=]或者[:]", token);
                    }
                } else if (token.Type == TokenType.Function) {
                    UndoToken();
                    ret._Functions.Add(ParseFunctionDeclaration(true));
                } else {
                    throw new ParserException("Table开始关键字必须为[变量名称]或者[function]关键字", token);
                }
            }
            ReadRightBrace();
            ret.Init();
            return ret;
        }
        //返回执行一段字符串
        private CodeEval GetEval()
        {
            CodeEval ret = new CodeEval();
            ret.EvalObject = GetObject();
            return ret;
        }
    }
}
