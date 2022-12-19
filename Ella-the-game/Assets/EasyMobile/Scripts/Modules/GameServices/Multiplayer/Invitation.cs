using UnityEngine;
using System.Collections;

namespace EasyMobile
{
#if UNITY_IOS
    using EasyMobile.iOS.GameKit;
#endif

#if UNITY_ANDROID && EM_GPGS && EM_OBSOLETE_GPGS
    using GPGSInvitation = GooglePlayGames.BasicApi.Multiplayer.Invitation;
    using EasyMobile.Internal.GameServices.Android;
#endif

    /// <summary>
    /// Represents an invitation to a multiplayer game. 
    /// The invitation may be for a turn-based or real-time match.
    /// </summary>
    public class Invitation
    {
        private MatchType mInvitationType;
        private Participant mInviter;
        private uint mVariant;

        /// <summary>
        /// Gets the type of the invitation.
        /// </summary>
        /// <value>The type of the invitation (real-time or turn-based).</value>
        public MatchType InvitationType
        {
            get
            {
                return mInvitationType;
            }
        }

        /// <summary>
        /// Gets the participant who sent the invitation.
        /// </summary>
        /// <value>The participant who issued the invitation.</value>
        public Participant Inviter
        {
            get
            {
                return mInviter;
            }
        }

        /// <summary>
        /// Gets the match variant. The meaning of this parameter is defined by the game.
        /// It usually indicates a particular game type or mode (for example "capture the flag", 
        /// "first to 10 points", etc). It allows the player to match only with players whose 
        /// match request shares the same variant number. This value must
        /// be between 0 and 511 (inclusive). Default value is 0.
        /// </summary>
        /// <value>The match variant. 0 means default (unset).</value>
        public uint Variant
        {
            get
            {
                return mVariant;
            }
        }

        public override string ToString()
        {
            return string.Format("[Invitation: InvitationType={0}, Inviter={1}, " +
                "Variant={2}]", InvitationType, Inviter, Variant);
        }

        protected Invitation(MatchType invType, Participant inviter, uint variant)
        {
            mInvitationType = invType;
            mInviter = inviter;
            mVariant = variant;
        }

#if UNITY_IOS
        
        internal GKInvite GK_Invite { get; private set; }

        internal static Invitation FromGKInvite(GKInvite gkInvite, MatchType type)
        {
            if (gkInvite == null)
                return null;

            return new Invitation(
                type,
                Participant.FromGKPlayer(gkInvite.Sender, Participant.ParticipantStatus.Joined, true),  // the inviter should be connected to room already
                gkInvite.PlayerGroup)
            {
                GK_Invite = gkInvite
            };
        }

#endif

#if UNITY_ANDROID && EM_GPGS && EM_OBSOLETE_GPGS

        internal GPGSInvitation GPGS_Invitation { get; private set; }

        /// <summary>
        /// Constructs a new instance
        /// from the <see cref="GooglePlayGames.BasicApi.Multiplayer.Invitation"/> object.
        /// </summary>
        internal static Invitation FromGPGSInvitation(GPGSInvitation inv)
        {
            if (inv == null)
                return null;

            return new Invitation(inv.ToEMMatchType(),
                Participant.FromGPGSParticipant(inv.Inviter),
                GPGTypeConverter.ToEMVariant(inv.Variant))
            {
                GPGS_Invitation = inv
            };
        }

#endif
    }
}