using UnityEngine;
using System.Collections;
using System;

namespace EasyMobile
{
    using EasyMobile.Internal;
    using EasyMobile.Internal.GameServices;

    #if UNITY_IOS
    using EasyMobile.Internal.GameServices.iOS;
    #endif

    #if UNITY_ANDROID && EM_GPGS
    using GooglePlayGames.BasicApi.SavedGame;
    #endif

    public class SavedGame
    {
        /// <summary>
        /// Returns true if this saved game is opened and can be used for other operations (i.e. read, write). 
        /// Saved games returned by Open operations will be "Open". After a Write or Resolve Conflicts operation, 
        /// the corresponding saved game will be closed, and this value will be false.
        /// </summary>
        /// <value><c>true</c> if this saved game is open; otherwise, <c>false</c>.</value>
        public bool IsOpen
        {
            get
            {
                #if UNITY_ANDROID && EM_GPGS
                return GPGSSavedGameMetadata.IsOpen;
                #elif UNITY_IOS
                return GKSavedGame.IsOpen;
                #else
                return false;
                #endif               
            }
        }

        /// <summary>
        /// Returns the name of this saved game. A saved game name will only consist of
        /// non-URL reserved characters (i.e. a-z, A-Z, 0-9, or the symbols "-", ".", "_", or "~")
        /// and have between 1 and 100 characters in length (inclusive).
        /// </summary>
        /// <value>The name of the saved game.</value>
        public string Name
        {
            get
            {
                #if UNITY_ANDROID && EM_GPGS
                return GPGSSavedGameMetadata.Filename;
                #elif UNITY_IOS
                return GKSavedGame.Name;
                #else
                return null;
                #endif 
            }
        }

        /// <summary>
        /// A timestamp corresponding to the last modification to the underlying saved game. If the
        /// saved game is newly created, this value will correspond to the time the first Open
        /// occurred. Otherwise, this corresponds to time the last successful write or conflict resolution occurred.
        /// </summary>
        /// <value>The last modification timestamp.</value>
        public DateTime ModificationDate
        {
            get
            {
                #if UNITY_ANDROID && EM_GPGS
                return GPGSSavedGameMetadata.LastModifiedTimestamp;
                #elif UNITY_IOS
                return GKSavedGame.ModificationDate;
                #else
                return DateTime.MinValue;
                #endif 
            }
        }

        /// <summary>
        /// [iOS only] The name of the device that committed the saved game data.
        /// If the saved game has just been opened with no data committed, this will be null.
        /// This value will be null on non-iOS platforms.
        /// </summary>
        /// <value>The name of the iOS device.</value>
        public string DeviceName
        {
            get
            {
                #if UNITY_IOS
                return GKSavedGame.DeviceName;
                #else
                return null;
                #endif 
            }
        }

        /// <summary>
        /// [GPGS only] If Google Play Game Services platform is in use, this returns  
        /// a human-readable description of the saved game, which may be null.
        /// This value will always be null if Google Play Games Services platform is not in use.
        /// </summary>
        /// <value>The saved game description.</value>
        public string Description
        {
            get
            {
                #if UNITY_ANDROID && EM_GPGS
                return GPGSSavedGameMetadata.Description;
                #else
                return null;
                #endif 
            }
        }

        /// <summary>
        /// [GPGS only] If Google Play Game Services platform is in use, this returns a URL corresponding to the 
        /// PNG-encoded image corresponding to this saved game, which may be null if the saved game does not have a cover image.
        /// This value will always be null if Google Play Games Services platform is not in use.
        /// </summary>
        /// <value>The cover image URL.</value>
        public string CoverImageURL
        {
            get
            {
                #if UNITY_ANDROID && EM_GPGS
                return GPGSSavedGameMetadata.CoverImageURL;
                #else
                return null;
                #endif 
            }
        }

        /// <summary>
        /// [GPGS only] If Google Play Game Services platform is in use, this returns the total time played by 
        /// the player for this saved game. This value is developer-specified and may be tracked in 
        /// any way that is appropriate to the game. Note that this value is specific to this specific
        /// saved game (unless the developer intentionally sets the same value on all saved games). 
        /// If the value was not set, or Google Play Games Services platform is not in use,
        /// this will be equal to <code>TimeSpan.FromMilliseconds(0)</code>.
        /// </summary>
        /// <value>The total time played.</value>
        public TimeSpan TotalTimePlayed
        {
            get
            {
                #if UNITY_ANDROID && EM_GPGS
                return GPGSSavedGameMetadata.TotalTimePlayed;
                #else
                return TimeSpan.FromMilliseconds(0);
                #endif 
            }
        }

        #if UNITY_ANDROID && EM_GPGS
        internal ISavedGameMetadata GPGSSavedGameMetadata { get; private set; }

        internal SavedGame(ISavedGameMetadata metadata)
        {
            Util.NullArgumentTest(metadata);
            GPGSSavedGameMetadata = metadata;
        }
        #endif

        #if UNITY_IOS
        internal iOSGKSavedGame GKSavedGame { get; private set; }

        internal SavedGame(iOSGKSavedGame gkSavedGame)
        {
            Util.NullArgumentTest(gkSavedGame);
            GKSavedGame = gkSavedGame;
        }
        #endif
    }
}

