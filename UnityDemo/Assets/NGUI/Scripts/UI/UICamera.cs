//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This script should be attached to each camera that's used to draw the objects with
/// UI components on them. This may mean only one camera (main camera or your UI camera),
/// or multiple cameras if you happen to have multiple viewports. Failing to attach this
/// script simply means that objects drawn by this camera won't receive UI notifications:
/// 
/// - OnHover (isOver) is sent when the mouse hovers over a collider or moves away.
/// - OnPress (isDown) is sent when a mouse button gets pressed on the collider.
/// - OnSelect (selected) is sent when a mouse button is released on the same object as it was pressed on.
/// - OnClick () is sent with the same conditions as OnSelect, with the added check to see if the mouse has not moved much. UICamera.currentTouchID tells you which button was clicked.
/// - OnDoubleClick () is sent when the click happens twice within a fourth of a second. UICamera.currentTouchID tells you which button was clicked.
/// - OnDrag (delta) is sent when a mouse or touch gets pressed on a collider and starts dragging it.
/// - OnDrop (gameObject) is sent when the mouse or touch get released on a different collider than the one that was being dragged.
/// - OnInput (text) is sent when typing (after selecting a collider by clicking on it).
/// - OnTooltip (show) is sent when the mouse hovers over a collider for some time without moving.
/// - OnScroll (float delta) is sent out when the mouse scroll wheel is moved.
/// - OnKey (KeyCode key) is sent when keyboard or controller input is used.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Camera")]
[RequireComponent(typeof(Camera))]
public class UICamera : MonoBehaviour
{
	/// <summary>
	/// Whether the touch event will be sending out the OnClick notification at the end.
	/// </summary>

	public enum ClickNotification
	{
		None,
		Always,
		BasedOnDelta,
	}

	/// <summary>
	/// Ambiguous mouse, touch, or controller event.
	/// </summary>

	public class MouseOrTouch
	{
		public Vector2 pos;				// Current position of the mouse or touch event
		public Vector2 delta;			// Delta since last update
		public Vector2 totalDelta;		// Delta since the event started being tracked

		public Camera pressedCam;		// Camera that the OnPress(true) was fired with

		public GameObject current;		// The current game object under the touch or mouse
		public GameObject pressed;		// The last game object to receive OnPress
		public GameObject dragged;		// The last game object to receive OnDrag

		public float clickTime = 0f;	// The last time a click event was sent out

		public ClickNotification clickNotification = ClickNotification.Always;
		public bool touchBegan = true;
		public bool pressStarted = false;
		public bool dragStarted = false;
	}

	class Highlighted
	{
		public GameObject go;
		public int counter = 0;
	}

	/// <summary>
	/// If 'true', currently hovered object will be shown in the top left corner.
	/// </summary>

	public bool debug = false;

	/// <summary>
	/// Whether the mouse input is used.
	/// </summary>

	public bool useMouse = true;

	/// <summary>
	/// Whether the touch-based input is used.
	/// </summary>

	public bool useTouch = true;

	/// <summary>
	/// Whether multi-touch is allowed.
	/// </summary>

	public bool allowMultiTouch = true;

	/// <summary>
	/// Whether the keyboard events will be processed.
	/// </summary>

	public bool useKeyboard = true;

	/// <summary>
	/// Whether the joystick and controller events will be processed.
	/// </summary>

	public bool useController = true;

	/// <summary>
	/// If 'true', once a press event is started on some object, that object will be the only one that will be
	/// receiving future events until the press event is finally released, regardless of where that happens.
	/// If 'false', the press event won't be locked to the original object, and other objects will be receiving
	/// OnPress(true) and OnPress(false) events as the touch enters and leaves their area.
	/// </summary>

	public bool stickyPress = true;

	/// <summary>
	/// Which layers will receive events.
	/// </summary>

	public LayerMask eventReceiverMask = -1;

	/// <summary>
	/// Whether raycast events will be clipped just like widgets. This essentially means that clicking on a collider that
	/// happens to have been clipped will not produce a hit. Note that having this enabled will slightly reduce performance.
	/// </summary>

	public bool clipRaycasts = true;

	/// <summary>
	/// How long of a delay to expect before showing the tooltip.
	/// </summary>

	public float tooltipDelay = 1f;

	/// <summary>
	/// Whether the tooltip will disappear as soon as the mouse moves (false) or only if the mouse moves outside of the widget's area (true).
	/// </summary>

	public bool stickyTooltip = true;

