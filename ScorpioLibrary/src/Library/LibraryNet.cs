using Scorpio;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace ScorpioLibrary {
    public class LibraryNet {
        const string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        const int READ_LENGTH = 8192;
        static byte[] READ_BYTES = new byte[READ_LENGTH];
        static Encoding DEFAULT_ENCODING = Encoding.UTF8;
        public static void Load(Script script) {
            var map = new ScriptMapString(script);
            map.SetValue("get", script.CreateFunction(new get()));
            map.SetValue("post", script.CreateFunction(new post()));
            map.SetValue("urlencode", script.CreateFunction(new urlencode()));
            map.SetValue("urldecode", script.CreateFunction(new urldecode()));
            map.SetValue("qpencode", script.CreateFunction(new qpencode()));
            map.SetValue("qpdecode", script.CreateFunction(new qpdecode()));
            script.SetGlobal("net", new ScriptValue(map));
        }
        static HttpWebRequest CreateRequest(string url, string method) {
            //创建 SL/TLS 安全通道
            try {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                | (SecurityProtocolType)0x300 //Tls11
                                | (SecurityProtocolType)0xC00; //Tls12
            } catch (Exception) { }
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = method;
            request.ProtocolVersion = HttpVersion.Version10;
            request.UserAgent = DefaultUserAgent;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Timeout = 30000;                    //设定超时时间30秒
            return request;
        }
        static void ReadStream(HttpWebRequest request, Stream writer) {
            using (var response = request.GetResponse()) {
                using (var stream = response.GetResponseStream()) {
                    while (true) {
                        var readSize = stream.Read(READ_BYTES, 0, READ_LENGTH);
                        if (readSize <= 0) { break; }
                        writer.Write(READ_BYTES, 0, readSize);
                    }
                }
            }
        }
        private class get : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                var request = CreateRequest(Parameters[0].ToString(), "GET");
                if (length > 1) Parameters[1].Get<ScriptFunction>().call(ScriptValue.Null, request);
                if (length > 2) {
                    ReadStream(request, Parameters[2].Value as Stream);
                    return ScriptValue.Null;
                } else {
                    using (var stream = new MemoryStream()) {
                        ReadStream(request, stream);
                        return new ScriptValue(stream.ToArray());
                    }
                }
            }
        }
        private class post : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                var request = CreateRequest(Parameters[0].ToString(), "POST");
                if (length > 1) {
                    using (var stream = request.GetRequestStream()) {
                        var value = Parameters[1];
                        var buffer = value.valueType == ScriptValue.stringValueType ? DEFAULT_ENCODING.GetBytes(value.ToString()) : value.Value as byte[];
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
                if (length > 2 ) Parameters[2].Get<ScriptFunction>().call(ScriptValue.Null, request);
                using (var stream = new MemoryStream()) {
                    ReadStream(request, stream);
                    return new ScriptValue(stream.ToArray());
                }
            }
        }
        private class urlencode : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                return new ScriptValue(UriTranscoder.URLEncode(Parameters[0].ToString(), length > 1 ? Encoding.GetEncoding(Parameters[1].ToString()) : DEFAULT_ENCODING));
            }
        }
        private class urldecode : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                return new ScriptValue(UriTranscoder.URLDecode(Parameters[0].ToString(), length > 1 ? Encoding.GetEncoding(Parameters[1].ToString()) : DEFAULT_ENCODING));
            }
        }
        private class qpencode : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                return new ScriptValue(UriTranscoder.QPEncode(Parameters[0].ToString(), length > 1 ? Encoding.GetEncoding(Parameters[1].ToString()) : DEFAULT_ENCODING));
            }
        }
        private class qpdecode : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                return new ScriptValue(UriTranscoder.QPDecode(Parameters[0].ToString(), length > 1 ? Encoding.GetEncoding(Parameters[1].ToString()) : DEFAULT_ENCODING));
            }
        }
    }
}
