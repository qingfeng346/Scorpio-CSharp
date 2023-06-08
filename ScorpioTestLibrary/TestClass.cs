using Scorpio;
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