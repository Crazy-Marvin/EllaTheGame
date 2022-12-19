#if UNITY_EDITOR
using UnityEngine;
using System;

namespace EasyMobile.Internal.GameServices
{
    internal class EditorSavedGameClient : ISavedGameClient
    {
        const string UNAVAILABLE_MESSAGE = "Please test the Saved Game feature on an iOS or Android device.";

        public void OpenWithAutomaticConflictResolution(string name, Action<SavedGame, string> callback)
        {
            Debug.LogWarning(UNAVAILABLE_MESSAGE);
        }

        public void OpenWithManualConflictResolution(string name, bool prefetchDataOnConflict, 
                                                     SavedGameConflictResolver resolverFunction, 
                                                     Action<SavedGame, string> completedCallback)
        {
            Debug.LogWarning(UNAVAILABLE_MESSAGE);
        }

        public void ReadSavedGameData(SavedGame savedGame, Action<SavedGame, byte[], string> callback)
        {
            Debug.LogWarning(UNAVAILABLE_MESSAGE);
        }

        public void WriteSavedGameData(SavedGame savedGame, byte[] data, Action<SavedGame, string> callback)
        {
            Debug.LogWarning(UNAVAILABLE_MESSAGE);
        }

        public void WriteSavedGameData(SavedGame savedGame, byte[] data, SavedGameInfoUpdate infoUpdate, Action<SavedGame, string> callback)
        {
            Debug.LogWarning(UNAVAILABLE_MESSAGE);
        }

        public void FetchAllSavedGames(Action<SavedGame[], string> callback)
        {
            Debug.LogWarning(UNAVAILABLE_MESSAGE);
        }

        public void DeleteSavedGame(SavedGame savedGame)
        {
            Debug.LogWarning(UNAVAILABLE_MESSAGE);
        }

        public void ShowSelectSavedGameUI(string uiTitle, uint maxDisplayedSavedGames, bool showCreateSaveUI,
                                          bool showDeleteSaveUI, Action<SavedGame, string> callback)
        {
            Debug.LogWarning(UNAVAILABLE_MESSAGE);
        }
    }
}

#endif