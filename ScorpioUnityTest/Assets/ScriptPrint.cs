using UnityEngine;
using System.Text;
using System.Collections;
using Scorpio;
public class ScriptPrint : ScorpioHandle {
    public ScriptValue Call(ScriptValue thisObject, ScriptValue[] Parameters, int length)
    {
        var builder = new StringBuilder();
        for (int i = 0; i < length; ++i) {
            if (i != 0) {
                builder.Append("    ");
            }
            builder.Append(Parameters[i].ToString());

        }
        Debug.Log(builder.ToString());
        return ScriptValue.Null;
	}
}