	/// <summary>
	/// How much the mouse has to be moved after pressing a button before it starts to send out drag events.
	/// </summary>

	public float mouseDragThreshold = 4f;

	/// <summary>
	/// How far the mouse is allowed to move in pixels before it's no longer considered for click events, if the click notification is based on delta.
	/// </summary>

	public float mouseClickThreshold = 10f;

	/// <summary>
	/// How much the mouse has to be moved after pressing a button before it starts to send out drag events.
	/// </summary>

	public float touchDragThreshold = 40f;

	/// <summary>
	/// How far the touch is allowed to move in pixels before it's no longer considered for click events, if the click notification is based on delta.
	/// </summary>

	public float touchClickThreshold = 40f;

	/// <summary>
	/// Raycast range distance. By default it's as far as the camera can see.
	/// </summary>

	public float rangeDistance = -1f;

	/// <summary>
	/// Name of the axis used for scrolling.
	/// </summary>

	public string scrollAxisName = "Mouse ScrollWheel";

	/// <summary>
	/// Name of the axis used to send up and down key events.
	/// </summary>

	public string verticalAxisName = "Vertical";

	/// <summary>
	/// Name of the axis used to send left and right key events.
	/// </summary>

	public string horizontalAxisName = "Horizontal";

	/// <summary>
	/// Various keys used by the camera.
	/// </summary>

	public KeyCode submitKey0 = KeyCode.Return;
	public KeyCode submitKey1 = KeyCode.JoystickButton0;
	public KeyCode cancelKey0 = KeyCode.Escape;
	public KeyCode cancelKey1 = KeyCode.JoystickButton1;

	public delegate void OnCustomInput ();

	/// <summary>
	/// Custom input processing logic, if desired. For example: WP7 touches.
	/// Use UICamera.current to get the current camera.
	/// </summary>

	static public OnCustomInput onCustomInput;

	/// <summary>
	/// Whether tooltips will be shown or not.
	/// </summary>

	static public bool showTooltips = true;

	/// <summary>
	/// Position of the last touch (or mouse) event.
	/// </summary>

	static public Vector2 lastTouchPosition = Vector2.zero;

	/// <summary>
	/// Last raycast hit prior to sending out the event. This is useful if you want detailed information
	/// about what was actually hit in your OnClick, OnHover, and other event functions.
	/// </summary>

	static public RaycastHit lastHit;

	/// <summary>
	/// UICamera that sent out the event.
	/// </summary>

	static public UICamera current = null;

	/// <summary>
	/// Last camera active prior to sending out the event. This will always be the camera that actually sent out the event.
	/// </summary>

	static public Camera currentCamera = null;

	/// <summary>
	/// ID of the touch or mouse operation prior to sending out the event. Mouse ID is '-1' for left, '-2' for right mouse button, '-3' for middle.
	/// </summary>

	static public int currentTouchID = -1;

	/// <summary>
	/// Current touch, set before any event function gets called.
	/// </summary>

	static public MouseOrTouch currentTouch = null;

	/// <summary>
	/// Whether an input field currently has focus.
	/// </summary>

	static public bool inputHasFocus = false;

	/// <summary>
	/// If set, this game object will receive all events regardless of whether they were handled or not.
	/// </summary>

	static public GameObject genericEventHandler;

	/// <summary>
	/// If events don't get handled, they will be forwarded to this game object.
	/// </summary>

	static public GameObject fallThrough;

	// List of all active cameras in the scene
	static List<UICamera> mList = new List<UICamera>();

	// List of currently highlighted items
	static List<Highlighted> mHighlighted = new List<Highlighted>();

	// Selected widget (for input)
	static GameObject mSel = null;

	// Mouse events
	static MouseOrTouch[] mMouse = new MouseOrTouch[] { new MouseOrTouch(), new MouseOrTouch(), new MouseOrTouch() };

	// The last object to receive OnHover
	static GameObject mHover;

	// Joystick/controller/keyboard event
	static MouseOrTouch mController = new MouseOrTouch();

	// Used to ensure that joystick-based controls don't trigger that often
	static float mNextEvent = 0f;

	// List of currently active touches
	static Dictionary<int, MouseOrTouch> mTouches = new Dictionary<int, MouseOrTouch>();

	// Tooltip widget (mouse only)
	GameObject mTooltip = null;

	// Mouse input is turned off on iOS
	Camera mCam = null;
	LayerMask mLayerMask;
	float mTooltipTime = 0f;
	bool mIsEditor = false;

