using UnityEngine;
using System.Collections;
using System;
using EasyMobile.Internal;

#if UNITY_IOS
using EasyMobile.iOS.GameKit;
using EasyMobile.Internal.GameServices.iOS;
#endif

#if UNITY_ANDROID && EM_GPGS && EM_OBSOLETE_GPGS
using GPGS_Participant = GooglePlayGames.BasicApi.Multiplayer.Participant;
using EasyMobile.Internal.GameServices.Android;
#endif

namespace EasyMobile
{
    /// <summary>
    /// Represents a participant in a real-time or turn-based multiplayer match. A Participant
    /// is different from a Player! A Player is a real-world person with a name
    /// and a Player ID. A Participant is an entity that participates in a real-time
    /// or turn-based match; it may be tied to a Player or not. For example, on Game Center a Participant
    /// is not associated with any Player if the player hasn't actually joined the match; on Google Play Games
    /// Participants without Players represent the anonymous participants in an automatch game.
    /// </summary>
    public class Participant : IComparable<Participant>
    {
        /// <summary>
        /// Represents a participant status in a match.
        /// </summary>
        /// <remarks>
        /// The table below describes how this enum translates to the participant status 
        /// of the underlaying Game Center and Google Play Games platforms.
        /// 
        /// Cross-Platform          GameKit                                         GPGS
        /// -----------------------------------------------------------------------------------------------------
        /// Unknown                 GKTurnBasedParticipantStatusUnknown             ParticipantStatus.Unknown
        /// Matching                GKTurnBasedParticipantStatusMatching            N/A
        /// NotInvitedYet           N/A                                             ParticipantStatus.NotInvitedYet
        /// Invited                 GKTurnBasedParticipantStatusInvited             ParticipantStatus.Invited 
        /// Declined                GKTurnBasedParticipantStatusDeclined            ParticipantStatus.Declined
        /// Joined                  GKTurnBasedParticipantStatusActive              ParticipantStatus.Joined
        /// Unresponsive            N/A                                             ParticipantStatus.Unresponsive
        /// Left                    N/A                                             ParticipantStatus.Left                            
        /// Done                    GKTurnBasedParticipantStatusDone                ParticipantStatus.Finished                           
        /// </remarks>
        public enum ParticipantStatus
        {
            /// <summary>
            /// The participant is in an unexpected state.
            /// </summary>
            Unknown,

            /// <summary>
            /// [Google Play Games only] The participant has not yet been sent an invitation 
            /// (that will be sent upon the invitee's first turn in a turn-based match).
            /// </summary>
            /// <remarks>
            /// This status is only available on Google Play Games platform. A participant never
            /// has this status on Game Center platform, on which the invitations are sent 
            /// immediately when a match is created.
            /// </remarks>
            NotInvitedYet,

            /// <summary>
            /// [Game Center only] The participant represents an unfilled position in the match 
            /// that Game Center promises to fill when needed (when the participant's first turn comes
            /// in a turn-based match).
            /// </summary>
            /// <remarks>
            /// This status is only available on Game Center platform. A participant never has this status
            /// on Google Play Games platform, on which auto-matched participants are not included in a match's
            /// participant list until they actually join the match, see <see cref="TurnBasedMatch.Participants"/>.
            /// </remarks>
            Matching,

            /// <summary>
            /// The participant has been sent an invitation to join the match.
            /// </summary>
            Invited,

            /// <summary>
            /// The participant declined the invitation to join the match. 
            /// When a participant declines an invitation to join a match, the match is automatically terminated.
            /// </summary>
            Declined,

            /// <summary>
            /// The participant has joined the match.
            /// </summary>
            Joined,

            /// <summary>
            /// [Google Play Games only] The participant did not respond to the match in the allotted time.
            /// </summary>
            /// <remarks>
            /// This status is only available on Google Play Games platform. A participant never has this
            /// status on Game Center platform.
            /// </remarks>
            Unresponsive,

            /// <summary>
            /// [Google Play Games only] The participant previously joined and now left the match.
            /// </summary>
            /// <remarks>
            /// This status is only available on Google Play Games platform. A participant never has this
            /// status on Game Center platform.
            /// </remarks> 
            Left,

            /// <summary>
            /// The participant has exited the match. Your game should set a 
            /// matchOutcome for this participant.
            /// </summary>
            Done
        }

        private string mDisplayName = string.Empty;
        private string mParticipantId = string.Empty;
        private ParticipantStatus mStatus = ParticipantStatus.Unknown;
        private Player mPlayer = null;
        private bool mIsConnectedToRoom = false;

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get
            {
                return mDisplayName;
            }
        }

