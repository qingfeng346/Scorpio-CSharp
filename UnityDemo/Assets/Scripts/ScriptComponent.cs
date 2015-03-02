using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scorpio;
//组件过渡类
public class ScriptComponent : MonoBehaviour {
    const string StringAwake = "Awake";
    const string StringStart = "Start";
    const string StringUpdate = "Update";
    const string StringOnDestroy = "OnDestroy";

    private List<string> registerFunctions = new List<string>();
    public ScriptTable Table { get; private set; }
    private bool containsFunction(string str)
    {
        return registerFunctions.Contains(str);
    }
    public void Initialize(Script script, ScriptTable table)
    {
        Table = table;
        table.SetValue("com", script.CreateUserdata(this));
        ScriptArray functions = table.GetValue("registerFunction") as ScriptArray;
        if (functions != null)
        {
            for (int i = 0; i < functions.Count();++i )
            {
                registerFunctions.Add((string)(functions.GetValue(i).ObjectValue));
            }
        }
        OnStart();
    }
    private object CallFunction(string func)
    {
        try {
            ScriptFunction fun = Table.GetValue(func) as ScriptFunction;
            if (fun != null) return fun.Call();
        } catch (System.Exception ex) {
            Debug.LogError("Stack : " + Launch.Script.GetStackInfo());
            Debug.LogError("CallFunction " + func + " is error " + ex.ToString());
        }
        return null;
    }
    private void OnStart()
    {
        if (containsFunction(StringAwake))
            CallFunction(StringAwake);
    }
	// Use this for initialization
	void Start () {
        if (containsFunction(StringStart))
            CallFunction(StringStart);
	}
	// Update is called once per frame
	void Update () {
        if (containsFunction(StringUpdate))
            CallFunction(StringUpdate);
	}
}
