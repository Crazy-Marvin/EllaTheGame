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
    internal class GKInviteEventListenerForwarder : iOSMulticastDelegateForwarder<GKInviteEventListener>
    {
        internal GKInviteEventListenerForwarder(IntPtr selfPtr)
            : base(selfPtr)
        {
        }

        internal static GKInviteEventListenerForwarder FromPointer(IntPtr pointer)
        {
            return InteropObjectFactory<GKInviteEventListenerForwarder>.FromPointer(
                pointer,
                ptr => new GKInviteEventListenerForwarder(ptr));
        }

        internal GKInviteEventListenerForwarder()
            : this(C.GKInviteEventListener_new(
                    InternalDidAcceptInviteCallback,
                    InternalDidRequestMatchWithRecipientsCallback))
        {
            // We're using a pointer returned by a native constructor: call CFRelease to balance native ref count
            CFFunctions.CFRelease(this.ToPointer());
        }

        [MonoPInvokeCallback(typeof(C.InternalPlayerDidAcceptInvite))]
        private static void InternalDidAcceptInviteCallback(IntPtr listenerPtr, IntPtr player, IntPtr invite)
        {
            var forwarder = FromPointer(listenerPtr);

            if (forwarder != null && forwarder.ListenerCount > 0)
            {
                var gkPlayer = InteropObjectFactory<GKPlayer>.FromPointer(player, ptr => new GKPlayer(ptr));
                var gkInvite = InteropObjectFactory<GKInvite>.FromPointer(invite, ptr => new GKInvite(ptr));

                forwarder.InvokeOnAllListeners(l => l.PlayerDidAcceptInvite(gkPlayer, gkInvite));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalPlayerDidRequestMatchWithRecipients))]
        private static void InternalDidRequestMatchWithRecipientsCallback(IntPtr listenerPtr, IntPtr player, IntPtr recipientPlayers)
        {
            var forwarder = FromPointer(listenerPtr);

            if (forwarder != null && forwarder.ListenerCount > 0)
            {
                // GKPlayer.
                GKPlayer gkPlayer = InteropObjectFactory<GKPlayer>.FromPointer(player, ptr => new GKPlayer(ptr));

                // GKPlayer[].
                GKPlayer[] recipientGKPlayers = null;

                if (PInvokeUtil.IsNotNull(recipientPlayers))
                {
                    // Creating a one-time usage NSArray binder, no need to use the factory.
                    using (var nsArray = new NSArray<GKPlayer>(recipientPlayers))
                    {
                        recipientGKPlayers = nsArray.ToArray(ptr => InteropObjectFactory<GKPlayer>.FromPointer(ptr, p => new GKPlayer(p)));
                    }
                }

                forwarder.InvokeOnAllListeners(l => l.PlayerDidRequestMatchWithRecipients(gkPlayer, recipientGKPlayers));
            }
        }

        #region C wrapper

        private static class C
        {
            internal delegate void InternalPlayerDidAcceptInvite(
            /* InteropGKInviteEventListener */ IntPtr listenerPtr,
            /* InteropGKPlayer */ IntPtr player,
            /* InteropGKInvite */ IntPtr invite);

            internal delegate void InternalPlayerDidRequestMatchWithRecipients(
            /* InteropGKInviteEventListener */ IntPtr listenerPtr,
            /* InteropGKPlayer */ IntPtr player,
            /* InteropNSArray<InteropGKPlayer> */ IntPtr recipientPlayers);

            [DllImport("__Internal")]
            internal static extern /* InteropGKInviteEventListener */ IntPtr GKInviteEventListener_new(
                InternalPlayerDidAcceptInvite didAcceptInviteCallback,
                InternalPlayerDidRequestMatchWithRecipients didRequestMatchCallback);
        }

        #endregion
    }
}
#endif