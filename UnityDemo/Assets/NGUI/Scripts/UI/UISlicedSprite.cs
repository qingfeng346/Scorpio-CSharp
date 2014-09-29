//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Sliced sprite is obsolete. It's only kept for backwards compatibility.
/// </summary>

[ExecuteInEditMode]
public class UISlicedSprite : UISprite
{
	public override Type type { get { return UISprite.Type.Sliced; } }
}
