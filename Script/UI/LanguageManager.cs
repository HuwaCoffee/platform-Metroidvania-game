using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;
using System.Linq;
using System;

[RequireComponent(typeof(TMP_Dropdown))]
public class LanguageManager : MonoBehaviour
{
    #region Singleton
    public static LanguageManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Serialized Fields
    [System.Serializable]
    public class LanguageOption
    {
        public string localeCode;
        public LocalizedString displayName;
    }

    [Header("Font Settings")]
    [SerializeField] private TMP_FontAsset defaultFont;
    [SerializeField] private TMP_FontAsset chineseFont;

    [Header("Language Settings")]
    [SerializeField] private LanguageOption[] languageOptions = 
    {
        new LanguageOption { localeCode = "zh", displayName = new LocalizedString("UI", "language.zh") },
        new LanguageOption { localeCode = "en", displayName = new LocalizedString("UI", "language.en") }
    };

    [Header("UI Reference")]
    [SerializeField] private TMP_Dropdown languageDropdown;
    #endregion

    #region Private Variables
    private const string PrefsKey = "SelectedLanguage";
    private bool isChangingLanguage = false;
    #endregion

    #region Initialization
    void Initialize()
    {
        ConfigureFontFallback();
        SetupDropdown();
        LoadSavedLanguage();
        RegisterEvents();
    }

    void ConfigureFontFallback()
    {
        if (defaultFont != null && chineseFont != null)
        {
            defaultFont.fallbackFontAssetTable = new System.Collections.Generic.List<TMP_FontAsset> { chineseFont };
        }
    }
    #endregion

    #region Dropdown Setup
    void SetupDropdown()
    {
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(languageOptions.Select(opt => 
            new TMP_Dropdown.OptionData(opt.displayName.GetLocalizedString())
        ).ToList());
    }

    void UpdateDropdownOptions()
    {
        var currentIndex = languageDropdown.value;
        var options = languageOptions.Select(opt => 
            new TMP_Dropdown.OptionData(opt.displayName.GetLocalizedString())
        ).ToList();
        
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(options);
        languageDropdown.SetValueWithoutNotify(currentIndex);
    }
    #endregion

    #region Language Switching
    public void SwitchLanguage(int index)
    {
        if (index < 0 || index >= languageOptions.Length) return;
        if (isChangingLanguage) return;

        StartCoroutine(LanguageSwitchRoutine(languageOptions[index].localeCode));
    }

    private System.Collections.IEnumerator LanguageSwitchRoutine(string localeCode)
    {
        isChangingLanguage = true;
        
        // 等待本地化系统初始化
        if (!LocalizationSettings.InitializationOperation.IsDone)
        {
            yield return LocalizationSettings.InitializationOperation;
        }

        var targetLocale = LocalizationSettings.AvailableLocales.Locales
            .FirstOrDefault(l => l.Identifier.Code == localeCode);

        if (targetLocale != null && LocalizationSettings.SelectedLocale != targetLocale)
        {
            LocalizationSettings.SelectedLocale = targetLocale;
            SaveLanguageSetting(localeCode);
        }

        // 延迟更新UI组件
        yield return new WaitForEndOfFrame();
        UpdateTextComponents();
        UpdateDropdownOptions();
        
        isChangingLanguage = false;
    }
    #endregion

    #region Save/Load
    void LoadSavedLanguage()
    {
        var savedLang = PlayerPrefs.GetString(PrefsKey, 
            Application.systemLanguage == SystemLanguage.Chinese ? "zh" : "en");
        
        var index = Array.FindIndex(languageOptions, x => x.localeCode == savedLang);
        if (index != -1)
        {
            languageDropdown.SetValueWithoutNotify(index);
            StartCoroutine(LanguageSwitchRoutine(savedLang));
        }
    }

    void SaveLanguageSetting(string localeCode)
    {
        PlayerPrefs.SetString(PrefsKey, localeCode);
        PlayerPrefs.Save();
    }
    #endregion

    #region Event Handling
    void RegisterEvents()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        languageDropdown.onValueChanged.AddListener(SwitchLanguage);
    }

    void UnregisterEvents()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        languageDropdown.onValueChanged.RemoveListener(SwitchLanguage);
    }

    void OnLocaleChanged(Locale locale)
    {
        UpdateTextComponents();
        UpdateDropdownOptions();
    }
    #endregion

    #region UI Update
    void UpdateTextComponents()
    {
        var textComponents = FindObjectsOfType<TMP_Text>(true);
        foreach (var text in textComponents)
        {
            text.font = GetLocaleFont(LocalizationSettings.SelectedLocale.Identifier.Code);
            text.SetAllDirty();
        }
    }

    TMP_FontAsset GetLocaleFont(string localeCode)
    {
        return localeCode == "zh" ? chineseFont : defaultFont;
    }
    #endregion

    #region Lifecycle
    void OnEnable()
    {
        if (Instance == this)
        {
            RegisterEvents();
        }
    }

    void OnDisable()
    {
        if (Instance == this)
        {
            UnregisterEvents();
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            UnregisterEvents();
            Instance = null;
        }
    }
    #endregion
}