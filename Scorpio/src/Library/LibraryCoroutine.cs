using System.Collections;
using Scorpio.Coroutine;
using Scorpio.Tools;
namespace Scorpio.Library {
    public class LibraryCoroutine {
        public static void Load(Script script) {
            var functions = new (string, ScorpioHandle)[] {
                ("start", new start(script)),
                ("stop", new stop()),
                ("stopAll", new stopAll(script)),
                ("poll", new poll()),
                ("epoll", new epoll()),
                ("done", new done()),
            };
            script.AddLibrary("coroutine", functions);
            script.SetGlobal("sleep", script.CreateFunction(new sleep()));
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
                return ScriptValue.CreateValue(new CoroutinePoll(args.GetArgsThrow(0, length), args.GetArgsThrow(1, length)));
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
                    coroutineCallback.Done(args.GetArgs(1, length));
                }
                return ScriptValue.Null;
            }
        }
        private class sleep : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var start = LibraryIO.UnixNow;
                var end = start + args.GetArgsThrow(0, length).ToLong() * 1000;
                return ScriptValue.CreateValue(new CoroutineFuncPoll(() => {
                    return LibraryIO.UnixNow >= end;
                }));
            }
        }
    }
}
