//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// This class makes it possible to activate a button by pressing a key (such as space bar for example).
/// </summary>

[AddComponentMenu("Game/UI/Button Key Binding")]
public class UIButtonKeyBinding : MonoBehaviour
{
	public KeyCode keyCode = KeyCode.None;

	void Update ()
	{
		if (!UICamera.inputHasFocus)
		{
			if (keyCode == KeyCode.None) return;
			
			if (Input.GetKeyDown(keyCode))
			{
				SendMessage("OnPress", true, SendMessageOptions.DontRequireReceiver);
			}

			if (Input.GetKeyUp(keyCode))
			{
				SendMessage("OnPress", false, SendMessageOptions.DontRequireReceiver);
				SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}