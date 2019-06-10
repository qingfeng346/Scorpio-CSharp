namespace Scorpio.CodeDom {
    //成员类型  a.b["c"].d[1]
    public abstract class CodeMember : CodeObject {
        public CodeObject Parent;
        public int index;
        public string key;
        public CodeObject codeKey;
        public CodeMember(int line) : base(line) { }
    }
    //根据索引获取变量
    public class CodeMemberIndex : CodeMember {
        public CodeMemberIndex(int index, int line) : base(line) {
            this.index = index;
        }
        public CodeMemberIndex(int index, CodeObject parment, int line) : base(line) {
            this.index = index;
            this.Parent = parment;
        }
    }
    //根据索引获取变量
    public class CodeMemberInternal : CodeMember {
        public CodeMemberInternal(int index, int line) : base(line) {
            this.index = index;
        }
        public CodeMemberInternal(int index, CodeObject parment, int line) : base(line) {
            this.index = index;
            this.Parent = parment;
        }
    }
    //根据字符串获取变量
    public class CodeMemberString : CodeMember {
        public CodeMemberString(string key, int line) : base(line) {
            this.key = key;
        }
        public CodeMemberString(string key, CodeObject parment, int line) : base(line) {
            this.key = key;
            this.Parent = parment;
        }
    }
    public class CodeMemberObject : CodeMember {
        public CodeMemberObject(CodeObject obj, int line) : base(line) {
            this.codeKey = obj;
        }
        public CodeMemberObject(CodeObject obj, CodeObject parment, int line) : base(line) {
            this.codeKey = obj;
            this.Parent = parment;
        }
    }
}
