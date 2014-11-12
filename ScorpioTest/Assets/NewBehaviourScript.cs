using UnityEngine;
using System.Collections;
using Scorpio;
public class NewBehaviourScript : MonoBehaviour {
	private string text = "";
    private string url = "";
	private string output = "";
    private int width;
    private int height;
    private int windowHeight;
    void Awake()
    {
        width = Screen.width;
        height = Screen.height;
        windowHeight = height / 2 - 30;
        text = PlayerPrefs.GetString("__Text", "");
        url = PlayerPrefs.GetString("__Url", "");
    }
	void OnGUI()
	{
        text = GUI.TextArea(new Rect(0, 0, width, windowHeight), text);
        url = GUI.TextField(new Rect(0, windowHeight, width, 30), url);
        PlayerPrefs.SetString("__Text", text);
        PlayerPrefs.SetString("__Url", url);
        if (GUI.Button(new Rect(0, windowHeight + 30, width / 2, 60), "RunScript (" + Script.Version + ")"))
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
        if (GUI.Button(new Rect(width / 2, windowHeight + 30, width / 2, 60), "GetScript"))
        {
            SendMessage("GetScript");
        }
        GUI.TextArea(new Rect(0, windowHeight + 90, width, windowHeight), output);
	}
    IEnumerator GetScript()
    {
        WWW www = new WWW(url);
        yield return www;
        if (!string.IsNullOrEmpty(www.error)) {
            OutPut(www.error);
        } else {
            text = www.text;
        }
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
