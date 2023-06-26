using Scorpio.Exception;
using System;
using System.Text;
using System.Collections.Generic;
namespace Scorpio.Library {
    internal class ScorpioJsonDeserializer : IDisposable {
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
        const string WORD_BREAK = " ,:{}[]\"\t\n\r";

        private Script m_Script;
        private string m_Buffer;
        private bool m_SupportLong;         //是否支持 数字无[.]解析成long值
        private bool m_SupportIntern;       //Map的key使用string.Intern
        private int m_Index;
        private int m_Length;
        private StringBuilder m_Builder;
        public ScorpioJsonDeserializer(Script script) {
            m_Script = script;
            m_Builder = new StringBuilder();
        }

        char Read() { return m_Index == m_Length ? END_CHAR : m_Buffer[m_Index++]; }
        char Peek() { return m_Index == m_Length ? END_CHAR : m_Buffer[m_Index]; }
        public ScriptValue Parse(string buffer, bool supportLong, bool supportIntern) {
            m_Buffer = buffer;
            m_Index = 0;
            m_Length = buffer.Length;
            m_SupportLong = supportLong;
            m_SupportIntern = supportIntern;
            return ReadObject();
        }
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
                m_Builder.Clear();
                while (WORD_BREAK.IndexOf(Peek()) == -1) {
                    m_Builder.Append(Read());
                    if (Peek() == END_CHAR) {
                        return m_Builder.ToString();
                    }
                }
                return m_Builder.ToString();
            }
        }
        ScriptValue ReadObject() {
            var ch = EatWhiteSpace;
            if (ch == END_CHAR) {
                return ScriptValue.Null;
            }
            switch (ch) {
                case QUOTES:
                    return new ScriptValue(ParseString());
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
                case LEFT_BRACE: 
                    return ParseMap();
                case LEFT_BRACKET: 
                    return ParseArray();
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
            m_Builder.Clear();
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
                                m_Builder.Append((char)Convert.ToUInt16(hex.ToString(), 16));
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
                    return new ScriptValue(Convert.ToDouble(parsedLong));
                }
            }
        }
        ScriptValue ParseMap() {
            var map = m_Script.NewMapObject();
            while (true) {
                var ch = EatWhiteSpace;
                switch (ch) {
                    case RIGHT_BRACE: return new ScriptValue(map);
                    case COMMA: continue;
                    case END_CHAR:
                        throw new ExecutionException("Json解析, 未找到 map 结尾 [}]");
                    case QUOTES: {
                        var key = ParseString();
                        if (EatWhiteSpace != ':') {
                            throw new ExecutionException("Json解析, key值后必须跟 [:] 赋值");
                        }
                        map.SetValueNoReference(m_SupportIntern ? string.Intern(key) : key, ReadObject());
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
                        map.SetValueNoReference(key.Value, ReadObject());
                        break;
                    }
                    default: {
                        throw new ExecutionException("Json解析, key值 未知符号 : " + ch);
                    }
                }
            }
        }
        ScriptValue ParseArray() {
            var array = m_Script.NewArray();
            while (true) {
                var ch = EatWhiteSpace;
                switch (ch) {
                    case RIGHT_BRACKET: return new ScriptValue(array);
                    case COMMA: continue;
                    case END_CHAR:
                        throw new ExecutionException("Json解析, 未找到array结尾 ]");
                    default: {
                        --m_Index;
                        array.AddNoReference(ReadObject());
                        continue;
                    }
                }
            }
        }
        public void Dispose() {
            m_Buffer = null;
            m_Builder.Clear();
        }
    }
    internal class ScorpioJsonSerializer : IDisposable {
        internal StringBuilder m_Builder;
        private HashSet<ScriptObject> m_Recurve;
        public ScorpioJsonSerializer() {
            m_Builder = new StringBuilder();
            m_Recurve = new HashSet<ScriptObject>();
        }
        public string ToJson(ScriptValue value) {
            Serializer(value);
            return m_Builder.ToString();
        }
        public string ToJson(ScriptObject scriptObject) {
            Serializer(scriptObject);
            return m_Builder.ToString();
        }
        internal void Serializer(ScriptValue value) {
            switch (value.valueType) {
                case ScriptValue.nullValueType:
                    m_Builder.Append("null");
                    break;
                case ScriptValue.trueValueType:
                    m_Builder.Append("true");
                    break;
                case ScriptValue.falseValueType:
                    m_Builder.Append("false");
                    break;
                case ScriptValue.doubleValueType:
                    m_Builder.Append(value.doubleValue);
                    break;
                case ScriptValue.int64ValueType:
                    m_Builder.Append(value.longValue);
                    break;
                case ScriptValue.stringValueType:
                    Serializer(value.stringValue);
                    break;
                case ScriptValue.scriptValueType:
                    Serializer(value.scriptValue);
                    break;
            }
        }
        internal void Serializer(string value) {
            m_Builder.Append('\"');
            foreach (var c in value.ToCharArray()) {
                switch (c) {
                    case '"':
                        m_Builder.Append("\\\"");
                        break;
                    case '\\':
                        m_Builder.Append("\\\\");
                        break;
                    case '\b':
                        m_Builder.Append("\\b");
                        break;
                    case '\f':
                        m_Builder.Append("\\f");
                        break;
                    case '\n':
                        m_Builder.Append("\\n");
                        break;
                    case '\r':
                        m_Builder.Append("\\r");
                        break;
                    case '\t':
                        m_Builder.Append("\\t");
                        break;
                    default:
                        m_Builder.Append(c);
                        break;
                }
            }
            m_Builder.Append('\"');
        }
        internal void Serializer(ScriptObject scriptObject) {
            if (scriptObject is ScriptInstance) {
                if (!m_Recurve.Contains(scriptObject)) {
                    m_Recurve.Add(scriptObject);
                    ((ScriptInstance)scriptObject).ToJson(this);
                } else {
                    m_Builder.Append("\"Inline\"");
                }
            } else {
                Serializer(scriptObject.ToString());
            }
        }
        public void Dispose() {
            m_Builder.Clear();
            m_Recurve.Clear();
        }
    }
}
