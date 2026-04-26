using System.Collections.Generic;
using UnityEngine;

public class CircuitCalculator : MonoBehaviour
{
    public static CircuitCalculator Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Recalculate()
    {
        Debug.Log("Recalculate called");
        ARCard[] cards = FindObjectsOfType<ARCard>();

        foreach (var card in cards)
            if (card.cardType != CardType.Input)
                card.currentValue = false;

        foreach (var card in cards)
            if (card.cardType == CardType.Input)
            {
                Debug.Log($"Propagating from input {card.name} with value {card.currentValue}");
                Propagate(card, new HashSet<ARCard>());
            }

        if (LevelManager.Instance != null)
            LevelManager.Instance.CheckLevelCompletion();
    }

    private bool Propagate(ARCard source, HashSet<ARCard> visited)
    {
        if (visited.Contains(source)) return false;
        visited.Add(source);

        List<Wire> wires = ConnectionManager.Instance.GetWiresFromSourceCard(source);
        foreach (Wire wire in wires)
        {
            ARCard targetCard = wire.targetPin.parentCard;
            bool result = ComputeTargetValue(targetCard);
            if (targetCard.currentValue != result)
            {
                targetCard.currentValue = result;
                if (targetCard.cardType == CardType.Output)
                    targetCard.UpdateLedVisual();
                Propagate(targetCard, new HashSet<ARCard>(visited));
            }
        }
        return true;
    }

    private bool ComputeTargetValue(ARCard card)
    {
        List<Wire> incoming = ConnectionManager.Instance.GetWiresToCard(card);
        if (incoming.Count == 0 && card.cardType != CardType.Input) return false;

        switch (card.cardType)
        {
            case CardType.And:
                foreach (Wire w in incoming)
                    if (!w.sourcePin.parentCard.currentValue) return false;
                return true;

            case CardType.Or:
                foreach (Wire w in incoming)
                    if (w.sourcePin.parentCard.currentValue) return true;
                return false;

            case CardType.Not:
                if (incoming.Count == 0) return false;
                bool inputVal = incoming[0].sourcePin.parentCard.currentValue;
                bool result = !inputVal;
                Debug.Log($"NOT {card.name}: input={inputVal} output={result}");
                return result;

            case CardType.Input:
                return card.currentValue;

            case CardType.Output:
                if (incoming.Count == 0) return false;
                return incoming[0].sourcePin.parentCard.currentValue;

            default:
                return false;
        }
    }
}