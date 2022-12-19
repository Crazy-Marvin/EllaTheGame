using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
using EasyMobile.Internal;

#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif

#if UNITY_ANDROID && EM_GPGS
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GPGSSavedGame = GooglePlayGames.BasicApi.SavedGame;
#endif

namespace EasyMobile
{

#if UNITY_ANDROID && EM_GPGS
    /// <summary> Native response status codes for GPGS UI operations.</summary>
    /// </remarks>
    public enum AskFriendResolutionStatus
    {
        /// <summary>The result is valid.</summary>
        Valid = 1,

        /// <summary>An internal error occurred.</summary>
        InternalError = -2,

        /// <summary>The player is not authorized to perform the operation.</summary>
        NotAuthorized = -3,

        /// <summary>The installed version of Google Play services is out of date.</summary>
        VersionUpdateRequired = -4,

        /// <summary>Timed out while awaiting the result.</summary>
        Timeout = -5,

        /// <summary>UI closed by user.</summary>
        UserClosedUI = -6,
        UiBusy = -12,

        /// <summary>An network error occurred.</summary>
        NetworkError = -20,

        /// <sumary>The service haven't been fully initialized.</sumary>
        NotInitialized = -30,
    }
#endif
    [AddComponentMenu("")]
    public partial class GameServices : MonoBehaviour
    {
        public static GameServices Instance { get; private set; }

        /// <summary>
        /// Occurs when user login succeeded.
        /// </summary>
        public static event Action UserLoginSucceeded;

        /// <summary>
        /// Occurs when user login failed.
        /// </summary>
        public static event Action UserLoginFailed;

        /// <summary>
        /// The local or currently logged in user.
        /// Returns null if the user has not logged in.
        /// </summary>
        /// <value>The local user.</value>
        public static ILocalUser LocalUser
        {
            get
            {
                if (IsInitialized())
                {
                    return Social.localUser;
                }
                else
                {
                    return null;
                }
            }
        }

        struct LoadScoreRequest
        {
            public bool useLeaderboardDefault;
            public bool loadLocalUserScore;
            public string leaderboardName;
            public string leaderboardId;
            public int fromRank;
            public int scoreCount;
            public TimeScope timeScope;
            public UserScope userScope;
            public Action<string, IScore[]> callback;
        }

        private static bool isLoadingScore = false;
        private static List<LoadScoreRequest> loadScoreRequests = new List<LoadScoreRequest>();

#if UNITY_ANDROID
        private const string ANDROID_LOGIN_REQUEST_NUMBER_PPKEY = "SGLIB_ANDROID_LOGIN_REQUEST_NUMBER";
#endif
        private const string USER_CALLED_LOG_OUT_IN_PREVIOUS_SESSION = "SGLIB_USER_CALLED_LOG_OUT_IN_PREVIOUS_SESSION";

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
            // Init the module if automatic init is enabled.
            if (!EM_Settings.GameServices.IsAutoInit)
                return;
            if (!EM_Settings.GameServices.IsAutoInitAfterUserLogout && StorageUtil.GetInt(USER_CALLED_LOG_OUT_IN_PREVIOUS_SESSION, 0) == 1)
                return;
            StartCoroutine(CRAutoInit(EM_Settings.GameServices.AutoInitDelay));
        }

        IEnumerator CRAutoInit(float delay)
        {
            yield return new WaitForSeconds(delay);
            ManagedInit();
        }

        /// <summary>
        /// Internally calls the Init() method. If the user hasn't logged in
        /// to the service, a login UI will popup. Otherwise, it will initialize silently.
        /// On iOS, the OS automatically shows the login popup when the app gets focus for the first 3 times.
        /// Subsequent init calls will be ignored.
        /// On Android, if the user dismisses the login popup for a number of times determined
        /// by AndroidMaxLoginRequests, we'll stop showing it (all subsequent init calls will be ignored).
        /// </summary>
        public static void ManagedInit()
        {
#if UNITY_IOS
            if (!IsInitialized())
                Init();
#elif UNITY_ANDROID
            if (!IsInitialized())
            {
                int loginRequestNumber = StorageUtil.GetInt(ANDROID_LOGIN_REQUEST_NUMBER_PPKEY, 0);

                if (loginRequestNumber < EM_Settings.GameServices.AndroidMaxLoginRequests || EM_Settings.GameServices.AndroidMaxLoginRequests <= 0)
                {
                    loginRequestNumber++;
                    StorageUtil.SetInt(ANDROID_LOGIN_REQUEST_NUMBER_PPKEY, loginRequestNumber);
                    StorageUtil.Save();
                    Init();
                }
                else
                {
                    Debug.Log("Failed to initialize Game Services module: AndroidMaxLoginRequests exceeded. Requests attempted: " + loginRequestNumber);
                }
            }
#endif
        }

