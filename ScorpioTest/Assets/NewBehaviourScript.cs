using UnityEngine;
using System.Collections;
using Scorpio;
public class NewBehaviourScript : MonoBehaviour {
	private static string output = "";
	private class ScriptPrint : ScorpioHandle {
		public object Call(ScriptObject[] args) {
			for (int i = 0; i < args.Length; ++i) {
				output += args[i].ToString() + "\n";
			}
			return null;
		}
	}
	private string text = "print (\"hello world\")";
    private int width;
    private int height;
    private int windowHeight;
    void Awake()
    {
        width = Screen.width;
        height = Screen.height;
        windowHeight = height / 2 - 30;
        text = PlayerPrefs.GetString("__Text", "");
		Application.RegisterLogCallbackThreaded (OnLogCallback);
    }
	void OnGUI()
	{
        text = GUI.TextArea(new Rect(0, 0, width, windowHeight), text);
        PlayerPrefs.SetString("__Text", text);
        if (GUI.Button(new Rect(0, windowHeight, width, 90), "RunScript (" + Script.Version + ")")) {
			output = "";
			Script script = new Script();
			try {
				script.LoadLibrary();
                script.PushAssembly(GetType().Assembly);
                script.PushAssembly(typeof(GameObject).Assembly);
				script.SetObject("print", script.CreateFunction(new ScriptPrint()));
				ScriptObject ret = script.LoadString(text);
                OutPut("ReturnValue : " + ret);
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
