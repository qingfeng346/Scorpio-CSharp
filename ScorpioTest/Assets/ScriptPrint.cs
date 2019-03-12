using UnityEngine;
using System.Collections;
using Scorpio;
public class ScriptPrint : ScorpioHandle {
	public object Call(ScriptObject[] args) {
		for (int i = 0; i < args.Length; ++i) {
			Debug.Log(args[i].ToString());
		}
		return null;
	}
}
