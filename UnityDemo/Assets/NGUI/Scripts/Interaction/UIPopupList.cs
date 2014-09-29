//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Popup list can be used to display pop-up menus and drop-down lists.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Popup List")]
public class UIPopupList : MonoBehaviour
{
	/// <summary>
	/// Current popup list. Only available during the OnSelectionChange event callback.
	/// </summary>

	static public UIPopupList current;

	const float animSpeed = 0.15f;

	public enum Position
	{
		Auto,
		Above,
		Below,
	}

	public delegate void OnSelectionChange (string item);

	/// <summary>
	/// Atlas used by the sprites.
	/// </summary>

	public UIAtlas atlas;

	/// <summary>
	/// Font used by the labels.
	/// </summary>

	public UIFont font;

	/// <summary>
	/// Label with text to auto-update, if any.
	/// </summary>

	public UILabel textLabel;

	/// <summary>
	/// Name of the sprite used to create the popup's background.
	/// </summary>

	public string backgroundSprite;

	/// <summary>
	/// Name of the sprite used to highlight items.
	/// </summary>

	public string highlightSprite;

	/// <summary>
	/// Popup list's display style.
	/// </summary>

	public Position position = Position.Auto;

	/// <summary>
	/// New line-delimited list of items.
	/// </summary>

	public List<string> items = new List<string>();

	/// <summary>
	/// Amount of padding added to labels.
	/// </summary>

	public Vector2 padding = new Vector3(4f, 4f);

	/// <summary>
	/// Scaling factor applied to labels within the drop-down menu.
	/// </summary>

	public float textScale = 1f;

	/// <summary>
	/// Color tint applied to labels inside the list.
	/// </summary>

	public Color textColor = Color.white;

	/// <summary>
	/// Color tint applied to the background.
	/// </summary>

	public Color backgroundColor = Color.white;

	/// <summary>
	/// Color tint applied to the highlighter.
	/// </summary>

	public Color highlightColor = new Color(152f / 255f, 1f, 51f / 255f, 1f);

	/// <summary>
	/// Whether the popup list is animated or not. Disable for better performance.
	/// </summary>

	public bool isAnimated = true;

	/// <summary>
	/// Whether the popup list's values will be localized.
	/// </summary>

	public bool isLocalized = false;

	/// <summary>
	/// Target game object that will be notified when selection changes.
	/// </summary>

	public GameObject eventReceiver;

	/// <summary>
	/// Function to call when the selection changes. Function prototype: void OnSelectionChange (string selectedItemName);
	/// </summary>

	public string functionName = "OnSelectionChange";

	/// <summary>
	/// Delegate that will be called when the selection changes. Faster than using the 'eventReceiver'.
	/// </summary>

	public OnSelectionChange onSelectionChange;

	[HideInInspector][SerializeField] string mSelectedItem;
	UIPanel mPanel;
	GameObject mChild;
	UISprite mBackground;
	UISprite mHighlight;
	UILabel mHighlightedLabel = null;
	List<UILabel> mLabelList = new List<UILabel>();
	float mBgBorder = 0f;

	/// <summary>
	/// Whether the popup list is currently open.
	/// </summary>

	public bool isOpen { get { return mChild != null; } }

	/// <summary>
	/// Current selection.
	/// </summary>

	public string selection
	{
		get
		{
			return mSelectedItem;
		}
		set
		{
			if (mSelectedItem != value)
			{
				mSelectedItem = value;
				if (mSelectedItem == null) return;
				
				if (textLabel != null)
				{
					textLabel.text = (isLocalized) ? Localization.Localize(value) : value;
#if UNITY_EDITOR
					UnityEditor.EditorUtility.SetDirty(textLabel.gameObject);
#endif
				}

				current = this;
				if (onSelectionChange != null) onSelectionChange(mSelectedItem);

				if (eventReceiver != null && !string.IsNullOrEmpty(functionName) && Application.isPlaying)
				{
					eventReceiver.SendMessage(functionName, mSelectedItem, SendMessageOptions.DontRequireReceiver);
				}
				current = null;
			}
		}
	}

	/// <summary>
	/// Whether the popup list will be handling keyboard, joystick and controller events.
	/// </summary>

