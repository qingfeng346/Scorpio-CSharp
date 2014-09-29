//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sprite is a textured element in the UI hierarchy.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Sprite")]
public class UISprite : UIWidget
{
	public enum Type
	{
		Simple,
		Sliced,
		Tiled,
		Filled,
	}

	public enum FillDirection
	{
		Horizontal,
		Vertical,
		Radial90,
		Radial180,
		Radial360,
	}

	// Cached and saved values
	[HideInInspector][SerializeField] UIAtlas mAtlas;
	[HideInInspector][SerializeField] string mSpriteName;
	[HideInInspector][SerializeField] bool mFillCenter = true;
	[HideInInspector][SerializeField] Type mType = Type.Simple;
	[HideInInspector][SerializeField] FillDirection mFillDirection = FillDirection.Radial360;
	[HideInInspector][SerializeField] float mFillAmount = 1.0f;
	[HideInInspector][SerializeField] bool mInvert = false;

	protected UIAtlas.Sprite mSprite;
	protected Rect mInner;
	protected Rect mInnerUV;
	protected Rect mOuter;
	protected Rect mOuterUV;
	protected Vector3 mScale = Vector3.one;

	bool mSpriteSet = false;

	/// <summary>
	/// How the sprite is drawn.
	/// </summary>

	virtual public Type type
	{
		get
		{
			return mType;
		}
		set
		{
			if (mType != value)
			{
				mType = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Atlas used by this widget.
	/// </summary>
 
	public UIAtlas atlas
	{
		get
		{
			return mAtlas;
		}
		set
		{
			if (mAtlas != value)
			{
				mAtlas = value;
				mSpriteSet = false;
				mSprite = null;

				// Update the material
				material = (mAtlas != null) ? mAtlas.spriteMaterial : null;

				// Automatically choose the first sprite
				if (string.IsNullOrEmpty(mSpriteName))
				{
					if (mAtlas != null && mAtlas.spriteList.Count > 0)
					{
						SetAtlasSprite(mAtlas.spriteList[0]);
						mSpriteName = mSprite.name;
					}
				}

				// Re-link the sprite
				if (!string.IsNullOrEmpty(mSpriteName))
				{
					string sprite = mSpriteName;
					mSpriteName = "";
					spriteName = sprite;
					mChanged = true;
					UpdateUVs(true);
				}
			}
		}
	}

	/// <summary>
	/// Sprite within the atlas used to draw this widget.
	/// </summary>
 
	public string spriteName
	{
		get
		{
			return mSpriteName;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				// If the sprite name hasn't been set yet, no need to do anything
				if (string.IsNullOrEmpty(mSpriteName)) return;

				// Clear the sprite name and the sprite reference
				mSpriteName = "";
				mSprite = null;
				mChanged = true;
				mSpriteSet = false;
			}
			else if (mSpriteName != value)
			{
				// If the sprite name changes, the sprite reference should also be updated
				mSpriteName = value;
				mSprite = null;
				mChanged = true;
				mSpriteSet = false;
				if (isValid) UpdateUVs(true);
			}
		}
	}

	/// <summary>
	/// Is there a valid sprite to work with?
	/// </summary>

	public bool isValid { get { return GetAtlasSprite() != null; } }

	/// <summary>
	/// Retrieve the material used by the font.
	/// </summary>

	public override Material material
	{
		get
		{
			Material mat = base.material;

			if (mat == null)
			{
				mat = (mAtlas != null) ? mAtlas.spriteMaterial : null;
				mSprite = null;
				material = mat;
				if (mat != null) UpdateUVs(true);
			}
			return mat;
		}
	}

	/// <summary>
	/// Inner set of UV coordinates.
	/// </summary>

	public Rect innerUV { get { UpdateUVs(false); return mInnerUV; } }

	/// <summary>
	/// Outer set of UV coordinates.
	/// </summary>

	public Rect outerUV { get { UpdateUVs(false); return mOuterUV; } }

	/// <summary>
	/// Whether the center part of the sprite will be filled or not. Turn it off if you want only to borders to show up.
	/// </summary>

	public bool fillCenter { get { return mFillCenter; } set { if (mFillCenter != value) { mFillCenter = value; MarkAsChanged(); } } }

	/// <summary>
	/// Direction of the cut procedure.
	/// </summary>

	public FillDirection fillDirection
	{
		get
		{
			return mFillDirection;
		}
		set
		{
			if (mFillDirection != value)
			{
				mFillDirection = value;
				mChanged = true;
			}
		}
	}

	/// <summary>
	/// Amount of the sprite shown. 0-1 range with 0 being nothing shown, and 1 being the full sprite.
	/// </summary>

	public float fillAmount
	{
		get
		{
			return mFillAmount;
		}
		set
		{
			float val = Mathf.Clamp01(value);

			if (mFillAmount != val)
			{
				mFillAmount = val;
				mChanged = true;
			}
		}
	}

	/// <summary>
	/// Whether the sprite should be filled in the opposite direction.
	/// </summary>

	public bool invert
	{
		get
		{
			return mInvert;
		}
		set
		{
			if (mInvert != value)
			{
				mInvert = value;
				mChanged = true;
			}
		}
	}

	/// <summary>
	/// Extra padding around the sprite, in pixels.
	/// </summary>

	public override Vector4 relativePadding
	{
		get
		{
			if (isValid && type == Type.Simple)
			{
				return new Vector4(mSprite.paddingLeft, mSprite.paddingTop, mSprite.paddingRight, mSprite.paddingBottom);
			}
			return base.relativePadding;
		}
	}

	/// <summary>
	/// Sliced sprites generally have a border.
	/// </summary>

	public override Vector4 border
	{
		get
		{
			if (type == Type.Sliced)
			{
				UIAtlas.Sprite sp = GetAtlasSprite();
				if (sp == null) return Vector2.zero;

				Rect outer = sp.outer;
				Rect inner = sp.inner;

				Texture tex = mainTexture;

				if (atlas.coordinates == UIAtlas.Coordinates.TexCoords && tex != null)
				{
					outer = NGUIMath.ConvertToPixels(outer, tex.width, tex.height, true);
					inner = NGUIMath.ConvertToPixels(inner, tex.width, tex.height, true);
				}
				return new Vector4(inner.xMin - outer.xMin, inner.yMin - outer.yMin, outer.xMax - inner.xMax, outer.yMax - inner.yMax) * atlas.pixelSize;
			}
			return base.border;
		}
	}

	/// <summary>
	/// Whether this widget will automatically become pixel-perfect after resize operation finishes.
	/// </summary>

	public override bool pixelPerfectAfterResize { get { return type == Type.Sliced; } }

	/// <summary>
	/// Retrieve the atlas sprite referenced by the spriteName field.
	/// </summary>

	public UIAtlas.Sprite GetAtlasSprite ()
	{
		if (!mSpriteSet) mSprite = null;

		if (mSprite == null && mAtlas != null)
		{
			if (!string.IsNullOrEmpty(mSpriteName))
			{
				UIAtlas.Sprite sp = mAtlas.GetSprite(mSpriteName);
				if (sp == null) return null;
				SetAtlasSprite(sp);
			}

			if (mSprite == null && mAtlas.spriteList.Count > 0)
			{
				UIAtlas.Sprite sp = mAtlas.spriteList[0];
				if (sp == null) return null;
				SetAtlasSprite(sp);

				if (mSprite == null)
				{
					Debug.LogError(mAtlas.name + " seems to have a null sprite!");
					return null;
				}
				mSpriteName = mSprite.name;
			}

			// If the sprite has been set, update the material
			if (mSprite != null)
			{
				material = mAtlas.spriteMaterial;
				UpdateUVs(true);
			}
		}
		return mSprite;
	}

	/// <summary>
	/// Set the atlas sprite directly.
	/// </summary>

	protected void SetAtlasSprite (UIAtlas.Sprite sp)
	{
		mChanged = true;
		mSpriteSet = true;

		if (sp != null)
		{
			mSprite = sp;
			mSpriteName = mSprite.name;
		}
		else
		{
			mSpriteName = (mSprite != null) ? mSprite.name : "";
			mSprite = sp;
		}
	}

	/// <summary>
	/// Update the texture UVs used by the widget.
	/// </summary>

	virtual public void UpdateUVs (bool force)
	{
		if ((type == Type.Sliced || type == Type.Tiled) && cachedTransform.localScale != mScale)
		{
			mScale = cachedTransform.localScale;
			mChanged = true;
		}

		if (isValid && (force
#if UNITY_EDITOR
			|| !Application.isPlaying && (mOuter != mSprite.outer || mInner != mSprite.inner)
#endif
			))
		{
			Texture tex = mainTexture;

			if (tex != null)
			{
				mInner = mSprite.inner;
				mOuter = mSprite.outer;

				mInnerUV = mInner;
				mOuterUV = mOuter;

				if (atlas.coordinates == UIAtlas.Coordinates.Pixels)
				{
					mOuterUV = NGUIMath.ConvertToTexCoords(mOuterUV, tex.width, tex.height);
					mInnerUV = NGUIMath.ConvertToTexCoords(mInnerUV, tex.width, tex.height);
				}
			}
		}
	}

	/// <summary>
	/// Adjust the scale of the widget to make it pixel-perfect.
	/// </summary>

	public override void MakePixelPerfect ()
	{
		if (!isValid) return;

		UpdateUVs(false);

		UISprite.Type t = type;

		if (t == Type.Sliced)
		{
			// Sliced sprite should have dimensions divisible by 2 for best results
			Vector3 pos = cachedTransform.localPosition;
			pos.x = Mathf.RoundToInt(pos.x);
			pos.y = Mathf.RoundToInt(pos.y);
			pos.z = Mathf.RoundToInt(pos.z);
			cachedTransform.localPosition = pos;

			Vector3 scale = cachedTransform.localScale;
			scale.x = Mathf.RoundToInt(scale.x * 0.5f) << 1;
			scale.y = Mathf.RoundToInt(scale.y * 0.5f) << 1;
			scale.z = 1f;
			cachedTransform.localScale = scale;
		}
		else if (t == Type.Tiled)
		{
			// Tiled sprite just needs whole integers
			Vector3 pos = cachedTransform.localPosition;
			pos.x = Mathf.RoundToInt(pos.x);
			pos.y = Mathf.RoundToInt(pos.y);
			pos.z = Mathf.RoundToInt(pos.z);
			cachedTransform.localPosition = pos;

			Vector3 scale = cachedTransform.localScale;
			scale.x = Mathf.RoundToInt(scale.x);
			scale.y = Mathf.RoundToInt(scale.y);
			scale.z = 1f;
			cachedTransform.localScale = scale;
		}
		else
		{
			// Other sprites should assume the original dimensions of the sprite
			Texture tex = mainTexture;
			Vector3 scale = cachedTransform.localScale;

			if (tex != null)
			{
				Rect rect = NGUIMath.ConvertToPixels(outerUV, tex.width, tex.height, true);
				float pixelSize = atlas.pixelSize;
				scale.x = Mathf.RoundToInt(rect.width * pixelSize) * Mathf.Sign(scale.x);
				scale.y = Mathf.RoundToInt(rect.height * pixelSize) * Mathf.Sign(scale.y);
				scale.z = 1f;
				cachedTransform.localScale = scale;
			}

			int width = Mathf.RoundToInt(Mathf.Abs(scale.x) * (1f + mSprite.paddingLeft + mSprite.paddingRight));
			int height = Mathf.RoundToInt(Mathf.Abs(scale.y) * (1f + mSprite.paddingTop + mSprite.paddingBottom));

			Vector3 pos = cachedTransform.localPosition;
			pos.x = (Mathf.CeilToInt(pos.x * 4f) >> 2);
			pos.y = (Mathf.CeilToInt(pos.y * 4f) >> 2);
			pos.z = Mathf.RoundToInt(pos.z);

			if (width % 2 == 1 && (pivot == Pivot.Top || pivot == Pivot.Center || pivot == Pivot.Bottom))
				pos.x += 0.5f;

			if (height % 2 == 1 && (pivot == Pivot.Left || pivot == Pivot.Center || pivot == Pivot.Right))
				pos.y += 0.5f;

			cachedTransform.localPosition = pos;
		}
	}

	/// <summary>
	/// Set the atlas and the sprite.
	/// </summary>

	protected override void OnStart ()
	{
		if (mAtlas != null)
		{
			UpdateUVs(true);
		}
	}

	/// <summary>
	/// Update the UV coordinates.
	/// </summary>

	public override void Update ()
	{
		base.Update();

		if (mChanged || !mSpriteSet)
		{
			mSpriteSet = true;
			mSprite = null;
			mChanged = true;
			UpdateUVs(true);
		}
		else UpdateUVs(false);
	}

	/// <summary>
	/// Virtual function called by the UIScreen that fills the buffers.
	/// </summary>

	public override void OnFill (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		switch (type)
		{
			case Type.Simple:
			SimpleFill(verts, uvs, cols);
			break;

			case Type.Sliced:
			SlicedFill(verts, uvs, cols);
			break;

			case Type.Filled:
			FilledFill(verts, uvs, cols);
			break;

			case Type.Tiled:
			TiledFill(verts, uvs, cols);
			break;
		}
	}

#region Various fill functions
	/// <summary>
	/// Regular sprite fill function is quite simple.
	/// </summary>

	protected void SimpleFill (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Vector2 uv0 = new Vector2(mOuterUV.xMin, mOuterUV.yMin);
		Vector2 uv1 = new Vector2(mOuterUV.xMax, mOuterUV.yMax);

		verts.Add(new Vector3(1f,  0f, 0f));
		verts.Add(new Vector3(1f, -1f, 0f));
		verts.Add(new Vector3(0f, -1f, 0f));
		verts.Add(new Vector3(0f,  0f, 0f));

		uvs.Add(uv1);
		uvs.Add(new Vector2(uv1.x, uv0.y));
		uvs.Add(uv0);
		uvs.Add(new Vector2(uv0.x, uv1.y));

		Color colF = color;
		colF.a *= mPanel.alpha;
		Color32 col = atlas.premultipliedAlpha ? NGUITools.ApplyPMA(colF) : colF;
		
		cols.Add(col);
		cols.Add(col);
		cols.Add(col);
		cols.Add(col);
	}

	/// <summary>
	/// Sliced sprite fill function is more complicated as it generates 9 quads instead of 1.
	/// </summary>

	protected void SlicedFill (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if (mOuterUV == mInnerUV)
		{
			SimpleFill(verts, uvs, cols);
			return;
		}

		Vector2[] v = new Vector2[4];
		Vector2[] uv = new Vector2[4];

		Texture tex = mainTexture;

		v[0] = Vector2.zero;
		v[1] = Vector2.zero;
		v[2] = new Vector2(1f, -1f);
		v[3] = new Vector2(1f, -1f);

		if (tex != null)
		{
			float pixelSize = atlas.pixelSize;
			float borderLeft = (mInnerUV.xMin - mOuterUV.xMin) * pixelSize;
			float borderRight = (mOuterUV.xMax - mInnerUV.xMax) * pixelSize;
			float borderTop = (mInnerUV.yMax - mOuterUV.yMax) * pixelSize;
			float borderBottom = (mOuterUV.yMin - mInnerUV.yMin) * pixelSize;

			Vector3 scale = cachedTransform.localScale;
			scale.x = Mathf.Max(0f, scale.x);
			scale.y = Mathf.Max(0f, scale.y);

			Vector2 sz = new Vector2(scale.x / tex.width, scale.y / tex.height);
			Vector2 tl = new Vector2(borderLeft / sz.x, borderTop / sz.y);
			Vector2 br = new Vector2(borderRight / sz.x, borderBottom / sz.y);

			Pivot pv = pivot;

			// We don't want the sliced sprite to become smaller than the summed up border size
			if (pv == Pivot.Right || pv == Pivot.TopRight || pv == Pivot.BottomRight)
			{
				v[0].x = Mathf.Min(0f, 1f - (br.x + tl.x));
				v[1].x = v[0].x + tl.x;
				v[2].x = v[0].x + Mathf.Max(tl.x, 1f - br.x);
				v[3].x = v[0].x + Mathf.Max(tl.x + br.x, 1f);
			}
			else
			{
				v[1].x = tl.x;
				v[2].x = Mathf.Max(tl.x, 1f - br.x);
				v[3].x = Mathf.Max(tl.x + br.x, 1f);
			}

			if (pv == Pivot.Bottom || pv == Pivot.BottomLeft || pv == Pivot.BottomRight)
			{
				v[0].y = Mathf.Max(0f, -1f - (br.y + tl.y));
				v[1].y = v[0].y + tl.y;
				v[2].y = v[0].y + Mathf.Min(tl.y, -1f - br.y);
				v[3].y = v[0].y + Mathf.Min(tl.y + br.y, -1f);
			}
			else
			{
				v[1].y = tl.y;
				v[2].y = Mathf.Min(tl.y, -1f - br.y);
				v[3].y = Mathf.Min(tl.y + br.y, -1f);
			}

			uv[0] = new Vector2(mOuterUV.xMin, mOuterUV.yMax);
			uv[1] = new Vector2(mInnerUV.xMin, mInnerUV.yMax);
			uv[2] = new Vector2(mInnerUV.xMax, mInnerUV.yMin);
			uv[3] = new Vector2(mOuterUV.xMax, mOuterUV.yMin);
		}
		else
		{
			// No texture -- just use zeroed out texture coordinates
			for (int i = 0; i < 4; ++i) uv[i] = Vector2.zero;
		}

		Color colF = color;
		colF.a *= mPanel.alpha;
		Color32 col = atlas.premultipliedAlpha ? NGUITools.ApplyPMA(colF) : colF;

		for (int x = 0; x < 3; ++x)
		{
			int x2 = x + 1;

			for (int y = 0; y < 3; ++y)
			{
				if (!mFillCenter && x == 1 && y == 1) continue;

				int y2 = y + 1;

				verts.Add(new Vector3(v[x2].x, v[y].y, 0f));
				verts.Add(new Vector3(v[x2].x, v[y2].y, 0f));
				verts.Add(new Vector3(v[x].x, v[y2].y, 0f));
				verts.Add(new Vector3(v[x].x, v[y].y, 0f));

				uvs.Add(new Vector2(uv[x2].x, uv[y].y));
				uvs.Add(new Vector2(uv[x2].x, uv[y2].y));
				uvs.Add(new Vector2(uv[x].x, uv[y2].y));
				uvs.Add(new Vector2(uv[x].x, uv[y].y));

				cols.Add(col);
				cols.Add(col);
				cols.Add(col);
				cols.Add(col);
			}
		}
	}

	/// <summary>
	/// Adjust the specified quad, making it be radially filled instead.
	/// </summary>

	protected bool AdjustRadial (Vector2[] xy, Vector2[] uv, float fill, bool invert)
	{
		// Nothing to fill
		if (fill < 0.001f) return false;

		// Nothing to adjust
		if (!invert && fill > 0.999f) return true;

		// Convert 0-1 value into 0 to 90 degrees angle in radians
		float angle = Mathf.Clamp01(fill);
		if (!invert) angle = 1f - angle;
		angle *= 90f * Mathf.Deg2Rad;

		// Calculate the effective X and Y factors
		float fx = Mathf.Sin(angle);
		float fy = Mathf.Cos(angle);

		// Normalize the result, so it's projected onto the side of the rectangle
		if (fx > fy)
		{
			fy *= 1f / fx;
			fx = 1f;

			if (!invert)
			{
				xy[0].y = Mathf.Lerp(xy[2].y, xy[0].y, fy);
				xy[3].y = xy[0].y;

				uv[0].y = Mathf.Lerp(uv[2].y, uv[0].y, fy);
				uv[3].y = uv[0].y;
			}
		}
		else if (fy > fx)
		{
			fx *= 1f / fy;
			fy = 1f;

			if (invert)
			{
				xy[0].x = Mathf.Lerp(xy[2].x, xy[0].x, fx);
				xy[1].x = xy[0].x;

				uv[0].x = Mathf.Lerp(uv[2].x, uv[0].x, fx);
				uv[1].x = uv[0].x;
			}
		}
		else
		{
			fx = 1f;
			fy = 1f;
		}

		if (invert)
		{
			xy[1].y = Mathf.Lerp(xy[2].y, xy[0].y, fy);
			uv[1].y = Mathf.Lerp(uv[2].y, uv[0].y, fy);
		}
		else
		{
			xy[3].x = Mathf.Lerp(xy[2].x, xy[0].x, fx);
			uv[3].x = Mathf.Lerp(uv[2].x, uv[0].x, fx);
		}
		return true;
	}

	/// <summary>
	/// Helper function that copies the contents of the array, rotated by the specified offset.
	/// </summary>

	protected void Rotate (Vector2[] v, int offset)
	{
		for (int i = 0; i < offset; ++i)
		{
			Vector2 v0 = new Vector2(v[3].x, v[3].y);

			v[3].x = v[2].y;
			v[3].y = v[2].x;

			v[2].x = v[1].y;
			v[2].y = v[1].x;

			v[1].x = v[0].y;
			v[1].y = v[0].x;

			v[0].x = v0.y;
			v[0].y = v0.x;
		}
	}

	/// <summary>
	/// Filled sprite fill function.
	/// </summary>

	protected void FilledFill (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		float x0 = 0f;
		float y0 = 0f;
		float x1 = 1f;
		float y1 = -1f;

		float u0 = mOuterUV.xMin;
		float v0 = mOuterUV.yMin;
		float u1 = mOuterUV.xMax;
		float v1 = mOuterUV.yMax;

		// Horizontal and vertical filled sprites are simple -- just end the sprite prematurely
		if (mFillDirection == FillDirection.Horizontal || mFillDirection == FillDirection.Vertical)
		{
			float du = (u1 - u0) * mFillAmount;
			float dv = (v1 - v0) * mFillAmount;

			if (fillDirection == FillDirection.Horizontal)
			{
				if (mInvert)
				{
					x0 = (1f - mFillAmount);
					u0 = u1 - du;
				}
				else
				{
					x1 *= mFillAmount;
					u1 = u0 + du;
				}
			}
			else if (fillDirection == FillDirection.Vertical)
			{
				if (mInvert)
				{
					y1 *= mFillAmount;
					v0 = v1 - dv;
				}
				else
				{
					y0 = -(1f - mFillAmount);
					v1 = v0 + dv;
				}
			}
		}

		// Starting quad for the sprite
		Vector2[] xy = new Vector2[4];
		Vector2[] uv = new Vector2[4];

		xy[0] = new Vector2(x1, y0);
		xy[1] = new Vector2(x1, y1);
		xy[2] = new Vector2(x0, y1);
		xy[3] = new Vector2(x0, y0);

		uv[0] = new Vector2(u1, v1);
		uv[1] = new Vector2(u1, v0);
		uv[2] = new Vector2(u0, v0);
		uv[3] = new Vector2(u0, v1);

		Color colF = color;
		colF.a *= mPanel.alpha;
		Color32 col = atlas.premultipliedAlpha ? NGUITools.ApplyPMA(colF) : colF;

		if (fillDirection == FillDirection.Radial90)
		{
			// Adjust the quad radially, and if 'false' is returned (it's not visible), just exit
			if (!AdjustRadial(xy, uv, mFillAmount, mInvert)) return;
		}
		else if (fillDirection == FillDirection.Radial180)
		{
			// Working in 0-1 coordinates is easier
			Vector2[] oxy = new Vector2[4];
			Vector2[] ouv = new Vector2[4];

			for (int i = 0; i < 2; ++i)
			{
				oxy[0] = new Vector2(0f, 0f);
				oxy[1] = new Vector2(0f, 1f);
				oxy[2] = new Vector2(1f, 1f);
				oxy[3] = new Vector2(1f, 0f);

				ouv[0] = new Vector2(0f, 0f);
				ouv[1] = new Vector2(0f, 1f);
				ouv[2] = new Vector2(1f, 1f);
				ouv[3] = new Vector2(1f, 0f);

				// Each half must be rotated 90 degrees clockwise in order for it to fill properly
				if (mInvert)
				{
					if (i > 0)
					{
						Rotate(oxy, i);
						Rotate(ouv, i);
					}
				}
				else if (i < 1)
				{
					Rotate(oxy, 1 - i);
					Rotate(ouv, 1 - i);
				}

				// Each half must fill in only a part of the space
				float x, y;

				if (i == 1)
				{
					x = mInvert ? 0.5f : 1f;
					y = mInvert ? 1f : 0.5f;
				}
				else
				{
					x = mInvert ? 1f : 0.5f;
					y = mInvert ? 0.5f : 1f;
				}

				oxy[1].y = Mathf.Lerp(x, y, oxy[1].y);
				oxy[2].y = Mathf.Lerp(x, y, oxy[2].y);
				ouv[1].y = Mathf.Lerp(x, y, ouv[1].y);
				ouv[2].y = Mathf.Lerp(x, y, ouv[2].y);

				float amount = (mFillAmount) * 2 - i;
				bool odd = (i % 2) == 1;

				if (AdjustRadial(oxy, ouv, amount, !odd))
				{
					if (mInvert) odd = !odd;

					// Add every other side in reverse order so they don't come out backface-culled due to rotation
					if (odd)
					{
						for (int b = 0; b < 4; ++b)
						{
							x = Mathf.Lerp(xy[0].x, xy[2].x, oxy[b].x);
							y = Mathf.Lerp(xy[0].y, xy[2].y, oxy[b].y);

							float u = Mathf.Lerp(uv[0].x, uv[2].x, ouv[b].x);
							float v = Mathf.Lerp(uv[0].y, uv[2].y, ouv[b].y);

							verts.Add(new Vector3(x, y, 0f));
							uvs.Add(new Vector2(u, v));
							cols.Add(col);
						}
					}
					else
					{
						for (int b = 3; b > -1; --b)
						{
							x = Mathf.Lerp(xy[0].x, xy[2].x, oxy[b].x);
							y = Mathf.Lerp(xy[0].y, xy[2].y, oxy[b].y);

							float u = Mathf.Lerp(uv[0].x, uv[2].x, ouv[b].x);
							float v = Mathf.Lerp(uv[0].y, uv[2].y, ouv[b].y);

							verts.Add(new Vector3(x, y, 0f));
							uvs.Add(new Vector2(u, v));
							cols.Add(col);
						}
					}
				}
			}
			return;
		}
		else if (fillDirection == FillDirection.Radial360)
		{
			float[] matrix = new float[]
			{
				// x0 y0  x1   y1
				0.5f, 1f, 0f, 0.5f, // quadrant 0
				0.5f, 1f, 0.5f, 1f, // quadrant 1
				0f, 0.5f, 0.5f, 1f, // quadrant 2
				0f, 0.5f, 0f, 0.5f, // quadrant 3
			};

			Vector2[] oxy = new Vector2[4];
			Vector2[] ouv = new Vector2[4];

			for (int i = 0; i < 4; ++i)
			{
				oxy[0] = new Vector2(0f, 0f);
				oxy[1] = new Vector2(0f, 1f);
				oxy[2] = new Vector2(1f, 1f);
				oxy[3] = new Vector2(1f, 0f);

				ouv[0] = new Vector2(0f, 0f);
				ouv[1] = new Vector2(0f, 1f);
				ouv[2] = new Vector2(1f, 1f);
				ouv[3] = new Vector2(1f, 0f);

				// Each quadrant must be rotated 90 degrees clockwise in order for it to fill properly
				if (mInvert)
				{
					if (i > 0)
					{
						Rotate(oxy, i);
						Rotate(ouv, i);
					}
				}
				else if (i < 3)
				{
					Rotate(oxy, 3 - i);
					Rotate(ouv, 3 - i);
				}

				// Each quadrant must fill in only a quarter of the space
				for (int b = 0; b < 4; ++b)
				{
					int index = (mInvert) ? (3 - i) * 4 : i * 4;

					float fx0 = matrix[index];
					float fy0 = matrix[index + 1];
					float fx1 = matrix[index + 2];
					float fy1 = matrix[index + 3];

					oxy[b].x = Mathf.Lerp(fx0, fy0, oxy[b].x);
					oxy[b].y = Mathf.Lerp(fx1, fy1, oxy[b].y);
					ouv[b].x = Mathf.Lerp(fx0, fy0, ouv[b].x);
					ouv[b].y = Mathf.Lerp(fx1, fy1, ouv[b].y);
				}

				float amount = (mFillAmount) * 4 - i;
				bool odd = (i % 2) == 1;

				if (AdjustRadial(oxy, ouv, amount, !odd))
				{
					if (mInvert) odd = !odd;

					// Add every other side in reverse order so they don't come out backface-culled due to rotation
					if (odd)
					{
						for (int b = 0; b < 4; ++b)
						{
							float x = Mathf.Lerp(xy[0].x, xy[2].x, oxy[b].x);
							float y = Mathf.Lerp(xy[0].y, xy[2].y, oxy[b].y);
							float u = Mathf.Lerp(uv[0].x, uv[2].x, ouv[b].x);
							float v = Mathf.Lerp(uv[0].y, uv[2].y, ouv[b].y);

							verts.Add(new Vector3(x, y, 0f));
							uvs.Add(new Vector2(u, v));
							cols.Add(col);
						}
					}
					else
					{
						for (int b = 3; b > -1; --b)
						{
							float x = Mathf.Lerp(xy[0].x, xy[2].x, oxy[b].x);
							float y = Mathf.Lerp(xy[0].y, xy[2].y, oxy[b].y);
							float u = Mathf.Lerp(uv[0].x, uv[2].x, ouv[b].x);
							float v = Mathf.Lerp(uv[0].y, uv[2].y, ouv[b].y);

							verts.Add(new Vector3(x, y, 0f));
							uvs.Add(new Vector2(u, v));
							cols.Add(col);
						}
					}
				}
			}
			return;
		}

		// Fill the buffer with the quad for the sprite
		for (int i = 0; i < 4; ++i)
		{
			verts.Add(xy[i]);
			uvs.Add(uv[i]);
			cols.Add(col);
		}
	}

	/// <summary>
	/// Tiled sprite fill function.
	/// </summary>

	protected void TiledFill (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Texture tex = material.mainTexture;
		if (tex == null) return;

		Rect rect = mInner;

		if (atlas.coordinates == UIAtlas.Coordinates.TexCoords)
		{
			rect = NGUIMath.ConvertToPixels(rect, tex.width, tex.height, true);
		}

		Vector2 scale = cachedTransform.localScale;
		float pixelSize = atlas.pixelSize;
		float width = Mathf.Abs(rect.width / scale.x) * pixelSize;
		float height = Mathf.Abs(rect.height / scale.y) * pixelSize;

		// Safety check. Useful so Unity doesn't run out of memory if the sprites are too small.
		if (width < 0.01f || height < 0.01f)
		{
			Debug.LogWarning("The tiled sprite (" + NGUITools.GetHierarchy(gameObject) + ") is too small.\nConsider using a bigger one.");

			width = 0.01f;
			height = 0.01f;
		}

		Vector2 min = new Vector2(rect.xMin / tex.width, rect.yMin / tex.height);
		Vector2 max = new Vector2(rect.xMax / tex.width, rect.yMax / tex.height);
		Vector2 clipped = max;

		Color colF = color;
		colF.a *= mPanel.alpha;
		Color32 col = atlas.premultipliedAlpha ? NGUITools.ApplyPMA(colF) : colF;
		float y = 0f;

		while (y < 1f)
		{
			float x = 0f;
			clipped.x = max.x;

			float y2 = y + height;

			if (y2 > 1f)
			{
				clipped.y = min.y + (max.y - min.y) * (1f - y) / (y2 - y);
				y2 = 1f;
			}

			while (x < 1f)
			{
				float x2 = x + width;

				if (x2 > 1f)
				{
					clipped.x = min.x + (max.x - min.x) * (1f - x) / (x2 - x);
					x2 = 1f;
				}

				verts.Add(new Vector3(x2, -y, 0f));
				verts.Add(new Vector3(x2, -y2, 0f));
				verts.Add(new Vector3(x, -y2, 0f));
				verts.Add(new Vector3(x, -y, 0f));

				uvs.Add(new Vector2(clipped.x, 1f - min.y));
				uvs.Add(new Vector2(clipped.x, 1f - clipped.y));
				uvs.Add(new Vector2(min.x, 1f - clipped.y));
				uvs.Add(new Vector2(min.x, 1f - min.y));

				cols.Add(col);
				cols.Add(col);
				cols.Add(col);
				cols.Add(col);

				x += width;
			}
			y += height;
		}
	}
#endregion
}
