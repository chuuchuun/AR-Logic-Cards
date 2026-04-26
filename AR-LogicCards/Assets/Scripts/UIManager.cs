using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject topBar;                     // assign the top bar GameObject
    public TextMeshProUGUI taskText;
    public Button resetWiresButton;
    public Button levelSelectionButton;
    public GameObject levelSelectionPanel;
    public GameObject levelCompletePopup;
    public TextMeshProUGUI popupMessageText;
    public Button nextLevelButton;
    public float autoAdvanceDelay = 2f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (resetWiresButton != null)
            resetWiresButton.onClick.AddListener(ResetAllWires);

        if (levelSelectionButton != null)
            levelSelectionButton.onClick.AddListener(ToggleLevelSelection);

        if (levelSelectionPanel != null)
            levelSelectionPanel.SetActive(false);

        if (levelCompletePopup != null)
            levelCompletePopup.SetActive(false);

        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
    }

    private void ResetAllWires()
    {
        if (ConnectionManager.Instance != null)
            ConnectionManager.Instance.RemoveAllWires();
    }

    private void ToggleLevelSelection()
    {
        bool isActive = !levelSelectionPanel.activeSelf;
        levelSelectionPanel.SetActive(isActive);

        if (topBar != null)
            topBar.SetActive(!isActive);
    }

    // Call this method when you need to close the level selection panel externally
    public void CloseLevelSelection()
    {
        if (levelSelectionPanel != null && levelSelectionPanel.activeSelf)
        {
            levelSelectionPanel.SetActive(false);
            if (topBar != null)
                topBar.SetActive(true);
        }
    }

    public void SetTask(string description)
    {
        if (taskText != null)
            taskText.text = description;
    }

    public void ShowLevelComplete(string message)
    {
        if (levelCompletePopup != null)
        {
            if (popupMessageText != null)
                popupMessageText.text = message;
            levelCompletePopup.SetActive(true);
            CancelInvoke(nameof(AutoLoadNextLevel));
            Invoke(nameof(AutoLoadNextLevel), autoAdvanceDelay);
        }
        Debug.Log("Level Complete: " + message);
    }

    private void AutoLoadNextLevel()
    {
        if (levelCompletePopup != null && levelCompletePopup.activeSelf)
        {
            levelCompletePopup.SetActive(false);
            LevelManager.Instance?.LoadNextLevel();
        }
    }

    private void OnNextLevelClicked()
    {
        CancelInvoke(nameof(AutoLoadNextLevel));
        if (levelCompletePopup != null)
            levelCompletePopup.SetActive(false);
        LevelManager.Instance?.LoadNextLevel();
    }

    public void HideLevelCompletePopup()
    {
        if (levelCompletePopup != null)
        {
            CancelInvoke(nameof(AutoLoadNextLevel));
            levelCompletePopup.SetActive(false);
        }
    }
}