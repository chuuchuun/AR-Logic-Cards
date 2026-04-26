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

    public void LoadNextLevel()
    {
        if (currentLevelIndex + 1 < levels.Count)
            LoadLevel(currentLevelIndex + 1);
        else
            Debug.Log("All levels completed!");
    }

    public void CheckLevelCompletion()
    {
        if (levels.Count == 0) return;
        LevelData currentLevel = levels[currentLevelIndex];

        ARCard outputCard = FindOutputCard();
        if (outputCard == null || !outputCard.currentValue)
            return;

        if (currentLevel.requiredCardNames != null && currentLevel.requiredCardNames.Length > 0)
        {
            ARCard[] allCards = FindObjectsOfType<ARCard>();
            foreach (string requiredName in currentLevel.requiredCardNames)
            {
                bool found = System.Array.Exists(allCards, card => card.name == requiredName);
                if (!found) return;
            }
        }

        Debug.Log($"Level {currentLevel.levelName} completed!");
        if (UIManager.Instance != null)
            UIManager.Instance.ShowLevelComplete(currentLevel.successMessage);

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