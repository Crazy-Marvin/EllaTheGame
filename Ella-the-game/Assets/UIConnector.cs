using GooglePlayGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIConnector : MonoBehaviour
{
    public Button loginButton;
    public Button leaderboardButton;
    public Button achievementsButton;
    public TextMeshProUGUI statusText;

    private void Start()
    {
      


        if (PlayGames.Instance == null)
        {
            Debug.LogError("PlayGames instance not found!");
            return;
        }

        // Connect status text (optional)
        if (statusText != null)
        {
            PlayGames.Instance.SetStatusText(statusText);
        }

        // Connect login button (optional)
        if (loginButton != null)
        {
            loginButton.onClick.RemoveAllListeners();
            loginButton.onClick.AddListener(() => PlayGames.Instance.Authenticate());
        }

        // Connect leaderboard button using existing BindButtons method
        if (leaderboardButton != null && achievementsButton != null)
        {
            PlayGames.Instance.BindButtons(leaderboardButton, achievementsButton);
        }
        else
        {
            // Individual binding fallback
            if (leaderboardButton != null)
            {
                leaderboardButton.onClick.RemoveAllListeners();
                leaderboardButton.onClick.AddListener(() => PlayGames.Instance.ShowLeaderboard());
            }
            if (achievementsButton != null)
            {
                achievementsButton.onClick.RemoveAllListeners();
                achievementsButton.onClick.AddListener(() => PlayGames.Instance.ShowAchievements());
            }
        }
    }
}