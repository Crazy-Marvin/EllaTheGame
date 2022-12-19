using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

namespace EasyMobile.Demo
{
    public class GameServicesDemo_SavedGames : MonoBehaviour
    {
        #if EASY_MOBILE_PRO
        [Header("Object References")]
        public DemoUtils demoUtils;
        public GameObject scrollableListPrefab;

        [Header("Saved Games Demo")]
        public GameObject isSavedGamesEnabledInfo;
        public GameObject isSavedGameSelectedInfo;
        public GameObject isSavedGameOpenedInfo;
        public GameObject autoConflictResolutionInfo;
        public InputField savedGameNameInput;
        public InputField savedGameDataInput;
        [Tooltip("Check to enable manual conflict resolution. The implemented policy is pick the saved game with biggest stored value.")]
        public bool resolveConflictsManually = false;

        public class DemoSavedGameData
        {
            public int demoInt;
            // Dummy data to add additional weight to the packet
            public byte[] largeData;

            public DemoSavedGameData(int data)
            {
                demoInt = data;
                largeData = new byte[TEST_DATA_BYTE_COUNT];

                for (int i = 0; i < TEST_DATA_BYTE_COUNT; i++)
                {
                    largeData[i] = 0xAA;
                }
            }
        }

        const int TEST_DATA_BYTE_COUNT = 10 * 1024;
        SavedGame selectedSavedGame;
        SavedGame[] allSavedGames;

        void Start()
        {
            if (EM_Settings.GameServices.IsSavedGamesEnabled)
                demoUtils.DisplayBool(isSavedGamesEnabledInfo, true, "Saved Games Service Enabled: TRUE");
            else
                demoUtils.DisplayBool(isSavedGamesEnabledInfo, false, "Saved Games Service Enabled: FALSE");

            if (!resolveConflictsManually)
                demoUtils.DisplayBool(autoConflictResolutionInfo, true,
                    "Conflict Resolution Strategy: " + EM_Settings.GameServices.AutoConflictResolutionStrategy.ToString());
            else
                demoUtils.DisplayBool(autoConflictResolutionInfo, false, "Conflict Resolution: Manual");
        }

        void Update()
        {
            // Check if any saved game is selected and opened.
            if (selectedSavedGame != null)
                demoUtils.DisplayBool(isSavedGameSelectedInfo, true, "Selected Saved Game: " + selectedSavedGame.Name);
            else
                demoUtils.DisplayBool(isSavedGameSelectedInfo, false, "No Saved Game Selected");

            if (selectedSavedGame != null && selectedSavedGame.IsOpen)
                demoUtils.DisplayBool(isSavedGameOpenedInfo, true, "Saved Game Is Opened: TRUE");
            else
                demoUtils.DisplayBool(isSavedGameOpenedInfo, false, "Saved Game Is Opened: FALSE");
        }

        public void ShowGPGSSelectSavedGameUI()
        {
            GameServices.SavedGames.ShowSelectSavedGameUI(
                "Select Saved Game",
                5,
                true,
                true,
                (SavedGame game, string error) =>
                {
                    selectedSavedGame = game;
                    if (string.IsNullOrEmpty(error))
                        NativeUI.Alert("Saved Game Selected", "You selected saved game: " + game.Name);
                    else
                        NativeUI.Alert("No Saved Game Selected", error);
                });
        }

        public void OpenSavedGame()
        {
            if (string.IsNullOrEmpty(savedGameNameInput.text))
            {
                NativeUI.Alert("Alert", "Please enter a saved game name.");
            }
            else
            {
                string name = savedGameNameInput.text;
                DoOpenSavedGame(name,
                    (SavedGame game, string error) =>
                    {
                        selectedSavedGame = game;

                        if (string.IsNullOrEmpty(error))
                            NativeUI.Alert("Alert", "Saved game: " + game.Name + " is opened.");
                        else
                            NativeUI.Alert("Alert", "Saved game opening failed with error: " + error);
                    });
            }
        }

