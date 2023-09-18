#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;

namespace EasyMobile.iOS.GameKit
{
    /// <summary>
    /// A matchmaking invitation sent by another player to the local player.
    /// </summary>
    internal class GKInvite : iOSObjectProxy
    {
        private GKPlayer mSender;

        internal GKInvite(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// A Boolean value that states whether the game is hosted on your servers.
        /// </summary>
        /// <returns><c>true</c> if this instance is hosted; otherwise, <c>false</c>.</returns>
        public bool IsHosted()
        {
            return C.GKInvite_isHosted(SelfPtr());
        }

        /// <summary>
        /// The player attributes for the match.
        /// </summary>
        /// <value>The player attributes.</value>
        public uint PlayerAttributes
        {
            get { return C.GKInvite_playerAttributes(SelfPtr()); }
        }

        /// <summary>
        /// The player group for the match.
        /// </summary>
        /// <value>The player group.</value>
        public uint PlayerGroup
        {
            get { return C.GKInvite_playerGroup(SelfPtr()); }
        }

        /// <summary>
        /// The identifier for the player who sent the invitation.
        /// </summary>
        /// <value>The sender.</value>
        public GKPlayer Sender
        {
            get
            {
                if (mSender == null)
                {
                    IntPtr senderPtr = C.GKInvite_sender(SelfPtr());
                    mSender = InteropObjectFactory<GKPlayer>.FromPointer(senderPtr, ptr => new GKPlayer(ptr));
                    CoreFoundation.CFFunctions.CFRelease(senderPtr);
                }

                return mSender;
            }
        }

        #region C wrapper

        private static class C
        {
            [DllImport("__Internal")][return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool GKInvite_isHosted(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern uint GKInvite_playerAttributes(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern uint GKInvite_playerGroup(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern /* InteropGKPlayer */IntPtr GKInvite_sender(HandleRef selfPtr);
        }

        #endregion
    }
}
#endif
