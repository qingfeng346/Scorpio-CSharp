using Scorpio.Userdata;
namespace Scorpio.Proto {
    public class ProtoObject {
        public static ScriptType Load(Script script, ScriptType ret) {
            ret.SetValue("toString", script.CreateFunction(new toString()));
            ret.SetValue("getHashCode", script.CreateFunction(new getHashCode()));
            ret.SetValue("instanceOf", script.CreateFunction(new instanceOf(script)));
            ret.SetValue("referenceEquals", script.CreateFunction(new referenceEquals()));
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
                    case ScriptValue.longValueType:
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
                            type = (thisObject.scriptValue as ScriptInstance).Prototype;
                        } else if (thisObject.scriptValue is ScriptType) {
                            type = thisObject;
                        } else {
                            type = TypeManager.GetUserdataType(thisObject.scriptValue.Type);
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
                        if ((scriptType = scriptType.Prototype.Get<ScriptType>()) == null) {
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
    }
}