        public void FetchAllSavedGames()
        {
            GameServices.SavedGames.FetchAllSavedGames((SavedGame[] games, string error) =>
                {
                    if (string.IsNullOrEmpty(error))
                    {
                        allSavedGames = games;

                        var items = new Dictionary<string, string>();

                        foreach (SavedGame game in allSavedGames)
                        {
                            #if UNITY_IOS
                            items.Add(game.Name, game.DeviceName + " " + game.ModificationDate.ToString("d"));
                            #else
                            items.Add(game.Name, game.ModificationDate.ToString("d"));
                            #endif
                        }

                        var scrollableList = ScrollableList.Create(scrollableListPrefab, "ALL SAVED GAMES", items);
                        scrollableList.ItemSelected += OnSavedGameSelectedFromScrollableList;
                    }
                    else
                    {
                        NativeUI.Alert("Alert", "Fetch all saved games failed with error: " + error);
                    }
                });
        }

        public void ReadSavedGame()
        {
            if (selectedSavedGame == null)
            {
                NativeUI.Alert("Alert", "Please select a saved game by using the select UI or entering its name into the input box.");
                return;
            }
            else
            {
                if (selectedSavedGame.IsOpen)
                {
                    GameServices.SavedGames.ReadSavedGameData(selectedSavedGame, OnSavedGameRead);
                }
                else
                {
                    DoOpenSavedGame(selectedSavedGame.Name,
                        (SavedGame game, string error) =>
                        {
                            if (string.IsNullOrEmpty(error))
                            {
                                selectedSavedGame = game;
                                GameServices.SavedGames.ReadSavedGameData(selectedSavedGame, OnSavedGameRead);
                            }
                            else
                            {
                                NativeUI.Alert("Alert", "Couldn't read saved game data because it's failed to open with error: " + error);
                            }
                        });
                }
            }   
        }

        public void WriteSavedGame()
        {
            if (selectedSavedGame == null)
            {
                NativeUI.Alert("Alert", "Please select a saved game by using the select UI or entering its name into the input box.");
                return;
            }
            else if (string.IsNullOrEmpty(savedGameDataInput.text))
            {
                NativeUI.Alert("Alert", "Please enter a value to save.");
            }
            else
            {
                // Get the input data
                int value = System.Convert.ToInt32(savedGameDataInput.text);
                byte[] data = SavedGameDataToByteArray(new DemoSavedGameData(value));

                if (selectedSavedGame.IsOpen)
                {
                    GameServices.SavedGames.WriteSavedGameData(selectedSavedGame, data, OnSavedGameUpdated);
                }
                else
                {
                    DoOpenSavedGame(selectedSavedGame.Name,
                        (SavedGame game, string error) =>
                        {
                            if (string.IsNullOrEmpty(error))
                            {
                                selectedSavedGame = game;
                                GameServices.SavedGames.WriteSavedGameData(selectedSavedGame, data, OnSavedGameUpdated);
                            }
                            else
                            {
                                NativeUI.Alert("Alert", "Couldn't write saved game data because it's failed to open with error: " + error);
                            }
                        });
                }
            } 
        }

        public void DeleteSavedGame()
        {
            if (selectedSavedGame == null)
            {
                NativeUI.Alert("Alert", "Please select a saved game by using the select UI or entering its name into the input box.");
                return;
            }
            else
            {
                NativeUI.AlertPopup alert = NativeUI.ShowTwoButtonAlert(
                                                "Delete Saved Game?",
                                                "Do you want to delete the saved game '" + selectedSavedGame.Name + "'?",
                                                "Yes",
                                                "No");

                if (alert != null)
                {
                    alert.OnComplete += (int button) =>
                    {
                        if (button == 0)
                        {
                            GameServices.SavedGames.DeleteSavedGame(selectedSavedGame);
                            selectedSavedGame = null;
                            NativeUI.Alert("Alert", "Saved game deleted!");
                        }
                    };
                }
            } 
        }

