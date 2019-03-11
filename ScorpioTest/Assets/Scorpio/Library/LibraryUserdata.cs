using Scorpio;
using Scorpio.Userdata;
namespace Scorpio.Library {
    public class LibraryUserdata {
        public static void Load(Script script) {
            ScriptTable Table = script.CreateTable();
            Table.SetValue("rename", script.CreateFunction(new rename(script)));
            Table.SetValue("typeof", script.CreateFunction(new utypeof(script)));
            script.SetObjectInternal("userdata", Table);
        }
        private class rename : ScorpioHandle {
            private readonly Script m_script;
            public rename(Script script) {
                m_script = script;
            }
            public object Call(ScriptObject[] args) {
                ScriptObject type = args[0];
                if (type is ScriptUserdataObject || type is ScriptUserdataObjectType) {
                    m_script.GetScorpioType(((ScriptUserdata)type).ValueType).Rename(args[1].ToString(), args[2].ToString());
                }
                return null;
            }
        }
        private class utypeof : ScorpioHandle {
            private readonly Script m_script;
            public utypeof(Script script) {
                m_script = script;
            }
            public object Call(ScriptObject[] args) {
                ScriptObject type = args[0];
                if (type is ScriptUserdataObject || type is ScriptUserdataObjectType) {
                    m_script.GetScorpioType(((ScriptUserdata)type).ValueType).GetVariableType(args[1].ToString());
                }
                return null;

            }
        }
    }
}
