using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Scorpio;
using Scorpio.Userdata;
using System.IO;
namespace ScorpioExec {
    public class TestClass {
        public int num = 0;
        public void TestFunc(ref int refNum, out string outNum) {
			if (refNum == 100) {
				outNum = "out1";
			} else {
				outNum = "out2";
			}
			refNum = refNum + 500;
		}
	}
    public static class ClassEx {
		public static void TestFunc1(this TestClass t, int num1) {
            t.num += num1;
        }
	}
}

public class Example1 : MonoBehaviour {
    public ScrollRect Scroll;
    public GameObject Grid;
    public GameObject Button;
	public InputField Input;
	public Text Output;
	void Awake() {
#if UNITY_EDITOR
		if (Directory.Exists("../ExampleScripts")) {
			var resources = Directory.GetFiles("Assets/Resources");
			foreach (var resource in resources) {
				File.Delete(resource);
			}
			var examples = System.IO.Directory.GetFiles("../ExampleScripts");
			foreach (var file in examples) {
				var fileName = Path.GetFileNameWithoutExtension(file);
				File.Copy(file, $"Assets/Resources/sco_{fileName}.txt");
			}
			UnityEditor.AssetDatabase.Refresh();
		}

#endif
        var files = Resources.LoadAll("", typeof(TextAsset));
        foreach (var file in files) {
            var textAsset = file as TextAsset;
            var name = textAsset.name;
        	if (name.StartsWith("sco_")) {
                name = name.Substring(4);
                var obj = Instantiate(Button);
        		obj.transform.SetParent(Grid.transform, false);
        		obj.SetActive(true);
        		var text = obj.transform.Find("Text").GetComponent<Text>();
        		text.text = name;
        		var button = obj.GetComponent<Button>();
        		var str = textAsset.text;
        		button.onClick.AddListener(() => {
        			Input.text = str;
        		});
        		Resources.UnloadAsset(textAsset);
        	}
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
			script.LoadLibraryV1();
			TypeManager.PushAssembly(GetType().Assembly);
			TypeManager.PushAssembly(typeof(GameObject).Assembly);
			script.SetGlobal("print", script.CreateFunction(new ScriptPrint()));
			OutPut("=====================");
			OutPut("ReturnValue : " + script.LoadString(Input.text));
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
