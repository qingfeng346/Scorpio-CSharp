//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the camera's orthographic size.
/// </summary>

[RequireComponent(typeof(Camera))]
[AddComponentMenu("NGUI/Tween/Orthographic Size")]
public class TweenOrthoSize : UITweener
{
	public float from;
	public float to;

	Camera mCam;

	/// <summary>
	/// Camera that's being tweened.
	/// </summary>

	public Camera cachedCamera { get { if (mCam == null) mCam = camera; return mCam; } }

	/// <summary>
	/// Current field of view value.
	/// </summary>

	public float orthoSize
	{
		get { return cachedCamera.orthographicSize; }
		set { cachedCamera.orthographicSize = value; }
	}

	/// <summary>
	/// Perform the tween.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished)
	{
		cachedCamera.orthographicSize = from * (1f - factor) + to * factor;
	}

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenOrthoSize Begin (GameObject go, float duration, float to)
	{
		TweenOrthoSize comp = UITweener.Begin<TweenOrthoSize>(go, duration);
		comp.from = comp.orthoSize;
		comp.to = to;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}
}
