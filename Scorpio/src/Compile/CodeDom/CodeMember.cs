namespace Scorpio.Compile.CodeDom {
    //成员类型  a.b["c"].d[1]
    public abstract class CodeMember : CodeObject {
        public CodeObject Parent;       //父级
        public int index;               //索引
        public string key;              //字符串
        public CodeObject codeKey;      //变量
        public bool nullTo = false;     ///?. 获取变量
        public CodeMember(int line) : base(line) { }
    }
    //根据索引获取变量
    public class CodeMemberIndex : CodeMember {
        public CodeMemberIndex(int index, int line) : base(line) {
            this.index = index;
        }
        public CodeMemberIndex(int index, CodeObject parent, int line) : base(line) {
            this.index = index;
            this.Parent = parent;
        }
    }
    //根据索引获取变量
    public class CodeMemberInternal : CodeMember {
        public CodeMemberInternal(int index, int line) : base(line) {
            this.index = index;
        }
    }
    //根据字符串获取变量
    public class CodeMemberString : CodeMember {
        public CodeMemberString(string key, int line) : base(line) {
            this.key = key;
        }
        public CodeMemberString(string key, CodeObject parent, int line) : base(line) {
            this.key = key;
            this.Parent = parent;
        }
    }
    public class CodeMemberObject : CodeMember {
        public CodeMemberObject(CodeObject obj, CodeObject parent, int line) : base(line) {
            this.codeKey = obj;
            this.Parent = parent;
        }
    }
}