	bool handleEvents
	{
		get
		{
			UIButtonKeys keys = GetComponent<UIButtonKeys>();
			return (keys == null || !keys.enabled);
		}
		set
		{
			UIButtonKeys keys = GetComponent<UIButtonKeys>();
			if (keys != null) keys.enabled = !value;
		}
	}

	/// <summary>
	/// Send out the selection message on start.
	/// </summary>

	void Start ()
	{
		// Automatically choose the first item
		if (string.IsNullOrEmpty(mSelectedItem))
		{
			if (items.Count > 0) selection = items[0];
		}
		else
		{
			string s = mSelectedItem;
			mSelectedItem = null;
			selection = s;
		}
	}

	/// <summary>
	/// Localize the text label.
	/// </summary>

	void OnLocalize (Localization loc)
	{
		if (isLocalized && textLabel != null)
		{
			textLabel.text = loc.Get(mSelectedItem);
		}
	}

	/// <summary>
	/// Visibly highlight the specified transform by moving the highlight sprite to be over it.
	/// </summary>

	void Highlight (UILabel lbl, bool instant)
	{
		if (mHighlight != null)
		{
			// Don't allow highlighting while the label is animating to its intended position
			TweenPosition tp = lbl.GetComponent<TweenPosition>();
			if (tp != null && tp.enabled) return;

			mHighlightedLabel = lbl;

			UIAtlas.Sprite sp = mHighlight.GetAtlasSprite();
			if (sp == null) return;

			float scaleFactor = atlas.pixelSize;
			float offsetX = (sp.inner.xMin - sp.outer.xMin) * scaleFactor;
			float offsetY = (sp.inner.yMin - sp.outer.yMin) * scaleFactor;

			Vector3 pos = lbl.cachedTransform.localPosition + new Vector3(-offsetX, offsetY, 1f);

			if (instant || !isAnimated)
			{
				mHighlight.cachedTransform.localPosition = pos;
			}
			else
			{
				TweenPosition.Begin(mHighlight.gameObject, 0.1f, pos).method = UITweener.Method.EaseOut;
			}
		}
	}

	/// <summary>
	/// Event function triggered when the mouse hovers over an item.
	/// </summary>

	void OnItemHover (GameObject go, bool isOver)
	{
		if (isOver)
		{
			UILabel lbl = go.GetComponent<UILabel>();
			Highlight(lbl, false);
		}
	}

	/// <summary>
	/// Select the specified label.
	/// </summary>

	void Select (UILabel lbl, bool instant)
	{
		Highlight(lbl, instant);
		
		UIEventListener listener = lbl.gameObject.GetComponent<UIEventListener>();
		selection = listener.parameter as string;

		UIButtonSound[] sounds = GetComponents<UIButtonSound>();

		for (int i = 0, imax = sounds.Length; i < imax; ++i)
		{
			UIButtonSound snd = sounds[i];

			if (snd.trigger == UIButtonSound.Trigger.OnClick)
			{
				NGUITools.PlaySound(snd.audioClip, snd.volume, 1f);
			}
		}
	}

	/// <summary>
	/// Event function triggered when the drop-down list item gets clicked on.
	/// </summary>

	void OnItemPress (GameObject go, bool isPressed) { if (isPressed) Select(go.GetComponent<UILabel>(), true); }

	/// <summary>
	/// React to key-based input.
	/// </summary>

	void OnKey (KeyCode key)
	{
		if (enabled && NGUITools.GetActive(gameObject) && handleEvents)
		{
			int index = mLabelList.IndexOf(mHighlightedLabel);

			if (key == KeyCode.UpArrow)
			{
				if (index > 0)
				{
					Select(mLabelList[--index], false);
				}
			}
			else if (key == KeyCode.DownArrow)
			{
				if (index + 1 < mLabelList.Count)
				{
					Select(mLabelList[++index], false);
				}
			}
			else if (key == KeyCode.Escape)
			{
				OnSelect(false);
			}
		}
	}

	/// <summary>
	/// Get rid of the popup dialog when the selection gets lost.
	/// </summary>

