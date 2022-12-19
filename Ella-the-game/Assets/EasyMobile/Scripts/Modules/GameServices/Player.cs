using UnityEngine;
using System.Collections;
using EasyMobile.Internal;
using EasyMobile.Internal.GameServices;
using UnityEngine.SocialPlatforms;
using System;
using System.Collections.Generic;

#if UNITY_ANDROID && EM_GPGS
using GooglePlayGames;
#endif

#if UNITY_IOS
using EasyMobile.iOS.GameKit;
using EasyMobile.Internal.GameServices.iOS;
#endif

namespace EasyMobile
{
    /// <summary>
    /// Represents a Game Services user. Your game should never create Player object directly.
    /// Instead, these objects are to be created and delivered to your game by Game Services API.
    /// </summary>
    public class Player : IUserProfile, IComparable<Player>
    {
        private string mPlayerName = string.Empty;
        private string mPlayerId = string.Empty;
        private bool mIsFriend = false;
        private UserState mState = UserState.Offline;
#if UNITY_2019_4_OR_NEWER
        private string mGameId = string.Empty;
#endif

        #region IUserProfile implementation

        public string userName
        {
            get
            {
                return mPlayerName;
            }
        }

        public string id
        {
            get
            {
                return mPlayerId;
            }
        }

        // The gameId property was introduced since Unity 2019.4.2f1.
#if UNITY_2019_4_OR_NEWER
        public string gameId
        {
            get
            {
                return mGameId;
            }
        }
#endif

        public bool isFriend
        {
            get
            {
                return mIsFriend;
            }
        }

        public UserState state
        {
            get
            {
                return mState;
            }
        }

        public Texture2D image
        {
            get
            {
#if UNITY_IOS
                if (!mIsLoadingImage && mImage == null)
                {
                    mIsLoadingImage = true;
                    LoadImage();
                }
                return mImage;
#elif UNITY_ANDROID && EM_GPGS
                return mPlayGamesUserProfile.image;
#else
                return null;
#endif
            }
        }

        #endregion

        public int CompareTo(Player other)
        {
            return mPlayerId.CompareTo(other.mPlayerId);
        }

        public bool Equals(Player obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return IsEqual(obj);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (!(obj is Player))
            {
                return false;
            }

            return IsEqual((Player)obj);
        }

        public override int GetHashCode()
        {
            return mPlayerId != null ? mPlayerId.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return string.Format("[Player: '{0}' (id {1})]", mPlayerName, mPlayerId);
        }

        private bool IsEqual(Player other)
        {
            return mPlayerId.Equals(other.mPlayerId);
        }

        protected Player(string playerName, string playerId, bool isFriend, UserState state)
        {
            mPlayerName = playerName;
            mPlayerId = playerId;
            mIsFriend = isFriend;
            mState = state;
        }

#if UNITY_IOS

        /// <summary>
        /// The underlying <see cref="EasyMobile.iOS.GameKit.GKPlayer"/> object used to create this instance.
        /// </summary>
        /// <value>The m GK player.</value>
        internal GKPlayer mGKPlayer { get; private set; }

        /// <summary>
        /// Create a cross-platform player from a <see cref="EasyMobile.iOS.GameKit.GKPlayer"/> object.
        /// </summary>
        /// <param name="gkPlayer">Gk player.</param>
        internal static Player FromGKPlayer(GKPlayer gkPlayer)
        {
            if (gkPlayer == null)
                return null;

            return new Player(
                gkPlayer.DisplayName,
                gkPlayer.PlayerID,
                gkPlayer.IsFriend,
                UserState.Online
            )
            {
                mGKPlayer = gkPlayer
            };
        }

        private Texture2D mImage = null;
        private bool mIsLoadingImage = false;

        /// <summary>
        /// Loads the player's image. Once the operation completes successfully,
        /// the <see cref="image"/> property will be filled with the loaded image.
        /// Null is returned up to that point.
        /// </summary>
        protected virtual void LoadImage()
        {
            mGKPlayer.LoadPhotoForSize(GKPlayer.GKPhotoSize.Small, (photo, loadPhotoError) =>
                {
                    if (loadPhotoError == null)
                    {
                        mImage = photo != null ? photo.ToTexture2D() : null;
                    }
                    else
                    {
                        mImage = Texture2D.blackTexture;
                        Debug.Log("Error downloading player image: " + loadPhotoError.LocalizedDescription);
                    }

                    mIsLoadingImage = false;
                });
        }
#endif

#if UNITY_ANDROID && EM_GPGS
        
        /// <summary>
        /// The underlying GPGS player used to create this instance.
        /// </summary>
        /// <value>The m play games user profile.</value>
        internal PlayGamesUserProfile mPlayGamesUserProfile { get; private set; }

        /// <summary>
        /// Create a cross-platform player from GPGS player.
        /// </summary>
        /// <returns>The GPGS player.</returns>
        /// <param name="userProfile">User profile.</param>
        internal static Player FromGPGSPlayer(PlayGamesUserProfile userProfile)
        {
            if (userProfile == null)
                return null;

            return new Player(
                userProfile.userName,
                userProfile.id,
                userProfile.isFriend,
                userProfile.state
            )
            { 
                mPlayGamesUserProfile = userProfile 
            };
        }

#endif
    }
}
