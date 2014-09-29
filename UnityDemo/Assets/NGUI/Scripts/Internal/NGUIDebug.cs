//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This class is meant to be used only internally. It's like Debug.Log, but prints using OnGUI to screen instead.
/// </summary>

[AddComponentMenu("NGUI/Internal/Debug")]
public class NGUIDebug : MonoBehaviour
{
	static List<string> mLines = new List<string>();
	static NGUIDebug mInstance = null;
	
	static public void Log (string text)
	{
		if (Application.isPlaying)
		{
			//Debug.Log(text);

			if (mLines.Count > 20) mLines.RemoveAt(0);
			mLines.Add(text);

			if (mInstance == null)
			{
				GameObject go = new GameObject("_NGUI Debug");
				mInstance = go.AddComponent<NGUIDebug>();
				DontDestroyOnLoad(go);
			}
		}
		else
		{
			Debug.Log(text);
		}
	}

	static public void DrawBounds (Bounds b)
	{
		Vector3 c = b.center;
		Vector3 v0 = b.center - b.extents;
		Vector3 v1 = b.center + b.extents;
		Debug.DrawLine(new Vector3(v0.x, v0.y, c.z), new Vector3(v1.x, v0.y, c.z), Color.red);
		Debug.DrawLine(new Vector3(v0.x, v0.y, c.z), new Vector3(v0.x, v1.y, c.z), Color.red);
		Debug.DrawLine(new Vector3(v1.x, v0.y, c.z), new Vector3(v1.x, v1.y, c.z), Color.red);
		Debug.DrawLine(new Vector3(v0.x, v1.y, c.z), new Vector3(v1.x, v1.y, c.z), Color.red);
	}
	
	void OnGUI()
	{
		for (int i = 0, imax = mLines.Count; i < imax; ++i)
		{
			GUILayout.Label(mLines[i]);
		}
	}
}