using UnityEngine;
using UnityEngine.UI;
using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;


public class PlayGames : MonoBehaviour
{
    #region Singleton
    private static PlayGames _instance;
    public static PlayGames Instance
    {
        get
        {
            return _instance;
        }
    }
    #endregion
    //public int playerScore;
    Button leaderBoardBtn;
    string leaderboardID = "CgkIway7ibscEAIQDw";
    string achievementID = "";
    public static PlayGamesPlatform platform;
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
    }
    void Start()
    {
        leaderBoardBtn = GameObject.FindGameObjectWithTag("LeaderBoardBtn").GetComponent<Button>();
        //leaderBoardBtn.onClick.AddListener(ShowLeaderboard);
        if (platform == null)
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;
            platform = PlayGamesPlatform.Activate();
        }

        Social.Active.localUser.Authenticate(success =>
        {
            if (success)
            {
                Debug.Log("Logged in successfully");
            }
            else
            {
                Debug.Log("Login Failed");
            }
        });
        //UnlockAchievement();
    }
    
    public void AddScoreToLeaderboard(int score)
    {
        //Debug.Log("Addingggg tooo LeaderBoad");
        if (Social.Active.localUser.authenticated)
        {
            Social.ReportScore(score, leaderboardID, success => { });
        }
    }
    string leaderBoardMessage = "No Message";
    public void ShowLeaderboard()
    {
        //Debug.Log("Loading LeaderBoad");
        if (Social.Active.localUser.authenticated)
        {
            leaderBoardMessage = "User Authenticated ::: Showing the LeaderBoard";
            platform.ShowLeaderboardUI();
        }
        {
            leaderBoardMessage = "User Not Authenticated ::: Erroooooooooor";
        }
    }
    /*void OnGUI()
    {
        GUI.Label(new Rect(200, 100, 200, 20), leaderBoardMessage);
    }*/

    public void ShowAchievements()
    {
        if (Social.Active.localUser.authenticated)
        {
            platform.ShowAchievementsUI();
        }
    }

    public void UnlockAchievement()
    {
        if (Social.Active.localUser.authenticated)
        {
            Social.ReportProgress(achievementID, 100f, success => { });
        }
    }
}