	/// <summary>
	/// Helper function that determines if this script should be handling the events.
	/// </summary>

	bool handlesEvents { get { return eventHandler == this; } }

	/// <summary>
	/// Caching is always preferable for performance.
	/// </summary>

	public Camera cachedCamera { get { if (mCam == null) mCam = camera; return mCam; } }

	/// <summary>
	/// Set to 'true' just before OnDrag-related events are sent (this includes OnPress events that resulted from dragging).
	/// </summary>

	static public bool isDragging = false;

	/// <summary>
	/// The object hit by the last Raycast that was the result of a mouse or touch event.
	/// </summary>

	static public GameObject hoveredObject;

	/// <summary>
	/// Option to manually set the selected game object.
	/// </summary>

	static public GameObject selectedObject
	{
		get
		{
			return mSel;
		}
		set
		{
			if (mSel != value)
			{
				if (mSel != null)
				{
					UICamera uicam = FindCameraForLayer(mSel.layer);

					if (uicam != null)
					{
						current = uicam;
						currentCamera = uicam.mCam;
						Notify(mSel, "OnSelect", false);
						if (uicam.useController || uicam.useKeyboard) Highlight(mSel, false);
						current = null;
					}
				}

				mSel = value;

				if (mSel != null)
				{
					UICamera uicam = FindCameraForLayer(mSel.layer);

					if (uicam != null)
					{
						current = uicam;
						currentCamera = uicam.mCam;
						if (uicam.useController || uicam.useKeyboard) Highlight(mSel, true);
						Notify(mSel, "OnSelect", true);
						current = null;
					}
					else Debug.Log("The fuck? " + mList.Count);
				}
			}
		}
	}

	/// <summary>
	/// Number of active touches from all sources.
	/// </summary>

	static public int touchCount
	{
		get
		{
			int count = 0;

			for (int i = 0; i < mTouches.Count; ++i)
				if (mTouches[i].pressed != null)
					++count;

			for (int i = 0; i < mMouse.Length; ++i)
				if (mMouse[i].pressed != null)
					++count;

			if (mController.pressed != null)
				++count;

			return count;
		}
	}

	/// <summary>
	/// Number of active drag events from all sources.
	/// </summary>

	static public int dragCount
	{
		get
		{
			int count = 0;

			for (int i = 0; i < mTouches.Count; ++i)
			{
				if (mTouches[i].dragged != null)
					++count;
			}

			for (int i = 0; i < mMouse.Length; ++i)
				if (mMouse[i].dragged != null)
					++count;

			if (mController.dragged != null)
				++count;

			return count;
		}
	}

	/// <summary>
	/// Clear the list on application quit (also when Play mode is exited)
	/// </summary>

	void OnApplicationQuit () { mHighlighted.Clear(); }

	/// <summary>
	/// Convenience function that returns the main HUD camera.
	/// </summary>

	static public Camera mainCamera
	{
		get
		{
			UICamera mouse = eventHandler;
			return (mouse != null) ? mouse.cachedCamera : null;
		}
	}

	/// <summary>
	/// Event handler for all types of events.
	/// </summary>

	static public UICamera eventHandler
	{
		get
		{
			for (int i = 0; i < mList.Count; ++i)
			{
				// Invalid or inactive entry -- keep going
				UICamera cam = mList[i];
				if (cam == null || !cam.enabled || !NGUITools.GetActive(cam.gameObject)) continue;
				return cam;
			}
			return null;
		}
	}

	/// <summary>
	/// Static comparison function used for sorting.
	/// </summary>

	static int CompareFunc (UICamera a, UICamera b)
	{
		if (a.cachedCamera.depth < b.cachedCamera.depth) return 1;
		if (a.cachedCamera.depth > b.cachedCamera.depth) return -1;
		return 0;
	}

	/// <summary>
	/// Returns the object under the specified position.
	/// </summary>

