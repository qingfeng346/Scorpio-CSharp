//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Similar to SpringPosition, but also moves the panel's clipping. Works in local coordinates.
/// </summary>

[RequireComponent(typeof(UIPanel))]
[AddComponentMenu("NGUI/Internal/Spring Panel")]
public class SpringPanel : IgnoreTimeScale
{
	public Vector3 target = Vector3.zero;
	public float strength = 10f;

	public delegate void OnFinished ();
	public OnFinished onFinished;

	UIPanel mPanel;
	Transform mTrans;
	float mThreshold = 0f;
	UIDraggablePanel mDrag;

	/// <summary>
	/// Cache the transform.
	/// </summary>

	void Start ()
	{
		mPanel = GetComponent<UIPanel>();
		mDrag = GetComponent<UIDraggablePanel>();
		mTrans = transform;
	}

	/// <summary>
	/// Advance toward the target position.
	/// </summary>

	void Update ()
	{
		float delta = UpdateRealTimeDelta();

		if (mThreshold == 0f) mThreshold = (target - mTrans.localPosition).magnitude * 0.005f;

		bool trigger = false;
		Vector3 before = mTrans.localPosition;
		Vector3 after = NGUIMath.SpringLerp(mTrans.localPosition, target, strength, delta);

		if (mThreshold >= Vector3.Magnitude(after - target))
		{
			after = target;
			enabled = false;
			trigger = true;
		}
		mTrans.localPosition = after;

		Vector3 offset = after - before;
		Vector4 cr = mPanel.clipRange;
		cr.x -= offset.x;
		cr.y -= offset.y;
		mPanel.clipRange = cr;

		if (mDrag != null) mDrag.UpdateScrollbars(false);
		if (trigger && onFinished != null) onFinished();
	}

	/// <summary>
	/// Start the tweening process.
	/// </summary>

	static public SpringPanel Begin (GameObject go, Vector3 pos, float strength)
	{
		SpringPanel sp = go.GetComponent<SpringPanel>();
		if (sp == null) sp = go.AddComponent<SpringPanel>();
		sp.target = pos;
		sp.strength = strength;
		sp.onFinished = null;

		if (!sp.enabled)
		{
			sp.mThreshold = 0f;
			sp.enabled = true;
		}
		return sp;
	}
}