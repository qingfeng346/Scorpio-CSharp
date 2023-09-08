using Scorpio.Exception;
using System;
using System.Text;
namespace Scorpio.Library {
    internal class ScorpioJsonDeserializer2 : IDisposable {
        private ScriptValue[] CacheValues = new ScriptValue[8192];
        private int CacheLength = 8192;
        private int CacheIndex = 0;
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
        public ScorpioJsonDeserializer2(Script script) {
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
            CacheIndex = 0;
            try {
                ReadObject();
                return CacheValues[--CacheIndex];
            } finally {
                Array.Clear(CacheValues, 0, CacheLength);
            }
        }
        char EatWhiteSpace {
            get {
                while (WHITE_SPACE.IndexOf(Peek()) != -1) {
                    if (++m_Index == m_Length) {
                        return END_CHAR;
                    }
                }
                return Read();
            }
        }
        string NextWord {
            get {
                m_Builder.Length = 0;
                while (WORD_BREAK.IndexOf(Peek()) == -1) {
                    m_Builder.Append(Read());
                    if (m_Index == m_Length) {
                        return m_Builder.ToString();
                    }
                }
                return m_Builder.ToString();
            }
        }
        void AddNull() {
            if (CacheIndex == CacheLength) {
                CacheLength *= 2;
                Array.Resize(ref CacheValues, CacheLength);
            }
            CacheValues[CacheIndex++].valueType = ScriptValue.nullValueType;
        }
        void AddTrue() {
            if (CacheIndex == CacheLength) {
                CacheLength *= 2;
                Array.Resize(ref CacheValues, CacheLength);
            }
            CacheValues[CacheIndex++].valueType = ScriptValue.trueValueType;
        }
        void AddFalse() {
            if (CacheIndex == CacheLength) {
                CacheLength *= 2;
                Array.Resize(ref CacheValues, CacheLength);
            }
            CacheValues[CacheIndex++].valueType = ScriptValue.falseValueType;
        }
        void AddString(string value) {
            if (CacheIndex == CacheLength) {
                CacheLength *= 2;
                Array.Resize(ref CacheValues, CacheLength);
            }
            CacheValues[CacheIndex].valueType = ScriptValue.stringValueType;
            CacheValues[CacheIndex++].stringValue = value;
        }
        void AddDouble(double value) {
            if (CacheIndex == CacheLength) {
                CacheLength *= 2;
                Array.Resize(ref CacheValues, CacheLength);
            }
            CacheValues[CacheIndex].valueType = ScriptValue.doubleValueType;
            CacheValues[CacheIndex++].doubleValue = value;
        }
        void AddLong(long value) {
            if (CacheIndex == CacheLength) {
                CacheLength *= 2;
                Array.Resize(ref CacheValues, CacheLength);
            }
            CacheValues[CacheIndex].valueType = ScriptValue.longValueType;
            CacheValues[CacheIndex++].longValue = value;
        }
        void AddScriptValue(ScriptObject value) {
            if (CacheIndex == CacheLength) {
                CacheLength *= 2;
                Array.Resize(ref CacheValues, CacheLength);
            }
            CacheValues[CacheIndex].valueType = ScriptValue.scriptValueType;
            CacheValues[CacheIndex++].scriptValue = value;
        }
        void ReadObject() {
            var ch = EatWhiteSpace;
            if (ch == END_CHAR) {
                AddNull();
                return;
            }
            switch (ch) {
                case QUOTES:
                    AddString(ParseString());
                    return;
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
                    ParseNumber();
                    return;
                case LEFT_BRACE:
                    ParseMap();
                    return;
                case LEFT_BRACKET:
                    ParseArray();
                    return;
                default:
                    --m_Index;
                    var word = NextWord;
                    switch (word) {
                        case TRUE:
                            AddTrue();
                            return;
                        case FALSE:
                            AddFalse();
                            return;
                        case NULL:
                            AddNull();
                            return;
                        default: throw new ExecutionException("Json解析, 未知标识符 : " + word);
                    }
            }
        }
        string ParseString() {
            m_Builder.Length = 0;
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
        void ParseNumber() {
            var number = NextWord;
            if (number.IndexOf('.') >= 0) {
                AddDouble(double.Parse(number));
            } else {
                var parsedLong = long.Parse(number);
                if (m_SupportLong || parsedLong < MinInt || parsedLong > MaxInt) {
                    AddLong(parsedLong);
                } else {
                    AddDouble(parsedLong);
                }
            }
        }
        void ParseMap() {
            var map = new ScriptMapObject(m_Script);
            while (true) {
                var ch = EatWhiteSpace;
                switch (ch) {
                    case RIGHT_BRACE:
                        AddScriptValue(map);
                        return;
                    case COMMA: continue;
                    case END_CHAR:
                        throw new ExecutionException("Json解析, 未找到 map 结尾 [}]");
                    case QUOTES:
                        var key = ParseString();
                        if (EatWhiteSpace != ':') {
                            throw new ExecutionException("Json解析, key值后必须跟 [:] 赋值");
                        }
                        ReadObject();
                        map.SetValue(key, CacheValues[--CacheIndex]);
                        break;
                    //case '0':
                    //case '1':
                    //case '2':
                    //case '3':
                    //case '4':
                    //case '5':
                    //case '6':
                    //case '7':
                    //case '8':
                    //case '9':
                    //case '-': {
                    //        --m_Index;
                    //        var key = ParseNumber();
                    //        if (EatWhiteSpace != ':') {
                    //            throw new ExecutionException("Json解析, key值后必须跟 [:] 赋值");
                    //        }
                    //        map.SetValue(key.Value, ReadObject());
                    //        break;
                    //    }
                    default: throw new ExecutionException("Json解析, key值 未知符号 : " + ch);
                }
            }
        }
        void ParseArray() {
            var array = new ScriptArray(m_Script);
            var index = CacheIndex;
            while (true) {
                var ch = EatWhiteSpace;
                switch (ch) {
                    case RIGHT_BRACKET:
                        array.SetArrayCapacity(CacheIndex - index);
                        for (var i = index; i <= CacheIndex; ++i) {
                            array.Add(CacheValues[i]);
                        }
                        CacheIndex = index;
                        AddScriptValue(array);
                        return;
                    case COMMA: continue;
                    case END_CHAR:
                        throw new ExecutionException("Json解析, 未找到array结尾 ]");
                    default:
                        --m_Index;
                        ReadObject();
                        continue;
                }
            }
        }
        public void Dispose() {
            m_Buffer = null;
            m_Builder.Clear();
        }
    }
}
