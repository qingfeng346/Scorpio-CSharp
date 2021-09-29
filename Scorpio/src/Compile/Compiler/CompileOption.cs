using System.Collections.Generic;
namespace Scorpio.Compile.Compiler {
    /// <summary> 编译选项 </summary>
    public class CompileOption {
        public static readonly CompileOption Default = new CompileOption();
        private readonly string[] EmptyArrayString = new string[0];
        public HashSet<string> ignoreFunctions;    //编译忽略的全局函数
        public HashSet<string> defines;            //全局defines
        public HashSet<string> staticTypes;        //静态类,类下所有函数都会把取值变量动态编译成全局索引取值,加快运行效率
        public HashSet<string> staticVariables;    //静态全局变量和函数,会把取值变量动态编译成全局索引取值,加快运行效率
        public ScriptConst scriptConst;            //全局const,把变量编译成常量
        public CompileOption(IEnumerable<string> ignoreFunctions = null, IEnumerable<string> defines = null, IEnumerable<string> staticTypes = null, IEnumerable<string> staticVariables = null, ScriptConst scriptConst = null) {
            this.ignoreFunctions = new HashSet<string>(ignoreFunctions ?? EmptyArrayString);
            this.defines = new HashSet<string>(defines ?? EmptyArrayString);
            this.staticTypes = new HashSet<string>(staticTypes ?? EmptyArrayString);
            this.staticVariables = new HashSet<string>(staticVariables ?? EmptyArrayString);
            this.scriptConst = scriptConst ?? new ScriptConst();
        }
        public CompileOption SetIgnoreFunctions(IEnumerable<string> ignoreFunctions) {
            this.ignoreFunctions = new HashSet<string>(ignoreFunctions ?? EmptyArrayString);
            return this;
        }
        public CompileOption SetDefines(IEnumerable<string> defines) {
            this.defines = new HashSet<string>(defines ?? EmptyArrayString);
            return this;
        }
        public CompileOption SetStaticTypes(IEnumerable<string> staticTypes) {
            this.staticTypes = new HashSet<string>(staticTypes ?? EmptyArrayString);
            return this;
        }
        public CompileOption SetStaticVariables(IEnumerable<string> staticVariables) {
            this.staticVariables = new HashSet<string>(staticVariables ?? EmptyArrayString);
            return this;
        }
        public CompileOption SetScriptConst(ScriptConst scriptConst) {
            this.scriptConst = scriptConst ?? new ScriptConst();
            return this;
        }
        internal bool IsStaticVariable(string name) {
            return staticVariables.Contains(name);
        }
        internal bool IsStaticFunction(string name) {
            if (staticVariables.Contains(name)) {
                return true;
            }
            foreach (var type in staticTypes) {
                if (name.StartsWith(type + ".")) {
                    return true;
                }
            }
            return false;
        }
    }
}
