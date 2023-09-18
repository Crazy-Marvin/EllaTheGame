#if UNITY_IOS
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.iOS;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.iOS.GameKit
{
    using EasyMobile.Internal.iOS.GameKit;
    using Listener = GKLocalPlayerListener;
    using ListenerForwarder = EasyMobile.Internal.iOS.GameKit.GKLocalPlayerListenerForwarder;

    /// <summary>
    /// An object representing the authenticated Game Center player on a device.
    /// </summary>
    internal class GKLocalPlayer : GKPlayer
    {
        private static GKLocalPlayer sLocalPlayer;

        private ListenerForwarder mListenerForwarder;

        internal GKLocalPlayer(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// Retrieves the shared instance of the local player.
        /// </summary>
        /// <value>The local player.</value>
        public static GKLocalPlayer LocalPlayer
        {
            get
            {
                if (sLocalPlayer == null)
                {
                    var ptr = C.GKLocalPlayer_localPlayer();
                    sLocalPlayer = InteropObjectFactory<GKLocalPlayer>.FromPointer(ptr, p => new GKLocalPlayer(p));
                    CoreFoundation.CFFunctions.CFRelease(ptr);   // balance native reference count.
                }
                return sLocalPlayer;
            }
        }

        /// <summary>
        /// A Boolean value that indicates whether a local player is currently signed in to Game Center.
        /// </summary>
        /// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
        public bool IsAuthenticated
        {
            get { return C.GKLocalPlayer_isAuthenticated(SelfPtr()); }
        }

        /// <summary>
        /// A Boolean value that declares whether the local player is underage.
        /// </summary>
        /// <value><c>true</c> if this instance is underage; otherwise, <c>false</c>.</value>
        public bool IsUnderage
        {
            get { return C.GKLocalPlayer_isUnderage(SelfPtr()); }
        }

        /// <summary>
        /// Returns an array of players the local player recently played with.
        /// </summary>
        /// <param name="completionHandler">Completion handler.</param>
        public void LoadRecentPlayers(Action<GKPlayer[], NSError> completionHandler)
        {
            C.GKLocalPlayer_loadRecentPlayers(
                SelfPtr(),
                InternalLoadRecentPlayersCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Register a listener for a particular event.
        /// </summary>
        /// <param name="listener">Listener.</param>
        public void RegisterListener(GKLocalPlayerListener listener)
        {
            if (listener == null)
                return;

            // Create an interal listener forwarder if needed.
            if (mListenerForwarder == null)
            {
                // Create a listener forwarder and register it in native side.
                mListenerForwarder = CreateListenerForwarder();
                C.GKLocalPlayer_registerListener(SelfPtr(), mListenerForwarder.ToPointer());
            }

            // Register the listener with the forwarder.
            mListenerForwarder.RegisterListener(listener);
        }

        /// <summary>
        /// Unregister a specific listener.
        /// </summary>
        /// <param name="listener">Listener.</param>
        public void UnregisterListener(GKLocalPlayerListener listener)
        {
            if (mListenerForwarder != null)
                mListenerForwarder.UnregisterListener(listener);
        }

        /// <summary>
        /// Unregister all listeners in your game.
        /// </summary>
        public void UnregisterAllListeners()
        {
            if (mListenerForwarder != null)
                mListenerForwarder.UnregisterAllListeners();
        }

        #region Private Stuff

        private static ListenerForwarder CreateListenerForwarder()
        {
            // First create subforwarder.
            var inviteEventLF = InteropObjectFactory<GKInviteEventListenerForwarder>.Create(
                                    () => new GKInviteEventListenerForwarder(),
                                    fwd => fwd.ToPointer());
            var tbEventLF = InteropObjectFactory<GKTurnBasedEventListenerForwarder>.Create(
                                () => new GKTurnBasedEventListenerForwarder(),
                                fwd => fwd.ToPointer());

            // Now create and return main forwarder composited from subforwarders.
            return InteropObjectFactory<ListenerForwarder>.Create(
                () => new ListenerForwarder(inviteEventLF, tbEventLF),
                fwd => fwd.ToPointer());
        }

        [MonoPInvokeCallback(typeof(C.LoadRecentPlayersCallback))]
        private static void InternalLoadRecentPlayersCallback(IntPtr players, IntPtr error, IntPtr secondaryCallback)
        {
            if (PInvokeUtil.IsNull(secondaryCallback))
                return;

            GKPlayer[] gkPlayers = null;

            if (PInvokeUtil.IsNotNull(players))
            {
                // Creating a one-time usage NSArray binder, so need to use the factory.
                using (var nsArray = new NSArray<GKPlayer>(players))
                {
                    gkPlayers = nsArray.ToArray(ptr => InteropObjectFactory<GKPlayer>.FromPointer(ptr, p => new GKPlayer(p)));
                }
            }

            // A new NSError object is always created on native side, so no need
            // to check the binder pool for reusing an existing one.
            NSError nsError = PInvokeUtil.IsNotNull(error) ? new NSError(error) : null;

            // Invoke consumer callback.
            PInvokeCallbackUtil.PerformInternalCallback(
                "GKLocalPlayer#InternalLoadRecentPlayersCallback",
                PInvokeCallbackUtil.Type.Temporary,
                gkPlayers, nsError, secondaryCallback);
        }

        #endregion

        #region C wrapper

        private static class C
        {
            internal delegate void LoadRecentPlayersCallback(
            /* InteropNSArray<InteropGKPlayer> */IntPtr players,
            /* InteropNSError */ IntPtr error,
                IntPtr secondaryCallbackPtr);

            [DllImport("__Internal")]
            internal static extern void GKLocalPlayer_loadRecentPlayers(
                HandleRef selfPtr, LoadRecentPlayersCallback callback, IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern /* InteropGKPlayer */IntPtr GKLocalPlayer_localPlayer();

            [DllImport("__Internal")][return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool GKLocalPlayer_isAuthenticated(HandleRef selfPtr);

            [DllImport("__Internal")][return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool GKLocalPlayer_isUnderage(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void GKLocalPlayer_registerListener(HandleRef selfPtr, /* InteropGKLocalPlayerListener */IntPtr listener);

            [DllImport("__Internal")]
            internal static extern void GKLocalPlayer_unregisterAllListeners(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void GKLocalPlayer_unregisterListener(HandleRef selfPtr, /* InteropGKLocalPlayerListener */IntPtr listener);

        }

        #endregion
    }
}
#endif
