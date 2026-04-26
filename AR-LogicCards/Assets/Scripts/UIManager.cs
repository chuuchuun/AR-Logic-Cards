using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject topBar;                     // assign the top bar GameObject
    public TextMeshProUGUI taskText;
    public Button levelSelectionButton;
    public GameObject levelSelectionPanel;
    public GameObject levelCompletePopup;
    public TextMeshProUGUI popupMessageText;
    public Button nextLevelButton;
    public float autoAdvanceDelay = 2f;

    [Header("Invalid Cards Popup")]
    public GameObject invalidCardsPopup;
    public TextMeshProUGUI invalidCardsMessageText;
    public Button closeInvalidPopupButton;

    [Header("Allowed Cards Info")]
    public Button allowedCardsButton;
    public GameObject allowedCardsPopup;
    public TextMeshProUGUI allowedCardsListText;
    public Button closeAllowedPopupButton;

    [Header("Hamburger Menu")]
    public Button hamburgerButton;               // the ☰ button
    public GameObject hamburgerMenuPanel;        // panel that appears
    public Button resetWiresMenuButton;          // button inside menu
    public Button hintMenuButton;                // button inside menu
    public Button allowedCardsMenuButton;        // button inside menu

    [Header("Hint Popup")]
    public GameObject hintPopup;
    public TextMeshProUGUI hintText;
    public Button closeHintButton;

    private bool isMenuOpen = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {

        if (levelSelectionButton != null)
            levelSelectionButton.onClick.AddListener(ToggleLevelSelection);

        if (levelSelectionPanel != null)
            levelSelectionPanel.SetActive(false);

        if (levelCompletePopup != null)
            levelCompletePopup.SetActive(false);

        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);

        if (closeInvalidPopupButton != null)
            closeInvalidPopupButton.onClick.AddListener(HideInvalidCardsPopup);

        if (invalidCardsPopup != null)
            invalidCardsPopup.SetActive(false);

        if (allowedCardsButton != null)
            allowedCardsButton.onClick.AddListener(ShowAllowedCardsPopup);

        if (closeAllowedPopupButton != null)
            closeAllowedPopupButton.onClick.AddListener(HideAllowedCardsPopup);

        if (allowedCardsPopup != null)
            allowedCardsPopup.SetActive(false);

        if (hamburgerButton != null)
            hamburgerButton.onClick.AddListener(ToggleHamburgerMenu);

        if (resetWiresMenuButton != null)
            resetWiresMenuButton.onClick.AddListener(() => {
                ResetAllWires();
                CloseHamburgerMenu();
            });

        if (hintMenuButton != null)
            hintMenuButton.onClick.AddListener(() => {
                ShowHintPopup();
                CloseHamburgerMenu();
            });

        if (allowedCardsMenuButton != null)
            allowedCardsMenuButton.onClick.AddListener(() => {
                ShowAllowedCardsPopup();
                CloseHamburgerMenu();
            });

        if (closeHintButton != null)
            closeHintButton.onClick.AddListener(HideHintPopup);

        if (hintPopup != null)
            hintPopup.SetActive(false);

        if (hamburgerMenuPanel != null)
            hamburgerMenuPanel.SetActive(false);
    }

    private void ToggleHamburgerMenu()
    {
        isMenuOpen = !isMenuOpen;
        if (hamburgerMenuPanel != null)
            hamburgerMenuPanel.SetActive(isMenuOpen);
    }

    private void CloseHamburgerMenu()
    {
        isMenuOpen = false;
        if (hamburgerMenuPanel != null)
            hamburgerMenuPanel.SetActive(false);
    }

    private void ShowHintPopup()
    {
        if (hintPopup == null) return;
        // Get current level hint from LevelManager
        if (LevelManager.Instance != null)
        {
            string hint = LevelManager.Instance.GetCurrentLevelHint();
            hintText.text = hint;
        }
        hintPopup.SetActive(true);
    }

    private void HideHintPopup()
    {
        if (hintPopup != null)
            hintPopup.SetActive(false);
    }

    public void ShowAllowedCardsPopup()
    {
        if (allowedCardsPopup != null)
        {
            if (LevelManager.Instance != null)
            {
                string cardList = LevelManager.Instance.GetAllowedCardsList();
                allowedCardsListText.text = string.IsNullOrEmpty(cardList)
                    ? "No restrictions. Any card is allowed."
                    : cardList.Replace(", ", "\n");
            }
            allowedCardsPopup.SetActive(true);
        }
    }

    public void HideAllowedCardsPopup()
    {
        if (allowedCardsPopup != null)
            allowedCardsPopup.SetActive(false);
    }

    public void ShowInvalidCardsPopup(string message)
    {
        if (invalidCardsPopup != null)
        {
            if (invalidCardsMessageText != null)
                invalidCardsMessageText.text = message;
            invalidCardsPopup.SetActive(true);
        }
        Debug.LogWarning("Invalid cards: " + message);
    }

    public void HideInvalidCardsPopup()
    {
        if (invalidCardsPopup != null)
            invalidCardsPopup.SetActive(false);
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