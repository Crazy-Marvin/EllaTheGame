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
    using Delegate = GKMatchmakerViewController.GKMatchmakerViewControllerDelegate;

    internal class GKMatchmakerViewControllerDelegateForwarder : iOSDelegateForwarder<Delegate>
    {
        internal GKMatchmakerViewControllerDelegateForwarder(IntPtr selfPtr)
            : base(selfPtr)
        {
        }

        internal static GKMatchmakerViewControllerDelegateForwarder FromPointer(IntPtr pointer)
        {
            return InteropObjectFactory<GKMatchmakerViewControllerDelegateForwarder>.FromPointer(
                pointer, 
                ptr => new GKMatchmakerViewControllerDelegateForwarder(ptr));
        }

        internal GKMatchmakerViewControllerDelegateForwarder()
            : this(C.GKMatchmakerViewControllerDelegate_new(
                    InternalMatchmakerViewControllerDidFindMatchCallback,
                    InternalMatchmakerViewControllerDidFindHostedPlayersCallback,
                    InternalMatchmakerViewControllerWasCancelledCallback,
                    InternalMatchmakerViewControllerDidFailWithErrorCallback,
                    InternalMatchmakerViewControllerHostedPlayerDidAcceptCallback))
        {
            // We're using a pointer returned by a native constructor: call CFRelease to balance native ref count
            CFFunctions.CFRelease(this.ToPointer());
        }

        [MonoPInvokeCallback(typeof(C.InternalMatchmakerViewControllerDidFindMatch))]
        private static void InternalMatchmakerViewControllerDidFindMatchCallback(
            /* InteropGKMatchmakerViewControllerDelegate */ IntPtr delegatePtr,
            /* InteropGKMatchmakerViewController */IntPtr viewControllerPtr,
            /* InteropGKMatch */IntPtr matchPtr)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                var vc = InteropObjectFactory<GKMatchmakerViewController>.FromPointer(
                             viewControllerPtr, ptr => new GKMatchmakerViewController(ptr));
                var match = InteropObjectFactory<GKMatch>.FromPointer(matchPtr, p => new GKMatch(p));

                forwarder.InvokeOnListener(l => l.MatchmakerViewControllerDidFindMatch(vc, match));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalMatchmakerViewControllerDidFindHostedPlayers))]
        private static void InternalMatchmakerViewControllerDidFindHostedPlayersCallback(
            /* InteropGKMatchmakerViewControllerDelegate */ IntPtr delegatePtr,
            /* InteropGKMatchmakerViewController */IntPtr viewControllerPtr,
            /* InteropNSArray<InteropGKPlayer> */IntPtr playerArrayPtr)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                var vc = InteropObjectFactory<GKMatchmakerViewController>.FromPointer(
                             viewControllerPtr, ptr => new GKMatchmakerViewController(ptr));
                var array = InteropObjectFactory<NSArray<GKPlayer>>.FromPointer(playerArrayPtr, p => new NSArray<GKPlayer>(p));

                forwarder.InvokeOnListener(l => l.MatchmakerViewControllerDidFindHostedPlayers(vc, array));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalMatchmakerViewControllerWasCancelled))]
        private static void InternalMatchmakerViewControllerWasCancelledCallback(
            /* InteropGKMatchmakerViewControllerDelegate */ IntPtr delegatePtr,
            /* InteropGKMatchmakerViewController */IntPtr viewControllerPtr)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                var vc = InteropObjectFactory<GKMatchmakerViewController>.FromPointer(
                             viewControllerPtr, ptr => new GKMatchmakerViewController(ptr));

                forwarder.InvokeOnListener(l => l.MatchmakerViewControllerWasCancelled(vc));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalMatchmakerViewControllerDidFailWithError))]
        private static void InternalMatchmakerViewControllerDidFailWithErrorCallback(
            /* InteropGKMatchmakerViewControllerDelegate */ IntPtr delegatePtr,
            /* InteropGKMatchmakerViewController */IntPtr viewControllerPtr,
            /* InteropNSError */IntPtr errorPtr)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                var vc = InteropObjectFactory<GKMatchmakerViewController>.FromPointer(
                             viewControllerPtr, ptr => new GKMatchmakerViewController(ptr));
                var error = InteropObjectFactory<NSError>.FromPointer(errorPtr, p => new NSError(p));

                forwarder.InvokeOnListener(l => l.MatchmakerViewControllerDidFailWithError(vc, error));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalMatchmakerViewControllerHostedPlayerDidAccept))]
        private static void InternalMatchmakerViewControllerHostedPlayerDidAcceptCallback(
            /* InteropGKMatchmakerViewControllerDelegate */ IntPtr delegatePtr,
            /* InteropGKMatchmakerViewController */IntPtr viewControllerPtr,
            /* InteropGKPlayer */IntPtr playerPtr)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                var vc = InteropObjectFactory<GKMatchmakerViewController>.FromPointer(
                             viewControllerPtr, ptr => new GKMatchmakerViewController(ptr));
                var player = InteropObjectFactory<GKPlayer>.FromPointer(playerPtr, p => new GKPlayer(p));

                forwarder.InvokeOnListener(l => l.MatchmakerViewControllerHostedPlayerDidAccept(vc, player));
            }
        }

        #region C wrapper

        private static class C
        {
            internal delegate void InternalMatchmakerViewControllerDidFindMatch(
            /* InteropGKMatchmakerViewControllerDelegate */ IntPtr delegatePtr,
            /* InteropGKMatchmakerViewController */ IntPtr viewControllerPtr,
            /* InteropGKMatch */ IntPtr matchPtr);

            internal delegate void InternalMatchmakerViewControllerDidFindHostedPlayers(
            /* InteropGKMatchmakerViewControllerDelegate */ IntPtr delegatePtr,
            /* InteropGKMatchmakerViewController */ IntPtr viewControllerPtr,
            /* InteropNSArray<InteropGKPlayer> */ IntPtr playerArrayPtr);

            internal delegate void InternalMatchmakerViewControllerWasCancelled(
            /* InteropGKMatchmakerViewControllerDelegate */ IntPtr delegatePtr,
            /* InteropGKMatchmakerViewController */ IntPtr viewControllerPtr);

            internal delegate void InternalMatchmakerViewControllerDidFailWithError(
            /* InteropGKMatchmakerViewControllerDelegate */ IntPtr delegatePtr,
            /* InteropGKMatchmakerViewController */ IntPtr viewControllerPtr,
            /* InteropNSError */ IntPtr errorPtr);

            internal delegate void InternalMatchmakerViewControllerHostedPlayerDidAccept(
            /* InteropGKMatchmakerViewControllerDelegate */ IntPtr delegatePtr,
            /* InteropGKMatchmakerViewController */ IntPtr viewControllerPtr,
            /* InteropGKPlayer */ IntPtr playerPtr);

            [DllImport("__Internal")]
            internal static extern /* InteropGKMatchmakerViewControllerDelegate */ IntPtr GKMatchmakerViewControllerDelegate_new(
                InternalMatchmakerViewControllerDidFindMatch didFindMatch,
                InternalMatchmakerViewControllerDidFindHostedPlayers didFindHostedPlayers,
                InternalMatchmakerViewControllerWasCancelled wasCancelled,
                InternalMatchmakerViewControllerDidFailWithError didFailWithError,
                InternalMatchmakerViewControllerHostedPlayerDidAccept hostedPlayerDidAccept);

        }

        #endregion
    }
}
#endif