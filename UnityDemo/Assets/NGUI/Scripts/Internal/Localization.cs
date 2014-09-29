//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

//#define SHOW_REPORT

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Localization manager is able to parse localization information from text assets.
/// Using it is simple: text = Localization.Localize(key), or just add a UILocalize script to your labels.
/// You can switch the language by using Localization.instance.currentLanguage = "French", for example.
/// This will attempt to load the file called "French.txt" in the Resources folder.
/// The localization file should contain key = value pairs, one pair per line.
/// </summary>

[AddComponentMenu("NGUI/Internal/Localization")]
public class Localization : MonoBehaviour
{
	static Localization mInstance;

	/// <summary>
	/// Whether there is an instance of the localization class present.
	/// </summary>

	static public bool isActive { get { return mInstance != null; } }

	/// <summary>
	/// The instance of the localization class. Will create it if one isn't already around.
	/// </summary>

	static public Localization instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = Object.FindObjectOfType(typeof(Localization)) as Localization;

				if (mInstance == null)
				{
					GameObject go = new GameObject("_Localization");
					DontDestroyOnLoad(go);
					mInstance = go.AddComponent<Localization>();
				}
			}
			return mInstance;
		}
	}

	/// <summary>
	/// Language the localization manager will start with.
	/// </summary>

	public string startingLanguage = "English";

	/// <summary>
	/// Available list of languages.
	/// </summary>

	public TextAsset[] languages;

	Dictionary<string, string> mDictionary = new Dictionary<string, string>();
	string mLanguage;

#if SHOW_REPORT
	BetterList<string> mUsed = new BetterList<string>();
#endif

	/// <summary>
	/// Name of the currently active language.
	/// </summary>

	public string currentLanguage
	{
		get
		{
			return mLanguage;
		}
		set
		{
			if (mLanguage != value)
			{
				startingLanguage = value;

				if (!string.IsNullOrEmpty(value))
				{
					// Check the referenced assets first
					if (languages != null)
					{
						for (int i = 0, imax = languages.Length; i < imax; ++i)
						{
							TextAsset asset = languages[i];

							if (asset != null && asset.name == value)
							{
								Load(asset);
								return;
							}
						}
					}

					// Not a referenced asset -- try to load it dynamically
					TextAsset txt = Resources.Load(value, typeof(TextAsset)) as TextAsset;

					if (txt != null)
					{
						Load(txt);
						return;
					}
				}

				// Either the language is null, or it wasn't found
				mDictionary.Clear();
				PlayerPrefs.DeleteKey("Language");
			}
		}
	}

	/// <summary>
	/// Determine the starting language.
	/// </summary>

	void Awake ()
	{
		if (mInstance == null)
		{
			mInstance = this;
			DontDestroyOnLoad(gameObject);

			currentLanguage = PlayerPrefs.GetString("Language", startingLanguage);

			if (string.IsNullOrEmpty(mLanguage) && (languages != null && languages.Length > 0))
			{
				currentLanguage = languages[0].name;
			}
		}
		else Destroy(gameObject);
	}

	/// <summary>
	/// Oddly enough... sometimes if there is no OnEnable function in Localization, it can get the Awake call after UILocalize's OnEnable.
	/// </summary>

	void OnEnable () { if (mInstance == null) mInstance = this; }

#if SHOW_REPORT
	/// <summary>
	/// It's often useful to be able to tell which keys are used in localization, and which are not.
	/// For this to work properly it's advised to play through the entire game and view all localized content before hitting the Stop button.
	/// </summary>

	void OnDisable ()
	{
		string final = "";
		BetterList<string> full = new BetterList<string>();

		// Create a list of all the known keys
		foreach (KeyValuePair<string, string> pair in mDictionary) full.Add(pair.Key);

		// Sort the full list
		full.Sort(delegate(string s1, string s2) { return s1.CompareTo(s2); });

		// Create the final string with the localization keys
		for (int i = 0; i < full.size; ++i)
		{
			string key = full[i];
			string val = mDictionary[key].Replace("\n", "\\n");
			if (mUsed.Contains(key)) final += key + " = " + val + "\n";
			else final += "//" + key + " = " + val + "\n";
		}
		
		// Show the final report in a format that makes it easy to copy/paste into the original localization file
		if (!string.IsNullOrEmpty(final))
			Debug.Log("// Localization Report\n\n" + final);
	}
#endif

	/// <summary>
	/// Remove the instance reference.
	/// </summary>

	void OnDestroy () { if (mInstance == this) mInstance = null; }

	/// <summary>
	/// Load the specified asset and activate the localization.
	/// </summary>

	void Load (TextAsset asset)
	{
#if SHOW_REPORT
		mUsed.Clear();
#endif
		mLanguage = asset.name;
		PlayerPrefs.SetString("Language", mLanguage);
		ByteReader reader = new ByteReader(asset);
		mDictionary = reader.ReadDictionary();
		UIRoot.Broadcast("OnLocalize", this);
	}

	/// <summary>
	/// Localize the specified value.
	/// </summary>

	public string Get (string key)
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return key;
#endif
#if SHOW_REPORT
		if (!mUsed.Contains(key)) mUsed.Add(key);
#endif
		string val;
#if UNITY_IPHONE || UNITY_ANDROID
		if (mDictionary.TryGetValue(key + " Mobile", out val)) return val;
#endif

#if UNITY_EDITOR
		if (mDictionary.TryGetValue(key, out val)) return val;
		Debug.LogWarning("Localization key not found: '" + key + "'");
		return key;
#else
		return (mDictionary.TryGetValue(key, out val)) ? val : key;
#endif
	}

	/// <summary>
	/// Localize the specified value.
	/// </summary>

	static public string Localize (string key) { return (instance != null) ? instance.Get(key) : key; }
}
