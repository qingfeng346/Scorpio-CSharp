namespace Scorpio.CodeDom
{
    //++或者--标识
    public enum CALC
    {
        NONE,
        PRE_INCREMENT,      //前置++
        POST_INCREMENT,     //后置++
        PRE_DECREMENT,      //前置--
        POST_DECREMENT,     //后置--
    }
    //成员类型
    public enum MEMBER_TYPE
    {
        VALUE,      //Value类型
        OBJECT,     //变量类型
    }
    //成员类型  a.b["c"].d[1]
    public class CodeMember : CodeObject
    {
        public CodeObject Parent;
        public CodeObject MemberObject;
        public object MemberValue;
        public MEMBER_TYPE Type;
        public CALC Calc;
        public CodeMember(string name) : this (name, null) { }
        public CodeMember(object value, CodeObject parent)
        {
            this.Parent = parent;
            this.MemberValue = value;
            this.Type = MEMBER_TYPE.VALUE;
        }
        public CodeMember(CodeObject member, CodeObject parent)
        {
            this.MemberObject = member;
            this.Parent = parent;
            this.Type = MEMBER_TYPE.OBJECT;
        }
    }
}
