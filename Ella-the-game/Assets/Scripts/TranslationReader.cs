using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;

public class TranslationReader : MonoBehaviour
{
    #region singleton
    private static TranslationReader _instance;
    public static TranslationReader Instance
    {
        get
        {
            return _instance;
        }
    }
    #endregion
    
    public List<Language> languages;
    public Language currentLanguage { get; private set; }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        StartCoroutine( GetTranslations() );
    }
    void Start()
    {

    }

    IEnumerator GetTranslations()
    {
        WWWForm form = new WWWForm();
	//using (UnityWebRequest www = UnityWebRequest.Get("https://jsonkeeper.com/b/REFD"))
        using (UnityWebRequest www = UnityWebRequest.Get("https://crazy-marvin.github.io/EllaTheGame/jsonFile.json"))
        {
            //www.SetRequestHeader("Access-Control-Allow-Origin", "*");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                JSONNode jsonResult;
                jsonResult = JSON.Parse(www.downloadHandler.text);
                ExtractTraslation(jsonResult);

            }
        }
    }
    public void ExtractTraslation(JSONNode inResults)
    {
        //// extracting Unlocked levels
        if (languages == null)
            languages = new List<Language>();
        for(int i=0; i<inResults.Count; i++)
        {
            //string[] words = inResults[i].Split(',');
            JSONArray words = inResults[i].AsArray;
            Language newLanguage = new Language(words[0], words[1], words[2], words[3], words[4], words[5], words[6], words[7], words[8]
                , words[9], words[10], words[11]);
            languages.Add(newLanguage);
        }
        if(PlayerPrefs.HasKey("currentLanguage"))
            currentLanguage = languages[PlayerPrefs.GetInt("currentLanguage")];
        else
        {
            PlayerPrefs.SetInt("currentLanguage",0);
            currentLanguage = languages[0];
        }
        StartCoroutine(LanguageChangedEvent_Execute(0.1f));
    }
    public delegate void LanguageChangedDelegate();
    public static event LanguageChangedDelegate LanguageChangedEvent;
    public IEnumerator LanguageChangedEvent_Execute(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        LanguageChangedEvent();
    }
    public void SetCurrentLanguage(int i)
    {
        currentLanguage = languages[i];
        PlayerPrefs.SetInt("currentLanguage", i);
        StartCoroutine(LanguageChangedEvent_Execute(0.1f));
    }
}
public struct Language
{
    public string languageCode;
    public string lvlSelect;
    public string difficulty;
    public string puppy;
    public string adult;
    public string senior;
    public string exit;
    public string score;
    public string share;
    public string replay;
    public string mainMenu;
    public string resume;
    public Language(string languageCode, string lvlSelect, string difficulty, string puppy, string adult, string senior, string exit, string score,
        string share, string replay, string mainmenu, string resume)
    {
        this.languageCode = languageCode;
        this.lvlSelect = lvlSelect;
        this.difficulty = difficulty;
        this.puppy = puppy;
        this.adult = adult;
        this.senior = senior;
        this.exit = exit;
        this.score = score;
        this.share = share;
        this.replay = replay;
        this.mainMenu = mainmenu;
        this.resume = resume;
    }
}
