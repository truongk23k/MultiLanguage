using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LocalizationManager : MonoBehaviour
{
    public delegate void LocalizationChangedDelegate();
    public static event LocalizationChangedDelegate OnLocalizationChanged;

    public Language currentLanguage = Language.EN;

    [SerializeField]
    private List<LocalizationEntry> allLocalizations;

    private Dictionary<string, LocalizationEntry> localizedText = new();
    private static LocalizationManager instance;

#if UNITY_EDITOR
    [Header("Source CSV")]
    [SerializeField] private TextAsset localizationSourceText;
#endif

    private List<string> missingKeys = new();

    private void Awake()
    {
        instance = this;
        localizedText = new Dictionary<string, LocalizationEntry>();

        foreach (var entry in allLocalizations)
        {
            if (!localizedText.ContainsKey(entry.key))
                localizedText.Add(entry.key, entry);
        }
    }

    private void OnDestroy()
    {
        string listkeys = string.Join(", ", missingKeys);
        Debug.Log("Missing keys: " + listkeys);
    }

    public static string GetLocalizedValue(string key)
    {
        if (string.IsNullOrEmpty(key))
            return "invalid_key";

        int langIndex = (int)instance.currentLanguage;

        if (!instance.localizedText.TryGetValue(key, out var result)
            || result.values == null
            || langIndex >= result.values.Length)
        {
#if UNITY_EDITOR
            if (!instance.missingKeys.Contains(key))
                instance.missingKeys.Add(key);
#endif
            Debug.LogWarning($"[Localization] Missing key: {key} in {instance.currentLanguage}");

            var keyParts = key.Split('-');
            return keyParts.Length == 2 ? keyParts[1] : key;
        }

        return result.values[langIndex];
    }

    public static Language GetCurrentLanguage()
    {
        return instance.currentLanguage;
    }

#if UNITY_EDITOR
    [ContextMenu("Load CSV")]
    public void EditorLoadAll()
    {
        if (localizationSourceText == null) return;

        var lines = localizationSourceText.text.Split('\n')
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        if (lines.Count < 2)
        {
            Debug.LogError("Localization CSV is empty or invalid.");
            return;
        }

        allLocalizations = new List<LocalizationEntry>();
        for (int i = 1; i < lines.Count; i++)
        {
            var fields = lines[i].Split(',');
            if (fields.Length < 2) continue;

            allLocalizations.Add(new LocalizationEntry
            {
                key = fields[0],
                values = fields.Skip(1).Select(f => f.Replace("{{comma}}", ",")).ToArray()
            });
        }

        EditorUtility.SetDirty(this);
        Debug.Log($"Loaded {allLocalizations.Count} localization entries.");
    }

    [ContextMenu("Change Language to English")]
    public void EditorChangeToEnglish()
    {
        currentLanguage = Language.EN;
        OnLocalizationChanged?.Invoke();
    }

    [ContextMenu("Change Language to Myanmar")]
    public void EditorChangeToMyanmar()
    {
        currentLanguage = Language.MM;
        OnLocalizationChanged?.Invoke();
    }

    [ContextMenu("Change Language to Vietnamese")]
    public void EditorChangeToVietnamese()
    {
        currentLanguage = Language.VI;
        OnLocalizationChanged?.Invoke();
    }
#endif
}

[System.Serializable]
public class LocalizationEntry
{
    public string key;
    public string[] values;
}

public enum Language
{
    EN = 0,
    MM = 1,
    VI = 2
}