        /// <summary>
        /// Initializes the service. This is required before any other actions can be done e.g reporting scores.
        /// During the initialization process, a login popup will show up if the user hasn't logged in, otherwise
        /// the process will carry on silently.
        /// Note that on iOS, the login popup will show up automatically when the app gets focus for the first 3 times
        /// while subsequent authentication calls will be ignored.
        /// </summary>
        public static void Init()
        {
            // Authenticate and register a ProcessAuthentication callback
            // This call needs to be made before we can proceed to other calls in the Social API
#if UNITY_IOS
            GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
            Social.localUser.Authenticate(ProcessAuthentication);

#if EASY_MOBILE_PRO && !UNITY_EDITOR
            // Register the default GKLocalPlayerListener for invitation delegate if Multiplayer is enabled.
            // EM Pro only.
            if (EM_Settings.GameServices.IsMultiplayerEnabled)
                RegisterDefaultGKLocalPlayerListener();
#endif

#elif UNITY_ANDROID && EM_GPGS
            PlayGamesClientConfiguration.Builder gpgsConfigBuilder = new PlayGamesClientConfiguration.Builder();
#if EASY_MOBILE_PRO
            // Enable Saved Games.
            if (EM_Settings.GameServices.IsSavedGamesEnabled)
            {
                gpgsConfigBuilder.EnableSavedGames();
            }

#if EM_OBSOLETE_GPGS
            // Register an internal invitation delegate and match delegate if Multiplayer is enabled.
            if (EM_Settings.GameServices.IsMultiplayerEnabled)
            {
                gpgsConfigBuilder.WithInvitationDelegate(OnGPGSInvitationReceived);
                gpgsConfigBuilder.WithMatchDelegate(OnGPGSTBMatchReceived);
            }
#endif
#endif
            // Add the OAuth scopes if any.
            if (EM_Settings.GameServices.GpgsOauthScopes != null && EM_Settings.GameServices.GpgsOauthScopes.Length > 0)
            {
                foreach (string scope in EM_Settings.GameServices.GpgsOauthScopes)
                    gpgsConfigBuilder.AddOauthScope(scope);
            }

            // Request ServerAuthCode if needed.
            if (EM_Settings.GameServices.GpgsShouldRequestServerAuthCode)
                gpgsConfigBuilder.RequestServerAuthCode(EM_Settings.GameServices.GpgsForceRefreshServerAuthCode);

            // Build the config
            PlayGamesClientConfiguration gpgsConfig = gpgsConfigBuilder.Build();

            // Initialize PlayGamesPlatform
            PlayGamesPlatform.InitializeInstance(gpgsConfig);

            // Enable logging if required
            PlayGamesPlatform.DebugLogEnabled = EM_Settings.GameServices.GgpsDebugLogEnabled;

            // Set PlayGamesPlatforms as active
            if (Social.Active != PlayGamesPlatform.Instance)
            {
                PlayGamesPlatform.Activate();
            }

            // Now authenticate
            Social.localUser.Authenticate(ProcessAuthentication);
#elif UNITY_ANDROID && !EM_GPGS
            Debug.LogError("SDK missing. Please import Google Play Games plugin for Unity.");
#else
            Debug.Log("Failed to initialize Game Services module: platform not supported.");
#endif
        }

        /// <summary>
        /// Determines whether this module is initialized (user is authenticated) and ready to use.
        /// </summary>
        /// <returns><c>true</c> if initialized; otherwise, <c>false</c>.</returns>
        public static bool IsInitialized()
        {
            return Social.localUser.authenticated;
        }

        /// <summary>
        /// Shows the leaderboard UI.
        /// </summary>
        public static void ShowLeaderboardUI()
        {
            if (IsInitialized())
                Social.ShowLeaderboardUI();
            else
            {
                Debug.Log("Couldn't show leaderboard UI: user is not logged in.");
            }
        }

