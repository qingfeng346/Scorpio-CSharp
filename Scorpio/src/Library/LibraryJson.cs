﻿using System.Text;
using Scorpio.Exception;
namespace Scorpio.Library {
    public partial class LibraryJson {
        public static void Load(Script script) {
            var map = script.CreateMap();
            map.SetValue("encode", script.CreateFunction(new encode()));
            map.SetValue("decode", script.CreateFunction(new decode(script)));
            script.SetGlobal("json", new ScriptValue(map));
        }
        private class encode : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].ToJson());
            }
        }
        private class decode : ScorpioHandle {
            private Script m_Script;
            public decode(Script script) {
                m_Script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new JsonParser(args[0].ToString(), m_Script).Parse();
            }
        }
        private class JsonParser {
            const char END_CHAR = (char)0;
            const char QUOTES = '"';            //引号
            const char LEFT_BRACE = '{';        //{
            const char RIGHT_BRACE = '}';       //}
            const char LEFT_BRACKET = '[';      //[
            const char RIGHT_BRACKET = ']';     //]
            const char COMMA = ',';             //,
            const string TRUE = "true";
            const string FALSE = "false";
            const string NULL = "null";

            const string WHITE_SPACE = " \t\n\r";
            const string WORD_BREAK = " \t\n\r{}[],:\"";

            private Script m_script;
            private string m_Buffer;
            private int m_Index;
            private int m_Length;
            public JsonParser(string buffer, Script script) {
                m_Buffer = buffer;
                m_Index = 0;
                m_Length = buffer.Length;
                m_script = script;
            }
            char Read() { return m_Index == m_Length ? END_CHAR : m_Buffer[m_Index++]; }
            char Peek() { return m_Index == m_Length ? END_CHAR : m_Buffer[m_Index]; }
            public ScriptValue Parse() { return ReadObject(); }
            char EatWhiteSpace {
                get {
                    while (WHITE_SPACE.IndexOf(Peek()) != -1) {
                        ++m_Index;
                        if (Peek() == END_CHAR) {
                            return END_CHAR;
                        }
                    }
                    return Read();
                }
            }
            string NextWord {
                get {
                    var builder = new StringBuilder();
                    while (WORD_BREAK.IndexOf(Peek()) == -1) {
                        builder.Append(Read());
                        if (Peek() == END_CHAR) {
                            return builder.ToString();
                        }
                    }
                    return builder.ToString();
                }
            }
            ScriptValue ReadObject() {
                var ch = EatWhiteSpace;
                if (ch == END_CHAR) {
                    return ScriptValue.Null;
                }
                switch (ch) {
                    case QUOTES: return new ScriptValue(ParseString());
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '-':
                        --m_Index;
                        return ParseNumber();
                    case LEFT_BRACE: return ParseMap();
                    case LEFT_BRACKET: return ParseArray();
                    default:
                        --m_Index;
                        var word = NextWord;
                        switch (word) {
                            case TRUE: return ScriptValue.True;
                            case FALSE: return ScriptValue.False;
                            case NULL: return ScriptValue.Null;
                            default: throw new ExecutionException("未知标识符 : " + word);
                        }
                }
            }
            string ParseString() {
                var m_Builder = new StringBuilder();
                while (true) {
                    if (Peek() == -1) {
                        return m_Builder.ToString();
                    }
                    var ch = Read();
                    if (ch == QUOTES) {
                        return m_Builder.ToString();
                    }
                    switch (ch) {
                        case '\\':
                            ch = Read();
                            switch (ch) {
                                case '\'': m_Builder.Append('\''); break;
                                case '\"': m_Builder.Append('\"'); break;
                                case '\\': m_Builder.Append('\\'); break;
                                case 'a': m_Builder.Append('\a'); break;
                                case 'b': m_Builder.Append('\b'); break;
                                case 'f': m_Builder.Append('\f'); break;
                                case 'n': m_Builder.Append('\n'); break;
                                case 'r': m_Builder.Append('\r'); break;
                                case 't': m_Builder.Append('\t'); break;
                                case 'v': m_Builder.Append('\v'); break;
                                case '0': m_Builder.Append('\0'); break;
                                case '/': m_Builder.Append("/"); break;
                                case 'u': {
                                    var hex = new StringBuilder();
                                    for (int i = 0; i < 4; i++) {
                                        hex.Append(Read());
                                    }
                                    m_Builder.Append((char)System.Convert.ToUInt16(hex.ToString(), 16));
                                    break;
                                }
                            }
                            break;
                        default:
                            m_Builder.Append(ch);
                            break;
                    }
                }
            }
            ScriptValue ParseNumber() {
                var number = NextWord;
                if (number.IndexOf('.') == -1) {
                    long parsedLong;
                    long.TryParse(number, out parsedLong);
                    return new ScriptValue(parsedLong);
                }
                double parsedDouble;
                double.TryParse(number, out parsedDouble);
                return new ScriptValue(parsedDouble);
            }
            ScriptValue ParseMap() {
                var map = new ScriptMap(m_script);
                var ret = new ScriptValue(map);
                while (true) {
                    var ch = EatWhiteSpace;
                    switch (ch) {
                        case RIGHT_BRACE: return ret;
                        case COMMA: continue;
                        case END_CHAR:
                            throw new ExecutionException("解析json, 未找到 map 结尾 }");
                        case QUOTES:
                            var key = ParseString();
                            if (EatWhiteSpace != ':') {
                                throw new ExecutionException("解析json, key值后必须跟 : 赋值");
                            }
                            map.SetValue(key, ReadObject());
                            break;
                        default: {
                            throw new ExecutionException("解析json, 未知符号 : " + ch);
                        }
                    }
                }
            }
            ScriptValue ParseArray() {
                var array = new ScriptArray(m_script);
                var ret = new ScriptValue(array);
                while (true) {
                    var ch = EatWhiteSpace;
                    switch (ch) {
                        case RIGHT_BRACKET: return ret;
                        case COMMA: continue;
                        case END_CHAR:
                            throw new ExecutionException("解析json, 未找到array结尾 ]");
                        default: {
                            --m_Index;
                            array.Add(ReadObject());
                            continue;
                        }
                    }
                }
            }
        }
    }
}