	static public bool Raycast (Vector3 inPos, ref RaycastHit hit)
	{
		for (int i = 0; i < mList.Count; ++i)
		{
			UICamera cam = mList[i];
			
			// Skip inactive scripts
			if (!cam.enabled || !NGUITools.GetActive(cam.gameObject)) continue;

			// Convert to view space
			currentCamera = cam.cachedCamera;
			Vector3 pos = currentCamera.ScreenToViewportPoint(inPos);

			// If it's outside the camera's viewport, do nothing
			if (pos.x < 0f || pos.x > 1f || pos.y < 0f || pos.y > 1f) continue;

			// Cast a ray into the screen
			Ray ray = currentCamera.ScreenPointToRay(inPos);

			// Raycast into the screen
			int mask = currentCamera.cullingMask & (int)cam.eventReceiverMask;
			float dist = (cam.rangeDistance > 0f) ? cam.rangeDistance : currentCamera.farClipPlane - currentCamera.nearClipPlane;

			// If raycasts should be clipped by panels, we need to find a panel for each hit
			if (cam.clipRaycasts)
			{
				RaycastHit[] hits = Physics.RaycastAll(ray, dist, mask);

				if (hits.Length > 1)
				{
					System.Array.Sort(hits, delegate(RaycastHit r1, RaycastHit r2) { return r1.distance.CompareTo(r2.distance); });

					for (int b = 0, bmax = hits.Length; b < bmax; ++b)
					{
						if (IsVisible(ref hits[b]))
						{
							hit = hits[b];
							return true;
						}
					}
				}
				else if (hits.Length == 1 && IsVisible(ref hits[0]))
				{
					hit = hits[0];
					return true;
				}
				continue;
			}
			if (Physics.Raycast(ray, out hit, dist, mask)) return true;
		}
		return false;
	}

	/// <summary>
	/// Helper function to check if the specified hit is visible by the panel.
	/// </summary>

