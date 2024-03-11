using System;
using System.Collections.Generic;
namespace Scorpio.Compile.Compiler {
    /// <summary> 编译选项 </summary>
    public class CompileOption {
        public static readonly CompileOption Default = new CompileOption();
        private readonly string[] EmptyArrayString = new string[0];
        private HashSet<string> mIgnoreFunctions;       //编译忽略的全局函数
        private IEnumerable<string> mDefines;           //全局defines
        private IEnumerable<string> mStaticTypes;       //静态类,类下所有函数都会把取值变量动态编译成全局索引取值,加快运行效率
        private HashSet<string> mStaticVariables;       //静态全局变量和函数,会把取值变量动态编译成全局索引取值,加快运行效率
        private IEnumerable<string> mSearchPaths;       //搜索目录
        private ScriptConst mScriptConst;               //全局const,把变量编译成常量
        public IEnumerable<string> ignoreFunctions {
            get { return mIgnoreFunctions; }
            set { mIgnoreFunctions = new HashSet<string>(value ?? EmptyArrayString); }
        }
        public IEnumerable<string> staticVariables {
            get { return mStaticVariables; }
            set { mStaticVariables = new HashSet<string>(value ?? EmptyArrayString); }
        }
        public ScriptConst scriptConst {
            get { return mScriptConst; }
            set { mScriptConst = value ?? new ScriptConst(); }
        }
        public IEnumerable<string> defines {
            get { return mDefines; }
            set { mDefines = value ?? EmptyArrayString; }
        }
        public IEnumerable<string> staticTypes {
            get { return mStaticTypes; }
            set { mStaticTypes = value ?? EmptyArrayString; }
        }
        public IEnumerable<string> searchPaths {
            get { return mSearchPaths; }
            set { mSearchPaths = value ?? EmptyArrayString; }
        }
        public Action<ScriptParser, string> preprocessImportFile { get; set; }

        public CompileOption() {
            this.ignoreFunctions = null;
            this.staticVariables = null;
            this.scriptConst = null;
            this.defines = null;
            this.staticTypes = null;
            this.searchPaths = null;
            this.preprocessImportFile = null;
        }
        internal bool IsIgnoreFunction(string name) {
            return mIgnoreFunctions.Contains(name);
        }
        internal bool IsStaticVariable(string name) {
            return mStaticVariables.Contains(name);
        }
        internal bool IsStaticFunction(string name) {
            if (mStaticVariables.Contains(name)) {
                return true;
            }
            foreach (var type in mStaticTypes) {
                if (name.StartsWith(type + ".")) {
                    return true;
                }
            }
            return false;
        }
    }
}