	void OnSelect (bool isSelected)
	{
		if (!isSelected && mChild != null)
		{
			mLabelList.Clear();
			handleEvents = false;

			if (isAnimated)
			{
				UIWidget[] widgets = mChild.GetComponentsInChildren<UIWidget>();

				for (int i = 0, imax = widgets.Length; i < imax; ++i)
				{
					UIWidget w = widgets[i];
					Color c = w.color;
					c.a = 0f;
					TweenColor.Begin(w.gameObject, animSpeed, c).method = UITweener.Method.EaseOut;
				}

				Collider[] cols = mChild.GetComponentsInChildren<Collider>();
				for (int i = 0, imax = cols.Length; i < imax; ++i) cols[i].enabled = false;
				Destroy(mChild, animSpeed);
			}
			else
			{
				Destroy(mChild);
			}

			mBackground = null;
			mHighlight = null;
			mChild = null;
		}
	}

	/// <summary>
	/// Helper function that causes the widget to smoothly fade in.
	/// </summary>

	void AnimateColor (UIWidget widget)
	{
		Color c = widget.color;
		widget.color = new Color(c.r, c.g, c.b, 0f);
		TweenColor.Begin(widget.gameObject, animSpeed, c).method = UITweener.Method.EaseOut;
	}

	/// <summary>
	/// Helper function that causes the widget to smoothly move into position.
	/// </summary>

	void AnimatePosition (UIWidget widget, bool placeAbove, float bottom)
	{
		Vector3 target = widget.cachedTransform.localPosition;
		Vector3 start = placeAbove ? new Vector3(target.x, bottom, target.z) : new Vector3(target.x, 0f, target.z);

		widget.cachedTransform.localPosition = start;

		GameObject go = widget.gameObject;
		TweenPosition.Begin(go, animSpeed, target).method = UITweener.Method.EaseOut;
	}

	/// <summary>
	/// Helper function that causes the widget to smoothly grow until it reaches its original size.
	/// </summary>

	void AnimateScale (UIWidget widget, bool placeAbove, float bottom)
	{
		GameObject go = widget.gameObject;
		Transform t = widget.cachedTransform;
		float minSize = font.size * textScale + mBgBorder * 2f;

		Vector3 scale = t.localScale;
		t.localScale = new Vector3(scale.x, minSize, scale.z);
		TweenScale.Begin(go, animSpeed, scale).method = UITweener.Method.EaseOut;

		if (placeAbove)
		{
			Vector3 pos = t.localPosition;
			t.localPosition = new Vector3(pos.x, pos.y - scale.y + minSize, pos.z);
			TweenPosition.Begin(go, animSpeed, pos).method = UITweener.Method.EaseOut;
		}
	}

	/// <summary>
	/// Helper function used to animate widgets.
	/// </summary>

	void Animate (UIWidget widget, bool placeAbove, float bottom)
	{
		AnimateColor(widget);
		AnimatePosition(widget, placeAbove, bottom);
	}

	/// <summary>
	/// Display the drop-down list when the game object gets clicked on.
	/// </summary>

