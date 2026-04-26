using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Levels")]
    public List<LevelData> levels;
    public int currentLevelIndex = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    public bool IsCardAllowed(string cardName)
    {
        if (levels == null || currentLevelIndex >= levels.Count) return false;
        LevelData level = levels[currentLevelIndex];
        if (level.requiredCardNames == null || level.requiredCardNames.Length == 0)
            return true;
        return System.Array.Exists(level.requiredCardNames, name => name == cardName);
    }

    public string GetAllowedCardsList()
    {
        LevelData level = levels[currentLevelIndex];
        if (level.requiredCardNames == null) return "";
        return string.Join(", ", level.requiredCardNames);
    }

    public string GetCurrentLevelHint()
    {
        if (levels == null || currentLevelIndex >= levels.Count) return "No hint.";
        return levels[currentLevelIndex].hint;
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levels.Count) return;
        currentLevelIndex = index;
        LevelData level = levels[currentLevelIndex];

        if (ConnectionManager.Instance != null)
            ConnectionManager.Instance.RemoveAllWires();

        if (UIManager.Instance != null)
            UIManager.Instance.SetTask(level.description);

        Debug.Log($"Loaded level {currentLevelIndex}: {level.levelName}");
    }

    private bool AreAllRequiredCardsUsed()
    {
        LevelData level = levels[currentLevelIndex];
        if (level.requiredCardNames == null || level.requiredCardNames.Length == 0)
            return true;

        HashSet<string> usedCardNames = new HashSet<string>();
        foreach (Wire wire in ConnectionManager.Instance.GetAllWires())
        {
            usedCardNames.Add(wire.sourcePin.parentCard.name);
            usedCardNames.Add(wire.targetPin.parentCard.name);
        }

        foreach (string required in level.requiredCardNames)
        {
            if (!usedCardNames.Contains(required))
                return false;
        }
        return true;
    }

    public void LoadNextLevel()
    {
        if (currentLevelIndex + 1 < levels.Count)
            LoadLevel(currentLevelIndex + 1);
        else
        {
            Debug.Log("All levels completed!");
            UIManager.Instance?.ShowMessagePopup("Congratulations!", "You finished all levels!");
        }
    }

    public bool ValidateAllowedCards(out string invalidList)
    {
        invalidList = "";
        LevelData currentLevel = levels[currentLevelIndex];
        if (currentLevel.requiredCardNames == null || currentLevel.requiredCardNames.Length == 0)
            return true;

        HashSet<string> allowedSet = new HashSet<string>(currentLevel.requiredCardNames);
        ARCard[] allCards = FindObjectsOfType<ARCard>();
        List<string> invalidCards = new List<string>();

        foreach (ARCard card in allCards)
        {
            if (card.IsTracked && !allowedSet.Contains(card.name))
                invalidCards.Add(card.name);
        }

        if (invalidCards.Count > 0)
        {
            invalidList = string.Join(", ", invalidCards);
            return false;
        }
        return true;
    }

    public void CheckLevelCompletion()
    {
        if (levels.Count == 0) return;
        LevelData currentLevel = levels[currentLevelIndex];

        ARCard output = FindOutputCard();
        if (output == null || !output.currentValue) return;

        if (!AreAllRequiredCardsUsed())
        {
            UIManager.Instance?.ShowMessagePopup("Missing Required Cards",
                "You haven't used all the required cards for this level. Make sure every specified card is connected in your circuit.");
            return;
        }

        UIManager.Instance?.ShowLevelComplete(levels[currentLevelIndex].successMessage);
        Invoke(nameof(LoadNextLevel), 2f);
    }

    private ARCard FindOutputCard()
    {
        ARCard[] cards = FindObjectsOfType<ARCard>();
        foreach (ARCard card in cards)
            if (card.cardType == CardType.Output)
                return card;
        return null;
    }
}