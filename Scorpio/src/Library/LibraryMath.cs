namespace Scorpio.Library {
    public class LibraryMath {
        public const double PI = 3.1415926535897931;
        public const double Deg2Rad = 0.0174532924;
        public const double Rad2Deg = 57.29578;
        public const double Epsilon = 1.401298E-45;
        public static void Load(Script script) {
            var map = script.CreateMap();
            map.SetValue("PI", new ScriptValue(PI));
            map.SetValue("Deg2Rad", new ScriptValue(Deg2Rad));
            map.SetValue("Rad2Deg", new ScriptValue(Rad2Deg));
            map.SetValue("Epsilon", new ScriptValue(Epsilon));
            map.SetValue("min", script.CreateFunction(new min()));
            map.SetValue("max", script.CreateFunction(new max()));
            map.SetValue("abs", script.CreateFunction(new abs()));
            map.SetValue("floor", script.CreateFunction(new floor()));
            map.SetValue("clamp", script.CreateFunction(new clamp()));
            map.SetValue("sqrt", script.CreateFunction(new sqrt()));
            map.SetValue("pow", script.CreateFunction(new pow()));
            script.SetGlobal("math", new ScriptValue(map));
        }
        private class min : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 0) return ScriptValue.Zero;
                var min = args[0];
                for (var i = 1; i < length; ++i) {
                    if (args[i].Less(min)) {
                        min = args[i];
                    }
                }
                return min;
            }
        }
        private class max : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 0) return ScriptValue.Zero;
                var max = args[0];
                for (var i = 1; i < length; ++i) {
                    if (args[i].Greater(max)) {
                        max = args[i];
                    }
                }
                return max;
            }
        }
        private class abs : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var value = args[0];
                switch (value.valueType) {
                    case ScriptValue.doubleValueType: return new ScriptValue(value.doubleValue >= 0 ? value.doubleValue : -value.doubleValue);
                    case ScriptValue.longValueType: return new ScriptValue(value.longValue >= 0 ? value.longValue : -value.longValue);
                    default: return value;
                }
            }
        }
        private class floor : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var value = args[0];
                switch (value.valueType) {
                    case ScriptValue.doubleValueType: return new ScriptValue(System.Math.Floor(value.doubleValue));
                    default: return value;
                }
            }
        }
        private class clamp : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var value = args[0];
                switch (value.valueType) {
                    case ScriptValue.doubleValueType: {
                        var min = System.Convert.ToDouble(args[1].Value);
                        if (value.doubleValue < min) return new ScriptValue(min);
                        var max = System.Convert.ToDouble(args[2].Value);
                        if (value.doubleValue > max) return new ScriptValue(max);
                        return value;
                    }
                    case ScriptValue.longValueType: {
                        var min = System.Convert.ToInt64(args[1].Value);
                        if (value.longValue < min) return new ScriptValue(min);
                        var max = System.Convert.ToInt64(args[2].Value);
                        if (value.longValue > max) return new ScriptValue(max);
                        return value;
                    }
                    default: return value;
                }
            }
        }
        private class sqrt : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(System.Math.Sqrt(System.Convert.ToDouble(args[0].Value)));
            }
        }
        private class pow : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var value = System.Convert.ToDouble(args[0].Value);
                var p = System.Convert.ToDouble(args[1].Value);
                return new ScriptValue(System.Math.Pow(value, p));
            }
        }
    }
}
