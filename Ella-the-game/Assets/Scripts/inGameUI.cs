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

    void Awake()
    {
        _instance = this;
       
    }
    // Use this for initialization
    void Start () {
        GameManager.GameOverEvent += GameOverEventExecuted;
        GameManager.GamePauseEvent += GamePauseEventExecuted;
        GameManager.GameResumeEvent += GameResumeEventExecuted;

        inGameMenu = transform.Find("inGameMenu").gameObject.transform;
        if(inGameMenu.gameObject.activeSelf)
            inGameMenu.gameObject.SetActive(false);
        levelGlobals = GameObject.FindGameObjectWithTag("LevelGlobalGO").GetComponent<LevelGlobals>();
        endMenu = GameObject.FindGameObjectWithTag("EndMenu");
        endMenu.SetActive(false);

    }

    // Update is called once per frame
    void Update () {
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
}
