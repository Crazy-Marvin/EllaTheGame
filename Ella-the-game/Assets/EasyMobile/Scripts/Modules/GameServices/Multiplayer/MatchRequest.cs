using UnityEngine;
using System.Collections;

#if UNITY_IOS
using EasyMobile.iOS.GameKit;
using EasyMobile.Internal.GameServices.iOS;
#endif

namespace EasyMobile
{
    /// <summary>
    /// Represents a request (a set of config parameters) for a new 
    /// real-time or turn-based multiplayer match.
    /// </summary>
    public class MatchRequest
    {
        /// <summary>
        /// The minimum variant allowed.
        /// </summary>
        public const uint MinVariant = 0;

        /// <summary>
        /// The maximum variant allowed.
        /// </summary>
        public const uint MaxVariant = 511;

        /// <summary>
        /// The minimum number of players that may join the match, including
        /// the player who is making the match request. 
        /// Must be at least 2 (default).
        /// </summary>
        public uint MinPlayers
        {
            get { return mMinPlayers; }
            set { mMinPlayers = value; }
        }

        /// <summary>
        /// The maximum number of players that may join the match, including
        /// the player who is making the match request. 
        /// Must be equal or greater than <see cref="minPlayers"/> and may be
        /// no more than the maximum number of players allowed for the type of
        /// match being requested, as returned by <see cref="GetMaxPlayersAllowed"/>. 
        /// Default value is 2.
        /// </summary>
        public uint MaxPlayers
        {
            get { return mMaxPlayers; }
            set { mMaxPlayers = value; }
        }

        /// <summary>
        /// The match variant. The meaning of this parameter is defined by the game.
        /// It usually indicates a particular game type or mode (for example "capture the flag", 
        /// "first to 10 points", etc). It allows the player to match only with players whose 
        /// match request shares the same variant number. This value must
        /// be between 0 and 511 (inclusive). Default value is 0.
        /// </summary>
        public uint Variant
        {
            get { return mVariant; }
            set { mVariant = (uint)Mathf.Clamp(value, MinVariant, MaxVariant); }
        }

        /// <summary>
        /// If your game has multiple player roles (such as farmer, archer, and wizard) 
        /// and you want to restrict auto-matched games to one player of each role, 
        /// add an exclusive bitmask to your match request. When auto-matching with this option, 
        /// players will only be considered for a match when the logical AND of their exclusive 
        /// bitmasks is equal to 0. In other words, this value represents the exclusive role the 
        /// player making this request wants to play in the created match. If this value is 0 (default) 
        /// it will be ignored. If you're creating a match with the standard matchmaker UI, this value
        /// will also be ignored.
        /// </summary>
        public uint ExclusiveBitmask
        {
            get { return mExclusiveBitmask; }
            set { mExclusiveBitmask = value; }
        }

        private uint mMinPlayers = 2;
        private uint mMaxPlayers = 2;
        private uint mVariant = 0;
        private uint mExclusiveBitmask = 0;

        /// <summary>
        /// Gets the maximum number of players allowed for the specified match type.
        /// </summary>
        /// <returns>The max players allowed.</returns>
        /// <param name="matchType">Match type.</param>
        public static uint GetMaxPlayersAllowed(MatchType matchType)
        {
            switch (matchType)
            {
                case MatchType.RealTime:
                    #if UNITY_ANDROID
                    // https://developers.google.com/games/services/common/concepts/realtimeMultiplayer
                    return 8; 
                    #elif UNITY_IOS
                    return GKMatchRequest.MaxPlayersAllowedForMatchType(matchType.ToGKMatchType());
                    #else
                    return 0;
                    #endif

                case MatchType.TurnBased:
                    #if UNITY_ANDROID && EM_GPGS
                    // https://developers.google.com/games/services/common/concepts/turnbasedMultiplayer
                    return 8;
                    #elif UNITY_IOS
                    return GKMatchRequest.MaxPlayersAllowedForMatchType(matchType.ToGKMatchType());
                    #else
                    return 0;
                    #endif

                case MatchType.Unknown:
                default:
                    return 0;
            }
        }
    }
}
