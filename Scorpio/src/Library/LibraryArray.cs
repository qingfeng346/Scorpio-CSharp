using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.Library
{
    public class LibraryArray
    {
        public static void Load(Script script)
        {
            ScriptTable Table = script.CreateTable();
            Table.SetValue("count", script.CreateFunction(new count()));
            Table.SetValue("insert", script.CreateFunction(new insert()));
            Table.SetValue("add", script.CreateFunction(new add()));
            Table.SetValue("remove", script.CreateFunction(new remove()));
            Table.SetValue("removeat", script.CreateFunction(new removeat()));
            Table.SetValue("resize", script.CreateFunction(new resize()));
            Table.SetValue("clear", script.CreateFunction(new clear()));
            Table.SetValue("contains", script.CreateFunction(new contains()));
			Table.SetValue("sort", script.CreateFunction(new sort()));
            Table.SetValue("indexof", script.CreateFunction(new indexof()));
            Table.SetValue("lastindexof", script.CreateFunction(new lastindexof()));
            Table.SetValue("first", script.CreateFunction(new first()));
            Table.SetValue("last", script.CreateFunction(new last()));
            Table.SetValue("pop", script.CreateFunction(new popfirst()));
            Table.SetValue("safepop", script.CreateFunction(new safepopfirst()));
            Table.SetValue("popfirst", script.CreateFunction(new popfirst()));
            Table.SetValue("safepopfirst", script.CreateFunction(new safepopfirst()));
            Table.SetValue("poplast", script.CreateFunction(new poplast()));
            Table.SetValue("safepoplast", script.CreateFunction(new safepoplast()));
            script.SetObjectInternal("array", Table);
        }
        private class count : ScorpioHandle
        {
            public object Call(ScriptObject[] args) {
                return ((ScriptArray)args[0]).Count();
            }
        }
        private class insert : ScorpioHandle
        {
            public object Call(ScriptObject[] args) {
                ScriptObject obj = args[2];
                ((ScriptArray)args[0]).Insert(((ScriptNumber)args[1]).ToInt32(), obj);
                return obj;
            }
        }
        private class add : ScorpioHandle
        {
            public object Call(ScriptObject[] args) {
                ScriptObject obj = args[1];
                ((ScriptArray)args[0]).Add(obj);
                return obj;
            }
        }
        private class remove : ScorpioHandle
        {
            public object Call(ScriptObject[] args) {
                ((ScriptArray)args[0]).Remove(args[1]);
                return null;
            }
        }
        private class removeat : ScorpioHandle
        {
            public object Call(ScriptObject[] args) {
                ((ScriptArray)args[0]).RemoveAt(((ScriptNumber)args[1]).ToInt32());
                return null;
            }
        }
        private class resize: ScorpioHandle {
            public object Call(ScriptObject[] args) {
                ((ScriptArray)args[0]).Resize(((ScriptNumber)args[1]).ToInt32());
                return null;
            }
        }
        private class clear : ScorpioHandle
        {
            public object Call(ScriptObject[] args) {
                ((ScriptArray)args[0]).Clear();
                return null;
            }
        }
        private class contains : ScorpioHandle
        {
            public object Call(ScriptObject[] args) {
                return ((ScriptArray)args[0]).Contains(args[1]);
            }
        }
		private class sort : ScorpioHandle
		{
			public object Call(ScriptObject[] args) {
				((ScriptArray)args[0]).Sort((ScriptFunction)args[1]);
				return null;
			}
		}
        private class indexof : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return ((ScriptArray)args[0]).IndexOf(args[1]);
            }
        }
        private class lastindexof : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return ((ScriptArray)args[0]).LastIndexOf(args[1]);
            }
        }
        private class first : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return ((ScriptArray)args[0]).First();
            }
        }
        private class last : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return ((ScriptArray)args[0]).Last();
            }
        }
        private class popfirst : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return ((ScriptArray)args[0]).PopFirst();
            }
        }
        private class safepopfirst : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return ((ScriptArray)args[0]).SafePopFirst();
            }
        }
        private class poplast : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return ((ScriptArray)args[0]).PopLast();
            }
        }
        private class safepoplast : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return ((ScriptArray)args[0]).SafePopLast();
            }
        }
    }
}
