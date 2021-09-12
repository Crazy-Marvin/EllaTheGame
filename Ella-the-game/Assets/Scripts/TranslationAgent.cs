using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TranslationAgent : MonoBehaviour
{
    Text targetText;
    public enum Word
    {
        lvlSelect,
        difficulty,
        puppy,
        adult,
        senior,
        exit,
        score,
        share,
        replay,
        mainMenu,
        resume
    }
    public Word word;
    void Start()
    {
        TranslationReader.LanguageChangedEvent += LanguageChanged;
        targetText = GetComponent<Text>();
 
    }
    void LanguageChanged()
    {
        GetTranslation();
    }
    void GetTranslation()
    {
        switch ((int)word)
        {
            case 0:
                targetText.text = TranslationReader.Instance.currentLanguage.lvlSelect;
                break;
            case 1:
                targetText.text = TranslationReader.Instance.currentLanguage.difficulty +" :";
                break;
            case 2:
                targetText.text = TranslationReader.Instance.currentLanguage.puppy;
                break;
            case 3:
                targetText.text = TranslationReader.Instance.currentLanguage.adult;
                break;
            case 4:
                targetText.text = TranslationReader.Instance.currentLanguage.senior;
                break;
            case 5:
                targetText.text = TranslationReader.Instance.currentLanguage.exit;
                break;
            case 6:
                targetText.text = TranslationReader.Instance.currentLanguage.score;
                break;
            case 7:
                targetText.text = TranslationReader.Instance.currentLanguage.share;
                break;
            case 8:
                targetText.text = TranslationReader.Instance.currentLanguage.replay;
                break;
            case 9:
                targetText.text = TranslationReader.Instance.currentLanguage.mainMenu;
                break;
            case 10:
                targetText.text = TranslationReader.Instance.currentLanguage.resume;
                break;
        }
    }
    private void OnEnable()
    {
        Invoke("GetTranslation", 1);
    }
    
    private void OnDestroy()
    {
        TranslationReader.LanguageChangedEvent -= LanguageChanged;
    }
}
