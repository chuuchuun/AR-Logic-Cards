using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance { get; private set; }

    private List<Wire> _wires = new List<Wire>();

    // События для других систем (например, для визуализации)
    public System.Action<Wire> OnWireAdded;
    public System.Action<Wire> OnWireRemoved;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool AddWire(Pin source, Pin target)
    {
        // Проверки
        if (source == null || target == null)
        {
            Debug.LogWarning("Input or output are not selected");
            return false;
        }
        if (source.pinType != PinType.Output || target.pinType != PinType.Input)
        {
            Debug.LogWarning("Attempting to connect output to input");
            return false;
        }
        if (source.parentCard == target.parentCard)
        {
            Debug.LogWarning("Cannot connect a card to itself");
            return false;
        }
        // Проверка на существующее соединение
        if (IsConnected(source, target))
        {
            Debug.LogWarning("Such connection already exists");
            return false;
        }

        Wire newWire = new Wire(source, target);
        _wires.Add(newWire);
        OnWireAdded?.Invoke(newWire);
        Debug.Log($"Wire added: {source.parentCard.name}.{source.pinIndex} -> {target.parentCard.name}.{target.pinIndex}");
        return true;
    }

    public bool RemoveWire(Wire wire)
    {
        if (_wires.Remove(wire))
        {
            OnWireRemoved?.Invoke(wire);
            Debug.Log("Wire removed");
            return true;
        }
        return false;
    }

    public void RemoveAllWires()
    {
        foreach (var wire in _wires.ToArray())
            RemoveWire(wire);
    }

    public List<Wire> GetWiresForCard(ARCard card, bool isInput)
    {
        List<Wire> result = new List<Wire>();
        foreach (var wire in _wires)
        {
            if (isInput && wire.targetPin.parentCard == card)
                result.Add(wire);
            else if (!isInput && wire.sourcePin.parentCard == card)
                result.Add(wire);
        }
        return result;
    }

    public List<Wire> GetAllWires() => _wires;

    private bool IsConnected(Pin source, Pin target)
    {
        return _wires.Exists(w => w.sourcePin == source && w.targetPin == target);
    }
}