        /// <summary>
        /// Shows the leaderboard UI for the given leaderboard.
        /// </summary>
        /// <param name="leaderboardName">Leaderboard name.</param>
        public static void ShowLeaderboardUI(string leaderboardName)
        {
            ShowLeaderboardUI(leaderboardName, TimeScope.AllTime);
        }

        /// <summary>
        /// Shows the leaderboard UI for the given leaderboard in the specified time scope.
        /// </summary>
        /// <param name="leaderboardName">Leaderboard name.</param>
        /// <param name="timeScope">Time scope to display scores in the leaderboard.</param>
        public static void ShowLeaderboardUI(string leaderboardName, TimeScope timeScope)
        {
            if (!IsInitialized())
            {
                Debug.Log("Couldn't show leaderboard UI: user is not logged in.");
                return;
            }

            Leaderboard ldb = GetLeaderboardByName(leaderboardName);

            if (ldb == null)
            {
                Debug.Log("Couldn't show leaderboard UI: unknown leaderboard name.");
                return;
            }

#if UNITY_IOS
            GameCenterPlatform.ShowLeaderboardUI(ldb.Id, timeScope);
#elif UNITY_ANDROID && EM_GPGS
            PlayGamesPlatform.Instance.ShowLeaderboardUI(ldb.Id, ToGpgsLeaderboardTimeSpan(timeScope), null);
#else
            // Fallback
            Social.ShowLeaderboardUI();
#endif
        }

        /// <summary>
        /// Shows the achievements UI.
        /// </summary>
        public static void ShowAchievementsUI()
        {
            if (IsInitialized())
                Social.ShowAchievementsUI();
            else
            {
                Debug.Log("Couldn't show achievements UI: user is not logged in.");
            }
        }

        /// <summary>
        /// Reports the score to the leaderboard with the given name.
        /// </summary>
        /// <param name="score">Score.</param>
        /// <param name="leaderboardName">Leaderboard name.</param>
        /// <param name="callback">Callback receives a <c>true</c> value if the score is reported successfully, otherwise it receives <c>false</c>.</param>
        public static void ReportScore(long score, string leaderboardName, Action<bool> callback = null)
        {
            Leaderboard ldb = GetLeaderboardByName(leaderboardName);

            if (ldb != null)
            {
                DoReportScore(score, ldb.Id, callback);
            }
            else
            {
                Debug.Log("Failed to report score: unknown leaderboard name.");
            }
        }

        /// <summary>
        /// Reveals the hidden achievement with the specified name.
        /// </summary>
        /// <param name="achievementName">Achievement name.</param>
        /// <param name="callback">Callback receives a <c>true</c> value if the achievement is revealed successfully, otherwise it receives <c>false</c>.</param>
        public static void RevealAchievement(string achievementName, Action<bool> callback = null)
        {
            Achievement acm = GetAchievementByName(achievementName);

            if (acm != null)
            {
                DoReportAchievementProgress(acm.Id, 0.0f, callback);
            }
            else
            {
                Debug.Log("Failed to reveal achievement: unknown achievement name.");
            }
        }

        /// <summary>
        /// Unlocks the achievement with the specified name.
        /// </summary>
        /// <param name="achievementName">Achievement name.</param>
        /// <param name="callback">Callback receives a <c>true</c> value if the achievement is unlocked successfully, otherwise it receives <c>false</c>.</param>
        public static void UnlockAchievement(string achievementName, Action<bool> callback = null)
        {
            Achievement acm = GetAchievementByName(achievementName);

            if (acm != null)
            {
                DoReportAchievementProgress(acm.Id, 100.0f, callback);
            }
            else
            {
                Debug.Log("Failed to unlocked achievement: unknown achievement name.");
            }
        }

        /// <summary>
        /// Reports the progress of the incremental achievement with the specified name.
        /// </summary>
        /// <param name="achievementName">Achievement name.</param>
        /// <param name="progress">Progress.</param>
        /// <param name="callback">Callback receives a <c>true</c> value if the achievement progress is reported successfully, otherwise it receives <c>false</c>.</param>
        public static void ReportAchievementProgress(string achievementName, double progress, Action<bool> callback = null)
        {
            Achievement acm = GetAchievementByName(achievementName);

            if (acm != null)
            {
                DoReportAchievementProgress(acm.Id, progress, callback);
            }
            else
            {
                Debug.Log("Failed to report incremental achievement progress: unknown achievement name.");
            }
        }

