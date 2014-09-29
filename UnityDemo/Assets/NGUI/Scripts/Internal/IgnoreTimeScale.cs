//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Implements common functionality for monobehaviours that wish to have a timeScale-independent deltaTime.
/// </summary>

[AddComponentMenu("NGUI/Internal/Ignore TimeScale Behaviour")]
public class IgnoreTimeScale : MonoBehaviour
{
	float mRt = 0f;
	float mTimeStart = 0f;
	float mTimeDelta = 0f;
	float mActual = 0f;
	bool mTimeStarted = false;

	/// <summary>
	/// Real time of the last time UpdateRealTimeDelta() was called.
	/// </summary>

	public float realTime { get { return mRt; } }

	/// <summary>
	/// Equivalent of Time.deltaTime not affected by timeScale, provided that UpdateRealTimeDelta() was called in the Update().
	/// </summary>

	public float realTimeDelta { get { return mTimeDelta; } }

	/// <summary>
	/// Clear the started flag;
	/// </summary>

	protected virtual void OnEnable ()
	{
		mTimeStarted = true;
		mTimeDelta = 0f;
		mTimeStart = Time.realtimeSinceStartup;
	}

	/// <summary>
	/// Update the 'realTimeDelta' parameter. Should be called once per frame.
	/// </summary>

	protected float UpdateRealTimeDelta ()
	{
		mRt = Time.realtimeSinceStartup;

		if (mTimeStarted)
		{
			float delta = mRt - mTimeStart;
			mActual += Mathf.Max(0f, delta);
			mTimeDelta = 0.001f * Mathf.Round(mActual * 1000f);
			mActual -= mTimeDelta;
			if (mTimeDelta > 1f) mTimeDelta = 1f;
			mTimeStart = mRt;
		}
		else
		{
			mTimeStarted = true;
			mTimeStart = mRt;
			mTimeDelta = 0f;
		}
		return mTimeDelta;
	}
}