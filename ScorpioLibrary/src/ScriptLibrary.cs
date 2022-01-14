using Scorpio;
namespace ScorpioLibrary {
    public static class ScriptLibrary {
        public static void LoadLibraryExtend(this Script script) {
            LibraryNet.Load(script);
            LibraryOS.Load(script);
        }
    }
}
