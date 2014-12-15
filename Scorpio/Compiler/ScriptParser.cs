using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.Exception;
using Scorpio.Runtime;
using Scorpio.CodeDom;
using Scorpio.CodeDom.Temp;
using Scorpio.Variable;
namespace Scorpio.Compiler
{
    //上下文解析
    internal partial class ScriptParser
    {
        private Script m_script;                                                        //脚本类
        private string m_strBreviary;                                                   //当前解析的脚本摘要
        private int m_iNextToken;                                                       //当前读到token
        private List<Token> m_listTokens;                                               //token列表
        private Stack<ScriptExecutable> m_Executables = new Stack<ScriptExecutable>();  //指令栈
        private ScriptExecutable m_scriptExecutable;                                    //当前指令栈
        public ScriptParser(Script script, List<Token> listTokens, string strBreviary)
        {
            m_script = script;
            m_strBreviary = strBreviary;
            m_iNextToken = 0;
            m_listTokens = new List<Token>(listTokens);
        }
        public void BeginExecutable(Executable_Block block)
        {
            m_scriptExecutable = new ScriptExecutable(m_script, block);
            m_Executables.Push(m_scriptExecutable);
        }
        public void EndExecutable()
        {
            m_Executables.Pop();
            m_scriptExecutable = (m_Executables.Count > 0) ? m_Executables.Peek() : null;
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
            if (readLeftBrace) ReadLeftBrace();
            TokenType tokenType;
            while (HasMoreTokens())
            {
                tokenType = ReadToken().Type;
                if (tokenType == finished) {
                    break;
                }
                UndoToken();
                ParseStatement();
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
            if (m_scriptExecutable.Block == Executable_Block.Context)
            {
                UndoToken();
                ScriptFunction func = ParseFunctionDeclaration(true);
                m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.MOV, new CodeMember(func.Name), new CodeFunction(func)));
            }
        }
        //解析函数（返回一个函数）
        private ScriptFunction ParseFunctionDeclaration(bool needName)
        {
            Token token = ReadToken();
            if (token.Type != TokenType.Function)
                throw new ParserException("Function declaration must start with the 'function' keyword.", token);
            String strFunctionName = needName ? ReadIdentifier() : "";
            ReadLeftParenthesis();
            List<String> listParameters = new List<String>();
            bool bParams = false;
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
            ScriptExecutable executable = ParseStatementBlock(Executable_Block.Function);
            return m_script.CreateFunction(strFunctionName, new ScorpioScriptFunction(m_script, listParameters, executable, bParams));
        }
        //解析Var关键字
        private void ParseVar()
        {
            for (; ; ) {
                m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.VAR, ReadIdentifier()));
                Token peek = PeekToken();
                if (peek.Type == TokenType.Assign) {
                    UndoToken();
                    ParseStatement();
                }
                peek = ReadToken();
                if (peek.Type != TokenType.Comma) {
                    UndoToken();
                    break;
                }
            }
        }
        //解析普通代码块 {}
        private void ParseBlock()
        {
            UndoToken();
            m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_BLOCK, new ScriptContext(m_script, ParseStatementBlock(Executable_Block.Block))));
        }
        //解析if(判断语句)
        private void ParseIf()
        {
            CodeIf ret = new CodeIf();
            ret.If = ParseCondition(true, Executable_Block.If);
            for (; ; )
            {
                Token token = ReadToken();
                if (token.Type == TokenType.ElseIf) {
                    ret.AddElseIf(ParseCondition(true, Executable_Block.If));
                } else if (token.Type == TokenType.Else) {
                    if (PeekToken().Type == TokenType.If) {
                        ReadToken();
                        ret.AddElseIf(ParseCondition(true, Executable_Block.If));
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
            m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_IF, ret));
        }
        //解析判断内容
        private TempCondition ParseCondition(bool condition, Executable_Block block)
        {
            CodeObject con = null;
            if (condition)
            {
                ReadLeftParenthesis();
                con = GetObject();
                ReadRightParenthesis();
            }
            return new TempCondition(m_script, con, ParseStatementBlock(block), block);
        }
        //解析for语句
        private void ParseFor()
        {
            ReadLeftParenthesis();
            int partIndex = m_iNextToken;
            Token token = ReadToken();
            if (token.Type == TokenType.Identifier)
            {
                Token assign = ReadToken();
                if (assign.Type == TokenType.Assign)
                {
                    CodeObject obj = GetObject();
                    Token comma = ReadToken();
                    if (comma.Type == TokenType.Comma)
                    {
                        ParseFor_Simple((string)token.Lexeme, obj);
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
            CodeForSimple ret = new CodeForSimple(m_script);
            ret.Identifier = Identifier;
            ret.Begin = obj;
            ret.Finished = GetObject();
            if (PeekToken().Type == TokenType.Comma)
            {
                ReadToken();
                ret.Step = GetObject();
            }
            ReadRightParenthesis();
            ret.SetContextExecutable(ParseStatementBlock(Executable_Block.For));
            m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_FORSIMPLE, ret));
        }
        //解析正规for循环
        private void ParseFor_impl()
        {
            CodeFor ret = new CodeFor(m_script);
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
            ret.SetContextExecutable(ParseStatementBlock(Executable_Block.For));
            m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_FOR, ret));
        }
        //解析foreach语句
        private void ParseForeach()
        {
            CodeForeach ret = new CodeForeach();
            ReadLeftParenthesis();
            ret.Identifier = ReadIdentifier();
            ReadIn();
            ret.LoopObject = GetObject();
            ReadRightParenthesis();
            ret.Context = new ScriptContext(m_script, ParseStatementBlock(Executable_Block.Foreach), null, Executable_Block.Foreach);
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
            for (; ; )
            {
                Token token = ReadToken();
                if (token.Type == TokenType.Case) {
                    List<object> vals = new List<object>();
                    ParseCase(vals);
                    ret.AddCase(new TempCase(m_script, vals, ParseStatementBlock(Executable_Block.Switch, false, TokenType.Break), Executable_Block.Switch));
                } else if (token.Type == TokenType.Default) {
                    ReadColon();
                    ret.Default = new TempCase(m_script, null, ParseStatementBlock(Executable_Block.Switch, false, TokenType.Break), Executable_Block.Switch);
                } else if (token.Type != TokenType.SemiColon) {
                    UndoToken();
                    break;
                }
            }
            ReadRightBrace();
            m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_SWITCH, ret));
        }
        //解析case
        private void ParseCase(List<object> vals)
        {
            Token val = ReadToken();
            if (val.Type == TokenType.String || val.Type == TokenType.Number)
                vals.Add(val.Lexeme);
            else
                throw new ParserException("case 语句 只支持 string和number类型");
            ReadColon();
            if (ReadToken().Type == TokenType.Case) {
                ParseCase(vals);
            } else {
                UndoToken();
            }
        }
        //解析try catch
        private void ParseTry()
        {
            CodeTry ret = new CodeTry();
            {
                ScriptExecutable exec = ParseStatementBlock(Executable_Block.Context);
                ret.TryContext = new ScriptContext(m_script, exec);
            }
            {
                ReadCatch();
                ReadLeftParenthesis();
                ret.Identifier = ReadIdentifier();
                ReadRightParenthesis();
                ScriptExecutable exec = ParseStatementBlock(Executable_Block.Context);
                ret.CatchContext = new ScriptContext(m_script, exec);
            }
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
                m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.RET, new CodeScriptObject(m_script, null)));
            else
                m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.RET, GetObject()));
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
            while (true)
            {
                objectStack.Push(GetOneObject());
                if (!P_Operator(operateStack, objectStack))
                    break;
            }
            while (true)
            {
                if (operateStack.Count <= 0)
                    break;
                TempOperator oper = operateStack.Pop();
                CodeOperator binexp = new CodeOperator(objectStack.Pop(), objectStack.Pop(), oper.Operator);
                objectStack.Push(binexp);
            }
            CodeObject ret = objectStack.Pop();
            if (ret is CodeMember)
            {
                CodeMember member = ret as CodeMember;
                if (member.Calc == CALC.NONE)
                {
                    Token token = ReadToken();
                    switch (token.Type)
                    {
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
                            return new CodeAssign(member, GetObject(), token.Type, m_strBreviary, token.SourceLine);
                        default:
                            UndoToken();
                            break;
                    }
                }
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
                TempOperator oper = operateStack.Peek();
                if (oper.Level >= curr.Level) {
                    operateStack.Pop();
                    CodeOperator binexp = new CodeOperator(objectStack.Pop(), objectStack.Pop(), oper.Operator);
                    objectStack.Push(binexp);
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
                    ret = GetObject();
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
                case TokenType.Boolean:
                case TokenType.Number:
                case TokenType.String:
                    ret = new CodeScriptObject(m_script, token.Lexeme);
                    break;
                default:
                    throw new ParserException("Object起始关键字错误 ", token);
            }
            ret.StackInfo = new StackInfo(m_strBreviary, token.SourceLine);
            ret = GetVariable(ret);
            ret.Not = not;
            ret = GetTernary(ret);
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
                        ScriptObject obj = ((CodeScriptObject)member).Object;
                        if (obj is ScriptNumber)
                            ret = new CodeMember((ScriptNumber)obj, ret);
                        else if (obj is ScriptString)
                            ret = new CodeMember(((ScriptString)obj).Value, ret);
                        else
                            throw new ParserException("获取变量只能是 number或string", m);
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
            }
            return ret;
        }
        //返回三元运算符
        private CodeObject GetTernary(CodeObject parent)
        {
            if (PeekToken().Type == TokenType.QuestionMark)
            {
                CodeTernary ret = new CodeTernary();
                ret.Allow = parent;
                ReadToken();
                ret.True = GetObject();
                ReadColon();
                ret.False = GetObject();
                return ret;
            }
            return parent;
        }
        //返回一个调用函数 Object
        private CodeCallFunction GetFunction(CodeObject member)
        {
            CodeCallFunction ret = new CodeCallFunction();
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
            ret.Member = member;
            ret.Parameters = pars;
            return ret;
        }
        //返回数组
        private CodeArray GetArray()
        {
            ReadLeftBracket();
            Token token = PeekToken();
            CodeArray ret = new CodeArray();
            while (token.Type != TokenType.RightBracket)
            {
                ret.Elements.Add(GetObject());
                token = PeekToken();
                if (token.Type == TokenType.Comma)
                    ReadComma();
                else if (token.Type == TokenType.RightBracket)
                    break;
                else
                    throw new ParserException("Comma ',' or right parenthesis ']' expected in array object.", token);
            }
            ReadRightBracket();
            return ret;
        }
        //返回Table数据
        private CodeTable GetTable()
        {
            CodeTable ret = new CodeTable();
            ReadLeftBrace();
            while (PeekToken().Type != TokenType.RightBrace)
            {
                Token token = ReadToken();
                if (token.Type == TokenType.Identifier || token.Type == TokenType.String || token.Type == TokenType.Number) {
                    Token next = ReadToken();
                    if (next.Type == TokenType.Assign || next.Type == TokenType.Colon) {
                        if (token.Lexeme is double)
                            ret.Variables.Add(new TableVariable(Util.ToInt32(token.Lexeme), GetObject()));
                        else
                            ret.Variables.Add(new TableVariable(token.Lexeme, GetObject()));
                        Token peek = PeekToken();
                        if (peek.Type == TokenType.Comma || peek.Type == TokenType.SemiColon) {
                            ReadToken();
                        }
                    } else {
                        throw new ParserException("Table变量赋值符号为[=]或者[:]", token);
                    }
                } else if (token.Type == TokenType.Function) {
                    UndoToken();
                    ret.Functions.Add(ParseFunctionDeclaration(true));
                } else {
                    throw new ParserException("Table开始关键字必须为 变量名称或者function关键字", token);
                }
            }
            ReadRightBrace();
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
