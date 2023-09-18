using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

namespace EasyMobile.Demo
{
    public class GameServicesDemo : MonoBehaviour
    {
        [Header("Object References")]
        public GameObject curtain;
        public GameObject isAutoInitInfo;
        public GameObject isInitializedInfo;
        public Text selectedAchievementInfo;
        public Text selectedLeaderboardInfo;
        public InputField scoreInput;
        public DemoUtils demoUtils;
        public GameObject scrollableListPrefab;
        public GameObject requestFriendListConsentUI;

        Achievement selectedAchievement;
        Leaderboard selectedLeaderboard;
        bool lastLoginState;

        void Awake()
        {
            // Init EM runtime if needed (useful in case only this scene is built).
            if (!RuntimeManager.IsInitialized())
                RuntimeManager.Init();
        }

        void Start()
        {
            curtain.SetActive(!EM_Settings.IsGameServicesModuleEnable);

            bool needToAskFriendListConsent = false;
#if UNITY_ANDROID && EM_GPGS
            needToAskFriendListConsent = true;
#endif
            requestFriendListConsentUI.SetActive(needToAskFriendListConsent);
        }

        void Update()
        {
            // Check if autoInit is on.
            if (EM_Settings.GameServices.IsAutoInit)
            {
                demoUtils.DisplayBool(isAutoInitInfo, true, "Auto Initialization: ON");
            }
            else
            {
                demoUtils.DisplayBool(isAutoInitInfo, false, "Auto Initialization: OFF");
            }

            // Check if the module is initalized.
            if (GameServices.IsInitialized())
            {
                demoUtils.DisplayBool(isInitializedInfo, true, "User Logged In: TRUE");  
            }
            else
            {
                demoUtils.DisplayBool(isInitializedInfo, false, "User Logged In: FALSE");
                if (lastLoginState)
                    NativeUI.Alert("User Logged Out", "User has logged out.");
            }
            lastLoginState = GameServices.IsInitialized();
        }

        #region Public

        public void Init()
        {
            if (GameServices.IsInitialized())
            {
                NativeUI.Alert("Alert", "The module is already initialized.");
            }
            else
            {
                GameServices.Init();
            }
        }

        public void ShowLeaderboardUI()
        {
            if (GameServices.IsInitialized())
            {
                GameServices.ShowLeaderboardUI();
            }
            else
            {
                #if UNITY_ANDROID
                GameServices.Init();
                #elif UNITY_IOS
                NativeUI.Alert("Service Unavailable", "The user is not logged in.");
                #else
                Debug.Log("Cannot show leaderboards: platform not supported.");
                #endif
            }
        }

        public void ShowSelectedLeaderboardUI()
        {
            if (GameServices.IsInitialized())
            {
                if (selectedLeaderboard != null)
                {
                    GameServices.ShowLeaderboardUI(selectedLeaderboard.Name);
                }
                else
                {
                    NativeUI.Alert("Alert", "Please select a leaderboard first.");
                }
            }
            else
            {
                #if UNITY_ANDROID
                GameServices.Init();
                #elif UNITY_IOS
                NativeUI.Alert("Service Unavailable", "The user is not logged in.");
                #else
                Debug.Log("Cannot show leaderboards: platform not supported.");
                #endif
            }
        }

        public void ShowAchievementUI()
        {            
            if (GameServices.IsInitialized())
            {
                GameServices.ShowAchievementsUI();
            }
            else
            {
                #if UNITY_ANDROID
                GameServices.Init();
                #elif UNITY_IOS
                NativeUI.Alert("Service Unavailable", "The user is not logged in.");
                #else
                Debug.Log("Cannot show achievements: platform not supported.");
                #endif
            }
        }

        public void SelectAchievement()
        {
            var achievements = EM_Settings.GameServices.Achievements;

            if (achievements == null || achievements.Length == 0)
            {
                NativeUI.Alert("Alert", "You haven't added any achievement. Please go to Window > Easy Mobile > Settings and add some.");
                selectedAchievement = null;
                return;
            }
                                
            var items = new Dictionary<string, string>();

            foreach (Achievement acm in achievements)
            {
                items.Add(acm.Name, acm.Id);
            }

            var scrollableList = ScrollableList.Create(scrollableListPrefab, "ACHIEVEMENTS", items);
            scrollableList.ItemSelected += OnAchievementSelected;
        }

        public void UnlockAchievement()
        {
            if (!GameServices.IsInitialized())
            {
                NativeUI.Alert("Alert", "You need to initialize the module first.");
                return;
            }

            if (selectedAchievement != null)
            {
                GameServices.UnlockAchievement(selectedAchievement.Name);
            }
            else
            {
                NativeUI.Alert("Alert", "Please select an achievement to unlock.");
            }
        }

