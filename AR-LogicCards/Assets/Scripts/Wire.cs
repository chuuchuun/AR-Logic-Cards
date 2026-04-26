using UnityEngine;

[System.Serializable]
public class Wire
{
    public Pin sourcePin;
    public Pin targetPin;
    public LineRenderer visual;
    public BoxCollider collider;

    public Wire(Pin source, Pin target)
    {
        sourcePin = source;
        targetPin = target;
        visual = null;
        collider = null;
    }
}