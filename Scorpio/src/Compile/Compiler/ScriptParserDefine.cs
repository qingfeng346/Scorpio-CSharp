using System.Collections.Generic;
using Scorpio.Compile.Exception;
namespace Scorpio.Compile.Compiler {
    public partial class ScriptParser {
        private class DefineObject {
            public bool Not;
        }
        private class DefineString : DefineObject {
            public string Define;
            public DefineString(string define) {
                Define = define;
            }
        }
        private class DefineOperate : DefineObject {
            public DefineObject Left;       //左边值
            public DefineObject Right;      //右边值
            public bool And;                //是否是并且操作
            public DefineOperate(DefineObject left, DefineObject right, bool and) {
                Left = left;
                Right = right;
                And = and;
            }
        }
        /// <summary> #define </summary>
        void ParseMacroDefine() {
            allDefines.Add(ReadIdentifier());
        }
        void ParseMacroIf() {
            if (!IsDefine()) {
                FindNextMacro();
            }
        }
        void ParseMacroIfndef() {
            if (IsDefine()) {
                FindNextMacro();
            }
        }
        void ParseMacroElse() {
            FindMacroEndif();
        }
        //查找下一个宏指令
        private bool FindNextMacro() {
            var index = 0;
            for (; ; ) {
                var token = ReadToken();
                if (token.Type == TokenType.MacroIf) {
                    ++index;
                } else if (token.Type == TokenType.MacroEndif) {
                    if (index == 0) {
                        return false;
                    } else {
                        --index;
                    }
                }
                if (index == 0) {
                    if (token.Type == TokenType.MacroElse) {
                        return true;
                    } else if (token.Type == TokenType.MacroElif) {
                        if (IsDefine()) {
                            return true;
                        }
                    }
                }
            }
        }
        private void FindMacroEndif() {
            var index = 0;
            for (; ; ) {
                var token = ReadToken();
                if (token.Type == TokenType.MacroIf) {
                    ++index;
                } else if (token.Type == TokenType.MacroEndif) {
                    if (index == 0) return;
                    --index;
                }
            }
        }
        private bool IsDefine() {
            return IsDefine_impl(ReadDefine());
        }
        private bool IsDefine_impl(DefineObject define) {
            bool ret;
            if (define is DefineString) {
                ret = allDefines.Contains((define as DefineString).Define);
            } else {
                DefineOperate oper = (DefineOperate)define;
                bool left = IsDefine_impl(oper.Left);
                if (left && !oper.And) {
                    ret = true;
                } else if (!left && oper.And) {
                    ret = false;
                } else if (oper.And) {
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
                throw new ParserException(this, "宏定义判断只支持字符串", token);
            }
        }
    }
}
