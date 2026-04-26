using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectionPanel : MonoBehaviour
{
    public LevelManager levelManager;
    public Transform buttonContainer;
    public Button closeButton;                     // assign this in Inspector
    public TMP_FontAsset buttonFont;
    public Sprite buttonSprite;

    private void OnEnable() => RefreshButtons();

    private void Start()
    {
        if (closeButton != null)
            // Use UIManager to close panel + reactivate top bar
            closeButton.onClick.AddListener(() => UIManager.Instance?.CloseLevelSelection());
    }

    private void RefreshButtons()
    {
        // Clear existing buttons
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        if (levelManager == null) levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null) return;

        // Create default sprite if none provided
        Sprite usedSprite = buttonSprite;
        if (usedSprite == null)
        {
            Texture2D tex = new Texture2D(2, 2);
            tex.SetPixels(new Color[] { Color.white, Color.white, Color.white, Color.white });
            tex.Apply();
            usedSprite = Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
        }

        for (int i = 0; i < levelManager.levels.Count; i++)
        {
            int index = i;
            LevelData level = levelManager.levels[i];

            GameObject btnObj = new GameObject("LevelButton_" + i);
            btnObj.transform.SetParent(buttonContainer);

            // RectTransform
            RectTransform rect = btnObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(800, 80);
            rect.anchorMin = new Vector2(0, 0.5f);
            rect.anchorMax = new Vector2(1, 0.5f);
            rect.anchoredPosition = Vector2.zero;

            // Layout Element
            LayoutElement layout = btnObj.AddComponent<LayoutElement>();
            layout.minWidth = 300;
            layout.preferredWidth = 400;
            layout.minHeight = 60;
            layout.preferredHeight = 80;

            // Image component (button background)
            Image img = btnObj.AddComponent<Image>();
            img.sprite = usedSprite;
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            img.color = new Color(0.2f, 0.2f, 0.2f);

            // Button component
            Button btn = btnObj.AddComponent<Button>();
            ColorBlock colors = btn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.8f, 0.8f, 0.8f);
            colors.pressedColor = new Color(0.6f, 0.6f, 0.6f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            btn.colors = colors;

            // Text child (TextMeshPro)
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = level.levelName;
            text.font = buttonFont;
            text.fontSize = 32;
            text.fontStyle = FontStyles.Normal;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            text.overflowMode = TextOverflowModes.Overflow;
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 0);
            textRect.offsetMax = new Vector2(-10, 0);

            // Listener: load level, then close panel + reactivate top bar via UIManager
            btn.onClick.AddListener(() => {
                levelManager.LoadLevel(index);
                UIManager.Instance?.CloseLevelSelection();
            });
        }
    }
}