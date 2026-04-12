using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Threading.Tasks; // --- NEW : pour async/await avec UGS
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayGames : MonoBehaviour
{
    public static PlayGames Instance { get; private set; }

    [Header("Google Play IDs")]
    [SerializeField] private string leaderboardID = "CgkIway7ibscEAIQDw";

    [Header("Optional UI")]
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button achievementsButton;

    private const string LastPlayTimeKey = "lastPlayTime";

    [Header("Status UI")]
    [SerializeField] private TextMeshProUGUI statusText;

    // ---------- DEBUG SETTINGS ----------
    [Header("Debug Settings")]
    [SerializeField] private bool enableVerboseLogging = true;
    [SerializeField] private float authenticationRetryDelay = 1.5f;
    [SerializeField] private int maxManualRetries = 2;
    [SerializeField] private bool showDebugOverlay = true;
    // ------------------------------------

    private int manualRetryCount = 0;
    private bool hasTriedManualLogin = false;
    private bool isDebugOverlayVisible = false;
    private string lastError = "None";

    // --- NEW : Stockage du token pour UGS ---
    public string GooglePlayGamesToken { get; private set; }

/*    // --- NEW : Référence optionnelle vers un contrôleur UI (peut être laissée null) ---
    [SerializeField] private MonoBehaviour accountUIController; // Remplace par ton type si nécessaire*/

    private void UpdateStatusUI()
    {
        if (statusText == null) return;

        if (IsAuthenticated())
        {
            string playerName = "Unknown";
            try { playerName = PlayGamesPlatform.Instance.GetUserDisplayName(); }
            catch (Exception e) { DebugLogError($"GetUserDisplayName failed: {e.Message}"); }

            statusText.text = $"🟢 Connected\n{playerName}";
        }
        else
        {
            statusText.text = "🔴 Not Connected";
        }
    }

    public void SetStatusText(TextMeshProUGUI newStatusText)
    {
        statusText = newStatusText;
        UpdateStatusUI();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        DebugLog("Awake: PlayGames instance created and set to DontDestroyOnLoad.");
    }

    private void Start()
    {
        DebugLog("Start: Initializing Google Play Games...");
        ValidatePluginConfiguration();

        PlayGamesPlatform.DebugLogEnabled = true;

        try
        {
            PlayGamesPlatform.Activate();
            DebugLog("PlayGamesPlatform.Activate() called successfully.");
        }
        catch (Exception e)
        {
            DebugLogError($"PlayGamesPlatform.Activate() threw exception: {e}");
        }

        FindAndBindButtons();

        if (!IsAuthenticated())
        {
            DebugLog("Start: User not authenticated. Calling Authenticate()...");
            Authenticate();
          
          
        }
        else
        {
            DebugLog("Start: User already authenticated.");
        }

        UpdateStatusUI();
    }


    private void ValidatePluginConfiguration()
    {
        DebugLog("--- Google Play Games Configuration Check ---");
        DebugLog($"PlayGamesPlatform.Instance == null? {PlayGamesPlatform.Instance == null}");
        DebugLog($"PlayGamesPlatform.DebugLogEnabled = {PlayGamesPlatform.DebugLogEnabled}");

#if UNITY_ANDROID
        DebugLog("Platform: Android");
#elif UNITY_IOS
        DebugLog("Platform: iOS");
#else
        DebugLog("Warning: Running on unsupported platform for Google Play Games.");
#endif

        DebugLog("Make sure you have configured 'Google Play Games -> Android Setup' with your OAuth 2.0 client ID.");
        DebugLog("------------------------------------------------");
    }

    private void FindAndBindButtons()
    {
        if (leaderboardButton == null)
        {
            GameObject btnObj = GameObject.FindGameObjectWithTag("LeaderBoardBtn");
            if (btnObj != null)
            {
                leaderboardButton = btnObj.GetComponent<Button>();
                DebugLog($"Leaderboard button found via tag: {leaderboardButton.name}");
            }
            else
            {
                DebugLogWarning("No GameObject with tag 'LeaderBoardBtn' found in scene.");
            }
        }

        if (achievementsButton == null)
        {
            GameObject btnObj = GameObject.FindGameObjectWithTag("AchievementsBtn");
            if (btnObj != null)
            {
                achievementsButton = btnObj.GetComponent<Button>();
                DebugLog($"Achievements button found via tag: {achievementsButton.name}");
            }
            else
            {
                DebugLogWarning("No GameObject with tag 'AchievementsBtn' found in scene.");
            }
        }

        BindButtonListeners();
    }

    private void BindButtonListeners()
    {
        if (leaderboardButton != null)
        {
            leaderboardButton.onClick.RemoveListener(ShowLeaderboard);
            leaderboardButton.onClick.AddListener(ShowLeaderboard);
            DebugLog("Leaderboard button listener bound.");
        }

        if (achievementsButton != null)
        {
            achievementsButton.onClick.RemoveListener(ShowAchievements);
            achievementsButton.onClick.AddListener(ShowAchievements);
            DebugLog("Achievements button listener bound.");
        }
    }

    public void Authenticate()
    {
        DebugLog($"Authenticate() called. IsAuthenticated? {IsAuthenticated()}");
        if (IsAuthenticated())
        {
            DebugLog("Already authenticated. Skipping login flow.");
            UpdateStatusUI();
            return;
        }

        try
        {
            DebugLog("Calling PlayGamesPlatform.Instance.Authenticate...");
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        }
        catch (Exception e)
        {
            DebugLogError($"Exception during Authenticate call: {e}");
            lastError = e.Message;
            UpdateStatusUI();
        }
    }

    public void AuthenticateWithDelay()
    {
        DebugLog("AuthenticateWithDelay() called. Starting coroutine...");
        StartCoroutine(DelayedAuthenticate());
    }

    private IEnumerator DelayedAuthenticate()
    {
        DebugLog($"Waiting 0.2 seconds before authenticating to allow UI to settle...");
        yield return new WaitForSeconds(0.2f);
        Authenticate();
    }

    private void ProcessAuthentication(SignInStatus status)
    {
        DebugLog($"ProcessAuthentication received status: {status}");
        lastError = $"SignInStatus: {status}";

        if (status == SignInStatus.Success)
        {
            DebugLog("✅ Google Play Games: logged in successfully.");
            hasTriedManualLogin = false;
            manualRetryCount = 0;

            bool isAuth = IsAuthenticated();
            DebugLog($"Post‑login IsAuthenticated() = {isAuth}");
            if (isAuth)
            {
                try
                {
                    string id = PlayGamesPlatform.Instance.GetUserId();
                    string name = PlayGamesPlatform.Instance.GetUserDisplayName();
                    DebugLog($"User ID: {id}");
                    DebugLog($"Display Name: {name}");

                    // --- NEW : Demander le code d'autorisation serveur pour UGS ---
                    RequestServerAuthCode();
                }
                catch (Exception e)
                {
                    DebugLogError($"Failed to retrieve user info after login: {e.Message}");
                }
            }

            RepeaterAchievement();
            SantaAchievement();
        }
        else
        {
            DebugLogWarning($"❌ Google Play Games: login failed -> {status}");

            if (!hasTriedManualLogin)
            {
                hasTriedManualLogin = true;
                DebugLog("Attempting manual authentication...");
                try
                {
                    PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
                }
                catch (Exception e)
                {
                    DebugLogError($"ManuallyAuthenticate threw exception: {e}");
                    StartCoroutine(RetryAuthenticationCoroutine());
                }
            }
            else if (manualRetryCount < maxManualRetries)
            {
                manualRetryCount++;
                DebugLog($"Manual login already tried once. Starting coroutine retry #{manualRetryCount} after {authenticationRetryDelay}s...");
                StartCoroutine(RetryAuthenticationCoroutine());
            }
            else
            {
                DebugLogError($"Max manual retries ({maxManualRetries}) reached. Giving up.");
                lastError = $"Max retries reached. Last status: {status}";
            }
        }

        UpdateStatusUI();
    }

    // --- NEW : Méthode pour obtenir le code serveur ---
    private void RequestServerAuthCode()
    {
        DebugLog("Requesting server auth code from Google Play Games...");
        PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
        {
            if (!string.IsNullOrEmpty(code))
            {
                DebugLog("✅ Authorization code received successfully.");
                GooglePlayGamesToken = code;
                // Tu peux appeler automatiquement la liaison UGS ici si souhaité
                // StartSignInOrLink(); // Décommente si tu veux lancer automatiquement
            }
            else
            {
                DebugLogError("❌ Failed to receive authorization code.");
                GooglePlayGamesToken = null;
            }
        });
    }

    // --- NEW : Méthode publique pour lancer la connexion/liaison UGS ---
    public void StartSignInOrLink()
    {
        if (!IsAuthenticated())
        {
            DebugLogWarning("Not yet authenticated with Google Play Games -- attempting login again");
            Authenticate();
            return;
        }

        if (string.IsNullOrEmpty(GooglePlayGamesToken))
        {
            DebugLogWarning("Authorization code is null or empty! Requesting a new one...");
            RequestServerAuthCode();
            return;
        }

        // Ici tu appellerais ta logique UGS (async)
        // Comme tu n'as pas inclus le code UGS complet, je te laisse un exemple commenté
        // SignInOrLinkWithGooglePlayGames();
        DebugLog("Ready to sign in/link with UGS using token.");
        // Exemple : StartCoroutine(CallUGSAuthentication());
    }

    // --- NEW : Gestion des échecs spécifiques (optionnel) ---
    private void HandleSilentAuthenticationFailure(SignInStatus status)
    {
        switch (status)
        {
            case SignInStatus.Canceled:
                DebugLogWarning("Google Play Games login was cancelled");
                break;
            case SignInStatus.InternalError:
                DebugLogError("Google Play Games internal error");
                break;
            default:
                DebugLogError($"Google Play Games login failed: {status}");
                break;
        }
    }

    private IEnumerator RetryAuthenticationCoroutine()
    {
        yield return new WaitForSeconds(authenticationRetryDelay);
        DebugLog($"RetryAuthenticationCoroutine: Retrying authentication (attempt {manualRetryCount})...");
        hasTriedManualLogin = false;
        Authenticate();
    }

    public void ShowLeaderboard()
    {
        DebugLog("ShowLeaderboard() called.");
        if (!IsAuthenticated())
        {
            DebugLogWarning("ShowLeaderboard: Not authenticated. Cannot show leaderboard.");
            lastError = "Not authenticated when trying to show leaderboard.";
            UpdateStatusUI();
            return;
        }

        try
        {
            DebugLog("Calling PlayGamesPlatform.Instance.ShowLeaderboardUI()");
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        }
        catch (Exception e)
        {
            DebugLogError($"ShowLeaderboardUI threw exception: {e}");
            lastError = e.Message;
        }
    }

    public void ShowAchievements()
    {
        DebugLog("ShowAchievements() called.");
        if (!IsAuthenticated())
        {
            DebugLogWarning("ShowAchievements: Not authenticated. Cannot show achievements.");
            lastError = "Not authenticated when trying to show achievements.";
            UpdateStatusUI();
            return;
        }

        try
        {
            DebugLog("Calling PlayGamesPlatform.Instance.ShowAchievementsUI()");
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }
        catch (Exception e)
        {
            DebugLogError($"ShowAchievementsUI threw exception: {e}");
            lastError = e.Message;
        }
    }

    private bool IsAuthenticated()
    {
        if (PlayGamesPlatform.Instance == null)
        {
            DebugLogWarning("IsAuthenticated: PlayGamesPlatform.Instance is null.");
            return false;
        }

        bool auth = false;
        try
        {
            auth = PlayGamesPlatform.Instance.IsAuthenticated();
        }
        catch (Exception e)
        {
            DebugLogError($"IsAuthenticated threw exception: {e}");
        }

        DebugLog($"IsAuthenticated() = {auth}");
        return auth;
    }

    // ---------------- Achievement & Leaderboard methods ----------------
    public void AddScoreToLeaderboard(int score)
    {
        DebugLog($"AddScoreToLeaderboard({score})");
        if (!IsAuthenticated())
        {
            DebugLogWarning("Not authenticated. Score not submitted.");
            return;
        }

        try
        {
            PlayGamesPlatform.Instance.ReportScore(score, leaderboardID, success =>
            {
                DebugLog($"Score submission result: {(success ? "Success" : "Failed")}");
            });
        }
        catch (Exception e)
        {
            DebugLogError($"ReportScore exception: {e}");
        }
    }

    public void ScoreAchievements(int score)
    {
        DebugLog($"ScoreAchievements({score})");
        if (!IsAuthenticated()) return;

        string[] ids = {
            "CgkIway7ibscEAIQAQ", // puppy
            "CgkIway7ibscEAIQAg", // junior
            "CgkIway7ibscEAIQAw", // dog
            "CgkIway7ibscEAIQBA"  // senior
        };

        foreach (var id in ids)
        {
            PlayGamesPlatform.Instance.IncrementAchievement(id, score, success =>
                DebugLog($"IncrementAchievement {id} : {success}"));
        }
    }

    public void RepeaterAchievement()
    {
        if (!IsAuthenticated()) return;

        string lastPlayTimeStr = PlayerPrefs.GetString(LastPlayTimeKey, string.Empty);
        if (DateTime.TryParse(lastPlayTimeStr, out DateTime lastPlayTime))
        {
            double dayDifference = (DateTime.Now.Date - lastPlayTime.Date).TotalDays;
            if (Math.Abs(dayDifference - 1) < 0.01)
            {
                PlayGamesPlatform.Instance.IncrementAchievement("CgkIway7ibscEAIQBQ", 1, success =>
                    DebugLog($"RepeaterAchievement incremented: {success}"));
            }
        }
    }

    public void SantaAchievement()
    {
        if (!IsAuthenticated()) return;
        DateTime date = DateTime.Now;
        if (date.Month == 12 && date.Day >= 24 && date.Day <= 26)
        {
            PlayGamesPlatform.Instance.UnlockAchievement("CgkIway7ibscEAIQBg", success =>
                DebugLog($"SantaAchievement unlocked: {success}"));
        }
    }

    public void KeepHitting() => IncrementAchievement("CgkIway7ibscEAIQDg");
    public void SendLove() => IncrementAchievement("CgkIway7ibscEAIQDA");
    public void SharingIsCaring() => IncrementAchievement("CgkIway7ibscEAIQCw");
    public void HugsAndkisses_Lover()
    {
        IncrementAchievement("CgkIway7ibscEAIQCg");
        IncrementAchievement("CgkIway7ibscEAIQCQ");
    }

    private void IncrementAchievement(string id)
    {
        if (!IsAuthenticated()) return;
        PlayGamesPlatform.Instance.IncrementAchievement(id, 1, success =>
            DebugLog($"IncrementAchievement {id}: {success}"));
    }

    // ------------------------- Scene & Lifecycle -------------------------
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        DebugLog("OnEnable: Subscribed to sceneLoaded.");
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        DebugLog("OnDisable: Unsubscribed from sceneLoaded.");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DebugLog($"OnSceneLoaded: {scene.name}");
        FindAndBindButtons();

        if (statusText == null)
        {
            GameObject txt = GameObject.Find("PGS_StatusText");
            if (txt != null)
                statusText = txt.GetComponent<TextMeshProUGUI>();
        }

        UpdateStatusUI();
    }

    public void BindButtons(Button leaderboard, Button achievements)
    {
        leaderboardButton = leaderboard;
        achievementsButton = achievements;
        BindButtonListeners();
        DebugLog("Buttons manually bound via BindButtons().");
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString(LastPlayTimeKey, DateTime.Now.ToString());
        PlayerPrefs.Save();
        DebugLog("OnApplicationQuit: Saved last play time.");
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PlayerPrefs.SetString(LastPlayTimeKey, DateTime.Now.ToString());
            PlayerPrefs.Save();
            DebugLog("OnApplicationPause(true): Saved last play time.");
        }
    }

    private void OnDestroy()
    {
        if (leaderboardButton != null)
            leaderboardButton.onClick.RemoveListener(ShowLeaderboard);
        if (achievementsButton != null)
            achievementsButton.onClick.RemoveListener(ShowAchievements);
        DebugLog("OnDestroy: Cleaned up button listeners.");
    }

    // ---------- Enhanced Debug Helpers ----------
    private void DebugLog(string msg)
    {
        if (!enableVerboseLogging) return;
        Debug.Log($"[PGS][{Time.time:F2}] {msg}");
    }

    private void DebugLogWarning(string msg)
    {
        Debug.LogWarning($"[PGS][{Time.time:F2}] {msg}");
    }

    private void DebugLogError(string msg)
    {
        Debug.LogError($"[PGS][{Time.time:F2}] {msg}");
    }

    // ---------- OnGUI Debug Overlay ----------
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            isDebugOverlayVisible = !isDebugOverlayVisible;
            DebugLog($"Debug overlay toggled: {isDebugOverlayVisible}");
        }
    }

    private void OnGUI()
    {
        if (!showDebugOverlay || !isDebugOverlayVisible) return;

        GUILayout.BeginArea(new Rect(10, 10, 400, 350));
        GUILayout.Box("Google Play Games Debug Info");
        GUILayout.Label($"IsAuthenticated: {IsAuthenticated()}");
        GUILayout.Label($"PlayGamesPlatform.Instance null? {PlayGamesPlatform.Instance == null}");
        GUILayout.Label($"Last Error: {lastError}");
        GUILayout.Label($"Manual Retry Count: {manualRetryCount}");
        GUILayout.Label($"HasTriedManualLogin: {hasTriedManualLogin}");
        GUILayout.Label($"Status Text Object: {(statusText != null ? statusText.name : "NULL")}");
        GUILayout.Label($"Auth Code: {(string.IsNullOrEmpty(GooglePlayGamesToken) ? "None" : "******")}");

        if (GUILayout.Button("Authenticate Now"))
        {
            Authenticate();
        }

        if (GUILayout.Button("Authenticate with Delay (Coroutine)"))
        {
            AuthenticateWithDelay();
        }

        if (GUILayout.Button("Force Refresh UI"))
        {
            UpdateStatusUI();
        }

        if (GUILayout.Button("Show Leaderboard (if auth)"))
        {
            ShowLeaderboard();
        }

        if (GUILayout.Button("Show Achievements (if auth)"))
        {
            ShowAchievements();
        }

        // --- NEW : Bouton pour lancer la liaison UGS ---
        if (GUILayout.Button("Start UGS SignIn/Link"))
        {
            StartSignInOrLink();
        }

        GUILayout.EndArea();
    }
}