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
        public CompileOption() : this(null, null, null, null, null) { }
        public CompileOption(IEnumerable<string> ignoreFunctions, IEnumerable<string> defines, IEnumerable<string> staticTypes, IEnumerable<string> staticVariables, ScriptConst scriptConst) {
            this.ignoreFunctions = new HashSet<string>(ignoreFunctions ?? EmptyArrayString);
            this.defines = new HashSet<string>(defines ?? EmptyArrayString);
            this.staticTypes = new HashSet<string>(staticTypes ?? EmptyArrayString);
            this.staticVariables = new HashSet<string>(staticVariables ?? EmptyArrayString);
            this.scriptConst = scriptConst ?? new ScriptConst();
        }
        public bool IsStaticVariable(string name) {
            return staticVariables.Contains(name);
        }
        public bool IsStaticFunction(string name) {
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
