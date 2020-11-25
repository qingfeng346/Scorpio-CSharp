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
            var map = new ScriptMap(script);
            map.SetValue("get", script.CreateFunction(new get()));
            map.SetValue("post", script.CreateFunction(new post()));
            map.SetValue("urlencode", script.CreateFunction(new urlencode()));
            map.SetValue("urldecode", script.CreateFunction(new urldecode()));
            map.SetValue("qpencode", script.CreateFunction(new qpencode()));
            map.SetValue("qpdecode", script.CreateFunction(new qpdecode()));
            script.SetGlobal("net", new ScriptValue(map));
        }
        private class get : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                //创建 SL/TLS 安全通道
                try {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                    | (SecurityProtocolType)0x300 //Tls11
                                    | (SecurityProtocolType)0xC00; //Tls12
                } catch (Exception) { }
                var request = (HttpWebRequest)HttpWebRequest.Create(Parameters[0].ToString());
                request.Method = "GET";
                request.ProtocolVersion = HttpVersion.Version10;
                request.UserAgent = DefaultUserAgent;
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Timeout = 30000;                    //设定超时时间30秒
                var post = Parameters.Length > 1 ? Parameters[1].Get<ScriptFunction>() : null;
                if (post != null) post.call(ScriptValue.Null, request);
                using (var response = request.GetResponse()) {
                    using (var stream = response.GetResponseStream()) {
                        using (var memoryStream = new MemoryStream()) {
                            while (true) {
                                var readSize = stream.Read(READ_BYTES, 0, READ_LENGTH);
                                if (readSize <= 0) { break; }
                                memoryStream.Write(READ_BYTES, 0, readSize);
                            }
                            return new ScriptValue(memoryStream.ToArray());
                        }
                    }
                }
            }
        }
        private class post : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                //创建 SL/TLS 安全通道
                try {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                    | (SecurityProtocolType)0x300 //Tls11
                                    | (SecurityProtocolType)0xC00; //Tls12
                } catch (Exception) { }
                var request = (HttpWebRequest)HttpWebRequest.Create(Parameters[0].ToString());
                request.Method = "POST";
                request.ProtocolVersion = HttpVersion.Version10;
                request.UserAgent = DefaultUserAgent;
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Timeout = 30000;                    //设定超时时间30秒
                var buffer = Parameters.Length > 1 ? Parameters[1].Value as byte[] : null;
                if (buffer != null) {
                    using (var stream = request.GetRequestStream()) {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
                var post = Parameters.Length > 2 ? Parameters[2].Get<ScriptFunction>() : null;
                if (post != null) post.call(ScriptValue.Null, request);
                using (var response = request.GetResponse()) {
                    using (var stream = response.GetResponseStream()) {
                        using (var memoryStream = new MemoryStream()) {
                            while (true) {
                                var readSize = stream.Read(READ_BYTES, 0, READ_LENGTH);
                                if (readSize <= 0) { break; }
                                memoryStream.Write(READ_BYTES, 0, readSize);
                            }
                            return new ScriptValue(memoryStream.ToArray());
                        }
                    }
                }
            }
        }
        private class urlencode : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                return new ScriptValue(UriTranscoder.URLEncode(Parameters[0].ToString(), Parameters.Length > 1 ? Encoding.GetEncoding(Parameters[1].ToString()) : DEFAULT_ENCODING));
            }
        }
        private class urldecode : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                return new ScriptValue(UriTranscoder.URLDecode(Parameters[0].ToString(), Parameters.Length > 1 ? Encoding.GetEncoding(Parameters[1].ToString()) : DEFAULT_ENCODING));
            }
        }
        private class qpencode : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                return new ScriptValue(UriTranscoder.QPEncode(Parameters[0].ToString(), Parameters.Length > 1 ? Encoding.GetEncoding(Parameters[1].ToString()) : DEFAULT_ENCODING));
            }
        }
        private class qpdecode : ScorpioHandle {
            public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
                return new ScriptValue(UriTranscoder.QPDecode(Parameters[0].ToString(), Parameters.Length > 1 ? Encoding.GetEncoding(Parameters[1].ToString()) : DEFAULT_ENCODING));
            }
        }
    }
}
