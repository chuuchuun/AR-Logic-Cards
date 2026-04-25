using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance { get; private set; }

    private List<Wire> _wires = new List<Wire>();

    public System.Action<Wire> OnWireAdded;
    public System.Action<Wire> OnWireRemoved;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool AddWire(Pin source, Pin target)
    {
        if (source == null || target == null) return false;
        if (source.pinType != PinType.Output || target.pinType != PinType.Input) return false;
        if (source.parentCard == target.parentCard) return false;
        if (IsConnected(source, target)) return false;

        Wire wire = new Wire(source, target);
        _wires.Add(wire);
        OnWireAdded?.Invoke(wire);
        Debug.Log($"Wire added: {source.parentCard.name}.{source.pinIndex} -> {target.parentCard.name}.{target.pinIndex}");

        CircuitCalculator.Instance?.Recalculate();
        return true;
    }

    public bool RemoveWire(Wire wire)
    {
        if (_wires.Remove(wire))
        {
            OnWireRemoved?.Invoke(wire);
            Debug.Log("Wire removed");
            CircuitCalculator.Instance?.Recalculate();
            return true;
        }
        return false;
    }

    public void RemoveAllWires()
    {
        foreach (var wire in _wires.ToArray())
            RemoveWire(wire);
    }

    public List<Wire> GetWiresFromSourceCard(ARCard card)
    {
        return _wires.FindAll(w => w.sourcePin.parentCard == card);
    }

    public List<Wire> GetWiresToCard(ARCard card)
    {
        return _wires.FindAll(w => w.targetPin.parentCard == card);
    }

    public List<Wire> GetAllWires() => _wires;

    private bool IsConnected(Pin source, Pin target)
    {
        return _wires.Exists(w => w.sourcePin == source && w.targetPin == target);
    }
}