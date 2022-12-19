#if UNITY_IOS
using UnityEngine;
using System.Collections;
using EasyMobile.iOS.GameKit;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.Internal.GameServices.iOS
{
    internal abstract class GKLocalPlayerListenerImpl : GKLocalPlayerListener
    {
        public virtual void PlayerDidAcceptInvite(GKPlayer player, GKInvite invite)
        {
        }

        public virtual void PlayerDidRequestMatchWithRecipients(GKPlayer player, GKPlayer[] recipientPlayers)
        {
        }

        public virtual void PlayerDidRequestMatchWithOtherPlayers(GKPlayer player, GKPlayer[] playersToInvite)
        {
        }

        public virtual void PlayerMatchEnded(GKPlayer player, GKTurnBasedMatch match)
        {
        }

        public virtual void PlayerReceivedTurnEventForMatch(GKPlayer player, GKTurnBasedMatch match, bool didBecomeActive)
        {
        }

        public virtual void PlayerWantsToQuitMatch(GKPlayer player, GKTurnBasedMatch match)
        {
        }
    }
}
#endif