        /// <summary>
        /// Gets the participant identifier. Important: everyone in a particular match
        /// agrees on what is the participant ID for each participant; however, participant
        /// IDs are not meaningful outside of the particular match where they are used.
        /// If the same user plays two subsequent matches, their Participant Id will likely
        /// be different in the two matches.
        /// </summary>
        /// <value>The participant identifier.</value>
        public string ParticipantId
        {
            get
            {
                return mParticipantId;
            }
        }

        /// <summary>
        /// Gets the participant's status.
        /// </summary>
        /// <value>The status.</value>
        public ParticipantStatus Status
        {
            get
            {
                return mStatus;
            }
        }

        /// <summary>
        /// Gets the player that corresponds to this participant. On Google Play Games
        /// platform, this will be null if this is an anonymous (automatch) participant.
        /// </summary>
        /// <value>The player, or null if this is an anonymous participant.</value>
        public Player Player
        {
            get
            {
                return mPlayer;
            }
        }

        /// <summary>
        /// Returns whether the participant is connected to the real-time room. This has no
        /// meaning in turn-based matches.
        /// </summary>
        public bool IsConnectedToRoom
        {
            get
            {
                return mIsConnectedToRoom;
            }
        }

        public override string ToString()
        {
            return string.Format("[Participant: '{0}' (id {1}), status={2}, " +
                "player={3}, connected={4}]", mDisplayName, mParticipantId, mStatus.ToString(),
                mPlayer == null ? "NULL" : mPlayer.ToString(), mIsConnectedToRoom);
        }

        public int CompareTo(Participant other)
        {
            return mParticipantId.CompareTo(other.mParticipantId);
        }

        public bool Equals(Participant obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return IsEqual(obj);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (!(obj is Participant))
            {
                return false;
            }

            return IsEqual((Participant)obj);
        }

        public override int GetHashCode()
        {
            return mParticipantId != null ? mParticipantId.GetHashCode() : 0;
        }

        private bool IsEqual(Participant other)
        {
            return mParticipantId.Equals(other.mParticipantId);
        }

        // Private constructor: cross-platform participants should ony be created from
        // an underlying GKTurnBasedParticipant or GPG Participant.
        protected Participant(string displayName, string participantId,
                              ParticipantStatus status, Player player, bool connectedToRoom)
        {
            mDisplayName = displayName;
            mParticipantId = participantId;
            mStatus = status;
            mPlayer = player;
            mIsConnectedToRoom = connectedToRoom;
        }

#if UNITY_IOS
        
        /// <summary>
        /// Constructs a new instance from the <see cref="EasyMobile.iOS.GameKit.GKTurnBasedParticipant"/> object.
        /// </summary>
        /// <returns>The GK turn based participant.</returns>
        /// <param name="gkParticipant">Gk participant.</param>
        internal static Participant FromGKTurnBasedParticipant(GKTurnBasedParticipant gkParticipant)
        {
            if (gkParticipant == null)
                return null;

            var gkPlayer = gkParticipant.Player;
            return new Participant(
                gkPlayer != null ? gkPlayer.DisplayName : null,
                gkPlayer != null ? gkPlayer.PlayerID : null,
                gkParticipant.Status.ToParticipantStatus(),
                Player.FromGKPlayer(gkPlayer),
                gkParticipant.IsConnectedToRoom());
        }

        /// <summary>
        /// Constructs a new instance from a <see cref="Player"/> object.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="status">Status.</param>
        /// <param name="connectedToRoom">If set to <c>true</c> connected to room.</param>
        internal static Participant FromGKPlayer(GKPlayer gkPlayer, ParticipantStatus status, bool connectedToRoom)
        {
            if (gkPlayer == null)
                return null;

            // First create a Player object from GKPlayer.
            var player = Player.FromGKPlayer(gkPlayer);

            if (player == null)
                return null;    

            // Now create participant.
            return new Participant(
                player.userName,
                player.id,
                status,
                player,
                connectedToRoom);
        }

#endif

#if UNITY_ANDROID && EM_GPGS && EM_OBSOLETE_GPGS

        /// <summary>
        /// Constructs a new instance
        /// from the <see cref="GooglePlayGames.BasicApi.Multiplayer.Participant"/> object.
        /// </summary>
        internal static Participant FromGPGSParticipant(GPGS_Participant participant)
        {
            if (participant == null)
                return null;

            return new Participant(participant.DisplayName,
                participant.ParticipantId,
                participant.Status.ToEMParticipantStatus(),
                Player.FromGPGSPlayer(participant.Player),
                participant.IsConnectedToRoom);
        }

#endif
    }
}
