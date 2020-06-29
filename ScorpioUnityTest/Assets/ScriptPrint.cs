using UnityEngine;
using System.Collections;
using Scorpio;
public class ScriptPrint : ScorpioHandle {
	public ScriptValue Call(ScriptValue thisObject, ScriptValue[] Parameters, int length) {
		for (int i = 0; i < length; ++i) {
			Debug.Log(Parameters[i].ToString());
		}
		return ScriptValue.Null;
	}
}
