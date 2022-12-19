#if UNITY_IOS
using UnityEngine;
using System.Collections;
using EasyMobile.iOS.GameKit;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.Internal.GameServices.iOS
{
    internal abstract class GKMatchDelegateImpl : GKMatch.GKMatchDelegate
    {
        public virtual void MatchDidReceiveDataForRecipient(GKMatch match, NSData data, GKPlayer recipient, GKPlayer remotePlayer)
        {
        }

        public virtual void MatchDidReceiveData(GKMatch match, NSData data, GKPlayer remotePlayer)
        {
        }

        public virtual void PlayerDidChangeConnectionState(GKMatch match, GKPlayer player, GKMatch.GKPlayerConnectionState state)
        {
        }

        public virtual void MatchDidFailWithError(GKMatch match, NSError error)
        {
        }

        // Must be implemented.
        public abstract bool ShouldReinviteDisconnectedPlayer(GKMatch match, GKPlayer player);
    }
}
#endif