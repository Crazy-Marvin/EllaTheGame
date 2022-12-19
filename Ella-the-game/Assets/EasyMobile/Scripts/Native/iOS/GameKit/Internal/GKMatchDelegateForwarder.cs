#if UNITY_IOS
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.iOS.CoreFoundation;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.GameKit;

namespace EasyMobile.Internal.iOS.GameKit
{
    using GKMatchDelegate = GKMatch.GKMatchDelegate;
    using GKPlayerConnectionState = GKMatch.GKPlayerConnectionState;

    internal class GKMatchDelegateForwarder : iOSDelegateForwarder<GKMatchDelegate>
    {
        internal GKMatchDelegateForwarder(IntPtr selfPtr)
            : base(selfPtr)
        {
        }

        internal static GKMatchDelegateForwarder FromPointer(IntPtr pointer)
        {
            return InteropObjectFactory<GKMatchDelegateForwarder>.FromPointer(
                pointer,
                ptr => new GKMatchDelegateForwarder(ptr));
        }

        internal GKMatchDelegateForwarder()
            : this(C.GKMatchDelegate_new(
                    InternalMatchDidReceiveDataForRecipientCallback,
                    InternalMatchDidReceiveDataCallback,
                    InternalPlayerDidChangeConnectionStateCallback,
                    InternalMatchDidFailWithErrorCallback,
                    InternalShouldReinviteDisconnectedPlayerCallback))
        {
            // We're using a pointer returned by a native constructor: call CFRelease to balance native ref count
            CFFunctions.CFRelease(this.ToPointer());
        }

        [MonoPInvokeCallback(typeof(C.InternalMatchDidReceiveDataForRecipient))]
        private static void InternalMatchDidReceiveDataForRecipientCallback(IntPtr delegatePtr, IntPtr match, IntPtr data, IntPtr recipient, IntPtr remotePlayer)
        {
            var forwarder = FromPointer(delegatePtr);
        
            if (forwarder != null && forwarder.Listener != null)
            {
                var gkMatch = InteropObjectFactory<GKMatch>.FromPointer(match, ptr => new GKMatch(ptr));
                var nsData = InteropObjectFactory<NSData>.FromPointer(data, ptr => new NSData(ptr));
                var gkRecipientPlayer = InteropObjectFactory<GKPlayer>.FromPointer(recipient, ptr => new GKPlayer(ptr));
                var gkRemotePlayer = InteropObjectFactory<GKPlayer>.FromPointer(remotePlayer, ptr => new GKPlayer(ptr));
        
                forwarder.InvokeOnListener(l => l.MatchDidReceiveDataForRecipient(gkMatch, nsData, gkRecipientPlayer, gkRemotePlayer));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalMatchDidReceiveData))]
        private static void InternalMatchDidReceiveDataCallback(IntPtr delegatePtr, IntPtr match, IntPtr data, IntPtr remotePlayer)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                var gkMatch = InteropObjectFactory<GKMatch>.FromPointer(match, ptr => new GKMatch(ptr));
                var nsData = InteropObjectFactory<NSData>.FromPointer(data, ptr => new NSData(ptr));
                var gkRemotePlayer = InteropObjectFactory<GKPlayer>.FromPointer(remotePlayer, ptr => new GKPlayer(ptr));

                forwarder.InvokeOnListener(l => l.MatchDidReceiveData(gkMatch, nsData, gkRemotePlayer));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalPlayerDidChangeConnectionState))]
        private static void InternalPlayerDidChangeConnectionStateCallback(IntPtr delegatePtr, IntPtr match, IntPtr player, GKPlayerConnectionState state)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                var gkMatch = InteropObjectFactory<GKMatch>.FromPointer(match, ptr => new GKMatch(ptr));
                var gkPlayer = InteropObjectFactory<GKPlayer>.FromPointer(player, ptr => new GKPlayer(ptr));

                forwarder.InvokeOnListener(l => l.PlayerDidChangeConnectionState(gkMatch, gkPlayer, state));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalMatchDidFailWithError))]
        private static void InternalMatchDidFailWithErrorCallback(IntPtr delegatePtr, IntPtr match, IntPtr error)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                var gkMatch = InteropObjectFactory<GKMatch>.FromPointer(match, ptr => new GKMatch(ptr));
                var nsError = InteropObjectFactory<NSError>.FromPointer(error, ptr => new NSError(ptr));

                forwarder.InvokeOnListener(l => l.MatchDidFailWithError(gkMatch, nsError));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalShouldReinviteDisconnectedPlayer))]
        private static bool InternalShouldReinviteDisconnectedPlayerCallback(IntPtr delegatePtr, IntPtr match, IntPtr player)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                var gkMatch = InteropObjectFactory<GKMatch>.FromPointer(match, ptr => new GKMatch(ptr));
                var gkPlayer = InteropObjectFactory<GKPlayer>.FromPointer(player, ptr => new GKPlayer(ptr));

                return forwarder.InvokeOnListener(l => l.ShouldReinviteDisconnectedPlayer(gkMatch, gkPlayer));
            }

            return false;
        }

        #region C wrapper

        private static class C
        {
            internal delegate void InternalMatchDidReceiveDataForRecipient(
            /* InteropGKMatchDelegate */ IntPtr delegatePtr,
            /* InteropGKMatch */ IntPtr match,
            /* InteropNSData */ IntPtr data,
            /* InteropGKPlayer */ IntPtr recipient,
            /* InteropGKPlayer */ IntPtr remotePlayer);

            internal delegate void InternalMatchDidReceiveData(
            /* InteropGKMatchDelegate */ IntPtr delegatePtr,
            /* InteropGKMatch */ IntPtr match,
            /* InteropNSData */ IntPtr data,
            /* InteropGKPlayer */ IntPtr remotePlayer);

            internal delegate void InternalPlayerDidChangeConnectionState(
            /* InteropGKMatchDelegate */ IntPtr delegatePtr,
            /* InteropGKMatch */ IntPtr match,
            /* InteropGKPlayer */ IntPtr player,
            GKPlayerConnectionState state);

            internal delegate void InternalMatchDidFailWithError(
            /* InteropGKMatchDelegate */ IntPtr delegatePtr,
            /* InteropGKMatch */ IntPtr match,
            /* InteropNSError */ IntPtr error);

            internal delegate bool InternalShouldReinviteDisconnectedPlayer(
            /* InteropGKMatchDelegate */ IntPtr delegatePtr,
            /* InteropGKMatch */ IntPtr match,
            /* InteropGKPlayer */ IntPtr player);

            [DllImport("__Internal")]
            internal static extern /* InteropGKMatchDelegate */ IntPtr GKMatchDelegate_new(
                InternalMatchDidReceiveDataForRecipient matchDidReceiveDataForRecipientCallback,
                InternalMatchDidReceiveData matchDidReceiveDataCallback,
                InternalPlayerDidChangeConnectionState playerDidChangeConnectionStateCallback,
                InternalMatchDidFailWithError matchDidFailWithErrorCallback,
                InternalShouldReinviteDisconnectedPlayer shouldReinviteDisconnectedPlayer
            );
        }

        #endregion
    }
}
#endif