	void OnClick()
	{
		if (mChild == null && atlas != null && font != null && items.Count > 0)
		{
			mLabelList.Clear();

			// Disable the navigation script
			handleEvents = true;

			// Automatically locate the panel responsible for this object
			if (mPanel == null) mPanel = UIPanel.Find(transform, true);

			// Calculate the dimensions of the object triggering the popup list so we can position it below it
			Transform myTrans = transform;
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(myTrans.parent, myTrans);

			// Create the root object for the list
			mChild = new GameObject("Drop-down List");
			mChild.layer = gameObject.layer;

			Transform t = mChild.transform;
			t.parent = myTrans.parent;
			t.localPosition = bounds.min;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;

			// Add a sprite for the background
			mBackground = NGUITools.AddSprite(mChild, atlas, backgroundSprite);
			mBackground.pivot = UIWidget.Pivot.TopLeft;
			mBackground.depth = NGUITools.CalculateNextDepth(mPanel.gameObject);
			mBackground.color = backgroundColor;

			// We need to know the size of the background sprite for padding purposes
			Vector4 bgPadding = mBackground.border;
			mBgBorder = bgPadding.y;

			mBackground.cachedTransform.localPosition = new Vector3(0f, bgPadding.y, 0f);

			// Add a sprite used for the selection
			mHighlight = NGUITools.AddSprite(mChild, atlas, highlightSprite);
			mHighlight.pivot = UIWidget.Pivot.TopLeft;
			mHighlight.color = highlightColor;

			UIAtlas.Sprite hlsp = mHighlight.GetAtlasSprite();
			if (hlsp == null) return;

			float hlspHeight = hlsp.inner.yMin - hlsp.outer.yMin;
			float fontScale = font.size * font.pixelSize * textScale;
			float x = 0f, y = -padding.y;
			List<UILabel> labels = new List<UILabel>();

			// Run through all items and create labels for each one
			for (int i = 0, imax = items.Count; i < imax; ++i)
			{
				string s = items[i];

				UILabel lbl = NGUITools.AddWidget<UILabel>(mChild);
				lbl.pivot = UIWidget.Pivot.TopLeft;
				lbl.font = font;
				lbl.text = (isLocalized && Localization.instance != null) ? Localization.instance.Get(s) : s;
				lbl.color = textColor;
				lbl.cachedTransform.localPosition = new Vector3(bgPadding.x + padding.x, y, -1f);
				lbl.MakePixelPerfect();

				if (textScale != 1f)
				{
					Vector3 scale = lbl.cachedTransform.localScale;
					lbl.cachedTransform.localScale = scale * textScale;
				}
				labels.Add(lbl);

				y -= fontScale;
				y -= padding.y;
				x = Mathf.Max(x, lbl.relativeSize.x * fontScale);

				// Add an event listener
				UIEventListener listener = UIEventListener.Get(lbl.gameObject);
				listener.onHover = OnItemHover;
				listener.onPress = OnItemPress;
				listener.parameter = s;

				// Move the selection here if this is the right label
				if (mSelectedItem == s) Highlight(lbl, true);

				// Add this label to the list
				mLabelList.Add(lbl);
			}

			// The triggering widget's width should be the minimum allowed width
			x = Mathf.Max(x, bounds.size.x - (bgPadding.x + padding.x) * 2f);

			Vector3 bcCenter = new Vector3((x * 0.5f) / fontScale, -0.5f, 0f);
			Vector3 bcSize = new Vector3(x / fontScale, (fontScale + padding.y) / fontScale, 1f);

			// Run through all labels and add colliders
			for (int i = 0, imax = labels.Count; i < imax; ++i)
			{
				UILabel lbl = labels[i];
				BoxCollider bc = NGUITools.AddWidgetCollider(lbl.gameObject);
				bcCenter.z = bc.center.z;
				bc.center = bcCenter;
				bc.size = bcSize;
			}

			x += (bgPadding.x + padding.x) * 2f;
			y -= bgPadding.y;

			// Scale the background sprite to envelop the entire set of items
			mBackground.cachedTransform.localScale = new Vector3(x, -y + bgPadding.y, 1f);

			// Scale the highlight sprite to envelop a single item
			float scaleFactor = 2f * atlas.pixelSize;
			mHighlight.cachedTransform.localScale = new Vector3(
				   x - (bgPadding.x + padding.x) * 2f + (hlsp.inner.xMin - hlsp.outer.xMin) * scaleFactor,
				   fontScale + hlspHeight * scaleFactor, 1f);

			bool placeAbove = (position == Position.Above);

			if (position == Position.Auto)
			{
				UICamera cam = UICamera.FindCameraForLayer(gameObject.layer);

				if (cam != null)
				{
					Vector3 viewPos = cam.cachedCamera.WorldToViewportPoint(myTrans.position);
					placeAbove = (viewPos.y < 0.5f);
				}
			}

			// If the list should be animated, let's animate it by expanding it
			if (isAnimated)
			{
				float bottom = y + fontScale;
				Animate(mHighlight, placeAbove, bottom);
				for (int i = 0, imax = labels.Count; i < imax; ++i) Animate(labels[i], placeAbove, bottom);
				AnimateColor(mBackground);
				AnimateScale(mBackground, placeAbove, bottom);
			}

			// If we need to place the popup list above the item, we need to reposition everything by the size of the list
			if (placeAbove)
			{
				t.localPosition = new Vector3(bounds.min.x, bounds.max.y - y - bgPadding.y, bounds.min.z);
			}
		}
		else OnSelect(false);
	}
}
