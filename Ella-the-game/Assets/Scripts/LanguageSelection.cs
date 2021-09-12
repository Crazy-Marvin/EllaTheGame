using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSelection : MonoBehaviour
{
    public Dropdown languages;
    public GameObject languagePanel;
    int currentLanguage;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        
    }
    public void PickLanguage()
    {
        currentLanguage = languages.value;
        TranslationReader.Instance.SetCurrentLanguage(languages.value);
    }
    public void ToggleLanguagePanel()
    {
        languagePanel.SetActive(!languagePanel.activeSelf);
        if (languagePanel.activeSelf)
            PopulateLanguageOptions();
    }
    void PopulateLanguageOptions()
    {
        currentLanguage = PlayerPrefs.GetInt("currentLanguage");
        languages.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < TranslationReader.Instance.languages.Count; i++)
        {
            options.Add(TranslationReader.Instance.languages[i].languageCode);

        }
        languages.AddOptions(options);
        languages.value = currentLanguage;
    }
}
