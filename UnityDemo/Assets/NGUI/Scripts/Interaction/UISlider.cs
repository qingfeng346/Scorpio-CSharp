//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Simple slider functionality.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Slider")]
public class UISlider : IgnoreTimeScale
{
	public enum Direction
	{
		Horizontal,
		Vertical,
	}

	public delegate void OnValueChange (float val);

	/// <summary>
	/// Current slider. This value is set prior to the callback function being triggered.
	/// </summary>

	static public UISlider current;

	/// <summary>
	/// Object used for the foreground.
	/// </summary>

	public Transform foreground;

	/// <summary>
	/// Object that acts as a thumb.
	/// </summary>

	public Transform thumb;

	/// <summary>
	/// Direction the slider will expand in.
	/// </summary>

	public Direction direction = Direction.Horizontal;

	/// <summary>
	/// Event receiver that will be notified of the value changes.
	/// </summary>

	public GameObject eventReceiver;

	/// <summary>
	/// Function on the event receiver that will receive the value changes.
	/// </summary>

	public string functionName = "OnSliderChange";

	/// <summary>
	/// Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
	/// </summary>

	public OnValueChange onValueChange;

	/// <summary>
	/// Number of steps the slider should be divided into. For example 5 means possible values of 0, 0.25, 0.5, 0.75, and 1.0.
	/// </summary>

	public int numberOfSteps = 0;

	// Used to be public prior to 1.87
	[HideInInspector][SerializeField] float rawValue = 1f;

	BoxCollider mCol;
	Transform mTrans;
	Transform mFGTrans;
	UIWidget mFGWidget;
	UISprite mFGFilled;
	bool mInitDone = false;
	Vector2 mSize = Vector2.zero;
	Vector2 mCenter = Vector3.zero;

	/// <summary>
	/// Value of the slider.
	/// </summary>

	public float sliderValue
	{
		get
		{
			float val = rawValue;
			if (numberOfSteps > 1) val = Mathf.Round(val * (numberOfSteps - 1)) / (numberOfSteps - 1);
			return val;
		}
		set
		{
			Set(value, false);
		}
	}

	/// <summary>
	/// Change the full size of the slider, in case you need to.
	/// </summary>

	public Vector2 fullSize { get { return mSize; } set { if (mSize != value) { mSize = value; ForceUpdate(); } } }

	/// <summary>
	/// Initialize the cached values.
	/// </summary>

	void Init ()
	{
		mInitDone = true;

		if (foreground != null)
		{
			mFGWidget = foreground.GetComponent<UIWidget>();
			mFGFilled = (mFGWidget != null) ? mFGWidget as UISprite : null;
			mFGTrans = foreground.transform;
			if (mSize == Vector2.zero) mSize = foreground.localScale;
			if (mCenter == Vector2.zero) mCenter = foreground.localPosition + foreground.localScale * 0.5f;
		}
		else if (mCol != null)
		{
			if (mSize == Vector2.zero) mSize = mCol.size;
			if (mCenter == Vector2.zero) mCenter = mCol.center;
		}
		else
		{
			Debug.LogWarning("UISlider expected to find a foreground object or a box collider to work with", this);
		}
	}

	/// <summary>
	/// Ensure that we have a background and a foreground object to work with.
	/// </summary>

	void Awake ()
	{
		mTrans = transform;
		mCol = collider as BoxCollider;
	}

	/// <summary>
	/// We want to receive drag events from the thumb.
	/// </summary>

	void Start ()
	{
		Init();

		if (Application.isPlaying && thumb != null && thumb.collider != null)
		{
			UIEventListener listener = UIEventListener.Get(thumb.gameObject);
			listener.onPress += OnPressThumb;
			listener.onDrag += OnDragThumb;
		}
		Set(rawValue, true);
	}

	/// <summary>
	/// Update the slider's position on press.
	/// </summary>

	void OnPress (bool pressed) { if (pressed && UICamera.currentTouchID != -100) UpdateDrag(); }

	/// <summary>
	/// When dragged, figure out where the mouse is and calculate the updated value of the slider.
	/// </summary>

	void OnDrag (Vector2 delta) { UpdateDrag(); }

	/// <summary>
	/// Callback from the thumb.
	/// </summary>

	void OnPressThumb (GameObject go, bool pressed) { if (pressed) UpdateDrag(); }

	/// <summary>
	/// Callback from the thumb.
	/// </summary>

	void OnDragThumb (GameObject go, Vector2 delta) { UpdateDrag(); }

