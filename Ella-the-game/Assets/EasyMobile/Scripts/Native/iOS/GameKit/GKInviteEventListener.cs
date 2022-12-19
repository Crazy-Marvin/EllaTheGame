#if UNITY_IOS
using System;
using EasyMobile.iOS.UIKit;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.iOS.GameKit
{
    /// <summary>
    /// A protocol that handles invite events from Game Center.
    /// </summary>
    internal interface GKInviteEventListener
    {
        /// <summary>
        /// Called when another player accepts a match invite from the local player.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="invite">Invite.</param>
        void PlayerDidAcceptInvite(GKPlayer player, GKInvite invite);

        /// <summary>
        /// Called when the local player starts a match with another player from Game Center.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="recipientPlayers">Recipient players.</param>
        void PlayerDidRequestMatchWithRecipients(GKPlayer player, GKPlayer[] recipientPlayers);
    }
}
#endif
