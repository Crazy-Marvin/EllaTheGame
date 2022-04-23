using UnityEngine;
using UnityEngine.UI;
using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

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
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                .EnableSavedGames().RequestEmail().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;
            platform = PlayGamesPlatform.Activate();
        }

        Social.Active.localUser.Authenticate(success =>
        {
            if (success)
            {
                Debug.Log("Logged in successfully");
                RepeaterAchievement();
                SantaAchievement();
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
    #region Achievements
    public void ShowAchievements()
    {
        if (Social.Active.localUser.authenticated)
        {
            platform.ShowAchievementsUI();
        }
    }

    public void ScoreAchievements(int score)
    {
        string puppyId = "CgkIway7ibscEAIQAQ"
            , juniorId = "CgkIway7ibscEAIQAg"
            , dogId = "CgkIway7ibscEAIQAw"
            , seniorId = "CgkIway7ibscEAIQBA";

        
        if (Social.Active.localUser.authenticated)
        {
            //Social.ReportProgress(achievementID, 100f, success => { });
            platform.IncrementAchievement(puppyId, score, (bool success) => { 
                
            });
            platform.IncrementAchievement(juniorId, score, (bool success) => {

            });
            platform.IncrementAchievement(dogId, score, (bool success) => {

            });
            platform.IncrementAchievement(seniorId, score, (bool success) => {

            });
        }
    }
    public void RepeaterAchievement()
    {
        DateTime lastPlayTime;
        if(DateTime.TryParse(PlayerPrefs.GetString("lastPlayTime"), out lastPlayTime))
        {
            if(DateTime.Now.Day - lastPlayTime.Day == 1 && DateTime.Now.Month == lastPlayTime.Month)
            {
                platform.IncrementAchievement("CgkIway7ibscEAIQBQ", 1, (bool success) => {});
            }
        }
    }
    public void SantaAchievement()
    {
        DateTime date = DateTime.Now;
        if (date.Month == 12 && (date.Day >= 24 && date.Day <= 26))
        {
            platform.UnlockAchievement("CgkIway7ibscEAIQBg", (bool success) => {});
        }
    }
    public void KeepHitting()
    {
        //Press Replay hundred times
        platform.IncrementAchievement("CgkIway7ibscEAIQDg", 1, (bool success) => {});
    }
    public void SendLove()
    {
        //Share your score hundred times
        platform.IncrementAchievement("CgkIway7ibscEAIQDA", 1, (bool success) => { });
    }
    public void SharingIsCaring()
    {
        //Share your score ten times
        platform.IncrementAchievement("CgkIway7ibscEAIQCw", 1, (bool success) => { });
    }
    public void HugsAndkisses_Lover()
    {
        //Play at least 1000 times & Play at least 500 times
        platform.IncrementAchievement("CgkIway7ibscEAIQCg", 1, (bool success) => { });
        platform.IncrementAchievement("CgkIway7ibscEAIQCQ", 1, (bool success) => { });
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("lastPlayTime", DateTime.Now.ToString());
    }
    #endregion
}