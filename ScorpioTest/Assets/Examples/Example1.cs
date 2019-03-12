using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Scorpio;
public class Example1 : MonoBehaviour {
    public ScrollRect Scroll;
    public GameObject Grid;
    public GameObject Button;
	public InputField Input;
	public Text Output;
	void Awake() {
        var files = Resources.LoadAll("", typeof(TextAsset));
        foreach (var file in files) {
            var textAsset = file as TextAsset;
            var obj = Instantiate(Button);
            obj.transform.SetParent(Grid.transform, false);
            obj.SetActive(true);
            var text = obj.transform.Find("Text").GetComponent<Text>();
            text.text = textAsset.name;
            var button = obj.GetComponent<Button>();
            var str = textAsset.text;
            button.onClick.AddListener(() => {
                Input.text = str;
            });
            Resources.UnloadAsset(textAsset);
        }
        Scroll.normalizedPosition = new Vector2(0, 1);
        Resources.UnloadUnusedAssets();
        Application.logMessageReceived += OnLogCallback;
	}
	private void OnLogCallback(string condition, string stackTrace, LogType type) {
		OutPut (condition);
	}
	private void OutPut(string message) {
		Output.text += (message + "\n");
	}
	public void OnClickRunScript() {
        Output.text = "";
		Script script = new Script();
		try {
			script.LoadLibrary();
			script.PushAssembly(GetType().Assembly);
			script.PushAssembly(typeof(GameObject).Assembly);
			script.SetObject("print", script.CreateFunction(new ScriptPrint()));
			ScriptObject ret = script.LoadString(Input.text);
			OutPut("=====================");
			OutPut("ReturnValue : " + ret);
		} catch (System.Exception e) {
			OutPut("=====================");
			OutPut("StackInfo : " + script.GetStackInfo());
			OutPut(e.ToString());
		}
	}
    public void OnClickClear() {
        Output.text = "";
    }
}
