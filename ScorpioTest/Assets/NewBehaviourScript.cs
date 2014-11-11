using UnityEngine;
using System.Collections;
using Scorpio;
public class NewBehaviourScript : MonoBehaviour {
	private string text = "";
	private string output = "";
    private int width;
    private int height;
    private int windowHeight;
    void Awake()
    {
        width = Screen.width;
        height = Screen.height;
        windowHeight = height / 2 - 30;
    }
	void OnGUI()
	{
        text = GUI.TextArea(new Rect(0, 0, width, windowHeight), text);
        if (GUI.Button(new Rect(0, windowHeight, width, 60), "RunScript (" + Script.Version + ")"))
        {
			output = "";
			Script script = new Script();
			try {
				script.LoadLibrary();
				script.SetObject("print", script.CreateFunction(print));
				ScriptObject ret = script.LoadString(text);
                OutPut("ReturnValue : " + ret);
			} catch (System.Exception e) {
                OutPut("StackInfo : " + script.GetStackInfo());
                OutPut(e.ToString());
			}
		}
        GUI.TextArea(new Rect(0, windowHeight + 60, width, windowHeight), output);
	}
	private object print(object[] Parameters)
	{
		for (int i = 0; i < Parameters.Length; ++i) {
			output += (Parameters[i].ToString() + "\n");
		}
		return null;
	}
	private void OutPut(string message)
	{
		output += (message + "\n");
	}
}
