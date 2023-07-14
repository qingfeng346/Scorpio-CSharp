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
            m_global.SetValue(newName, m_global.GetValue(name));
        }
    }
}