        /// <summary>
        /// Loads all friends of the authenticated user.
        /// Internally it will populate the LocalUsers.friends array and invoke the 
        /// callback with this array if the loading succeeded. 
        /// If the loading failed, the callback will be invoked with an empty array.
        /// If the LocalUsers.friends array is already populated then the callback will be invoked immediately
        /// without any loading request being made. 
        /// </summary>
        /// <param name="callback">Callback.</param>
        public static void LoadFriends(Action<IUserProfile[]> callback)
        {
            if (!IsInitialized())
            {
                Debug.Log("Failed to load friends: user is not logged in.");
                if (callback != null)
                    callback(new IUserProfile[0]);
                return;
            }

            if (Social.localUser.friends != null && Social.localUser.friends.Length > 0)
            {
                if (callback != null)
                    callback(Social.localUser.friends);
            }
            else
            {
                Social.localUser.LoadFriends(success =>
                    {
                        if (success)
                        {
                            if (callback != null)
                                callback(Social.localUser.friends);
                        }
                        else
                        {
                            if (callback != null)
                                callback(new IUserProfile[0]);
                        }
                    });
            }
        }

        /// <summary>
        /// Loads the user profiles associated with the given array of user IDs.
        /// </summary>
        /// <param name="userIds">User identifiers.</param>
        /// <param name="callback">Callback.</param>
        public static void LoadUsers(string[] userIds, Action<IUserProfile[]> callback)
        {
            if (!IsInitialized())
            {
                Debug.Log("Failed to load users: user is not logged in.");
                if (callback != null)
                    callback(new IUserProfile[0]);
                return;
            }

            Social.LoadUsers(userIds, callback);
        }

        /// <summary>
        /// Loads a set of scores using the default parameters of the given leaderboard.
        /// This returns the 25 scores that are around the local player's score
        /// in the Global userScope and AllTime timeScope.
        /// Note that each load score request is added into a queue and the
        /// next request is called after the callback of previous request has been invoked.
        /// </summary>
        /// <param name="leaderboardName">Leaderboard name.</param>
        /// <param name="callback">Callback receives the leaderboard name and an array of loaded scores.</param>
        public static void LoadScores(string leaderboardName, Action<string, IScore[]> callback)
        {
            if (!IsInitialized())
            {
                Debug.LogFormat("Failed to load scores from leaderboard {0}: user is not logged in.", leaderboardName);
                if (callback != null)
                    callback(leaderboardName, new IScore[0]);
                return;
            }

            Leaderboard ldb = GetLeaderboardByName(leaderboardName);

            if (ldb == null)
            {
                Debug.LogFormat("Failed to load scores: unknown leaderboard name {0}", leaderboardName);
                if (callback != null)
                    callback(leaderboardName, new IScore[0]);
                return;
            }

            // Create new request
            LoadScoreRequest request = new LoadScoreRequest();
            request.leaderboardName = ldb.Name;
            request.leaderboardId = ldb.Id;
            request.callback = callback;
            request.useLeaderboardDefault = true;
            request.loadLocalUserScore = false;

            // Add request to the queue
            loadScoreRequests.Add(request);

            DoNextLoadScoreRequest();
        }

        /// <summary>
        /// Loads the set of scores from the specified leaderboard within the specified timeScope and userScope.
        /// The range is defined by starting position fromRank and the number of scores to retrieve scoreCount.
        /// Note that each load score request is added into a queue and the
        /// next request is called after the callback of previous request has been invoked.
        /// </summary>
        /// <param name="leaderboardName">Leaderboard name.</param>
        /// <param name="fromRank">The rank of the first score to load.</param>
        /// <param name="scoreCount">The total number of scores to load.</param>
        /// <param name="timeScope">Time scope.</param>
        /// <param name="userScope">User scope.</param>
        /// <param name="callback">Callback receives the leaderboard name and an array of loaded scores.</param>
        public static void LoadScores(string leaderboardName, int fromRank, int scoreCount, TimeScope timeScope, UserScope userScope, Action<string, IScore[]> callback)
        {
            // IMPORTANT: On Android, the fromRank argument is ignored and the score range always starts at 1.
            // (This is not the intended behavior according to the SocialPlatform.Range documentation, and may simply be
            // a bug of the current (0.9.34) GooglePlayPlatform implementation).
            if (!IsInitialized())
            {
                Debug.LogFormat("Failed to load scores from leaderboard {0}: user is not logged in.", leaderboardName);
                if (callback != null)
                    callback(leaderboardName, new IScore[0]);
                return;
            }

            Leaderboard ldb = GetLeaderboardByName(leaderboardName);

            if (ldb == null)
            {
                Debug.LogFormat("Failed to load scores: unknown leaderboard name {0}.", leaderboardName);
                if (callback != null)
                    callback(leaderboardName, new IScore[0]);
                return;
            }

            // Create new request
            LoadScoreRequest request = new LoadScoreRequest();
            request.leaderboardName = ldb.Name;
            request.leaderboardId = ldb.Id;
            request.callback = callback;
            request.useLeaderboardDefault = false;
            request.loadLocalUserScore = false;
            request.fromRank = fromRank;
            request.scoreCount = scoreCount;
            request.timeScope = timeScope;
            request.userScope = userScope;

            // Add request to the queue
            loadScoreRequests.Add(request);

            DoNextLoadScoreRequest();
        }

