#if UNITY_IOS
using System;
using EasyMobile.iOS.UIKit;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.iOS.GameKit
{
    /// <summary>
    /// The protocol you implement to handle exchanges and match-related events for turn-based games.
    /// </summary>
    internal interface GKTurnBasedEventListener
    {
        /// <summary>
        /// Initiates a match from Game Center with the requested players.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="playersToInvite">Players to invite.</param>
        void PlayerDidRequestMatchWithOtherPlayers(GKPlayer player, GKPlayer[] playersToInvite);

        /// <summary>
        /// Called when the match has ended.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="match">Match.</param>
        void PlayerMatchEnded(GKPlayer player, GKTurnBasedMatch match);

        /// <summary>
        /// Activates the player’s turn.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="match">Match.</param>
        /// <param name="didBecomeActive">If set to <c>true</c> did become active.</param>
        void PlayerReceivedTurnEventForMatch(GKPlayer player, GKTurnBasedMatch match, bool didBecomeActive);

        /// <summary>
        /// Indicates that the current player wants to quit the current match.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="match">Match.</param>
        void PlayerWantsToQuitMatch(GKPlayer player, GKTurnBasedMatch match);
    }
}
#endif
