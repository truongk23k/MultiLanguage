using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    public string key;

    [Header("Component")]
    [SerializeField] private TMP_Text tmpText;
    [SerializeField] private Text uiText;

    [SerializeField] private bool isDynamicText = false;
    private string originKey = "";

    private void Awake()
    {
        LocalizationManager.OnLocalizationChanged += UpdateLocalizedText;
    }

    private void OnEnable()
    {
        if (isDynamicText)
        {
            if (tmpText && !string.IsNullOrEmpty(tmpText.text))
                originKey = tmpText.text;

            if (uiText && !string.IsNullOrEmpty(uiText.text))
                originKey = uiText.text;

            if (!string.IsNullOrEmpty(originKey))
                UpdateLocalizedText();
        }
    }

    private void Start()
    {
        UpdateLocalizedText();
    }

    private void OnDestroy()
    {
        LocalizationManager.OnLocalizationChanged -= UpdateLocalizedText;
    }

    private void Reset()
    {
        tmpText = GetComponent<TMP_Text>();
        uiText = GetComponent<Text>();
    }

    private void UpdateLocalizedText()
    {
        string displayKey = string.IsNullOrEmpty(originKey) ? key : originKey;

        if (tmpText)
            tmpText.text = LocalizationManager.GetLocalizedValue(displayKey);

        if (uiText)
            uiText.text = LocalizationManager.GetLocalizedValue(displayKey);
    }

#if UNITY_EDITOR
    [ContextMenu("Auto Assign Components")]
    public void EditorConfig()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        uiText = GetComponent<Text>();
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
