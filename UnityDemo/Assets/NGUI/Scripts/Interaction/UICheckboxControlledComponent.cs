//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Example script showing how to activate or deactivate a MonoBehaviour when OnActivate event is received.
/// OnActivate event is sent out by the UICheckbox script.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Checkbox Controlled Component")]
public class UICheckboxControlledComponent : MonoBehaviour
{
	public MonoBehaviour target;
	public bool inverse = false;

	bool mUsingDelegates = false;

	void Start ()
	{
		UICheckbox chk = GetComponent<UICheckbox>();

		if (chk != null)
		{
			mUsingDelegates = true;
			chk.onStateChange += OnActivateDelegate;
		}
	}

	void OnActivateDelegate (bool isActive)
	{
		if (enabled && target != null) target.enabled = inverse ? !isActive : isActive;
	}

	/// <summary>
	/// Legacy functionality -- keeping it for backwards compatibility.
	/// </summary>

	void OnActivate (bool isActive) { if (!mUsingDelegates) OnActivateDelegate(isActive); }
}