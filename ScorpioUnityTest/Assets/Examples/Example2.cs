using UnityEngine;
using System.Collections;
using Scorpio;
using Scorpio.Userdata;
public class Example2 : MonoBehaviour {
	private static string output = "";
	private class ScriptPrint : ScorpioHandle {
		public ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length) {
			for (int i = 0; i < length; ++i) {
				output += Parameters[i].ToString() + "\n";
			}
			return ScriptValue.Null;
		}
	}
	private string text;
    private int width;
    private int height;
    private int windowHeight;
    void Awake()
    {
        width = Screen.width;
        height = Screen.height;
        windowHeight = height / 2 - 30;
        text = PlayerPrefs.GetString("__Text", "print (\"hello world\")");
		Application.logMessageReceived += OnLogCallback;
    }
	void OnGUI()
	{
        text = GUI.TextArea(new Rect(0, 0, width, windowHeight), text);
        PlayerPrefs.SetString("__Text", text);
        if (GUI.Button(new Rect(0, windowHeight, width, 90), "RunScript (" + Version.version + ")")) {
			output = "";
			Script script = new Script();
			try {
                script.LoadLibraryV1();
                TypeManager.PushAssembly(GetType().Assembly);
                TypeManager.PushAssembly(typeof(GameObject).Assembly);
				TypeManager.PushAssembly(typeof(UnityEngine.UI.SpriteState).Assembly);
				script.SetGlobal("print", script.CreateFunction(new ScriptPrint()));
				ScorpioClassManager.Initialize(script);
                OutPut("ReturnValue : " + script.LoadString(text));
			} catch (System.Exception e) {
                OutPut("StackInfo : " + script.GetStackInfo());
                OutPut(e.ToString());
			}
		}
        GUI.TextArea(new Rect(0, windowHeight + 90, width, windowHeight), output);
	}
	private void OnLogCallback(string condition, string stackTrace, LogType type) {
		OutPut (condition);
	}
	private void OutPut(string message) {
		output += (message + "\n");
	}
}
