using Scorpio;
namespace Scorpio.Library {
    public class LibraryFunc {
        public static void Load(Script script) {
            ScriptTable Table = script.CreateTable();
            Table.SetValue("count", script.CreateFunction(new count(script)));
            Table.SetValue("isparams", script.CreateFunction(new isparams(script)));
            Table.SetValue("isstatic", script.CreateFunction(new isstatic(script)));
            Table.SetValue("getparams", script.CreateFunction(new getparams(script)));
            script.SetObjectInternal("func", Table);
        }
        private class count : ScorpioHandle {
            private Script m_Script;
            public count(Script script) {
                m_Script = script;
            }
            public object Call(ScriptObject[] args) {
                Util.Assert(args[0] is ScriptFunction, m_Script, "func.count 参数必须为 function 类型");
                return (args[0] as ScriptFunction).GetParamCount();
            }
        }
        private class isparams : ScorpioHandle {
            private Script m_Script;
            public isparams(Script script) {
                m_Script = script;
            }
            public object Call(ScriptObject[] args) {
                Util.Assert(args[0] is ScriptFunction, m_Script, "func.isparams 参数必须为 function 类型");
                return (args[0] as ScriptFunction).IsParams();
            }
        }
        private class isstatic : ScorpioHandle {
            private Script m_Script;
            public isstatic(Script script) {
                m_Script = script;
            }
            public object Call(ScriptObject[] args) {
                Util.Assert(args[0] is ScriptFunction, m_Script, "func.isstatic 参数必须为 function 类型");
                return (args[0] as ScriptFunction).IsStatic();
            }
        }
        private class getparams : ScorpioHandle {
            private Script m_Script;
            public getparams(Script script) {
                m_Script = script;
            }
            public object Call(ScriptObject[] args) {
                Util.Assert(args[0] is ScriptFunction, m_Script, "func.getparams 参数必须为 function 类型");
                return (args[0] as ScriptFunction).GetParams();
            }
        }
    }
}
