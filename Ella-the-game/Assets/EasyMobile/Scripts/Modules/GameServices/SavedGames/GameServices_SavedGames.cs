using System;
using EasyMobile.Internal.GameServices;

namespace EasyMobile
{
    public partial class GameServices
    {
        public static ISavedGameClient SavedGames
        {
            get
            {
                if (sSavedGameClient == null)
                {
                    sSavedGameClient = GetSavedGameClient();
                }

                return sSavedGameClient;
            }
        }

        private static ISavedGameClient sSavedGameClient;

        #region Private methods

        private static ISavedGameClient GetSavedGameClient()
        {
            #if UNITY_EDITOR
            return new EditorSavedGameClient();
            #elif UNITY_IOS
            return new iOSSavedGameClient();
            #elif UNITY_ANDROID
            if (EM_Settings.GameServices.IsSavedGamesEnabled)
            {
            return new AndroidSavedGameClient();
            }
            else
            {
            return new UnsupportedSavedGameClient("Please enable Saved Game feature in the Game Services module settings first.");
            }
            #else
            return new UnsupportedSavedGameClient();
            #endif
        }

        #endregion
    }
}

