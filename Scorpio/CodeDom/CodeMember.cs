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
        STRING,
        NUMBER,
        OBJECT,
    }
    //成员类型  a.b["c"].d[1]
    public class CodeMember : CodeObject
    {
        public CodeObject Parent;
        public CodeObject Member;
        public string MemberString;
        public int MemberNumber;
        public object MemberNumberObject;
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
            this.MemberNumber = mem.ToInt32();
            this.MemberNumberObject = mem.ObjectValue;
            this.Type = MEMBER_TYPE.NUMBER;
        }
        public CodeMember(CodeObject member, CodeObject parent)
        {
            this.Member = member;
            this.Parent = parent;
            this.Type = MEMBER_TYPE.OBJECT;
        }
    }
}