	/// <summary>
	/// Watch for key events and adjust the value accordingly.
	/// </summary>

	void OnKey (KeyCode key)
	{
		float step = (numberOfSteps > 1f) ? 1f / (numberOfSteps - 1) : 0.125f;

		if (direction == Direction.Horizontal)
		{
			if		(key == KeyCode.LeftArrow)	Set(rawValue - step, false);
			else if (key == KeyCode.RightArrow) Set(rawValue + step, false);
		}
		else
		{
			if		(key == KeyCode.DownArrow)	Set(rawValue - step, false);
			else if (key == KeyCode.UpArrow)	Set(rawValue + step, false);
		}
	}

	/// <summary>
	/// Update the slider's position based on the mouse.
	/// </summary>

	void UpdateDrag ()
	{
		// Create a plane for the slider
		if (mCol == null || UICamera.currentCamera == null || UICamera.currentTouch == null) return;

		// Don't consider the slider for click events
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;

		// Create a ray and a plane
		Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
		Plane plane = new Plane(mTrans.rotation * Vector3.back, mTrans.position);

		// If the ray doesn't hit the plane, do nothing
		float dist;
		if (!plane.Raycast(ray, out dist)) return;

		// Collider's bottom-left corner in local space
		Vector3 localOrigin = mTrans.localPosition + (Vector3)(mCenter - mSize * 0.5f);
		Vector3 localOffset = mTrans.localPosition - localOrigin;

		// Direction to the point on the plane in scaled local space
		Vector3 localCursor = mTrans.InverseTransformPoint(ray.GetPoint(dist));
		Vector3 dir = localCursor + localOffset;

		// Update the slider
		Set((direction == Direction.Horizontal) ? dir.x / mSize.x : dir.y / mSize.y, false);
	}

	/// <summary>
	/// Update the visible slider.
	/// </summary>

	void Set (float input, bool force)
	{
		if (!mInitDone) Init();

		// Clamp the input
		float val = Mathf.Clamp01(input);
		if (val < 0.001f) val = 0f;

		float prevStep = sliderValue;

		// Save the raw value
		rawValue = val;

		// Take steps into account
		float stepValue = sliderValue;

		// If the stepped value doesn't match the last one, it's time to update
		if (force || prevStep != stepValue)
		{
			Vector3 scale = mSize;

#if UNITY_EDITOR
			if (Application.isPlaying)
			{
				if (direction == Direction.Horizontal) scale.x *= stepValue;
				else scale.y *= stepValue;
			}
#else
			if (direction == Direction.Horizontal) scale.x *= stepValue;
			else scale.y *= stepValue;
#endif

#if UNITY_EDITOR
			if (Application.isPlaying)
#endif
			{
				if (mFGFilled != null && mFGFilled.type == UISprite.Type.Filled)
				{
					mFGFilled.fillAmount = stepValue;
				}
				else if (foreground != null)
				{
					mFGTrans.localScale = scale;

					if (mFGWidget != null)
					{
						if (stepValue > 0.001f)
						{
							mFGWidget.enabled = true;
							mFGWidget.MarkAsChanged();
						}
						else
						{
							mFGWidget.enabled = false;
						}
					}
				}
			}

			if (thumb != null)
			{
				Vector3 pos = thumb.localPosition;

				if (mFGFilled != null && mFGFilled.type == UISprite.Type.Filled)
				{
					if (mFGFilled.fillDirection == UISprite.FillDirection.Horizontal)
					{
						pos.x = mFGFilled.invert ? mSize.x - scale.x : scale.x;
					}
					else if (mFGFilled.fillDirection == UISprite.FillDirection.Vertical)
					{
						pos.y = mFGFilled.invert ? mSize.y - scale.y : scale.y;
					}
					else
					{
						Debug.LogWarning("Slider thumb is only supported with Horizontal or Vertical fill direction", this);
					}
				}
				else if (direction == Direction.Horizontal)
				{
					pos.x = scale.x;
				}
				else
				{
					pos.y = scale.y;
				}
				thumb.localPosition = pos;
			}

			current = this;

			if (eventReceiver != null && !string.IsNullOrEmpty(functionName) && Application.isPlaying)
			{
				eventReceiver.SendMessage(functionName, stepValue, SendMessageOptions.DontRequireReceiver);
			}
			if (onValueChange != null) onValueChange(stepValue);
			current = null;
		}
	}

	/// <summary>
	/// Force-update the slider. Useful if you've changed the properties and want it to update visually.
	/// </summary>

	public void ForceUpdate () { Set(rawValue, true); }
}
