using System.Collections;
using Scorpio.Coroutine;
using Scorpio.Tools;
namespace Scorpio.Library {
    public class LibraryCoroutine {
        public static void Load(Script script) {
            var map = script.NewMapString();
            map.SetValue("start", script.CreateFunction(new start(script)));
            map.SetValue("stop", script.CreateFunction(new stop()));
            map.SetValue("stopAll", script.CreateFunction(new stopAll(script)));
            map.SetValue("poll", script.CreateFunction(new poll(script)));
            map.SetValue("epoll", script.CreateFunction(new epoll(script)));
            map.SetValue("done", script.CreateFunction(new done()));
            script.SetGlobal("sleep", script.CreateFunction(new sleep(script)));
            script.SetGlobal("coroutine", map);
        }
        private class start : ScorpioHandle {
            readonly Script script;
            public start(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var enumerator = args[0].Value as IEnumerator;
                if (enumerator != null) {
                    return ScriptValue.CreateValue(script, script.StartCoroutine(enumerator));
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
            private Script script;
            public poll(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return ScriptValue.CreateValue(script, new CoroutinePoll(args.GetArgsThrow(0, length), args.GetArgsThrow(1, length)));
            }
        }
        private class epoll : ScorpioHandle {
            private Script script;
            public epoll(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return ScriptValue.CreateValue(script, new CoroutineEpoll());
            }
        }
        private class done : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var coroutineCallback = args[0].Value as CoroutineEpoll;
                if (coroutineCallback != null) {
                    coroutineCallback.Done(args.GetArgs(1, length));
                }
                return ScriptValue.Null;
            }
        }
        private class sleep : ScorpioHandle {
            private Script script;
            public sleep(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var start = LibraryIO.UnixNow;
                var end = start + args.GetArgsThrow(0, length).ToLong() * 1000;
                return ScriptValue.CreateValue(script, new CoroutineFuncPoll(() => {
                    return LibraryIO.UnixNow >= end;
                }));
            }
        }
    }
}
