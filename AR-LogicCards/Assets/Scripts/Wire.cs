using System;
using UnityEngine;

[Serializable]
public class Wire
{
    public Pin sourcePin;
    public Pin targetPin;
    public LineRenderer visual;

    public Wire(Pin source, Pin target)
    {
        sourcePin = source;
        targetPin = target;
        visual = null;
    }
}