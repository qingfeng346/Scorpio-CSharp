using System;
using System.Reflection;
using Scorpio.Compiler;
using Scorpio.Exception;
namespace Scorpio.Userdata
{
    public class DefaultScriptUserdataEventInfo : ScriptUserdata {
        private object m_Target;
        private EventInfo m_EventInfo;
        private Type m_HandlerType;
        public DefaultScriptUserdataEventInfo(Script script, BridgeEventInfo value) : base(script) {
            m_Target = value.target;
            m_EventInfo = value.eventInfo;
            m_HandlerType = m_EventInfo.EventHandlerType;
        }
        public override ScriptObject AssignCompute(TokenType type, ScriptObject obj) {
            switch (type) {
                case TokenType.AssignPlus:
                    m_EventInfo.AddEventHandler(m_Target, (Delegate)Util.ChangeType(Script, obj, m_HandlerType));
                    return Script.Null;
                case TokenType.AssignMinus:
                    m_EventInfo.RemoveEventHandler(m_Target, (Delegate)Util.ChangeType(Script, obj, m_HandlerType));
                    return Script.Null;
                default:
                    throw new ExecutionException(Script, "event 不支持的运算符 " + type);
            }
        }
        public override ScriptObject GetValue(object key)
        {
            if (!(key is string) || !key.Equals("Type")) throw new ExecutionException(Script, "EventInfo GetValue只支持 Type 一个变量");
            return Script.CreateObject(m_HandlerType);
        }
    }
}
