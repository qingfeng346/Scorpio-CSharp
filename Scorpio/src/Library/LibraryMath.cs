using System;
using Scorpio.Exception;
namespace Scorpio.Library {
    public partial class LibraryMath {
        public const double PI = 3.1415926535897931;            //PI 三角函数
        public const double Deg2Rad = 0.0174532924;             //角度转弧度
        public const double Rad2Deg = 57.29578;                 //弧度转角度
        public const double Epsilon = 1.401298E-45;             //一个很小的浮点数
        public static void Load(Script script) {
            var functions = new (string, ScriptValue)[] {
                ("PI", new ScriptValue(PI)),
                ("Deg2Rad", new ScriptValue(Deg2Rad)),
                ("Rad2Deg", new ScriptValue(Rad2Deg)),
                ("Epsilon", new ScriptValue(Epsilon)),

                ("min", script.CreateFunction(new min())),
                ("max", script.CreateFunction(new max())),
                ("abs", script.CreateFunction(new abs())),
                ("floor", script.CreateFunction(new floor())),
                ("ceil", script.CreateFunction(new ceil())),
                ("round", script.CreateFunction(new round())),
                ("clamp", script.CreateFunction(new clamp())),
                ("sqrt", script.CreateFunction(new sqrt())),
                ("pow", script.CreateFunction(new pow())),
                ("log", script.CreateFunction(new log())),
                 
                ("sin", script.CreateFunction(new sin())),
                ("sinh", script.CreateFunction(new sinh())),
                ("asin", script.CreateFunction(new asin())),
                ("cos", script.CreateFunction(new cos())),
                ("cosh", script.CreateFunction(new cosh())),
                ("acos", script.CreateFunction(new acos())),
                ("tan", script.CreateFunction(new tan())),
                ("tanh", script.CreateFunction(new tanh())),
                ("atan", script.CreateFunction(new atan())),
            };
            var map = new ScriptMapString(script, functions.Length);
            foreach (var (name, func) in functions) {
                map.SetValue(name, func);
            }
            script.SetGlobal("math", new ScriptValue(map));
        }
        private class min : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 0) return ScriptValue.Zero;
                var min = args[0];
                for (var i = 1; i < length; ++i) {
                    var arg = args[i];
                    if (min.valueType == arg.valueType) {
                        switch (min.valueType) {
                            case ScriptValue.scriptValueType: {
                                if (arg.scriptValue.Less(min)) {
                                    min = arg;
                                }
                                break;
                            }
                            case ScriptValue.doubleValueType: {
                                if (arg.doubleValue < min.doubleValue) {
                                    min = arg;
                                }
                                break;
                            }
                            case ScriptValue.longValueType: {
                                if (arg.longValue < min.longValue) {
                                    min = arg;
                                }
                                break;
                            }
                            default: throw new ExecutionException($"【<】运算符不支持当前类型 : {min.ValueTypeName}");
                        }
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
                    var arg = args[i];
                    if (max.valueType == arg.valueType) {
                        switch (max.valueType) {
                            case ScriptValue.scriptValueType: {
                                if (arg.scriptValue.Greater(max)) {
                                    max = arg;
                                }
                                break;
                            }
                            case ScriptValue.doubleValueType: {
                                if (arg.doubleValue > max.doubleValue) {
                                    max = arg;
                                }
                                break;
                            }
                            case ScriptValue.longValueType: {
                                if (arg.longValue > max.longValue) {
                                    max = arg;
                                }
                                break;
                            }
                            default: throw new ExecutionException($"【>】运算符不支持当前类型 : {max.ValueTypeName}");
                        }
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
                    case ScriptValue.doubleValueType: return new ScriptValue(Math.Floor(value.doubleValue));
                    default: return value;
                }
            }
        }
        private class ceil : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var value = args[0];
                switch (value.valueType) {
                    case ScriptValue.doubleValueType: return new ScriptValue(Math.Ceiling(value.doubleValue));
                    default: return value;
                }
            }
        }
        private class round : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var value = args[0];
                var digit = length > 1 ? args[1].ToInt32() : 0;     //保留小输掉后位数
                switch (value.valueType) {
                    case ScriptValue.doubleValueType: return new ScriptValue(Math.Round(value.doubleValue, digit));
                    default: return value;
                }
            }
        }
        private class clamp : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var value = args[0];
                switch (value.valueType) {
                    case ScriptValue.doubleValueType: {
                        var min = Convert.ToDouble(args[1].Value);
                        if (value.doubleValue < min) return new ScriptValue(min);
                        var max = Convert.ToDouble(args[2].Value);
                        if (value.doubleValue > max) return new ScriptValue(max);
                        return value;
                    }
                    case ScriptValue.longValueType: {
                        var min = Convert.ToInt64(args[1].Value);
                        if (value.longValue < min) return new ScriptValue(min);
                        var max = Convert.ToInt64(args[2].Value);
                        if (value.longValue > max) return new ScriptValue(max);
                        return value;
                    }
                    default: return value;
                }
            }
        }
        private class sqrt : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Sqrt(Convert.ToDouble(args[0].Value)));
            }
        }
        private class pow : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var value = Convert.ToDouble(args[0].Value);
                var p = Convert.ToDouble(args[1].Value);
                return new ScriptValue(Math.Pow(value, p));
            }
        }
        private class log : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Log(Convert.ToDouble(args[0].Value), Convert.ToDouble(args[1].Value)));
            }
        }
        private class sin : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Sin(Convert.ToDouble(args[0].Value)));
            }
        }
        private class sinh : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Sinh(Convert.ToDouble(args[0].Value)));
            }
        }
        private class asin : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Asin(Convert.ToDouble(args[0].Value)));
            }
        }
        private class cos : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Cos(Convert.ToDouble(args[0].Value)));
            }
        }
        private class cosh : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Cosh(Convert.ToDouble(args[0].Value)));
            }
        }
        private class acos : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Acos(Convert.ToDouble(args[0].Value)));
            }
        }
        private class tan : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Tan(Convert.ToDouble(args[0].Value)));
            }
        }
        private class tanh : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Tanh(Convert.ToDouble(args[0].Value)));
            }
        }
        private class atan : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Atan(Convert.ToDouble(args[0].Value)));
            }
        }
    }
}
