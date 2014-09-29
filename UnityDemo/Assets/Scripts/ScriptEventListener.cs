using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Scorpio;

public class ScriptEventListener : MonoBehaviour
{
    public object parameter;
    public ScriptFunction onSubmit;
    public ScriptFunction onClick;
    public ScriptFunction onDoubleClick;
    public ScriptFunction onHover;
    public ScriptFunction onPress;
    public ScriptFunction onSelect;
    public ScriptFunction onScroll;
    public ScriptFunction onDrag;
    public ScriptFunction onDrop;
    public ScriptFunction onInput;
    public ScriptFunction onKey;

    void OnSubmit()
    {
        if (onSubmit != null)
            onSubmit.call(gameObject, parameter);
    }
    void OnClick()
    {
        if (onClick != null)
            onClick.call(gameObject, parameter);
    }
    void OnDoubleClick()
    {
        if (onDoubleClick != null)
            onDoubleClick.call(gameObject, parameter);
    }
    void OnHover(bool isOver)
    {
        if (onHover != null)
            onHover.call(gameObject, isOver, parameter);
    }
    void OnPress(bool isPressed)
    {
        if (onPress != null)
            onPress.call(gameObject, isPressed, parameter);
    }
    void OnSelect(bool selected)
    {
        if (onSelect != null)
            onSelect.call(gameObject, selected, parameter);
    }
    void OnScroll(float delta)
    {
        if (onScroll != null)
            onScroll.call(gameObject, delta, parameter);
    }
    void OnDrag(Vector2 delta)
    {
        if (onDrag != null)
            onDrag.call(gameObject, delta.x, delta.y, parameter);
    }
    void OnDrop(GameObject go)
    {
        if (onDrop != null)
            onDrop.call(gameObject, go, parameter);
    }
    void OnInput(string text)
    {
        if (onInput != null)
            onInput.call(gameObject, text, parameter);
    }
    void OnKey(KeyCode key)
    {
        if (onKey != null)
            onKey.call(gameObject, (int)key, parameter);
    }
    /// <summary>
    /// Get or add an event listener to the specified game object.
    /// </summary>
    static public ScriptEventListener Get(GameObject go)
    {
        if (go == null) return null;
        ScriptEventListener listener = go.GetComponent<ScriptEventListener>();
        if (listener == null) listener = go.AddComponent<ScriptEventListener>();
        return listener;
    }
    static public ScriptEventListener Get(Component com)
    {
        if (com == null) return null;
        return Get(com.gameObject);
    }
}

