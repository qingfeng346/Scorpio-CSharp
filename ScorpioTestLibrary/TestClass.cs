using Scorpio;
public enum TestEnum {
    Test1,
    Test2,
}
public enum TestEnum1 {
    Test1,
    Test2,
}
public static class TestClass {
    static ScriptValue wwww;
    public static ScriptInstance AddComponent(this ScriptInstance instance) {
        wwww = new ScriptValue(instance);
        wwww.Free();
        return instance;
    }
    public static void Update() {
        //wwww.GetValue("Update").call(wwww);
    }
}