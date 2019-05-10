using System;
using System.IO;
using System.Text;
using Scorpio;

namespace ScorpioLibrary {
    public class LibraryIO {
        public static void Load(Script script) {
            //var Table = script.GetValue("io") as ScriptTable ?? script.CreateTable();
            //Table.SetValue("file_exist", script.CreateFunction(new file_exist()));
            //Table.SetValue("path_exist", script.CreateFunction(new path_exist()));
            //Table.SetValue("create_path", script.CreateFunction(new create_path()));
            //Table.SetValue("create_file", script.CreateFunction(new create_file()));
            //Table.SetValue("get_file", script.CreateFunction(new get_file()));
            //Table.SetValue("del_file", script.CreateFunction(new del_file()));
            //Table.SetValue("get_filelist", script.CreateFunction(new get_filelist(script)));
            //script.SetObject("io", Table);
        }
        //private class file_exist : ScorpioHandle {
        //    public object Call(ScriptObject[] args) {
        //        string path = args[0].ToString();
        //        return !string.IsNullOrEmpty(path) && File.Exists(path);
        //    }
        //}
        //private class path_exist : ScorpioHandle {
        //    public object Call(ScriptObject[] args) {
        //        string path = args[0].ToString();
        //        return !string.IsNullOrEmpty(path) && Directory.Exists(path);
        //    }
        //}
        //private class create_path : ScorpioHandle {
        //    public object Call(ScriptObject[] args) {
        //        string path = args[0].ToString();
        //        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        //        return null;
        //    }
        //}
        //private class create_file : ScorpioHandle {
        //    public object Call(ScriptObject[] args) {
        //        string file = args[0].ToString();
        //        string buffer = args[1].ToString();
        //        string path = Path.GetDirectoryName(file);
        //        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        //        if (File.Exists(file)) File.Delete(file);
        //        File.WriteAllText(file, buffer, Encoding.UTF8);
        //        return null;
        //    }
        //}
        //private class get_file : ScorpioHandle {
        //    public object Call(ScriptObject[] args) {
        //        string file = args[0].ToString();
        //        if (!File.Exists(file)) return null;
        //        return File.ReadAllText(file);
        //    }
        //}
        //private class del_file : ScorpioHandle {
        //    public object Call(ScriptObject[] args) {
        //        string file = args[0].ToString();
        //        if (File.Exists(file)) File.Delete(file);
        //        return null;
        //    }
        //}
        //private class get_filelist : ScorpioHandle {
        //    Script m_script;
        //    public get_filelist(Script script) {
        //        m_script = script;
        //    }
        //    public object Call(ScriptObject[] args) {
        //        //string path = args[0].ToString();
        //        //string parttern = args[1].ToString();
        //        //bool recursive = (args[2] as ScriptBoolean).Value;
        //        //var files = Directory.GetFiles(parttern, parttern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        //        //ScriptArray array = m_script.CreateArray();
        //        //foreach (var file in files) {
        //        //    array.Add(m_script.CreateString(file));
        //        //}
        //        //return array;
        //        return null;
        //    }
        //}
    }
}
