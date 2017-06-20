using Scorpio;
using Scorpio.Userdata;
namespace Scorpio.Library {
    public class LibraryUserdata {
        public static void Load(Script script) {
            ScriptTable Table = script.CreateTable();
            Table.SetValue("rename", script.CreateFunction(new rename(script)));
            script.SetObjectInternal("userdata", Table);
        }
        private class rename : ScorpioHandle {
            private Script m_Script;
            public rename(Script script) {
                m_Script = script;
            }
            public object Call(ScriptObject[] args) {
                ScriptObject type = args[0];
                if (type is ScriptUserdataObject || type is ScriptUserdataObjectType) {
                    m_Script.GetScorpioType(((ScriptUserdata)type).ValueType).Rename(args[1].ToString(), args[2].ToString());
                }
                return null;
            }
        }
    }
}