        /// <summary>
        /// Loads the local user's score from the specified leaderboard.
        /// Note that each load score request is added into a queue and the
        /// next request is called after the callback of previous request has been invoked.
        /// </summary>
        /// <param name="leaderboardName">Leaderboard name.</param>
        /// <param name="callback">Callback receives the leaderboard name and the loaded score.</param>
        public static void LoadLocalUserScore(string leaderboardName, Action<string, IScore> callback)
        {
            if (!IsInitialized())
            {
                Debug.LogFormat("Failed to load local user's score from leaderboard {0}: user is not logged in.", leaderboardName);
                if (callback != null)
                    callback(leaderboardName, null);
                return;
            }

            Leaderboard ldb = GetLeaderboardByName(leaderboardName);

            if (ldb == null)
            {
                Debug.LogFormat("Failed to load local user's score: unknown leaderboard name {0}.", leaderboardName);
                if (callback != null)
                    callback(leaderboardName, null);
                return;
            }

            // Create new request
            LoadScoreRequest request = new LoadScoreRequest();
            request.leaderboardName = ldb.Name;
            request.leaderboardId = ldb.Id;
            request.callback = delegate (string ldbName, IScore[] scores)
            {
                if (scores != null)
                {
                    if (callback != null)
                        callback(ldbName, scores[0]);
                }
                else
                {
                    if (callback != null)
                        callback(ldbName, null);
                }
            };
            request.useLeaderboardDefault = false;
            request.loadLocalUserScore = true;
            request.fromRank = -1;
            request.scoreCount = -1;
            request.timeScope = TimeScope.AllTime;
            request.userScope = UserScope.Global;

            // Add request to the queue
            loadScoreRequests.Add(request);

            DoNextLoadScoreRequest();
        }

        /// <summary>
        /// Returns a leaderboard it one with a leaderboardName was declared before within leaderboards array.
        /// </summary>
        /// <returns>The leaderboard by name.</returns>
        /// <param name="leaderboardName">Leaderboard name.</param>
        public static Leaderboard GetLeaderboardByName(string leaderboardName)
        {
            foreach (Leaderboard ldb in EM_Settings.GameServices.Leaderboards)
            {
                if (ldb.Name.Equals(leaderboardName))
                    return ldb;
            }

            return null;
        }

        /// <summary>
        /// Returns an achievement it one with an achievementName was declared before within achievements array.
        /// </summary>
        /// <returns>The achievement by name.</returns>
        /// <param name="achievementName">Achievement name.</param>
        public static Achievement GetAchievementByName(string achievementName)
        {
            foreach (Achievement acm in EM_Settings.GameServices.Achievements)
            {
                if (acm.Name.Equals(achievementName))
                    return acm;
            }

            return null;
        }

        /// <summary>
        /// [Google Play Games] Gets the server auth code.
        /// </summary>
        /// <returns></returns>
        public static string GetServerAuthCode()
        {
            if (!IsInitialized())
            {
                return string.Empty;
            }

#if UNITY_ANDROID && EM_GPGS
            return PlayGamesPlatform.Instance.GetServerAuthCode();
#elif UNITY_ANDROID && !EM_GPGS
            Debug.LogError("SDK missing. Please import Google Play Games plugin for Unity.");
            return string.Empty;
#else
            Debug.Log("GetServerAuthCode is only available on Google Play Games platform.");
            return string.Empty;
#endif
        }

