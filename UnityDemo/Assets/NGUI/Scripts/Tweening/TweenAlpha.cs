//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's alpha.
/// </summary>

[AddComponentMenu("NGUI/Tween/Alpha")]
public class TweenAlpha : UITweener
{
	public float from = 1f;
	public float to = 1f;

	Transform mTrans;
	UIWidget mWidget;
	UIPanel mPanel;

	/// <summary>
	/// Current alpha.
	/// </summary>

	public float alpha
	{
		get
		{
			if (mWidget != null) return mWidget.alpha;
			if (mPanel != null) return mPanel.alpha;
			return 0f;
		}
		set
		{
			if (mWidget != null) mWidget.alpha = value;
			else if (mPanel != null) mPanel.alpha = value;
		}
	}

	/// <summary>
	/// Find all needed components.
	/// </summary>

	void Awake ()
	{
		mPanel = GetComponent<UIPanel>();
		if (mPanel == null) mWidget = GetComponentInChildren<UIWidget>();
	}

	/// <summary>
	/// Interpolate and update the alpha.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished) { alpha = Mathf.Lerp(from, to, factor); }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenAlpha Begin (GameObject go, float duration, float alpha)
	{
		TweenAlpha comp = UITweener.Begin<TweenAlpha>(go, duration);
		comp.from = comp.alpha;
		comp.to = alpha;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}
}
