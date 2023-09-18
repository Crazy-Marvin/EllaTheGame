using UnityEngine;
using System;
using EasyMobile.Internal;

namespace EasyMobile.Internal.GameServices
{
    internal class UnsupportedSavedGameClient : ISavedGameClient
    {
        const string DEFAULT_UNAVAILABLE_MESSAGE = "Saved Game feature is not available on this platform.";

        private string mMessage;

        internal UnsupportedSavedGameClient()
        {
            this.mMessage = DEFAULT_UNAVAILABLE_MESSAGE;
        }

        internal UnsupportedSavedGameClient(string msg)
        {
            this.mMessage = Util.NullArgumentTest(msg);
        }

        public void OpenWithAutomaticConflictResolution(string name, Action<SavedGame, string> callback)
        {
            Debug.LogWarning(mMessage);
        }

        public void OpenWithManualConflictResolution(string name, bool prefetchDataOnConflict, 
                                                     SavedGameConflictResolver resolverFunction, 
                                                     Action<SavedGame, string> completedCallback)
        {
            Debug.LogWarning(mMessage);
        }

        public void ReadSavedGameData(SavedGame savedGame, Action<SavedGame, byte[], string> callback)
        {
            Debug.LogWarning(mMessage);
        }

        public void WriteSavedGameData(SavedGame savedGame, byte[] data, Action<SavedGame, string> callback)
        {
            Debug.LogWarning(mMessage);
        }

        public void WriteSavedGameData(SavedGame savedGame, byte[] data, SavedGameInfoUpdate infoUpdate, Action<SavedGame, string> callback)
        {
            Debug.LogWarning(mMessage);
        }

        public void FetchAllSavedGames(Action<SavedGame[], string> callback)
        {
            Debug.LogWarning(mMessage);
        }

        public void DeleteSavedGame(SavedGame savedGame)
        {
            Debug.LogWarning(mMessage);
        }

        public void ShowSelectSavedGameUI(string uiTitle, uint maxDisplayedSavedGames, bool showCreateSaveUI,
                                          bool showDeleteSaveUI, Action<SavedGame, string> callback)
        {
            Debug.LogWarning(mMessage);
        }
    }
}
