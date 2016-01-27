using System;
using System.Reflection;

namespace Scorpio.Userdata
{
    public class BridgeEventInfo
    {
        public object target;
        public EventInfo eventInfo;
        public BridgeEventInfo(object target, EventInfo eventInfo)
        {
            this.target = target;
            this.eventInfo = eventInfo;
        }
    }
}
