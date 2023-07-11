namespace Scorpio.Proto
{
    public class ProtoObject {
        public static ScriptType Load(Script script, ScriptType ret) {
            var functions = new (string, ScorpioHandle)[] {
                ("toString", new toString()),
                ("getHashCode", new getHashCode()),
                ("instanceOf", new instanceOf(script)),
                ("referenceEquals", new referenceEquals()),
                ("addGetProperty", new addGetProperty()),
            };
            ret.SetFunctions(script, functions);
            return ret;
        }
        private class toString : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(thisObject.ToString());
            }
        }
        private class getHashCode : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((double)thisObject.GetHashCode());
            }
        }
        private class instanceOf : ScorpioHandle {
            readonly Script m_Script;
            public instanceOf(Script script) {
                m_Script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                ScriptValue type;
                switch (thisObject.valueType) {
                    case ScriptValue.trueValueType:
                    case ScriptValue.falseValueType:
                        type = m_Script.TypeBooleanValue;
                        break;
                    case ScriptValue.doubleValueType:
                    case ScriptValue.int64ValueType:
                        type = m_Script.TypeNumberValue;
                        break;
                    case ScriptValue.stringValueType:
                        type = m_Script.TypeStringValue;
                        break;
                    case ScriptValue.scriptValueType:
                        if (thisObject.scriptValue is ScriptArray) {
                            type = m_Script.TypeArrayValue;
                        } else if (thisObject.scriptValue is ScriptMap) {
                            type = m_Script.TypeMapValue;
                        } else if (thisObject.scriptValue is ScriptFunction) {
                            type = m_Script.TypeFunctionValue;
                        } else if (thisObject.scriptValue is ScriptInstance) {
                            type = new ScriptValue((thisObject.scriptValue as ScriptInstance).Prototype);
                        } else if (thisObject.scriptValue is ScriptType) {
                            type = thisObject;
                        } else {
                            type = m_Script.GetUserdataTypeValue(thisObject.scriptValue.Type);
                        }
                        break;
                    default:
                        type = m_Script.TypeObjectValue;
                        break;
                }
                var scriptObject = type.scriptValue;
                if (scriptObject is ScriptType) {
                    var scriptType = scriptObject as ScriptType;
                    var parentType = args[0];
                    while (true) {
                        if (scriptType.Equals(parentType)) {
                            return ScriptValue.True;
                        }
                        if ((scriptType = scriptType.Prototype) == null) {
                            return ScriptValue.False;
                        }
                    }
                } else if (scriptObject is ScriptUserdata) {
                    return ((scriptObject as ScriptUserdata).Type.IsAssignableFrom(args[0].Get<ScriptUserdata>().Type)) ? ScriptValue.True : ScriptValue.False;
                } else {
                    return ScriptValue.False;
                }
            }
        }
        private class referenceEquals : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return object.ReferenceEquals(args[0].Value, args[1].Value) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class addGetProperty : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                args[0].Get<ScriptType>().AddGetProperty(args[1].ToString(), args[2].Get<ScriptFunction>());
                return ScriptValue.Null;
            }
        }
    }
}