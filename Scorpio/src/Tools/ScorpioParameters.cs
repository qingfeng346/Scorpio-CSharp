using System;
using System.Collections.Generic;

namespace Scorpio.Tools {
    public static class ScorpioParameters {
        public const int Length = 128;
        public unsafe class Parameter : IDisposable {
            public ScriptValue[] values;
            public ScriptValue this[int index] {
                set {
                    values[index].CopyFrom(value);
                }
            }
            public Parameter() {
                values = new ScriptValue[Length];
            }
            public unsafe void Dispose() {
                fixed (ScriptValue* ptr = values) {
                    for (var i = 0; i < Length; ++i) {
                        (ptr + i)->Free();
                    }
                }
                Free(this);
            }
        }
        private static Stack<Parameter> pool = new Stack<Parameter>();
        public static Parameter Get() {
            if (pool.Count == 0) {
                return new Parameter();
            }
            return pool.Pop();
        }
        public static void Free(Parameter parameter) {
            pool.Push(parameter);
        }
    }
}
