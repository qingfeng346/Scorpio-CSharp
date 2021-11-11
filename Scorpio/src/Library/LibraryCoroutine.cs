using System.Collections;
using Scorpio.Coroutine;
namespace Scorpio.Library {
    public class LibraryCoroutine {
        public static void Load(Script script) {
            var map = new ScriptMapString(script);
            map.SetValue("start", script.CreateFunction(new start(script)));
            map.SetValue("stop", script.CreateFunction(new stop()));
            map.SetValue("stopAll", script.CreateFunction(new stopAll(script)));
            map.SetValue("poll", script.CreateFunction(new poll()));
            map.SetValue("epoll", script.CreateFunction(new epoll()));
            map.SetValue("done", script.CreateFunction(new done()));
            script.SetGlobal("sleep", script.CreateFunction(new sleep()));
            script.SetGlobal("coroutine", new ScriptValue(map));
        }
        private class start : ScorpioHandle {
            readonly Script m_Script;
            public start(Script script) {
                m_Script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var enumerator = args[0].Value as IEnumerator;
                if (enumerator != null) {
                    return ScriptValue.CreateValue(m_Script.StartCoroutine(enumerator));
                }
                return ScriptValue.Null;
            }
        }
        private class stop : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var scriptCoroutine = args[0].Value as ScriptCoroutine;
                if (scriptCoroutine != null) {
                    scriptCoroutine.Stop();
                }
                return ScriptValue.Null;
            }
        }
        private class stopAll : ScorpioHandle {
            readonly Script m_Script;
            public stopAll(Script script) {
                m_Script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                m_Script.StopAllCoroutine();
                return ScriptValue.Null;
            }
        }
        private class poll : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return ScriptValue.CreateValue(new CoroutinePoll(args[0]));
            }
        }
        private class epoll : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return ScriptValue.CreateValue(new CoroutineEpoll());
            }
        }
        private class done : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var coroutineCallback = args[0].Value as CoroutineEpoll;
                if (coroutineCallback != null) {
                    coroutineCallback.Done();
                }
                return ScriptValue.Null;
            }
        }
        private class sleep : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var start = LibraryIO.UnixNow;
                var milliseconds = args[0].ToDouble() * 1000;
                return ScriptValue.CreateValue(new CoroutineFuncPoll(() => {
                    return LibraryIO.UnixNow - start > milliseconds;
                }));
            }
        }
    }
}
