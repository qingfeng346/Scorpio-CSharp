//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using AnimationOrTween;

/// <summary>
/// Mainly an internal script used by UIButtonPlayAnimation, but can also be used to call
/// the specified function on the game object after it finishes animating.
/// </summary>

[RequireComponent(typeof(Animation))]
[AddComponentMenu("NGUI/Internal/Active Animation")]
public class ActiveAnimation : IgnoreTimeScale
{
	public delegate void OnFinished (ActiveAnimation anim);

	/// <summary>
	/// Delegate for subscriptions. Faster than using the 'eventReceiver' and allows for multiple receivers.
	/// </summary>

	public OnFinished onFinished;

	/// <summary>
	/// Game object on which to call the callback function.
	/// </summary>

	public GameObject eventReceiver;

	/// <summary>
	/// Function to call when the animation finishes playing.
	/// </summary>

	public string callWhenFinished;

	Animation mAnim;
	Direction mLastDirection = Direction.Toggle;
	Direction mDisableDirection = Direction.Toggle;
	bool mNotify = false;

	/// <summary>
	/// Whether the animation is currently playing.
	/// </summary>

	public bool isPlaying
	{
		get
		{
			if (mAnim == null) return false;

			foreach (AnimationState state in mAnim)
			{
				if (!mAnim.IsPlaying(state.name)) continue;

				if (mLastDirection == Direction.Forward)
				{
					if (state.time < state.length) return true;
				}
				else if (mLastDirection == Direction.Reverse)
				{
					if (state.time > 0f) return true;
				}
				else return true;
			}
			return false;
		}
	}

	/// <summary>
	/// Manually reset the active animation to the beginning.
	/// </summary>

	public void Reset ()
	{
		if (mAnim != null)
		{
			foreach (AnimationState state in mAnim)
			{
				if (mLastDirection == Direction.Reverse) state.time = state.length;
				else if (mLastDirection == Direction.Forward) state.time = 0f;
			}
		}
	}

	/// <summary>
	/// Notify the target when the animation finishes playing.
	/// </summary>

	void Update ()
	{
		float delta = UpdateRealTimeDelta();
		if (delta == 0f) return;

		if (mAnim != null)
		{
			bool isPlaying = false;

			foreach (AnimationState state in mAnim)
			{
				if (!mAnim.IsPlaying(state.name)) continue;
				float movement = state.speed * delta;
				state.time += movement;

				if (movement < 0f)
				{
					if (state.time > 0f) isPlaying = true;
					else state.time = 0f;
				}
				else
				{
					if (state.time < state.length) isPlaying = true;
					else state.time = state.length;
				}
			}

			mAnim.Sample();
			if (isPlaying) return;
			enabled = false;

			if (mNotify)
			{
				mNotify = false;

				// Notify the delegate
				if (onFinished != null) onFinished(this);

				// Notify the event listener
				if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
				{
					eventReceiver.SendMessage(callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
				}

				if (mDisableDirection != Direction.Toggle && mLastDirection == mDisableDirection)
				{
					NGUITools.SetActive(gameObject, false);
				}
			}
		}
		else enabled = false;
	}

	/// <summary>
	/// Play the specified animation.
	/// </summary>

	void Play (string clipName, Direction playDirection)
	{
		if (mAnim != null)
		{
			// We will sample the animation manually so that it works when the time is paused
			enabled = true;
			mAnim.enabled = false;

			// Determine the play direction
			if (playDirection == Direction.Toggle)
			{
				playDirection = (mLastDirection != Direction.Forward) ? Direction.Forward : Direction.Reverse;
			}

			bool noName = string.IsNullOrEmpty(clipName);

			// Play the animation if it's not playing already
			if (noName)
			{
				if (!mAnim.isPlaying) mAnim.Play();
			}
			else if (!mAnim.IsPlaying(clipName))
			{
				mAnim.Play(clipName);
			}

			// Update the animation speed based on direction -- forward or back
			foreach (AnimationState state in mAnim)
			{
				if (string.IsNullOrEmpty(clipName) || state.name == clipName)
				{
					float speed = Mathf.Abs(state.speed);
					state.speed = speed * (int)playDirection;

					// Automatically start the animation from the end if it's playing in reverse
					if (playDirection == Direction.Reverse && state.time == 0f) state.time = state.length;
					else if (playDirection == Direction.Forward && state.time == state.length) state.time = 0f;
				}
			}

			// Remember the direction for disable checks in Update()
			mLastDirection = playDirection;
			mNotify = true;
			mAnim.Sample();
		}
	}

	/// <summary>
	/// Play the specified animation on the specified object.
	/// </summary>

	static public ActiveAnimation Play (Animation anim, string clipName, Direction playDirection,
		EnableCondition enableBeforePlay, DisableCondition disableCondition)
	{
		if (!NGUITools.GetActive(anim.gameObject))
		{
			// If the object is disabled, don't do anything
			if (enableBeforePlay != EnableCondition.EnableThenPlay) return null;

			// Enable the game object before animating it
			NGUITools.SetActive(anim.gameObject, true);
			
			// Refresh all panels right away so that there is no one frame delay
			UIPanel[] panels = anim.gameObject.GetComponentsInChildren<UIPanel>();
			for (int i = 0, imax = panels.Length; i < imax; ++i) panels[i].Refresh();
		}

		ActiveAnimation aa = anim.GetComponent<ActiveAnimation>();
		if (aa == null) aa = anim.gameObject.AddComponent<ActiveAnimation>();
		aa.mAnim = anim;
		aa.mDisableDirection = (Direction)(int)disableCondition;
		aa.eventReceiver = null;
		aa.callWhenFinished = null;
		aa.onFinished = null;
		aa.Play(clipName, playDirection);
		return aa;
	}

	/// <summary>
	/// Play the specified animation.
	/// </summary>

	static public ActiveAnimation Play (Animation anim, string clipName, Direction playDirection)
	{
		return Play(anim, clipName, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
	}

	/// <summary>
	/// Play the specified animation.
	/// </summary>

	static public ActiveAnimation Play (Animation anim, Direction playDirection)
	{
		return Play(anim, null, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
	}
}
