using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Input,
    And,
    Or,
    Not,
    Output
}

public class ARCard : MonoBehaviour
{
    [Header("Card Type")]
    public CardType cardType;

    [Header("Logic Value")]
    public bool currentValue = false;

    [Header("Pins")]
    public List<Pin> inputPins = new List<Pin>();
    public List<Pin> outputPins = new List<Pin>();

    [Header("Input only")]
    public bool inputValue = false; 

    [Header("Output LED")]
    public Renderer ledRenderer;
    public Material ledOnMaterial;
    public Material ledOffMaterial;

    void Start()
    {
        FindPins();
        if (cardType == CardType.Output)
            UpdateLedVisual();
    }

    private void FindPins()
    {
        inputPins.Clear();
        outputPins.Clear();
        Pin[] pins = GetComponentsInChildren<Pin>();
        foreach (Pin pin in pins)
        {
            if (pin.pinType == PinType.Input)
                inputPins.Add(pin);
            else
                outputPins.Add(pin);
        }
    }

    public void SetInputValue(bool value)
    {
        if (cardType != CardType.Input) return;
        inputValue = value;
        currentValue = value;
        CircuitCalculator.Instance?.Recalculate();
    }

    public void UpdateLedVisual()
    {
        if (ledRenderer == null) return;
        if (currentValue && ledOnMaterial != null)
            ledRenderer.material = ledOnMaterial;
        else if (!currentValue && ledOffMaterial != null)
            ledRenderer.material = ledOffMaterial;
        else
            ledRenderer.material.color = currentValue ? Color.green : Color.red;
        Debug.Log($"LED {name} set to {currentValue}");
    }
}