        void OnSavedGameSelectedFromScrollableList(ScrollableList list, string title, string subtitle)
        {
            list.ItemSelected -= OnSavedGameSelectedFromScrollableList;

            foreach (SavedGame game in allSavedGames)
            {
                if (game.Name.Equals(title))
                {
                    selectedSavedGame = game;
                    NativeUI.Alert("Saved Game Selected", "You selected saved game: " + selectedSavedGame.Name);
                    return;
                }
            }
        }

        void DoOpenSavedGame(string name, Action<SavedGame, string> callback)
        {
            if (!resolveConflictsManually)
            {
                // Open with automatic conflict resolution: use the default strategy in EM Settings
                GameServices.SavedGames.OpenWithAutomaticConflictResolution(name, callback);
            }
            else
            {
                // Open with manual conflict resolution: in this demo, we'll pick the one with biggest value
                GameServices.SavedGames.OpenWithManualConflictResolution(name, true,
                    (baseGame, baseData, remoteGame, remoteData) =>
                    {
                        Debug.Log("Manually resolve conflict of saved game " + baseGame.Name);
                        var baseDataObj = ByteArrayToSavedGameData(baseData);
                        var remoteDataObj = ByteArrayToSavedGameData(remoteData);

                        Debug.LogFormat("Base value {0}; Remote value {1}", baseDataObj.demoInt, remoteDataObj.demoInt); 
                        return baseDataObj.demoInt >= remoteDataObj.demoInt ? SavedGameConflictResolutionStrategy.UseBase : SavedGameConflictResolutionStrategy.UseRemote;
                    },
                    callback);
            }
        }

        void OnSavedGameRead(SavedGame game, byte[] data, string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                NativeUI.Alert("Alert", "Saved game reading failed with error: " + error);
            }
            else
            {
                var savedData = ByteArrayToSavedGameData(data);

                if (savedData != null)
                    NativeUI.Alert("Saved Game Data Retrieved", "The data of saved game '" + game.Name + "' contains value: " + savedData.demoInt);
                else
                    NativeUI.Alert("Alert", "Saved game '" + game.Name + "' has no data!");
            }
        }

        DemoSavedGameData ByteArrayToSavedGameData(byte[] data)
        {
            if (data != null)
            {
                // Byte[] data to json string
                string jsonStr = System.Text.Encoding.UTF8.GetString(data);

                // Json string to object
                DemoSavedGameData savedData = JsonUtility.FromJson<DemoSavedGameData>(jsonStr);

                return savedData;
            }

            return null;
        }

        byte[] SavedGameDataToByteArray(DemoSavedGameData dataObj)
        {
            if (dataObj != null)
            {
                // Convert to json string
                string jsonStr = JsonUtility.ToJson(dataObj);

                // Json string to byte[]
                return System.Text.Encoding.UTF8.GetBytes(jsonStr);
            }

            return null;
        }

        void OnSavedGameUpdated(SavedGame game, string error)
        {
            if (!string.IsNullOrEmpty(error))
                NativeUI.Alert("Alert", "Save game data failed with error: " + error);
            else
                NativeUI.Alert("Alert", "The data of saved game '" + game.Name + "' was updated successfully.");
        }

        bool CanUseSavedGames()
        {
            if (!EM_Settings.GameServices.IsSavedGamesEnabled)
            {
                NativeUI.Alert("Error", "You didn't enable Saved Games feature in Easy Mobile settings.");
                return false;
            }

            if (!GameServices.IsInitialized())
            {
                NativeUI.Alert("Alert", "You need to initialize the module first.");
                return false;
            }

            return true;
        }

        void SavedGamesUnavailableAlert()
        {
            NativeUI.Alert("Feature Unavailable", "Saved Games feature is only available on Easy Mobile Pro.");
        }

        #endif
    }
}

