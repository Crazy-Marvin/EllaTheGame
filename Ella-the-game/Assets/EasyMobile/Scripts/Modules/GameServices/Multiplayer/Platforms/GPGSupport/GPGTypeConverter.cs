#if UNITY_ANDROID && EM_GPGS && EM_OBSOLETE_GPGS
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile.Internal.GameServices.Android
{
    using GPGS_Participant = GooglePlayGames.BasicApi.Multiplayer.Participant;
    using GPGS_TurnBasedMatch = GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch;
    using GPGS_Invitation = GooglePlayGames.BasicApi.Multiplayer.Invitation;
    using GPGS_InvType = GooglePlayGames.BasicApi.Multiplayer.Invitation.InvType;

    internal static partial class GPGTypeConverter
    {
        /// <summary>
        /// Convert from <see cref="Participant.ParticipantStatus"/>
        /// to <see cref="GPGS_Participant.ParticipantStatus"/>.
        /// </summary>
        public static GPGS_Participant.ParticipantStatus ToGPGSParticipantStatus(this Participant.ParticipantStatus status)
        {
            switch (status)
            {
                case Participant.ParticipantStatus.NotInvitedYet:
                    return GPGS_Participant.ParticipantStatus.NotInvitedYet;

                case Participant.ParticipantStatus.Invited:
                    return GPGS_Participant.ParticipantStatus.Invited;

                case Participant.ParticipantStatus.Declined:
                    return GPGS_Participant.ParticipantStatus.Declined;

                case Participant.ParticipantStatus.Joined:
                    return GPGS_Participant.ParticipantStatus.Joined;

                case Participant.ParticipantStatus.Unresponsive:
                    return GPGS_Participant.ParticipantStatus.Unresponsive;

                case Participant.ParticipantStatus.Left:
                    return GPGS_Participant.ParticipantStatus.Left;

                case Participant.ParticipantStatus.Done:
                    return GPGS_Participant.ParticipantStatus.Finished;

                case Participant.ParticipantStatus.Matching:
                case Participant.ParticipantStatus.Unknown:
                default:
                    return GPGS_Participant.ParticipantStatus.Unknown;
            }
        }

        /// <summary>
        /// Convert from <see cref="GooglePlayGames.BasicApi.Multiplayer.Participant.ParticipantStatus"/>
        /// to <see cref="Participant.ParticipantStatus"/>.
        /// </summary>
        public static Participant.ParticipantStatus ToEMParticipantStatus(this GPGS_Participant.ParticipantStatus status)
        {
            switch (status)
            {
                case GPGS_Participant.ParticipantStatus.NotInvitedYet:
                    return Participant.ParticipantStatus.NotInvitedYet;

                case GPGS_Participant.ParticipantStatus.Invited:
                    return Participant.ParticipantStatus.Invited;

                case GPGS_Participant.ParticipantStatus.Joined:
                    return Participant.ParticipantStatus.Joined;

                case GPGS_Participant.ParticipantStatus.Declined:
                    return Participant.ParticipantStatus.Declined;

                case GPGS_Participant.ParticipantStatus.Left:
                    return Participant.ParticipantStatus.Left;

                case GPGS_Participant.ParticipantStatus.Finished:
                    return Participant.ParticipantStatus.Done;

                case GPGS_Participant.ParticipantStatus.Unresponsive:
                    return Participant.ParticipantStatus.Unresponsive;

                case GPGS_Participant.ParticipantStatus.Unknown:
                default:
                    return Participant.ParticipantStatus.Unknown;
            }
        }

        /// <summary>
        /// Convert from <see cref="TurnBasedMatch.MatchStatus"/>
        /// to <see cref="GPGS_TurnBasedMatch.MatchStatus"/>.
        /// </summary>
        public static GPGS_TurnBasedMatch.MatchStatus ToGPGSMatchStatus(this TurnBasedMatch.MatchStatus status)
        {
            switch (status)
            {
                case TurnBasedMatch.MatchStatus.Active:
                    return GPGS_TurnBasedMatch.MatchStatus.Active;

                case TurnBasedMatch.MatchStatus.Ended:
                    return GPGS_TurnBasedMatch.MatchStatus.Complete;

                case TurnBasedMatch.MatchStatus.Matching:
                    return GPGS_TurnBasedMatch.MatchStatus.AutoMatching;

                case TurnBasedMatch.MatchStatus.Cancelled:
                    return GPGS_TurnBasedMatch.MatchStatus.Cancelled;

                case TurnBasedMatch.MatchStatus.Expired:
                    return GPGS_TurnBasedMatch.MatchStatus.Expired;

                case TurnBasedMatch.MatchStatus.Deleted:
                    return GPGS_TurnBasedMatch.MatchStatus.Deleted;

                case TurnBasedMatch.MatchStatus.Unknown:
                default:
                    return GPGS_TurnBasedMatch.MatchStatus.Unknown;
            }
        }

        /// <summary>
        /// Convert from <see cref="GPGS_TurnBasedMatch.MatchStatus"/>
        /// to <see cref="EasyMobile.TurnBasedMatch.MatchStatus"/>.
        /// </summary>
        public static TurnBasedMatch.MatchStatus ToEMMatchStatus(this GPGS_TurnBasedMatch.MatchStatus status)
        {
            switch (status)
            {   
                case GPGS_TurnBasedMatch.MatchStatus.Active:
                    return TurnBasedMatch.MatchStatus.Active;

                case GPGS_TurnBasedMatch.MatchStatus.AutoMatching:
                    return TurnBasedMatch.MatchStatus.Matching;

                case GPGS_TurnBasedMatch.MatchStatus.Cancelled:
                    return TurnBasedMatch.MatchStatus.Cancelled;

                case GPGS_TurnBasedMatch.MatchStatus.Complete:
                    return TurnBasedMatch.MatchStatus.Ended;

                case GPGS_TurnBasedMatch.MatchStatus.Expired:
                    return TurnBasedMatch.MatchStatus.Expired;

                case GPGS_TurnBasedMatch.MatchStatus.Deleted:
                    return TurnBasedMatch.MatchStatus.Deleted;

                case GPGS_TurnBasedMatch.MatchStatus.Unknown:
                default:
                    return TurnBasedMatch.MatchStatus.Unknown;
            }
        }

        public static MatchType ToEMMatchType(this GPGS_Invitation inv)
        {
            switch (inv.InvitationType())
            {
                case GPGS_InvType.RealTime:
                    return MatchType.RealTime;
                case GPGS_InvType.TurnBased:
                    return MatchType.TurnBased;
                case GPGS_InvType.Unknown:
                default:
                    return MatchType.Unknown;
            }
        }

        public static int ToGPGSVariant(uint variant, MatchType matchType)
        {
            // Catching internal errors.
            if (variant > MatchRequest.MaxVariant)
            {
                Debug.LogErrorFormat("Internal Error: variant {0} is bigger than the maximum allowed {1}!", variant, MatchRequest.MaxVariant);
                variant = MatchRequest.MaxVariant;
            }

            switch (matchType)
            {
                case MatchType.RealTime:
                    return (int)(variant | MatchRequest.MinVariant);
                case MatchType.TurnBased:
                    return (int)(variant | (MatchRequest.MaxVariant + 1));
                case MatchType.Unknown:
                default:
                    return 0;
            }
        }

        public static uint ToEMVariant(int variant)
        {
            variant = variant < 0 ? 0 : variant;
            return (uint)(variant) & MatchRequest.MaxVariant;
        }

        public static GPGS_InvType InvitationType(this GPGS_Invitation inv)
        {
            var variant = inv.Variant < 0 ? 0 : inv.Variant;
            return (variant & (MatchRequest.MaxVariant + 1)) > 0 ? GPGS_InvType.TurnBased : GPGS_InvType.RealTime;
        }
    }
}
#endif