        /// <summary>
        /// [Google Play Games] Gets another server auth code.
        /// </summary>
        /// <param name="reAuthenticateIfNeeded"></param>
        /// <param name="callback"></param>
        public static void GetAnotherServerAuthCode(bool reAuthenticateIfNeeded, Action<string> callback)
        {
            if (!IsInitialized())
            {
                return;
            }

#if UNITY_ANDROID && EM_GPGS
            PlayGamesPlatform.Instance.GetAnotherServerAuthCode(reAuthenticateIfNeeded, callback);
#elif UNITY_ANDROID && !EM_GPGS
            Debug.LogError("SDK missing. Please import Google Play Games plugin for Unity.");
#else
            Debug.Log("GetAnotherServerAuthCode is only available on Google Play Games platform.");
#endif
        }

        /// <summary>
        /// [Google Play Games] Signs the user out.
        /// </summary>
        public static void SignOut()
        {
            if (!IsInitialized())
            {
                return;
            }

#if UNITY_ANDROID && EM_GPGS
            PlayGamesPlatform.Instance.SignOut();
#elif UNITY_ANDROID && !EM_GPGS
            Debug.LogError("SDK missing. Please import Google Play Games plugin for Unity.");
#else
            Debug.Log("Signing out from script is not available on this platform.");
#endif
            StorageUtil.SetInt(USER_CALLED_LOG_OUT_IN_PREVIOUS_SESSION, 1);
        }

#if UNITY_ANDROID && EM_GPGS
        public static void AskForLoadFriendsResolution(Action<AskFriendResolutionStatus> callback)
        {
            if (!IsInitialized())
            {
                callback(AskFriendResolutionStatus.NotInitialized);
            }

            if (!PlayGamesPlatform.Instance.IsAuthenticated())
            {
                callback(AskFriendResolutionStatus.NotInitialized);
            }

            PlayGamesPlatform.Instance.AskForLoadFriendsResolution(status =>
            {
                switch (status)
                {
                    case UIStatus.Valid:
                        callback(AskFriendResolutionStatus.Valid);
                        break;
                    case UIStatus.InternalError:
                        callback(AskFriendResolutionStatus.InternalError);
                        break;
                    case UIStatus.NotAuthorized:
                        callback(AskFriendResolutionStatus.NotAuthorized);
                        break;
                    case UIStatus.VersionUpdateRequired:
                        callback(AskFriendResolutionStatus.VersionUpdateRequired);
                        break;
                    case UIStatus.Timeout:
                        callback(AskFriendResolutionStatus.Timeout);
                        break;
                    case UIStatus.UserClosedUI:
                        callback(AskFriendResolutionStatus.UserClosedUI);
                        break;
                    case UIStatus.UiBusy:
                        callback(AskFriendResolutionStatus.UiBusy);
                        break;
                    case UIStatus.NetworkError:
                        callback(AskFriendResolutionStatus.NetworkError);
                        break;
                };
            });
        }
#endif

        #region Private methods

        static void DoReportScore(long score, string leaderboardId, Action<bool> callback)
        {
            if (!IsInitialized())
            {
                Debug.LogFormat("Failed to report score to leaderboard {0}: user is not logged in.", leaderboardId);
                if (callback != null)
                    callback(false);
                return;
            }

            Social.ReportScore(
                score,
                leaderboardId,
                (bool success) =>
                {
                    if (callback != null)
                        callback(success);
                }
            );
        }

        // Progress of 0.0% means reveal the achievement.
        // Progress of 100.0% means unlock the achievement.
        static void DoReportAchievementProgress(string achievementId, double progress, Action<bool> callback)
        {
            if (!IsInitialized())
            {
                Debug.LogFormat("Failed to report progress for achievement {0}: user is not logged in.", achievementId);
                if (callback != null)
                    callback(false);
                return;
            }

            Social.ReportProgress(
                achievementId,
                progress,
                (bool success) =>
                {
                    if (callback != null)
                        callback(success);
                }
            );
        }

