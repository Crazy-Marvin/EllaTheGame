#if UNITY_IOS
using UnityEngine;
using System.Collections;
using EasyMobile.iOS.GameKit;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.Internal.GameServices.iOS
{
    using GKTBMVC = GKTurnBasedMatchmakerViewController;
    using GKTBMVC_Delegate = GKTurnBasedMatchmakerViewController.GKTurnBasedMatchmakerViewControllerDelegate;

    internal abstract class GKTurnBasedMatchmakerViewControllerDelegateImpl : GKTBMVC_Delegate
    {
        public virtual void TurnBasedMatchmakerViewControllerWasCancelled(GKTurnBasedMatchmakerViewController viewController)
        {
        }

        public virtual void TurnBasedMatchmakerViewControllerDidFailWithError(GKTurnBasedMatchmakerViewController viewController, NSError error)
        {
        }
    }
}
#endif