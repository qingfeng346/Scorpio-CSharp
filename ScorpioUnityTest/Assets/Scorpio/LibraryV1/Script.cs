using Scorpio.LibraryV1;
namespace Scorpio {
    public partial class Script {
        public void LoadLibraryV1() {
            LibraryBasis.Load(this);
            LibraryArray.Load(this);
            LibraryString.Load(this);
            LibraryTable.Load(this);
        }
        public void NewNameGlobal(string name, string newName) {
            Global.SetValue(newName, Global.GetValue(name));
        }
    }
}
