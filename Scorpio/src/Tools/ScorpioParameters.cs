using System;
using System.Collections.Generic;

namespace Scorpio.Tools {
    public static class ScorpioParameters {
        public class Parameter : IDisposable {
            public ScriptValue[] values;
            public int length;
            public ScriptValue this[int index] {
                set {
                    values[index].CopyFrom(value);
                }
            }
            public Parameter() {
                values = new ScriptValue[128];
            }
            public void Dispose() {
                Array.ForEach(values, _ => _.Free());
                Free(this);
            }
        }
        private static Queue<Parameter> pool = new Queue<Parameter>();
        public static Parameter Get() {
            if (pool.Count == 0) {
                return new Parameter();
            }
            return pool.Dequeue();
        }
        public static void Free(Parameter parameter) {
            pool.Enqueue(parameter);
        }
    }
}