        public void SelectLeaderboard()
        {
            var leaderboards = EM_Settings.GameServices.Leaderboards;

            if (leaderboards == null || leaderboards.Length == 0)
            {
                NativeUI.Alert("Alert", "You haven't added any leaderboard. Please go to Window > Easy Mobile > Settings and add some.");
                selectedAchievement = null;
                return;
            }

            var items = new Dictionary<string, string>();

            foreach (Leaderboard ldb in leaderboards)
            {
                items.Add(ldb.Name, ldb.Id);
            }

            var scrollableList = ScrollableList.Create(scrollableListPrefab, "LEADERBOARDS", items);
            scrollableList.ItemSelected += OnLeaderboardSelected;
        }

        public void ReportScore()
        {
            if (!GameServices.IsInitialized())
            {
                NativeUI.Alert("Alert", "You need to initialize the module first.");
                return;
            }

            if (selectedLeaderboard == null)
            {
                NativeUI.Alert("Alert", "Please select a leaderboard to report score to.");
            }
            else
            {
                if (string.IsNullOrEmpty(scoreInput.text))
                {
                    NativeUI.Alert("Alert", "Please enter a score to report.");
                }
                else
                {
                    int score = System.Convert.ToInt32(scoreInput.text);
                    GameServices.ReportScore(score, selectedLeaderboard.Name);
                    NativeUI.Alert("Alert", "Reported score " + score + " to leaderboard \"" + selectedLeaderboard.Name + "\".");
                }
            }
        }

        public void LoadLocalUserScore()
        {
            if (!GameServices.IsInitialized())
            {
                NativeUI.Alert("Alert", "You need to initialize the module first.");
                return;
            }

            if (selectedLeaderboard == null)
            {
                NativeUI.Alert("Alert", "Please select a leaderboard to load score from.");
            }
            else
            {
                GameServices.LoadLocalUserScore(selectedLeaderboard.Name, OnLocalUserScoreLoaded);
            }
        }

#if UNITY_ANDROID && EM_GPGS
        public void AskFriendListConsent()
        {
            if (!GameServices.IsInitialized())
            {
                NativeUI.Alert("Alert", "You need to initialize the module first.");
                return;
            }

            GameServices.AskForLoadFriendsResolution((status) =>
            {
                NativeUI.Alert("Friend list permission resolution", status.ToString());
            });
        }
#endif
        public void LoadFriends()
        {
            if (!GameServices.IsInitialized())
            {
                NativeUI.Alert("Alert", "You need to initialize the module first.");
                return;
            }

            GameServices.LoadFriends(OnFriendsLoaded);
        }

        public void SignOut()
        {
            GameServices.SignOut();
        }

        #endregion

        #region Private

        void OnAchievementSelected(ScrollableList list, string title, string subtitle)
        {
            list.ItemSelected -= OnAchievementSelected;
            selectedAchievement = GameServices.GetAchievementByName(title);
            selectedAchievementInfo.text = "Selected achievement: " + title;
        }

        void OnLeaderboardSelected(ScrollableList list, string title, string subtitle)
        {
            list.ItemSelected -= OnLeaderboardSelected;
            selectedLeaderboard = GameServices.GetLeaderboardByName(title);
            selectedLeaderboardInfo.text = "Selected leaderboard: " + title;
        }

        void OnLocalUserScoreLoaded(string leaderboardName, IScore score)
        {
            if (score != null)
            {
                NativeUI.Alert("Local User Score Loaded", "Your score on leaderboard \"" + leaderboardName + "\" is " + score.value);
            }
            else
            {
                NativeUI.Alert("Local User Score Load Failed", "You don't have any score reported to leaderboard \"" + leaderboardName + "\".");
            }
        }

        void OnFriendsLoaded(IUserProfile[] friends)
        {
            if (friends.Length > 0)
            {
                var items = new Dictionary<string, string>();

                foreach (IUserProfile user in friends)
                {
                    items.Add(user.userName, user.id);
                }

                ScrollableList.Create(scrollableListPrefab, "FRIEND LIST", items);
            }
            else
            {
                NativeUI.Alert("Load Friends Result", "Couldn't find any friend.");
            }
        }

        #endregion

        #region Saved Games Demo

        public void OpenSavedGamesDemo()
        {
            demoUtils.GameServiceDemo_SavedGames();
        }

        #endregion // Saved Games

        #region Multiplayer Demo

        public void OpenMultiplayerDemo()
        {
            demoUtils.GameServiceDemo_Multiplayer();
        }

        #endregion // Multiplayer
    }
}

