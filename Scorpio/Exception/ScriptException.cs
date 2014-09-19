using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Exception
{
    public class ScriptException : System.Exception
    {
        private String m_strMessage;
        private System.Exception m_exceptionInner;
        public ScriptException(String strMessage)
        {
            m_strMessage = strMessage;
            m_exceptionInner = null;
        }
        public ScriptException(System.Exception exceptionInner,String strMessage)
        {
            m_strMessage = strMessage;
            m_exceptionInner = exceptionInner;
        }
        public override String ToString()
        {
            return MessageTrace;
        }
        public override String Message { get { return m_strMessage; } }
        string MessageTrace {
            get {
                if (m_exceptionInner != null) {
                    string strMessageTrace = m_strMessage + "Reason : \n";
                    if (m_exceptionInner is ScriptException)
                        strMessageTrace += ((ScriptException)m_exceptionInner).MessageTrace;
                    else
                        strMessageTrace += m_exceptionInner.ToString();
                    return strMessageTrace;
                }
                else
                    return m_strMessage;
            }
        }
    }
}
