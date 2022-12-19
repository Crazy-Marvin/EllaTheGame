#if UNITY_IOS
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using EasyMobile.iOS.CoreFoundation;
using EasyMobile.iOS.GameKit;

namespace EasyMobile.Internal.iOS.GameKit
{
    internal class GKLocalPlayerListenerForwarder : iOSMulticastDelegateForwarder<GKLocalPlayerListener>
    {
        private GKLocalPlayerListener mDelegator;
        private GKInviteEventListenerForwarder mInviteEventListenerForwarder;
        private GKTurnBasedEventListenerForwarder mTurnBasedEventListenerForwarder;

        internal GKLocalPlayerListenerForwarder(IntPtr selfPtr)
            : base(selfPtr)
        {
        }

        internal GKLocalPlayerListenerForwarder(GKInviteEventListenerForwarder inviteEventListenerForwarder,
                                                GKTurnBasedEventListenerForwarder turnBasedEventListenerForwarder)
            : this(C.GKLocalPlayerListener_new(
                    inviteEventListenerForwarder != null ? inviteEventListenerForwarder.ToPointer() : IntPtr.Zero,
                    turnBasedEventListenerForwarder != null ? turnBasedEventListenerForwarder.ToPointer() : IntPtr.Zero))
        {
            // We're using a pointer returned by a native constructor: call CFRelease to balance native ref count
            CFFunctions.CFRelease(this.ToPointer());

            // Store the sub-forwarders.
            this.mInviteEventListenerForwarder = inviteEventListenerForwarder;
            this.mTurnBasedEventListenerForwarder = turnBasedEventListenerForwarder;
        }

        public override void RegisterListener(GKLocalPlayerListener listener)
        {
            base.RegisterListener(listener);

            // Register the same listener with subforwarders.
            if (mInviteEventListenerForwarder != null)
                mInviteEventListenerForwarder.RegisterListener(listener);

            if (mTurnBasedEventListenerForwarder != null)
                mTurnBasedEventListenerForwarder.RegisterListener(listener);
        }

        public override void UnregisterListener(GKLocalPlayerListener listener)
        {
            base.UnregisterListener(listener);

            // Unregister the listener on subforwarders.
            if (mInviteEventListenerForwarder != null)
                mInviteEventListenerForwarder.UnregisterListener(listener);

            if (mTurnBasedEventListenerForwarder != null)
                mTurnBasedEventListenerForwarder.UnregisterListener(listener);
        }

        public override void UnregisterAllListeners()
        {
            base.UnregisterAllListeners();

            // Unregister all listeners of subforwarders.
            if (mInviteEventListenerForwarder != null)
                mInviteEventListenerForwarder.UnregisterAllListeners();

            if (mTurnBasedEventListenerForwarder != null)
                mTurnBasedEventListenerForwarder.UnregisterAllListeners();
        }

        #region C wrapper

        private static class C
        {
            [DllImport("__Internal")]
            internal static extern /* InteropGKLocalPlayerListener */ IntPtr GKLocalPlayerListener_new(
                /* InteropGKInviteEventListener */ IntPtr interopGKInviteEventListener,
                /* InteropGKTurnBasedEventListener */IntPtr interopGKTurnBasedEventListener);
        }

        #endregion
    }
}
#endif