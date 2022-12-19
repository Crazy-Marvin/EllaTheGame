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
    internal class GKTurnBasedEventListenerForwarder : iOSMulticastDelegateForwarder<GKTurnBasedEventListener>
    {
        internal GKTurnBasedEventListenerForwarder(IntPtr selfPtr)
            : base(selfPtr)
        {
        }

        internal static GKTurnBasedEventListenerForwarder FromPointer(IntPtr pointer)
        {
            return InteropObjectFactory<GKTurnBasedEventListenerForwarder>.FromPointer(
                pointer,
                ptr => new GKTurnBasedEventListenerForwarder(ptr));
        }

        internal GKTurnBasedEventListenerForwarder()
            : this(C.GKTurnBasedEventListener_new(
                    InternalDidRequestMatchWithOtherPlayersCallback,
                    InternalMatchEndedCallback,
                    InternalReceivedTurnEventForMatchCallback,
                    InternalWantsToQuitMatchCallback))
        {
            // We're using a pointer returned by a native constructor: call CFRelease to balance native ref count
            CFFunctions.CFRelease(this.ToPointer());
        }

        [MonoPInvokeCallback(typeof(C.InternalPlayerDidRequestMatchWithOtherPlayers))]
        private static void InternalDidRequestMatchWithOtherPlayersCallback(IntPtr listenerPtr, IntPtr player, IntPtr playersToInvite)
        {
            var forwarder = FromPointer(listenerPtr);

            if (forwarder != null && forwarder.ListenerCount > 0)
            {
                // GKPlayer.
                GKPlayer gkPlayer = InteropObjectFactory<GKPlayer>.FromPointer(player, ptr => new GKPlayer(ptr));

                // GKPlayer[].
                GKPlayer[] gkPlayersToInvite = null;

                if (PInvokeUtil.IsNotNull(playersToInvite))
                {
                    // Creating a one-time usage NSArray binder, no need to use the factory.
                    using (var nsArray = new NSArray<GKPlayer>(playersToInvite))
                    {
                        gkPlayersToInvite = nsArray.ToArray(ptr => InteropObjectFactory<GKPlayer>.FromPointer(ptr, p => new GKPlayer(p)));
                    }
                }

                // Invoke consumer delegates.
                forwarder.InvokeOnAllListeners(l => l.PlayerDidRequestMatchWithOtherPlayers(gkPlayer, gkPlayersToInvite));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalPlayerMatchEnded))]
        private static void InternalMatchEndedCallback(IntPtr listenerPtr, IntPtr player, IntPtr match)
        {
            var forwarder = FromPointer(listenerPtr);

            if (forwarder != null && forwarder.ListenerCount > 0)
            {
                var gkPlayer = InteropObjectFactory<GKPlayer>.FromPointer(player, ptr => new GKPlayer(ptr));
                var gkTBMatch = InteropObjectFactory<GKTurnBasedMatch>.FromPointer(match, ptr => new GKTurnBasedMatch(ptr));
            
                forwarder.InvokeOnAllListeners(l => l.PlayerMatchEnded(gkPlayer, gkTBMatch));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalPlayerReceivedTurnEventForMatch))]
        private static void InternalReceivedTurnEventForMatchCallback(IntPtr listenerPtr, IntPtr player, IntPtr match, bool didBecomeActive)
        {
            var forwarder = FromPointer(listenerPtr);

            if (forwarder != null && forwarder.ListenerCount > 0)
            {
                var gkPlayer = InteropObjectFactory<GKPlayer>.FromPointer(player, ptr => new GKPlayer(ptr));
                var gkTBMatch = InteropObjectFactory<GKTurnBasedMatch>.FromPointer(match, ptr => new GKTurnBasedMatch(ptr));

                forwarder.InvokeOnAllListeners(l => l.PlayerReceivedTurnEventForMatch(gkPlayer, gkTBMatch, didBecomeActive));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalPlayerWantsToQuitMatch))]
        private static void InternalWantsToQuitMatchCallback(IntPtr listenerPtr, IntPtr player, IntPtr match)
        {
            var forwarder = FromPointer(listenerPtr);

            if (forwarder != null && forwarder.ListenerCount > 0)
            {
                var gkPlayer = InteropObjectFactory<GKPlayer>.FromPointer(player, ptr => new GKPlayer(ptr));
                var gkTBMatch = InteropObjectFactory<GKTurnBasedMatch>.FromPointer(match, ptr => new GKTurnBasedMatch(ptr));

                forwarder.InvokeOnAllListeners(l => l.PlayerWantsToQuitMatch(gkPlayer, gkTBMatch));
            }
        }

        #region C wrapper

        private static class C
        {
            internal delegate void InternalPlayerDidRequestMatchWithOtherPlayers(
            /* InteropGKTurnBasedEventListener */ IntPtr listenerPtr,
            /* InteropGKPlayer */ IntPtr player,
            /* InteropNSArray<InteropGKPlayer> */ IntPtr playersToInvite);

            internal delegate void InternalPlayerMatchEnded(
            /* InteropGKTurnBasedEventListener */ IntPtr listenerPtr,
            /* InteropGKPlayer */ IntPtr player,
            /* InteropGKTurnBasedMatch */ IntPtr match);

            internal delegate void InternalPlayerReceivedTurnEventForMatch(
            /* InteropGKTurnBasedEventListener */ IntPtr listenerPtr,
            /* InteropGKPlayer */ IntPtr player,
            /* InteropGKTurnBasedMatch */ IntPtr match,
                bool didBecomeActive);

            internal delegate void InternalPlayerWantsToQuitMatch(
            /* InteropGKTurnBasedEventListener */ IntPtr listenerPtr,
            /* InteropGKPlayer */ IntPtr player,
            /* InteropGKTurnBasedMatch */ IntPtr match);

            [DllImport("__Internal")]
            internal static extern /* InteropGKTurnBasedEventListener */ IntPtr 
            GKTurnBasedEventListener_new(InternalPlayerDidRequestMatchWithOtherPlayers didRequestMatchCallback,
                                         InternalPlayerMatchEnded matchEndedCallback,
                                         InternalPlayerReceivedTurnEventForMatch receivedTurnEventCallback,
                                         InternalPlayerWantsToQuitMatch wantsToQuitMatchCallback);
        }

        #endregion
    }
}
#endif