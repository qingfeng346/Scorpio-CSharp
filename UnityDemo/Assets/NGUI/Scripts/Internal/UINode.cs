//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

#if UNITY_3_5 || UNITY_4_0

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UIPanel creates one of these records for each child transform under it.
/// This makes it possible to watch for transform changes, and if something does
/// change -- rebuild the buffer as necessary.
/// </summary>

public class UINode
{
	int mVisibleFlag = -1;

	public Transform trans;		// Managed transform
	public UIWidget widget;		// Widget on this transform, if any
	public int changeFlag = -1;	// -1 = not checked, 0 = not changed, 1 = changed
	
	bool mLastActive = false;	// Last active state
	Vector3 mLastPos;			// Last local position, used to see if it has changed
	Quaternion mLastRot;		// Last local rotation
	Vector3 mLastScale;			// Last local scale

	GameObject mGo;
	float mLastAlpha = 0f;

	/// <summary>
	/// -1 = not initialized, 0 = not visible, 1 = visible.
	/// </summary>

	public int visibleFlag
	{
		get
		{
			return (widget != null) ? widget.visibleFlag : mVisibleFlag;
		}
		set
		{
			if (widget != null) widget.visibleFlag = value;
			else mVisibleFlag = value;
		}
	}

	/// <summary>
	/// Must always have a transform.
	/// </summary>

	public UINode (Transform t)
	{
		trans = t;
		mLastPos = trans.localPosition;
		mLastRot = trans.localRotation;
		mLastScale = trans.localScale;
		mGo = t.gameObject;
	}

	/// <summary>
	/// Check to see if the local transform has changed since the last time this function was called.
	/// </summary>

	public bool HasChanged ()
	{
		bool isActive = NGUITools.GetActive(mGo) && (widget == null || (widget.enabled && widget.isVisible));
		bool changed = (mLastActive != isActive);

		if (widget != null)
		{
			float alpha = widget.finalAlpha;

			if (alpha != mLastAlpha)
			{
				mLastAlpha = alpha;
				changed = true;
			}
		}

		if (changed || (isActive &&
			(mLastPos != trans.localPosition ||
			 mLastRot != trans.localRotation ||
			 mLastScale != trans.localScale)))
		{
			mLastActive = isActive;
			mLastPos = trans.localPosition;
			mLastRot = trans.localRotation;
			mLastScale = trans.localScale;
			return true;
		}
		return changed;
	}
}
#endif
