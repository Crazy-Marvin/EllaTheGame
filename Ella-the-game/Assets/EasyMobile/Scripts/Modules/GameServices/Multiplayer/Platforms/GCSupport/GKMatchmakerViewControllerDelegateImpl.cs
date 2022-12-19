#if UNITY_IOS
using UnityEngine;
using System.Collections;
using EasyMobile.iOS.GameKit;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.Internal.GameServices.iOS
{
    internal abstract class GKMatchmakerViewControllerDelegateImpl : GKMatchmakerViewController.GKMatchmakerViewControllerDelegate
    {
        public virtual void MatchmakerViewControllerDidFindMatch(GKMatchmakerViewController viewController, GKMatch match)
        {
        }

        public virtual void MatchmakerViewControllerDidFindHostedPlayers(GKMatchmakerViewController viewController, NSArray<GKPlayer> players)
        {
        }

        public virtual void MatchmakerViewControllerWasCancelled(GKMatchmakerViewController viewController)
        {
        }

        public virtual void MatchmakerViewControllerDidFailWithError(GKMatchmakerViewController viewController, NSError error)
        {
        }

        public virtual void MatchmakerViewControllerHostedPlayerDidAccept(GKMatchmakerViewController viewController, GKPlayer player)
        {
        }
    }
}
#endif