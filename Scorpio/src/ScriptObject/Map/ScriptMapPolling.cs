namespace Scorpio {
    //脚本map类型
    public class ScriptMapPolling : ScriptMapObject {
        public ScriptMapPolling(Script script) : base(script) { }
        public override void Free() {
            Release();
            Clear();
            m_Script.Free(this);
        }
    }
}
