#if UNITY_IOS
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.iOS.CoreFoundation;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.GameKit;

namespace EasyMobile.Internal.iOS.GameKit
{
    using GKTBMVC_Delegate = GKTurnBasedMatchmakerViewController.GKTurnBasedMatchmakerViewControllerDelegate;

    internal class GKTurnBasedMatchmakerViewControllerDelegateForwarder : iOSDelegateForwarder<GKTBMVC_Delegate>
    {
        internal GKTurnBasedMatchmakerViewControllerDelegateForwarder(IntPtr selfPtr)
            : base(selfPtr)
        {
        }

        internal static GKTurnBasedMatchmakerViewControllerDelegateForwarder FromPointer(IntPtr pointer)
        {
            return InteropObjectFactory<GKTurnBasedMatchmakerViewControllerDelegateForwarder>.FromPointer(
                pointer,
                ptr => new GKTurnBasedMatchmakerViewControllerDelegateForwarder(ptr));
        }

        internal GKTurnBasedMatchmakerViewControllerDelegateForwarder()
            : this(C.GKTurnBasedMatchmakerViewControllerDelegate_new(
                    InternalWasCancelledCallback,
                    InternalDidFailWithErrorCallback,
                    null /* didFindMatch is obsolete so we'll ignore it */))
        {
            // We're using a pointer returned by a native constructor: call CFRelease to balance native ref count
            CFFunctions.CFRelease(this.ToPointer());
        }

        [MonoPInvokeCallback(typeof(C.InternalTurnBasedMatchmakerViewControllerWasCancelled))]
        private static void InternalWasCancelledCallback(IntPtr delegatePtr, IntPtr viewController)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                var vc = InteropObjectFactory<GKTurnBasedMatchmakerViewController>.FromPointer(
                             viewController, ptr => new GKTurnBasedMatchmakerViewController(ptr));

                forwarder.InvokeOnListener(l => l.TurnBasedMatchmakerViewControllerWasCancelled(vc));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalTurnBasedMatchmakerViewControllerDidFailWithError))]
        private static void InternalDidFailWithErrorCallback(IntPtr delegatePtr, IntPtr viewController, IntPtr error)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                var vc = InteropObjectFactory<GKTurnBasedMatchmakerViewController>.FromPointer(
                             viewController, ptr => new GKTurnBasedMatchmakerViewController(ptr));
                var e = InteropObjectFactory<NSError>.FromPointer(error, ptr => new NSError(ptr));

                forwarder.InvokeOnListener(l => l.TurnBasedMatchmakerViewControllerDidFailWithError(vc, e));
            }
        }

        #region C wrapper

        private static class C
        {
            internal delegate void InternalTurnBasedMatchmakerViewControllerWasCancelled(
            /* InteropGKTurnBasedMatchmakerViewControllerDelegate */ IntPtr delegatePtr,
            /* InteropGKTurnBasedMatchmakerViewController */ IntPtr viewController);

            internal delegate void InternalTurnBasedMatchmakerViewControllerDidFailWithError(
            /* InteropGKTurnBasedMatchmakerViewControllerDelegate */ IntPtr delegatePtr,
            /* InteropGKTurnBasedMatchmakerViewController */ IntPtr viewController,
            /* InteropNSError */ IntPtr error);

            internal delegate void TurnBasedMatchmakerViewControllerDidFindMatch(
            /* InteropGKTurnBasedMatchmakerViewControllerDelegate */ IntPtr delegatePtr,
            /* InteropGKTurnBasedMatchmakerViewController */ IntPtr viewController,
            /* InteropGKTurnBasedMatch */ IntPtr match);

            [DllImport("__Internal")]
            internal static extern /* InteropGKTurnBasedMatchmakerViewControllerDelegate */ IntPtr 
            GKTurnBasedMatchmakerViewControllerDelegate_new(InternalTurnBasedMatchmakerViewControllerWasCancelled wasCancelled,
                                                            InternalTurnBasedMatchmakerViewControllerDidFailWithError didFailWithError,
                                                            TurnBasedMatchmakerViewControllerDidFindMatch didFindMatch);

        }

        #endregion
    }
}
#endif