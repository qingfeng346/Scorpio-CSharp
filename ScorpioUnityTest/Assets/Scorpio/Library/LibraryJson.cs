using System.Text;
using Scorpio.Exception;
namespace Scorpio.Library {
    public partial class LibraryJson {
        public static void Load(Script script) {
            var map = new ScriptMap(script);
            map.SetValue("encode", script.CreateFunction(new encode()));
            map.SetValue("decode", script.CreateFunction(new decode(script)));
            script.SetGlobal("json", new ScriptValue(map));
        }
        private class encode : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].ToJson(length > 1 ? args[1].IsTrue : false));
            }
        }
        private class decode : ScorpioHandle {
            private readonly Script m_Script;
            public decode(Script script) {
                m_Script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new JsonParser(m_Script, args[0].ToString(), length > 1 ? args[1].IsTrue : true).Parse();
            }
        }
        private class JsonParser {
            const long MinInt = int.MinValue;
            const long MaxInt = int.MaxValue;
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

            private readonly Script m_Script;
            private readonly string m_Buffer;
            private readonly bool m_SupportLong;         //是否支持 数字无[.]解析成long值
            private int m_Index;
            private readonly int m_Length;
            public JsonParser(Script script, string buffer, bool supportLong) {
                m_Script = script;
                m_SupportLong = supportLong;
                m_Buffer = buffer;
                m_Index = 0;
                m_Length = buffer.Length;
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
                            default: throw new ExecutionException("Json解析, 未知标识符 : " + word);
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
                var length = number.Length - 1;
                if (number.IndexOf('.') >= 0) {
                    return new ScriptValue(double.Parse(number));
                } else if (number[length] == 'L' || number[length] == 'N') {
                    return new ScriptValue(long.Parse(number.Substring(0, length)));
                } else {
                    var parsedLong = long.Parse(number);
                    if (m_SupportLong || parsedLong < MinInt || parsedLong > MaxInt) {
                        return new ScriptValue(parsedLong);
                    } else {
                        return new ScriptValue(System.Convert.ToDouble(parsedLong));
                    }
                }
            }
            ScriptValue ParseMap() {
                var map = new ScriptMap(m_Script);
                var ret = new ScriptValue(map);
                while (true) {
                    var ch = EatWhiteSpace;
                    switch (ch) {
                        case RIGHT_BRACE: return ret;
                        case COMMA: continue;
                        case END_CHAR:
                            throw new ExecutionException("Json解析, 未找到 map 结尾 [}]");
                        case QUOTES: {
                            var key = ParseString();
                            if (EatWhiteSpace != ':') {
                                throw new ExecutionException("Json解析, key值后必须跟 [:] 赋值");
                            }
                            map.SetValue(key, ReadObject());
                            break;
                        }
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
                        case '-': {
                            --m_Index;
                            var key = ParseNumber();
                            if (EatWhiteSpace != ':') {
                                throw new ExecutionException("Json解析, key值后必须跟 [:] 赋值");
                            }
                            map.SetValue(key, ReadObject());
                            break;
                        }
                        default: {
                            throw new ExecutionException("Json解析, key值 未知符号 : " + ch);
                        }
                    }
                }
            }
            ScriptValue ParseArray() {
                var array = new ScriptArray(m_Script);
                var ret = new ScriptValue(array);
                while (true) {
                    var ch = EatWhiteSpace;
                    switch (ch) {
                        case RIGHT_BRACKET: return ret;
                        case COMMA: continue;
                        case END_CHAR:
                            throw new ExecutionException("Json解析, 未找到array结尾 ]");
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
