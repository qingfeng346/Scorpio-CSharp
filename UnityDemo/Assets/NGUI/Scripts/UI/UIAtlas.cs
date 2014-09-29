//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// UI Atlas contains a collection of sprites inside one large texture atlas.
/// </summary>

[AddComponentMenu("NGUI/UI/Atlas")]
public class UIAtlas : MonoBehaviour
{
	// Ideally this class should be called UISpriteData and should be separate from UIAtlas...
	// But unfortunately I can't rename it or move it out as it will break backwards compatibility.

	[System.Serializable]
	public class Sprite
	{
		public string name = "Unity Bug";
		public Rect outer = new Rect(0f, 0f, 1f, 1f);
		public Rect inner = new Rect(0f, 0f, 1f, 1f);
		public bool rotated = false;

		// Padding is needed for trimmed sprites and is relative to sprite width and height
		public float paddingLeft	= 0f;
		public float paddingRight	= 0f;
		public float paddingTop		= 0f;
		public float paddingBottom	= 0f;

		public bool hasPadding { get { return paddingLeft != 0f || paddingRight != 0f || paddingTop != 0f || paddingBottom != 0f; } }
	}

	/// <summary>
	/// Pixels coordinates are values within the texture specified in pixels. They are more intuitive,
	/// but will likely change if the texture gets resized. TexCoord coordinates range from 0 to 1,
	/// and won't change if the texture is resized. You can switch freely from one to the other prior
	/// to modifying the texture used by the atlas.
	/// </summary>

	public enum Coordinates
	{
		Pixels,
		TexCoords,
	}

	// Material used by this atlas. Name is kept only for backwards compatibility, it used to be public.
	[HideInInspector][SerializeField] Material material;

	// List of all sprites inside the atlas. Name is kept only for backwards compatibility, it used to be public.
	[HideInInspector][SerializeField] List<Sprite> sprites = new List<Sprite>();

	// Currently active set of coordinates
	[HideInInspector][SerializeField] Coordinates mCoordinates = Coordinates.Pixels;

	// Size in pixels for the sake of MakePixelPerfect functions.
	[HideInInspector][SerializeField] float mPixelSize = 1f;

	// Replacement atlas can be used to completely bypass this atlas, pulling the data from another one instead.
	[HideInInspector][SerializeField] UIAtlas mReplacement;

	// Whether the atlas is using a pre-multiplied alpha material. -1 = not checked. 0 = no. 1 = yes.
	int mPMA = -1;

	/// <summary>
	/// Material used by the atlas.
	/// </summary>

