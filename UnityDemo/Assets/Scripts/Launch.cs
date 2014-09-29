using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Scorpio;
public class Launch : MonoBehaviour {
    public static Script Script;
    public GameObject obj;
	void Start () {
        try
        {
            List<string> scripts = new List<string>();
            scripts.Add("window");
            Script script = new Script();
            Launch.Script = script;
            script.LoadLibrary();
            script.SetObject("print", new ScorpioFunction(Print));
            for (int i = 0; i < scripts.Count; ++i)
            {
                script.LoadString(scripts[i], (Resources.Load(scripts[i]) as TextAsset).text);
            }
            Util.AddComponent(obj, (ScriptTable)script.GetValue("window"));
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Stack : " + Script.GetStackInfo());
            Debug.LogError("Start is error " + ex.ToString());
        }
	}
    object Print(object[] args)
    {
        for (int i = 0; i < args.Length; ++i)
        {
            Debug.Log(args[i].ToString());
        }
        return null;
    }
}
