using System;
using UnityEngine;
using System.Collections;
using Scorpio;
public static class Util {
    public static void AddComponent(Component com, ScriptTable table)
    {
        if (com == null) return;
        AddComponent(com.gameObject, table);
    }
    public static void AddComponent(GameObject obj, ScriptTable table)
    {
        if (obj == null) return;
        ScriptComponent com = obj.AddComponent<ScriptComponent>();
        com.Initialize(Launch.Script, table);
    }
    public static object GetComponent(Component com, Type type)
    {
        if (com == null) return null;
        return GetComponent(com.gameObject, type);
    }
    public static object GetComponent(GameObject obj, Type type)
    {
        if (obj == null) return null;
        return obj.GetComponent(type);
    }
    public static object FindChild(GameObject gameObject, string str, Type type)
    {
        if (gameObject == null) return null;
        return FindChild(gameObject.transform, str, type);
    }
    public static object FindChild(Component transform, string str, Type type)
    {
        GameObject obj = FindChild(transform, str);
        if (obj == null) return null;
        return obj.GetComponent(type);
    }
    public static GameObject FindChild(GameObject gameObject, string str)
    {
        if (gameObject == null) return null;
        return FindChild(gameObject.transform, str);
    }
    public static GameObject FindChild(Component transform, string str)
    {
        if (transform == null) return null;
        if (string.IsNullOrEmpty(str)) return null;
        Transform trans = transform.transform.FindChild(str);
        if (trans == null) return null;
        return trans.gameObject;
    }
    public static void AddChild(Component parent, GameObject child)
    {
        if (parent == null || child == null) return;
        AddChild(parent, child.transform);
    }
    public static void AddChild(GameObject parent, Component child)
    {
        if (parent == null || child == null) return;
        AddChild(parent.transform, child);
    }
    public static void AddChild(GameObject parent, GameObject child)
    {
        if (parent == null || child == null) return;
        AddChild(parent.transform, child.transform);
    }
    public static void AddChild(Component parent, Component child)
    {
        if (parent == null || child == null) return;
        child.transform.parent = parent.transform;
        child.transform.localScale = Vector3.one;
        child.transform.localPosition = Vector3.zero;
        child.transform.localEulerAngles = Vector3.zero;
        child.gameObject.layer = parent.gameObject.layer;
    }
    public static void ClearChildren(GameObject obj)
    {
        if (obj == null) return;
        int count = obj.transform.childCount;
        Transform[] trans = new Transform[count];
        for (int i = 0; i < count; ++i)
        {
            trans[i] = obj.transform.GetChild(i);
        }
        for (int i = 0; i < count; ++i)
        {
            GameObject.Destroy(trans[i]);
        }
        obj.transform.DetachChildren();
    }
    public static void ClearChildren(Component com)
    {
        if (com == null) return;
        ClearChildren(com.gameObject);
    }
    public static void SetActive(GameObject obj, bool active)
    {
        if (obj != null && obj.activeSelf != active) obj.SetActive(active);
    }
    public static void SetActive(Component com, bool active)
    {
        if (com == null) return;
        SetActive(com.gameObject, active);
    }
    public static int Range(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
}
