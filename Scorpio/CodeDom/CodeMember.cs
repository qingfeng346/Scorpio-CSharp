using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
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
        STRING,     //String
        NUMBER,     //long类型
        INDEX,      //double类型（自动转成int类型）
        OBJECT,     //变量类型
    }
    //成员类型  a.b["c"].d[1]
    public class CodeMember : CodeObject
    {
        public CodeObject Parent;
        public CodeObject MemberObject;
        public string MemberString;
        public int MemberIndex;
        public object MemberNumber;
        public MEMBER_TYPE Type;
        public CALC Calc;
        public CodeMember(string name) : this (name, null) { }
        public CodeMember(string name, CodeObject parent)
        {
            this.Parent = parent;
            this.MemberString = name;
            this.Type = MEMBER_TYPE.STRING;
        }
        public CodeMember(ScriptNumber mem, CodeObject parent)
        {
            this.Parent = parent;
            if (mem.ObjectValue is double) {
                this.MemberIndex = mem.ToInt32();
                this.Type = MEMBER_TYPE.INDEX;
            } else {
                this.MemberNumber = mem.ObjectValue;
                this.Type = MEMBER_TYPE.NUMBER;
            }
        }
        public CodeMember(CodeObject member, CodeObject parent)
        {
            this.MemberObject = member;
            this.Parent = parent;
            this.Type = MEMBER_TYPE.OBJECT;
        }
    }
}