        static void DoNextLoadScoreRequest()
        {
            LoadScoreRequest request;

            if (isLoadingScore)
                return;

            if (loadScoreRequests.Count == 0)
                return;

            isLoadingScore = true;
            request = loadScoreRequests[0]; // fetch the next request
            loadScoreRequests.RemoveAt(0);  // then remove it from the queue

            // Now create a new leaderboard and start loading scores
            ILeaderboard ldb = Social.CreateLeaderboard();
            ldb.id = request.leaderboardId;

            if (request.useLeaderboardDefault)
            {
                // The current iOS implementation of ISocialPlatform behaves weirdly with Social.LoadScores.
                // Experiment showed that only the first score on the leaderboard was returned.
                // On Android scores were returned properly.
                // We'll have different code for the two platforms in an attempt to provide consistent behavior from the outside.
#if UNITY_ANDROID
                // On Android, we'll use LoadScores directly from Social.
                Social.LoadScores(ldb.id, (IScore[] scores) =>
                    {
                        if (request.callback != null)
                            request.callback(request.leaderboardName, scores);

                        // Load next request
                        isLoadingScore = false;
                        DoNextLoadScoreRequest();
                    });
#elif UNITY_IOS
                // On iOS, we use LoadScores from ILeaderboard with default parameters.
                ldb.LoadScores((bool success) =>
                    {
                        if (request.callback != null)
                            request.callback(request.leaderboardName, ldb.scores);

                        // Load next request
                        isLoadingScore = false;
                        DoNextLoadScoreRequest();
                    });

#endif
            }
            else
            {
                ldb.timeScope = request.timeScope;
                ldb.userScope = request.userScope;

                if (request.fromRank > 0 && request.scoreCount > 0)
                {
                    ldb.range = new UnityEngine.SocialPlatforms.Range(request.fromRank, request.scoreCount);
                }

                ldb.LoadScores((bool success) =>
                    {
                        if (request.loadLocalUserScore)
                        {
                            IScore[] returnScores = new IScore[] { ldb.localUserScore };

                            if (request.callback != null)
                                request.callback(request.leaderboardName, returnScores);
                        }
                        else
                        {
                            if (request.callback != null)
                                request.callback(request.leaderboardName, ldb.scores);
                        }

                        // Load next request
                        isLoadingScore = false;
                        DoNextLoadScoreRequest();
                    });
            }
        }

        #endregion

        #region Authentication listeners

        // This function gets called when Authenticate completes
        // Note that if the operation is successful, Social.localUser will contain data from the server.
        static void ProcessAuthentication(bool success)
        {
            if (success)
            {
                if (UserLoginSucceeded != null)
                    UserLoginSucceeded();

#if UNITY_ANDROID
                // Reset login request number
                StorageUtil.SetInt(ANDROID_LOGIN_REQUEST_NUMBER_PPKEY, 0);
                StorageUtil.Save();
#endif

                // Set GPGS popup gravity, this needs to be done after authentication.
#if UNITY_ANDROID && EM_GPGS
                PlayGamesPlatform.Instance.SetGravityForPopups(ToGpgsGravity(EM_Settings.GameServices.GpgsPopupGravity));
#endif
                StorageUtil.SetInt(USER_CALLED_LOG_OUT_IN_PREVIOUS_SESSION, 0);
                StorageUtil.Save();
            }
            else
            {
                if (UserLoginFailed != null)
                    UserLoginFailed();
            }
        }

        static void ProcessLoadedAchievements(IAchievement[] achievements)
        {
            if (achievements.Length == 0)
            {
                Debug.Log("No achievements found.");
            }
            else
            {
                Debug.Log("Got " + achievements.Length + " achievements.");
            }
        }

        #endregion

        #region Helpers

#if UNITY_ANDROID && EM_GPGS
        static Gravity ToGpgsGravity(GameServicesSettings.GpgsGravity gravity)
        {
            switch (gravity)
            {
                case GameServicesSettings.GpgsGravity.Top:
                    return Gravity.TOP;
                case GameServicesSettings.GpgsGravity.Bottom:
                    return Gravity.BOTTOM;
                case GameServicesSettings.GpgsGravity.Left:
                    return Gravity.LEFT;
                case GameServicesSettings.GpgsGravity.Right:
                    return Gravity.RIGHT;
                case GameServicesSettings.GpgsGravity.CenterHorizontal:
                    return Gravity.CENTER_HORIZONTAL;
                default:
                    return Gravity.TOP;
            }
        }

        static LeaderboardTimeSpan ToGpgsLeaderboardTimeSpan(TimeScope timeScope)
        {
            switch (timeScope)
            {
                case TimeScope.AllTime:
                    return LeaderboardTimeSpan.AllTime;
                case TimeScope.Week:
                    return LeaderboardTimeSpan.Weekly;
                case TimeScope.Today:
                    return LeaderboardTimeSpan.Daily;
                default:
                    return LeaderboardTimeSpan.AllTime;
            }
        }
#endif

        #endregion

    }
}
