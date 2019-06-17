using System;
using System.Collections.Generic;
using System.Text;

public class JsonParser {
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

    private string m_Buffer;
    private bool m_SupportLong;         //是否支持 数字无[.]解析成long值
    private int m_Index;
    private int m_Length;
    public JsonParser(string buffer, bool supportLong) {
        m_SupportLong = supportLong;
        m_Buffer = buffer;
        m_Index = 0;
        m_Length = buffer.Length;
    }
    char Read() { return m_Index == m_Length ? END_CHAR : m_Buffer[m_Index++]; }
    char Peek() { return m_Index == m_Length ? END_CHAR : m_Buffer[m_Index]; }
    public object Parse() { return ReadObject(); }
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
    object ReadObject() {
        var ch = EatWhiteSpace;
        if (ch == END_CHAR) {
            return null;
        }
        switch (ch) {
            case QUOTES: return ParseString();
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
                    case TRUE: return true;
                    case FALSE: return false;
                    case NULL: return null;
                    default: throw new Exception("Json解析, 未知标识符 : " + word);
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
    object ParseNumber() {
        var number = NextWord;
        if (m_SupportLong && number.IndexOf('.') == -1) {
            long parsedLong;
            long.TryParse(number, out parsedLong);
            return parsedLong;
        }
        double parsedDouble;
        double.TryParse(number, out parsedDouble);
        return parsedDouble;
    }
    Dictionary<string, object> ParseMap() {
        var map = new Dictionary<string, object>();
        while (true) {
            var ch = EatWhiteSpace;
            switch (ch) {
                case RIGHT_BRACE: return map;
                case COMMA: continue;
                case END_CHAR:
                    throw new Exception("Json解析, 未找到 map 结尾 [}]");
                case QUOTES: {
                    var key = ParseString();
                    if (EatWhiteSpace != ':') {
                        throw new Exception("Json解析, key值后必须跟 [:] 赋值");
                    }
                    map[key] = ReadObject();
                    break;
                }
                default: {
                    throw new Exception("Json解析, key值 未知符号 : " + ch);
                }
            }
        }
    }
    List<object> ParseArray() {
        var array = new List<object>();
        while (true) {
            var ch = EatWhiteSpace;
            switch (ch) {
                case RIGHT_BRACKET: return array;
                case COMMA: continue;
                case END_CHAR:
                    throw new Exception("Json解析, 未找到array结尾 ]");
                default: {
                    --m_Index;
                    array.Add(ReadObject());
                    continue;
                }
            }
        }
    }
}
