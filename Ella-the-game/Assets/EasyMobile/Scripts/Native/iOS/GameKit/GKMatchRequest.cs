#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;

namespace EasyMobile.iOS.GameKit
{
    /// <summary>
    /// A set of parameters for a new live or turn-based match.
    /// </summary>
    internal class GKMatchRequest : iOSObjectProxy
    {
        /// <summary>
        /// The kind of match being created.
        /// </summary>
        public enum GKMatchType
        {
            /// <summary>
            /// A peer-to-peer match hosted by Game Center. It is represented by a <see cref="GKMatch"/> object.
            /// </summary>
            PeerToPeer = 0,
            /// <summary>
            /// A match hosted on your private server.
            /// </summary>
            Hosted,
            /// <summary>
            /// A turn-based match hosted by Game Center. It is represented by a <see cref="GKTurnBasedMatch"/> object.
            /// </summary>
            TurnBased
        }

        /// <summary>
        /// Possible responses from an invitation to a remote player.
        /// </summary>
        public enum GKInviteRecipientResponse
        {
            /// <summary>
            /// The player accepted the invitation.
            /// </summary>
            Accepted = 0,
            /// <summary>
            /// The player rejected the invitation.
            /// </summary>
            Declined,
            /// <summary>
            /// The invitation was unable to be delivered.
            /// </summary>
            Failed,
            /// <summary>
            /// The invitee is not running a compatible version of your game.
            /// </summary>
            Incompatible,
            /// <summary>
            /// The invitee could not be contacted.
            /// </summary>
            UnableToConnect,
            /// <summary>
            /// The invitation timed out without an answer.
            /// </summary>
            NoAnswer
        }

        internal GKMatchRequest(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        public GKMatchRequest()
            : this(C.GKMatchRequest_new())
        {
            // Balance reference count on native.
            CoreFoundation.CFFunctions.CFRelease(this.ToPointer());
        }

        /// <summary>
        /// Returns the maximum number of players allowed in the match request for a given match type.
        /// </summary>
        /// <returns>The players allowed for match type.</returns>
        /// <param name="matchType">Match type.</param>
        public static uint MaxPlayersAllowedForMatchType(GKMatchType matchType)
        {
            return C.GKMatchRequest_maxPlayersAllowedForMatchType(matchType);
        }

        /// <summary>
        /// The maximum number of players that may join the match.
        /// </summary>
        /// <value>The max players.</value>
        public uint MaxPlayers
        {
            get { return C.GKMatchRequest_maxPlayers(SelfPtr()); }
            set { C.GKMatchRequest_setMaxPlayers(SelfPtr(), value); }
        }

        /// <summary>
        /// The minimum number of players that may join the match.
        /// </summary>
        /// <value>The minimum players.</value>
        public uint MinPlayers
        {
            get { return C.GKMatchRequest_minPlayers(SelfPtr()); }
            set { C.GKMatchRequest_setMinPlayers(SelfPtr(), value); }
        }

        /// <summary>
        /// The default number of players for the match.
        /// </summary>
        /// <value>The default number of players.</value>
        public uint DefaultNumberOfPlayers
        {
            get { return C.GKMatchRequest_defaultNumberOfPlayers(SelfPtr()); }
            set { C.GKMatchRequest_setDefaultNumberOfPlayers(SelfPtr(), value); }
        }

        /// <summary>
        /// The string displayed on another player’s device when invited to join a match.
        /// </summary>
        /// <value>The invite message.</value>
        public string InviteMessage
        {
            get
            {
                return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                C.GKMatchRequest_inviteMessage(SelfPtr(), strBuffer, strLen));
            }
            set
            {
                C.GKMatchRequest_setInviteMessage(SelfPtr(), value);
            }
        }

        /// <summary>
        /// A number identifying a subset of players allowed to join the match.
        /// </summary>
        /// <value>The player group.</value>
        public uint PlayerGroup
        {
            get { return C.GKMatchRequest_playerGroup(SelfPtr()); }
            set { C.GKMatchRequest_setPlayerGroup(SelfPtr(), value); }
        }

        /// <summary>
        /// A mask that specifies the role that the local player would like to play in the game.
        /// </summary>
        /// <value>The player attributes.</value>
        public uint PlayerAttributes
        {
            get { return C.GKMatchRequest_playerAttributes(SelfPtr()); }
            set { C.GKMatchRequest_setPlayerAttributes(SelfPtr(), value); }
        }

        #region C wrapper

        private static class C
        {
            [DllImport("__Internal")]
            internal static extern /* InteropGKMatchRequest */ IntPtr GKMatchRequest_new();

            [DllImport("__Internal")]
            internal static extern uint /* from(uint32_t) */ GKMatchRequest_maxPlayersAllowedForMatchType(GKMatchType matchType);

            [DllImport("__Internal")]
            internal static extern uint /* from(uint32_t) */ GKMatchRequest_maxPlayers(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void GKMatchRequest_setMaxPlayers(HandleRef self, uint /* from(uint32_t) */ value);

            [DllImport("__Internal")]
            internal static extern uint /* from(uint32_t) */ GKMatchRequest_minPlayers(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void GKMatchRequest_setMinPlayers(HandleRef self, uint /* from(uint32_t) */ value);

            [DllImport("__Internal")]
            internal static extern uint /* from(uint32_t) */ GKMatchRequest_defaultNumberOfPlayers(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void GKMatchRequest_setDefaultNumberOfPlayers(HandleRef self, uint /* from(uint32_t) */ value);

            [DllImport("__Internal")]
            internal static extern int GKMatchRequest_inviteMessage(HandleRef self, [In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern void GKMatchRequest_setInviteMessage(HandleRef self, string /* from(const char*) */ message);

            [DllImport("__Internal")]
            internal static extern uint /* from(uint32_t) */ GKMatchRequest_playerGroup(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void GKMatchRequest_setPlayerGroup(HandleRef self, uint /* from(uint32_t) */ value);

            [DllImport("__Internal")]
            internal static extern uint /* from(uint32_t) */ GKMatchRequest_playerAttributes(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void GKMatchRequest_setPlayerAttributes(HandleRef self, uint /* from(uint32_t) */ value);
        }

        #endregion
    }
}
#endif
