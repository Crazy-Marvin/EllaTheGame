using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inGameUI : MonoBehaviour {

    #region Singleton
    private static inGameUI _instance;
    public static inGameUI Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("inGameUI");
                go.AddComponent<inGameUI>();
            }
            return _instance;
        }
    }
    #endregion

    private float healthAmount=1;
    private int scoreAmount=0;
    [SerializeField]
    private Image healthImage;
    [SerializeField]
    private Text scoreText;
    private Transform inGameMenu;
    private LevelGlobals levelGlobals;
    public Sprite unMuteSprite, muteSprite;
    private GameObject endMenu;
    public AudioClip failedClip;
    float playTime_timer = 0; // Used for "Hugs And kisses" & "Lover" achievements
    bool playTimeAchievementRegistred; // Used for "Hugs And kisses" & "Lover" achievements
    void Awake()
    {
        _instance = this;
       
    }
    // Use this for initialization
    void Start () {
        GameManager.GameOverEvent += GameOverEventExecuted;
        GameManager.GamePauseEvent += GamePauseEventExecuted;
        GameManager.GameResumeEvent += GameResumeEventExecuted;

        playTime_timer = Time.time + 60;
        inGameMenu = transform.Find("inGameMenu").gameObject.transform;
        if(inGameMenu.gameObject.activeSelf)
            inGameMenu.gameObject.SetActive(false);
        levelGlobals = GameObject.FindGameObjectWithTag("LevelGlobalGO").GetComponent<LevelGlobals>();
        endMenu = GameObject.FindGameObjectWithTag("EndMenu");
        endMenu.SetActive(false);

    }

    // Update is called once per frame
    void Update () {
        if (!playTimeAchievementRegistred)
        {
            if (Time.time > playTime_timer)
            {
                PlayGames.Instance.HugsAndkisses_Lover();
                playTimeAchievementRegistred = true;
            }
        }
        handleBars();
    }
    private void handleBars()
    {
        healthImage.fillAmount = healthAmount;
        scoreText.text = (scoreAmount).ToString("0000");
    }

    public void setHealthAmount(int value)
    {
        healthAmount = (float)value / 100;
    }
    public void setScoreAmount(int value)
    {
        scoreAmount = value;
       
    }
    public void muteGame(GameObject Btn)
    {
        if (AudioListener.volume == 0)
        {
            AudioListener.volume = 1;
            Btn.GetComponent<Image>().sprite = muteSprite;
        }
        else
        {
            AudioListener.volume = 0;
            Btn.GetComponent<Image>().sprite = unMuteSprite;
        }
    }
    public void showEndMenu()
    {
        endMenu.SetActive(true);
        GameObject.FindGameObjectWithTag("scoreText").GetComponent<Text>().text = scoreAmount.ToString();
        PlayGames.Instance.AddScoreToLeaderboard(scoreAmount);
        PlayGames.Instance.ScoreAchievements(scoreAmount);
    }
    public void GameOverEventExecuted()
    {
        showEndMenu();
        this.gameObject.GetComponent<AudioSource>().clip = failedClip;
        this.gameObject.GetComponent<AudioSource>().loop = false;
        this.gameObject.GetComponent<AudioSource>().Play();
    }
    private void GamePauseEventExecuted()
    {
        inGameMenu.gameObject.SetActive(true);
    }
    private void GameResumeEventExecuted()
    {
        inGameMenu.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        GameManager.GameOverEvent -= GameOverEventExecuted;
        GameManager.GamePauseEvent -= GamePauseEventExecuted;
        GameManager.GameResumeEvent -= GameResumeEventExecuted;
    }

    public void ExecutePauseEvent()
    {
        GameManager.Instance.ExecuteGamePauseEvent();
    }
    public void ExecuteResumeEvent()
    {
        GameManager.Instance.ExecuteGameResumeEvent();
    }
    public void ReplayGame()
    {
        PlayGames.Instance.KeepHitting();
        GameObject.FindGameObjectWithTag("LevelGlobalGO").GetComponent<LevelGlobals>().ReplayGame();
    }
    public void LoadMainMenu()
    {
        PlayGames.Instance.AddScoreToLeaderboard(scoreAmount);
        PlayGames.Instance.ScoreAchievements(scoreAmount);
        GameObject.FindGameObjectWithTag("LevelGlobalGO").GetComponent<LevelGlobals>().LoadMainMenu();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
