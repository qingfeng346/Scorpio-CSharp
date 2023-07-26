using System;
using Scorpio.Exception;

namespace Scorpio.Library {
    public partial class LibraryMath {
        public const double PI = 3.1415926535897931;            //PI 三角函数
        public const double Deg2Rad = 0.0174532924;             //角度转弧度
        public const double Rad2Deg = 57.29578;                 //弧度转角度
        public const double Epsilon = 1.401298E-45;             //一个很小的浮点数
        public static void Load(Script script) {
            var functions = new (string, ScorpioHandle)[] {
                ("min", new min()),
                ("max", new max()),
                ("abs", new abs()),
                ("floor", new floor()),
                ("ceil", new ceil()),
                ("round", new round()),
                ("clamp", new clamp()),
                ("sqrt", new sqrt()),
                ("pow", new pow()),
                ("log", new log()),
                ("sin", new sin()),
                ("sinh", new sinh()),
                ("asin", new asin()),
                ("cos", new cos()),
                ("cosh", new cosh()),
                ("acos", new acos()),
                ("tan", new tan()),
                ("tanh", new tanh()),
                ("atan", new atan()),
            };
            var map = script.AddLibrary("math", functions, 4);
            map.SetValueNoReference("PI", new ScriptValue(PI));
            map.SetValueNoReference("Deg2Rad", new ScriptValue(Deg2Rad));              //角度转弧度 角度*此值=弧度
            map.SetValueNoReference("Rad2Deg", new ScriptValue(Rad2Deg));              //弧度转角度 弧度*此值=角度
            map.SetValueNoReference("Epsilon", new ScriptValue(Epsilon));              //一个很小的浮点数
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
                                if (arg.GetScriptValue.Less(min)) {
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
                            case ScriptValue.int64ValueType: {
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
                                if (arg.GetScriptValue.Greater(max)) {
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
                            case ScriptValue.int64ValueType: {
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
                    case ScriptValue.int64ValueType: return new ScriptValue(value.longValue >= 0 ? value.longValue : -value.longValue);
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
                        var min = Convert.ToDouble(args[1].GetValue);
                        if (value.doubleValue < min) return new ScriptValue(min);
                        var max = Convert.ToDouble(args[2].GetValue);
                        if (value.doubleValue > max) return new ScriptValue(max);
                        return value;
                    }
                    case ScriptValue.int64ValueType: {
                        var min = Convert.ToInt64(args[1].GetValue);
                        if (value.longValue < min) return new ScriptValue(min);
                        var max = Convert.ToInt64(args[2].GetValue);
                        if (value.longValue > max) return new ScriptValue(max);
                        return value;
                    }
                    default: return value;
                }
            }
        }
        private class sqrt : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Sqrt(Convert.ToDouble(args[0].GetValue)));
            }
        }
        private class pow : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var value = Convert.ToDouble(args[0].GetValue);
                var p = Convert.ToDouble(args[1].GetValue);
                return new ScriptValue(Math.Pow(value, p));
            }
        }
        private class log : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Log(Convert.ToDouble(args[0].GetValue), Convert.ToDouble(args[1].GetValue)));
            }
        }
        private class sin : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Sin(Convert.ToDouble(args[0].GetValue)));
            }
        }
        private class sinh : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Sinh(Convert.ToDouble(args[0].GetValue)));
            }
        }
        private class asin : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Asin(Convert.ToDouble(args[0].GetValue)));
            }
        }
        private class cos : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Cos(Convert.ToDouble(args[0].GetValue)));
            }
        }
        private class cosh : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Cosh(Convert.ToDouble(args[0].GetValue)));
            }
        }
        private class acos : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Acos(Convert.ToDouble(args[0].GetValue)));
            }
        }
        private class tan : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Tan(Convert.ToDouble(args[0].GetValue)));
            }
        }
        private class tanh : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Tanh(Convert.ToDouble(args[0].GetValue)));
            }
        }
        private class atan : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Math.Atan(Convert.ToDouble(args[0].GetValue)));
            }
        }
    }
}
