//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using AnimationOrTween;

/// <summary>
/// Play the specified animation on click.
/// Sends out the "OnAnimationFinished()" notification to the target when the animation finishes.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button Play Animation")]
public class UIButtonPlayAnimation : MonoBehaviour
{
	/// <summary>
	/// Target animation to activate.
	/// </summary>

	public Animation target;

	/// <summary>
	/// Optional clip name, if the animation has more than one clip.
	/// </summary>

	public string clipName;

	/// <summary>
	/// Which event will trigger the animation.
	/// </summary>

	public Trigger trigger = Trigger.OnClick;

	/// <summary>
	/// Which direction to animate in.
	/// </summary>

	public Direction playDirection = Direction.Forward;

	/// <summary>
	/// Whether the animation's position will be reset on play or will continue from where it left off.
	/// </summary>

	public bool resetOnPlay = false;

	/// <summary>
	/// Whether the selected object (this button) will be cleared when the animation gets activated.
	/// </summary>

	public bool clearSelection = false;

	/// <summary>
	/// What to do if the target game object is currently disabled.
	/// </summary>

	public EnableCondition ifDisabledOnPlay = EnableCondition.DoNothing;

	/// <summary>
	/// What to do with the target when the animation finishes.
	/// </summary>

	public DisableCondition disableWhenFinished = DisableCondition.DoNotDisable;

	/// <summary>
	/// Event receiver to trigger the callback on when the animation finishes.
	/// </summary>

	public GameObject eventReceiver;

	/// <summary>
	/// Function to call on the event receiver when the animation finishes.
	/// </summary>

	public string callWhenFinished;

	/// <summary>
	/// Delegate to call. Faster than using 'eventReceiver', and allows for multiple receivers.
	/// </summary>

	public ActiveAnimation.OnFinished onFinished;

	bool mStarted = false;
	bool mHighlighted = false;

	void Start () { mStarted = true; }

	void OnEnable () { if (mStarted && mHighlighted) OnHover(UICamera.IsHighlighted(gameObject)); }

	void OnHover (bool isOver)
	{
		if (enabled)
		{
			if ( trigger == Trigger.OnHover ||
				(trigger == Trigger.OnHoverTrue && isOver) ||
				(trigger == Trigger.OnHoverFalse && !isOver))
			{
				Play(isOver);
			}
			mHighlighted = isOver;
		}
	}

	void OnPress (bool isPressed)
	{
		if (enabled)
		{
			if ( trigger == Trigger.OnPress ||
				(trigger == Trigger.OnPressTrue && isPressed) ||
				(trigger == Trigger.OnPressFalse && !isPressed))
			{
				Play(isPressed);
			}
		}
	}

	void OnClick ()
	{
		if (enabled && trigger == Trigger.OnClick)
		{
			Play(true);
		}
	}

	void OnDoubleClick ()
	{
		if (enabled && trigger == Trigger.OnDoubleClick)
		{
			Play(true);
		}
	}

	void OnSelect (bool isSelected)
	{
		if (enabled)
		{
			if (trigger == Trigger.OnSelect ||
				(trigger == Trigger.OnSelectTrue && isSelected) ||
				(trigger == Trigger.OnSelectFalse && !isSelected))
			{
				Play(true);
			}
		}
	}

	void OnActivate (bool isActive)
	{
		if (enabled)
		{
			if (trigger == Trigger.OnActivate ||
				(trigger == Trigger.OnActivateTrue && isActive) ||
				(trigger == Trigger.OnActivateFalse && !isActive))
			{
				Play(isActive);
			}
		}
	}

	void Play (bool forward)
	{
		if (target == null) target = GetComponentInChildren<Animation>();

		if (target != null)
		{
			if (clearSelection && UICamera.selectedObject == gameObject) UICamera.selectedObject = null;

			int pd = -(int)playDirection;
			Direction dir = forward ? playDirection : ((Direction)pd);
			ActiveAnimation anim = ActiveAnimation.Play(target, clipName, dir, ifDisabledOnPlay, disableWhenFinished);
			if (anim == null) return;
			if (resetOnPlay) anim.Reset();

			// Set the delegate
			anim.onFinished = onFinished;

			// Copy the event receiver
			if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
			{
				anim.eventReceiver = eventReceiver;
				anim.callWhenFinished = callWhenFinished;
			}
			else anim.eventReceiver = null;
		}
	}
}