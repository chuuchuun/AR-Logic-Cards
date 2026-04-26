using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject topBar;
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
    public Button hamburgerButton;
    public GameObject hamburgerMenuPanel;
    public Button resetWiresMenuButton;
    public Button hintMenuButton;
    public Button allowedCardsMenuButton;

    [Header("Hint Popup")]
    public GameObject hintPopup;
    public TextMeshProUGUI hintText;
    public Button closeHintButton;

    [Header("Generic Message Popup")]
    public GameObject messagePopup;
    public TextMeshProUGUI messageTitle;
    public TextMeshProUGUI messageBody;
    public Button closeMessageButton;

    [Header("Level Info Popup")]
    public GameObject levelInfoPopup;
    public TextMeshProUGUI levelInfoTitle;
    public TextMeshProUGUI levelInfoMessage;
    public Button levelInfoContinueButton;

    private Action onLevelInfoContinue;

    public void ShowLevelInfoPopup(string levelName, string allowedCards, Action onContinue)
    {
        if (levelInfoPopup != null)
        {
            levelInfoTitle.text = levelName;
            levelInfoMessage.text = $"Allowed cards:\n{allowedCards.Replace(',', '\n')}";
            onLevelInfoContinue = onContinue;
            levelInfoPopup.SetActive(true);
        }
    }

    public void HideLevelInfoPopup()
    {
        if (levelInfoPopup != null)
            levelInfoPopup.SetActive(false);
    }

    private bool isMenuOpen = false;

    public void ShowMessagePopup(string title, string body)
    {
        if (messagePopup != null)
        {
            messageTitle.text = title;
            messageBody.text = body;
            messagePopup.SetActive(true);
        }
    }

    public void HideMessagePopup()
    {
        if (messagePopup != null)
            messagePopup.SetActive(false);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Level selection button
        if (levelSelectionButton != null)
            levelSelectionButton.onClick.AddListener(ToggleLevelSelection);

        if (levelSelectionPanel != null)
            levelSelectionPanel.SetActive(false);

        // Level complete popup
        if (levelCompletePopup != null)
            levelCompletePopup.SetActive(false);
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);

        // Invalid cards popup
        if (closeInvalidPopupButton != null)
            closeInvalidPopupButton.onClick.AddListener(HideInvalidCardsPopup);
        if (invalidCardsPopup != null)
            invalidCardsPopup.SetActive(false);

        // Allowed cards popup (button on top bar)
        if (allowedCardsButton != null)
            allowedCardsButton.onClick.AddListener(ShowAllowedCardsPopup);
        if (closeAllowedPopupButton != null)
            closeAllowedPopupButton.onClick.AddListener(HideAllowedCardsPopup);
        if (allowedCardsPopup != null)
            allowedCardsPopup.SetActive(false);

        // Hamburger menu
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
        if (hamburgerMenuPanel != null)
            hamburgerMenuPanel.SetActive(false);

        // Hint popup
        if (closeHintButton != null)
            closeHintButton.onClick.AddListener(HideHintPopup);
        if (hintPopup != null)
            hintPopup.SetActive(false);

        // Generic message popup
        if (closeMessageButton != null)
            closeMessageButton.onClick.AddListener(HideMessagePopup);
        if (messagePopup != null)
            messagePopup.SetActive(false);

        if (levelInfoContinueButton != null)
            levelInfoContinueButton.onClick.AddListener(() => {
                HideLevelInfoPopup();
                onLevelInfoContinue?.Invoke();
            });
        if (levelInfoPopup != null)
            levelInfoPopup.SetActive(false);
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