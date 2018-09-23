using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameManager");
                go.AddComponent<GameManager>();
            }
            return _instance;
        }
    }
    #endregion
    public enum GameState
    {
        GamePaused,
        GameOver,
        GameRunning,
        MainMenu
    }
    public delegate void NotifyOnGameStart();
    public static event NotifyOnGameStart GameStartEvent;
    public delegate void NotifyOnGameOver();
    public static event NotifyOnGameOver GameOverEvent;

    public delegate void NotifyOnGamePause();
    public static event NotifyOnGameOver GamePauseEvent;
    public delegate void NotifyOnGameResume();
    public static event NotifyOnGameOver GameResumeEvent;

    public GameState gameState { get; set; }
    public int matchScore { get; private set; }


    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        Application.targetFrameRate = 60;
    }

    void Start () {
        GameOverEvent += GameOverEventExecuted;
        GamePauseEvent += GamePauseEventExecuted;
        GameResumeEvent += GameResumeEventExecuted;
        GameStartEvent += GameStartEventExecuted;
    }

    void Update () {
		if(gameState == GameState.GamePaused)
        {
           // Time.timeScale = 0;
        }
	}
    /// <summary>
    /// ////////////Score Management
    /// </summary>
    /// <param name="score"></param>
    public void SetMatchScore(int score)
    {
        this.matchScore = score;

    }
    public void SaveScore()
    {
        var tmpScore = PlayerPrefs.GetInt("playerGlobalScore");
        tmpScore += this.matchScore;
        PlayerPrefs.SetInt("playerGlobalScore", tmpScore);
        this.matchScore = 0;
    }
    /// <summary>
    /// /////////////////// Game Events Executions
    /// </summary>
    public void ExecuteGameStartEvent()
    {
        GameStartEvent();
    }
    public void ExecuteGameOverEvent()
    {
        GameOverEvent();
    }
    public void ExecuteGamePauseEvent()
    {
        GamePauseEvent();
    }
    public void ExecuteGameResumeEvent()
    {
        GameResumeEvent();
    }
    /// <summary>
    /// /////////////// Game Events Registred Functions
    /// </summary>
    private void GameOverEventExecuted()
    {
        gameState = GameState.GameOver;
        SaveScore();
    }
    private void GamePauseEventExecuted()
    {
        gameState = GameState.GamePaused;
        Time.timeScale = 0;
    }
    private void GameResumeEventExecuted()
    {
        gameState = GameState.GameRunning;
        Time.timeScale = 1;
    }

    private void GameStartEventExecuted()
    {
        gameState = GameState.GameRunning;
        Time.timeScale = 1;
    }
}
