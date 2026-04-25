using System;

[Serializable]
public class Wire
{
    public Pin sourcePin;
    public Pin targetPin;

    public Wire(Pin source, Pin target)
    {
        sourcePin = source;
        targetPin = target;
    }
}