	static bool IsVisible (ref RaycastHit hit)
	{
		UIPanel panel = NGUITools.FindInParents<UIPanel>(hit.collider.gameObject);

		if (panel == null || panel.IsVisible(hit.point))
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Find the camera responsible for handling events on objects of the specified layer.
	/// </summary>

	static public UICamera FindCameraForLayer (int layer)
	{
		int layerMask = 1 << layer;

		for (int i = 0; i < mList.Count; ++i)
		{
			UICamera cam = mList[i];
			Camera uc = cam.cachedCamera;
			if ((uc != null) && (uc.cullingMask & layerMask) != 0) return cam;
		}
		return null;
	}

	/// <summary>
	/// Using the keyboard will result in 1 or -1, depending on whether up or down keys have been pressed.
	/// </summary>

	static int GetDirection (KeyCode up, KeyCode down)
	{
		if (Input.GetKeyDown(up)) return 1;
		if (Input.GetKeyDown(down)) return -1;
		return 0;
	}

	/// <summary>
	/// Using the keyboard will result in 1 or -1, depending on whether up or down keys have been pressed.
	/// </summary>

	static int GetDirection (KeyCode up0, KeyCode up1, KeyCode down0, KeyCode down1)
	{
		if (Input.GetKeyDown(up0) || Input.GetKeyDown(up1)) return 1;
		if (Input.GetKeyDown(down0) || Input.GetKeyDown(down1)) return -1;
		return 0;
	}

	/// <summary>
	/// Using the joystick to move the UI results in 1 or -1 if the threshold has been passed, mimicking up/down keys.
	/// </summary>

	static int GetDirection (string axis)
	{
		float time = Time.realtimeSinceStartup;

		if (mNextEvent < time)
		{
			float val = Input.GetAxis(axis);

			if (val > 0.75f)
			{
				mNextEvent = time + 0.25f;
				return 1;
			}

			if (val < -0.75f)
			{
				mNextEvent = time + 0.25f;
				return -1;
			}
		}
		return 0;
	}

	/// <summary>
	/// Returns whether the widget should be currently highlighted as far as the UICamera knows.
	/// </summary>

	static public bool IsHighlighted (GameObject go)
	{
		for (int i = mHighlighted.Count; i > 0; )
		{
			Highlighted hl = mHighlighted[--i];
			if (hl.go == go) return true;
		}
		return false;
	}

	/// <summary>
	/// Apply or remove highlighted (hovered) state from the specified object.
	/// </summary>

	static void Highlight (GameObject go, bool highlighted)
	{
		if (go != null)
		{
			for (int i = mHighlighted.Count; i > 0; )
			{
				Highlighted hl = mHighlighted[--i];

				if (hl == null || hl.go == null)
				{
					mHighlighted.RemoveAt(i);
				}
				else if (hl.go == go)
				{
					if (highlighted)
					{
						++hl.counter;
					}
					else if (--hl.counter < 1)
					{
						mHighlighted.Remove(hl);
						Notify(go, "OnHover", false);
					}
					return;
				}
			}

			if (highlighted)
			{
				Highlighted hl = new Highlighted();
				hl.go = go;
				hl.counter = 1;
				mHighlighted.Add(hl);
				Notify(go, "OnHover", true);
			}
		}
	}

	/// <summary>
	/// Generic notification function. Used in place of SendMessage to shorten the code and allow for more than one receiver.
	/// </summary>

	static public void Notify (GameObject go, string funcName, object obj)
	{
		if (go != null)
		{
			go.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);

			if (genericEventHandler != null && genericEventHandler != go)
			{
				genericEventHandler.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	/// <summary>
	/// Get or create a touch event.
	/// </summary>

	static public MouseOrTouch GetTouch (int id)
	{
		MouseOrTouch touch = null;

		if (!mTouches.TryGetValue(id, out touch))
		{
			touch = new MouseOrTouch();
			touch.touchBegan = true;
			mTouches.Add(id, touch);
		}
		return touch;
	}

	/// <summary>
	/// Remove a touch event from the list.
	/// </summary>

	static public void RemoveTouch (int id) { mTouches.Remove(id); }

	/// <summary>
	/// Add this camera to the list.
	/// </summary>

	void Awake ()
	{
		mList.Add(this);

#if !UNITY_3_5 && !UNITY_4_0
		// We don't want the camera to send out any kind of mouse events
		cachedCamera.eventMask = 0;
#endif

		if (Application.platform == RuntimePlatform.Android ||
			Application.platform == RuntimePlatform.IPhonePlayer)
		{
			useMouse = false;
			useTouch = true;

			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				useKeyboard = false;
				useController = false;
			}
		}
		else if (Application.platform == RuntimePlatform.PS3 ||
				 Application.platform == RuntimePlatform.XBOX360)
		{
			useMouse = false;
			useTouch = false;
			useKeyboard = false;
			useController = true;
		}
		else if (Application.platform == RuntimePlatform.WindowsEditor ||
				 Application.platform == RuntimePlatform.OSXEditor)
		{
			mIsEditor = true;
		}

		// Save the starting mouse position
		mMouse[0].pos.x = Input.mousePosition.x;
		mMouse[0].pos.y = Input.mousePosition.y;
		lastTouchPosition = mMouse[0].pos;

		// If no event receiver mask was specified, use the camera's mask
		if (eventReceiverMask == -1) eventReceiverMask = cachedCamera.cullingMask;
	}

	/// <summary>
	/// Remove this camera from the list.
	/// </summary>

	void OnDestroy () { mList.Remove(this); }

	/// <summary>
	/// Sort the list when enabled.
	/// </summary>

	void OnEnable () { mList.Sort(CompareFunc); }

	/// <summary>
	/// Update the object under the mouse if we're not using touch-based input.
	/// </summary>

	void FixedUpdate ()
	{
		if (useMouse && Application.isPlaying && handlesEvents)
		{
			hoveredObject = Raycast(Input.mousePosition, ref lastHit) ? lastHit.collider.gameObject : fallThrough;
			if (hoveredObject == null) hoveredObject = genericEventHandler;
			for (int i = 0; i < 3; ++i) mMouse[i].current = hoveredObject;
		}
	}

	/// <summary>
	/// Check the input and send out appropriate events.
	/// </summary>

	void Update ()
	{
		// Only the first UI layer should be processing events
		if (!Application.isPlaying || !handlesEvents) return;

		current = this;

		// Update mouse input
		if (useMouse || (useTouch && mIsEditor)) ProcessMouse();

		// Process touch input
		if (useTouch) ProcessTouches();

		// Custom input processing
		if (onCustomInput != null) onCustomInput();

		// Clear the selection on the cancel key, but only if mouse input is allowed
		if (useMouse && mSel != null && ((cancelKey0 != KeyCode.None && Input.GetKeyDown(cancelKey0)) ||
			(cancelKey1 != KeyCode.None && Input.GetKeyDown(cancelKey1)))) selectedObject = null;

		// Forward the input to the selected object
		if (mSel != null)
		{
			string input = Input.inputString;

			// Adding support for some macs only having the "Delete" key instead of "Backspace"
			if (useKeyboard && Input.GetKeyDown(KeyCode.Delete)) input += "\b";

			if (input.Length > 0)
			{
				if (!stickyTooltip && mTooltip != null) ShowTooltip(false);
				Notify(mSel, "OnInput", input);
			}
		}
		else inputHasFocus = false;

		// Update the keyboard and joystick events
		if (mSel != null) ProcessOthers();

		// If it's time to show a tooltip, inform the object we're hovering over
		if (useMouse && mHover != null)
		{
			float scroll = Input.GetAxis(scrollAxisName);
			if (scroll != 0f) Notify(mHover, "OnScroll", scroll);

			if (showTooltips && mTooltipTime != 0f && (mTooltipTime < Time.realtimeSinceStartup ||
				Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
			{
				mTooltip = mHover;
				ShowTooltip(true);
			}
		}
		current = null;
	}

	/// <summary>
	/// Update mouse input.
	/// </summary>

	public void ProcessMouse ()
	{
		bool updateRaycast = (useMouse && Time.timeScale < 0.9f);

		if (!updateRaycast)
		{
			for (int i = 0; i < 3; ++i)
			{
				if (Input.GetMouseButton(i) || Input.GetMouseButtonUp(i))
				{
					updateRaycast = true;
					break;
				}
			}
		}

		// Update the position and delta
		mMouse[0].pos = Input.mousePosition;
		mMouse[0].delta = mMouse[0].pos - lastTouchPosition;

		bool posChanged = (mMouse[0].pos != lastTouchPosition);
		lastTouchPosition = mMouse[0].pos;

		// Update the object under the mouse
		if (updateRaycast)
		{
			hoveredObject = Raycast(Input.mousePosition, ref lastHit) ? lastHit.collider.gameObject : fallThrough;
			if (hoveredObject == null) hoveredObject = genericEventHandler;
			mMouse[0].current = hoveredObject;
		}

		// Propagate the updates to the other mouse buttons
		for (int i = 1; i < 3; ++i)
		{
			mMouse[i].pos = mMouse[0].pos;
			mMouse[i].delta = mMouse[0].delta;
			mMouse[i].current = mMouse[0].current;
		}

		// Is any button currently pressed?
		bool isPressed = false;

		for (int i = 0; i < 3; ++i)
		{
			if (Input.GetMouseButton(i))
			{
				isPressed = true;
				break;
			}
		}

		if (isPressed)
		{
			// A button was pressed -- cancel the tooltip
			mTooltipTime = 0f;
		}
		else if (useMouse && posChanged && (!stickyTooltip || mHover != mMouse[0].current))
		{
			if (mTooltipTime != 0f)
			{
				// Delay the tooltip
				mTooltipTime = Time.realtimeSinceStartup + tooltipDelay;
			}
			else if (mTooltip != null)
			{
				// Hide the tooltip
				ShowTooltip(false);
			}
		}

		// The button was released over a different object -- remove the highlight from the previous
		if (useMouse && !isPressed && mHover != null && mHover != mMouse[0].current)
		{
			if (mTooltip != null) ShowTooltip(false);
			Highlight(mHover, false);
			mHover = null;
		}

		// Process all 3 mouse buttons as individual touches
		if (useMouse)
		{
			for (int i = 0; i < 3; ++i)
			{
				bool pressed = Input.GetMouseButtonDown(i);
				bool unpressed = Input.GetMouseButtonUp(i);
	
				currentTouch = mMouse[i];
				currentTouchID = -1 - i;
	
				// We don't want to update the last camera while there is a touch happening
				if (pressed) currentTouch.pressedCam = currentCamera;
				else if (currentTouch.pressed != null) currentCamera = currentTouch.pressedCam;
	
				// Process the mouse events
				ProcessTouch(pressed, unpressed);
			}
			currentTouch = null;
		}

		// If nothing is pressed and there is an object under the touch, highlight it
		if (useMouse && !isPressed && mHover != mMouse[0].current)
		{
			mTooltipTime = Time.realtimeSinceStartup + tooltipDelay;
			mHover = mMouse[0].current;
			Highlight(mHover, true);
		}
	}

	/// <summary>
	/// Update touch-based events.
	/// </summary>

	public void ProcessTouches ()
	{
		for (int i = 0; i < Input.touchCount; ++i)
		{
			Touch input = Input.GetTouch(i);

			currentTouchID = allowMultiTouch ? input.fingerId : 1;
			currentTouch = GetTouch(currentTouchID);

			bool pressed = (input.phase == TouchPhase.Began) || currentTouch.touchBegan;
			bool unpressed = (input.phase == TouchPhase.Canceled) || (input.phase == TouchPhase.Ended);
			currentTouch.touchBegan = false;

			if (pressed)
			{
				currentTouch.delta = Vector2.zero;
			}
			else
			{
				// Although input.deltaPosition can be used, calculating it manually is safer (just in case)
				currentTouch.delta = input.position - currentTouch.pos;
			}

			currentTouch.pos = input.position;
			hoveredObject = Raycast(currentTouch.pos, ref lastHit) ? lastHit.collider.gameObject : fallThrough;
			if (hoveredObject == null) hoveredObject = genericEventHandler;
			currentTouch.current = hoveredObject;
			lastTouchPosition = currentTouch.pos;

			// We don't want to update the last camera while there is a touch happening
			if (pressed) currentTouch.pressedCam = currentCamera;
			else if (currentTouch.pressed != null) currentCamera = currentTouch.pressedCam;

			// Double-tap support
			if (input.tapCount > 1) currentTouch.clickTime = Time.realtimeSinceStartup;

			// Process the events from this touch
			ProcessTouch(pressed, unpressed);

			// If the touch has ended, remove it from the list
			if (unpressed) RemoveTouch(currentTouchID);
			currentTouch = null;

			// Don't consider other touches
			if (!allowMultiTouch) break;
		}
	}

	/// <summary>
	/// Process keyboard and joystick events.
	/// </summary>

	public void ProcessOthers ()
	{
		currentTouchID = -100;
		currentTouch = mController;

		// If this is an input field, ignore WASD and Space key presses
		inputHasFocus = (mSel != null && mSel.GetComponent<UIInput>() != null);

		bool submitKeyDown = (submitKey0 != KeyCode.None && Input.GetKeyDown(submitKey0)) || (submitKey1 != KeyCode.None && Input.GetKeyDown(submitKey1));
		bool submitKeyUp = (submitKey0 != KeyCode.None && Input.GetKeyUp(submitKey0)) || (submitKey1 != KeyCode.None && Input.GetKeyUp(submitKey1));

		if (submitKeyDown || submitKeyUp)
		{
			currentTouch.current = mSel;
			ProcessTouch(submitKeyDown, submitKeyUp);
			currentTouch.current = null;
		}

		int vertical = 0;
		int horizontal = 0;

		if (useKeyboard)
		{
			if (inputHasFocus)
			{
				vertical += GetDirection(KeyCode.UpArrow, KeyCode.DownArrow);
				horizontal += GetDirection(KeyCode.RightArrow, KeyCode.LeftArrow);
			}
			else
			{
				vertical += GetDirection(KeyCode.W, KeyCode.UpArrow, KeyCode.S, KeyCode.DownArrow);
				horizontal += GetDirection(KeyCode.D, KeyCode.RightArrow, KeyCode.A, KeyCode.LeftArrow);
			}
		}

		if (useController)
		{
			if (!string.IsNullOrEmpty(verticalAxisName)) vertical += GetDirection(verticalAxisName);
			if (!string.IsNullOrEmpty(horizontalAxisName)) horizontal += GetDirection(horizontalAxisName);
		}

		// Send out key notifications
		if (vertical != 0) Notify(mSel, "OnKey", vertical > 0 ? KeyCode.UpArrow : KeyCode.DownArrow);
		if (horizontal != 0) Notify(mSel, "OnKey", horizontal > 0 ? KeyCode.RightArrow : KeyCode.LeftArrow);
		if (useKeyboard && Input.GetKeyDown(KeyCode.Tab)) Notify(mSel, "OnKey", KeyCode.Tab);

		// Send out the cancel key notification
		if (cancelKey0 != KeyCode.None && Input.GetKeyDown(cancelKey0)) Notify(mSel, "OnKey", KeyCode.Escape);
		if (cancelKey1 != KeyCode.None && Input.GetKeyDown(cancelKey1)) Notify(mSel, "OnKey", KeyCode.Escape);

		currentTouch = null;
	}

	/// <summary>
	/// Process the events of the specified touch.
	/// </summary>

	public void ProcessTouch (bool pressed, bool unpressed)
	{
		// Whether we're using the mouse
		bool isMouse = (currentTouch == mMouse[0] || currentTouch == mMouse[1] || currentTouch == mMouse[2]);
		float drag   = isMouse ? mouseDragThreshold : touchDragThreshold;
		float click  = isMouse ? mouseClickThreshold : touchClickThreshold;

		// Send out the press message
		if (pressed)
		{
			if (mTooltip != null) ShowTooltip(false);

			currentTouch.pressStarted = true;
			Notify(currentTouch.pressed, "OnPress", false);
			currentTouch.pressed = currentTouch.current;
			currentTouch.dragged = currentTouch.current;
			currentTouch.clickNotification = isMouse ? ClickNotification.BasedOnDelta : ClickNotification.Always;
			currentTouch.totalDelta = Vector2.zero;
			currentTouch.dragStarted = false;
			Notify(currentTouch.pressed, "OnPress", true);

			// Clear the selection
			if (currentTouch.pressed != mSel)
			{
				if (mTooltip != null) ShowTooltip(false);
				selectedObject = null;
			}
		}
		else
		{
			// If the user is pressing down and has dragged the touch away from the original object,
			// unpress the original object and notify the new object that it is now being pressed on.
			if (!stickyPress && !unpressed && currentTouch.pressStarted && currentTouch.pressed != hoveredObject)
			{
				isDragging = true;
				Notify(currentTouch.pressed, "OnPress", false);
				currentTouch.pressed = hoveredObject;
				Notify(currentTouch.pressed, "OnPress", true);
				isDragging = false;
			}

			if (currentTouch.pressed != null)
			{
				float mag = currentTouch.delta.magnitude;

				if (mag != 0f)
				{
					// Keep track of the total movement
					currentTouch.totalDelta += currentTouch.delta;
					mag = currentTouch.totalDelta.magnitude;

					// If the drag event has not yet started, see if we've dragged the touch far enough to start it
					if (!currentTouch.dragStarted && drag < mag)
					{
						currentTouch.dragStarted = true;
						currentTouch.delta = currentTouch.totalDelta;
					}

					// If we're dragging the touch, send out drag events
					if (currentTouch.dragStarted)
					{
						if (mTooltip != null) ShowTooltip(false);

						isDragging = true;
						bool isDisabled = (currentTouch.clickNotification == ClickNotification.None);
						Notify(currentTouch.dragged, "OnDrag", currentTouch.delta);
						isDragging = false;

						if (isDisabled)
						{
							// If the notification status has already been disabled, keep it as such
							currentTouch.clickNotification = ClickNotification.None;
						}
						else if (currentTouch.clickNotification == ClickNotification.BasedOnDelta && click < mag)
						{
							// We've dragged far enough to cancel the click
							currentTouch.clickNotification = ClickNotification.None;
						}
					}
				}
			}
		}

		// Send out the unpress message
		if (unpressed)
		{
			currentTouch.pressStarted = false;
			if (mTooltip != null) ShowTooltip(false);

			if (currentTouch.pressed != null)
			{
				Notify(currentTouch.pressed, "OnPress", false);

				// Send a hover message to the object, but don't add it to the list of hovered items
				// as it's already present. This happens when the mouse is released over the same button
				// it was pressed on, and since it already had its 'OnHover' event, it never got
				// Highlight(false), so we simply re-notify it so it can update the visible state.
				if (useMouse && currentTouch.pressed == mHover) Notify(currentTouch.pressed, "OnHover", true);

				// If the button/touch was released on the same object, consider it a click and select it
				if (currentTouch.dragged == currentTouch.current ||
					(currentTouch.clickNotification != ClickNotification.None &&
					currentTouch.totalDelta.magnitude < drag))
				{
					if (currentTouch.pressed != mSel)
					{
						mSel = currentTouch.pressed;
						Notify(currentTouch.pressed, "OnSelect", true);
					}
					else
					{
						mSel = currentTouch.pressed;
					}

					// If the touch should consider clicks, send out an OnClick notification
					if (currentTouch.clickNotification != ClickNotification.None)
					{
						float time = Time.realtimeSinceStartup;

						Notify(currentTouch.pressed, "OnClick", null);

						if (currentTouch.clickTime + 0.35f > time)
						{
							Notify(currentTouch.pressed, "OnDoubleClick", null);
						}
						currentTouch.clickTime = time;
					}
				}
				else // The button/touch was released on a different object
				{
					// Send a drop notification (for drag & drop)
					Notify(currentTouch.current, "OnDrop", currentTouch.dragged);
				}
			}
			currentTouch.dragStarted = false;
			currentTouch.pressed = null;
			currentTouch.dragged = null;
		}
	}

	/// <summary>
	/// Show or hide the tooltip.
	/// </summary>

	public void ShowTooltip (bool val)
	{
		mTooltipTime = 0f;
		Notify(mTooltip, "OnTooltip", val);
		if (!val) mTooltip = null;
	}

#if UNITY_EDITOR
	void OnGUI ()
	{
		if (debug && lastHit.collider != null)
		{
			GUILayout.Label("Last Hit: " + NGUITools.GetHierarchy(lastHit.collider.gameObject).Replace("\"", ""));
		}
	}
#endif
}