	public Material spriteMaterial
	{
		get
		{
			return (mReplacement != null) ? mReplacement.spriteMaterial : material;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.spriteMaterial = value;
			}
			else
			{
				if (material == null)
				{
					mPMA = 0;
					material = value;
				}
				else
				{
					MarkAsDirty();
					mPMA = -1;
					material = value;
					MarkAsDirty();
				}
			}
		}
	}

	/// <summary>
	/// Whether the atlas is using a premultiplied alpha material.
	/// </summary>

	public bool premultipliedAlpha
	{
		get
		{
			if (mReplacement != null) return mReplacement.premultipliedAlpha;

			if (mPMA == -1)
			{
				Material mat = spriteMaterial;
				mPMA = (mat != null && mat.shader != null && mat.shader.name.Contains("Premultiplied")) ? 1 : 0;
			}
			return (mPMA == 1);
		}
	}

	/// <summary>
	/// List of sprites within the atlas.
	/// </summary>

	public List<Sprite> spriteList
	{
		get
		{
			return (mReplacement != null) ? mReplacement.spriteList : sprites;
		}
		set
		{
			if (mReplacement != null) mReplacement.spriteList = value;
			else sprites = value;
		}
	}

	/// <summary>
	/// Texture used by the atlas.
	/// </summary>

	public Texture texture { get { return (mReplacement != null) ? mReplacement.texture : (material != null ? material.mainTexture as Texture : null); } }

	/// <summary>
	/// Allows switching of the coordinate system from pixel coordinates to texture coordinates.
	/// </summary>

	public Coordinates coordinates
	{
		get
		{
			return (mReplacement != null) ? mReplacement.coordinates : mCoordinates;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.coordinates = value;
			}
			else if (mCoordinates != value)
			{
				if (material == null || material.mainTexture == null)
				{
					Debug.LogError("Can't switch coordinates until the atlas material has a valid texture");
					return;
				}

				mCoordinates = value;
				Texture tex = material.mainTexture;

				for (int i = 0, imax = sprites.Count; i < imax; ++i)
				{
					Sprite s = sprites[i];

					if (mCoordinates == Coordinates.TexCoords)
					{
						s.outer = NGUIMath.ConvertToTexCoords(s.outer, tex.width, tex.height);
						s.inner = NGUIMath.ConvertToTexCoords(s.inner, tex.width, tex.height);
					}
					else
					{
						s.outer = NGUIMath.ConvertToPixels(s.outer, tex.width, tex.height, true);
						s.inner = NGUIMath.ConvertToPixels(s.inner, tex.width, tex.height, true);
					}
				}
			}
		}
	}

	/// <summary>
	/// Pixel size is a multiplier applied to widgets dimensions when performing MakePixelPerfect() pixel correction.
	/// Most obvious use would be on retina screen displays. The resolution doubles, but with UIRoot staying the same
	/// for layout purposes, you can still get extra sharpness by switching to an HD atlas that has pixel size set to 0.5.
	/// </summary>

	public float pixelSize
	{
		get
		{
			return (mReplacement != null) ? mReplacement.pixelSize : mPixelSize;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.pixelSize = value;
			}
			else
			{
				float val = Mathf.Clamp(value, 0.25f, 4f);

				if (mPixelSize != val)
				{
					mPixelSize = val;
					MarkAsDirty();
				}
			}
		}
	}

	/// <summary>
	/// Setting a replacement atlas value will cause everything using this atlas to use the replacement atlas instead.
	/// Suggested use: set up all your widgets to use a dummy atlas that points to the real atlas. Switching that atlas
	/// to another one (for example an HD atlas) is then a simple matter of setting this field on your dummy atlas.
	/// </summary>

	public UIAtlas replacement
	{
		get
		{
			return mReplacement;
		}
		set
		{
			UIAtlas rep = value;
			if (rep == this) rep = null;

			if (mReplacement != rep)
			{
				if (rep != null && rep.replacement == this) rep.replacement = null;
				if (mReplacement != null) MarkAsDirty();
				mReplacement = rep;
				MarkAsDirty();
			}
		}
	}

	/// <summary>
	/// Convenience function that retrieves a sprite by name.
	/// </summary>

	public Sprite GetSprite (string name)
	{
		if (mReplacement != null)
		{
			return mReplacement.GetSprite(name);
		}
		else if (!string.IsNullOrEmpty(name))
		{
			for (int i = 0, imax = sprites.Count; i < imax; ++i)
			{
				Sprite s = sprites[i];

				// string.Equals doesn't seem to work with Flash export
				if (!string.IsNullOrEmpty(s.name) && name == s.name)
				{
					return s;
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Function used for sorting in the GetListOfSprites() function below.
	/// </summary>

	static int CompareString (string a, string b) { return a.CompareTo(b); }

	/// <summary>
	/// Convenience function that retrieves a list of all sprite names.
	/// </summary>

	public BetterList<string> GetListOfSprites ()
	{
		if (mReplacement != null) return mReplacement.GetListOfSprites();
		BetterList<string> list = new BetterList<string>();
		
		for (int i = 0, imax = sprites.Count; i < imax; ++i)
		{
			Sprite s = sprites[i];
			if (s != null && !string.IsNullOrEmpty(s.name)) list.Add(s.name);
		}
		//list.Sort(CompareString);
		return list;
	}

	/// <summary>
	/// Convenience function that retrieves a list of all sprite names that contain the specified phrase
	/// </summary>

	public BetterList<string> GetListOfSprites (string match)
	{
		if (mReplacement != null) return mReplacement.GetListOfSprites(match);
		if (string.IsNullOrEmpty(match)) return GetListOfSprites();
		BetterList<string> list = new BetterList<string>();

		// First try to find an exact match
		for (int i = 0, imax = sprites.Count; i < imax; ++i)
		{
			Sprite s = sprites[i];
			
			if (s != null && !string.IsNullOrEmpty(s.name) && string.Equals(match, s.name, StringComparison.OrdinalIgnoreCase))
			{
				list.Add(s.name);
				return list;
			}
		}

		// No exact match found? Split up the search into space-separated components.
		string[] keywords = match.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < keywords.Length; ++i) keywords[i] = keywords[i].ToLower();

		// Try to find all sprites where all keywords are present
		for (int i = 0, imax = sprites.Count; i < imax; ++i)
		{
			Sprite s = sprites[i];
			
			if (s != null && !string.IsNullOrEmpty(s.name))
			{
				string tl = s.name.ToLower();
				int matches = 0;

				for (int b = 0; b < keywords.Length; ++b)
				{
					if (tl.Contains(keywords[b])) ++matches;
				}
				if (matches == keywords.Length) list.Add(s.name);
			}
		}
		//list.Sort(CompareString);
		return list;
	}

	/// <summary>
	/// Helper function that determines whether the atlas uses the specified one, taking replacements into account.
	/// </summary>

	bool References (UIAtlas atlas)
	{
		if (atlas == null) return false;
		if (atlas == this) return true;
		return (mReplacement != null) ? mReplacement.References(atlas) : false;
	}

	/// <summary>
	/// Helper function that determines whether the two atlases are related.
	/// </summary>

	static public bool CheckIfRelated (UIAtlas a, UIAtlas b)
	{
		if (a == null || b == null) return false;
		return a == b || a.References(b) || b.References(a);
	}

	/// <summary>
	/// Mark all widgets associated with this atlas as having changed.
	/// </summary>

	public void MarkAsDirty ()
	{
#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
		if (mReplacement != null) mReplacement.MarkAsDirty();

		UISprite[] list = NGUITools.FindActive<UISprite>();

		for (int i = 0, imax = list.Length; i < imax; ++i)
		{
			UISprite sp = list[i];

			if (CheckIfRelated(this, sp.atlas))
			{
				UIAtlas atl = sp.atlas;
				sp.atlas = null;
				sp.atlas = atl;
#if UNITY_EDITOR
				UnityEditor.EditorUtility.SetDirty(sp);
#endif
			}
		}

		UIFont[] fonts = Resources.FindObjectsOfTypeAll(typeof(UIFont)) as UIFont[];

		for (int i = 0, imax = fonts.Length; i < imax; ++i)
		{
			UIFont font = fonts[i];

			if (CheckIfRelated(this, font.atlas))
			{
				UIAtlas atl = font.atlas;
				font.atlas = null;
				font.atlas = atl;
#if UNITY_EDITOR
				UnityEditor.EditorUtility.SetDirty(font);
#endif
			}
		}

		UILabel[] labels = NGUITools.FindActive<UILabel>();

		for (int i = 0, imax = labels.Length; i < imax; ++i)
		{
			UILabel lbl = labels[i];

			if (lbl.font != null && CheckIfRelated(this, lbl.font.atlas))
			{
				UIFont font = lbl.font;
				lbl.font = null;
				lbl.font = font;
#if UNITY_EDITOR
				UnityEditor.EditorUtility.SetDirty(lbl);
#endif
			}
		}
	}
}
