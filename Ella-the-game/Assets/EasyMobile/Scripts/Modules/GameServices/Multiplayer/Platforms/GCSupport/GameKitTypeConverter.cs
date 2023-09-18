#if UNITY_IOS
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile.iOS.GameKit;

namespace EasyMobile.Internal.GameServices.iOS
{
    using GKMatchType = GKMatchRequest.GKMatchType;
    using ParticipantStatus = Participant.ParticipantStatus;
    using GKTurnBasedParticipantStatus = GKTurnBasedParticipant.GKTurnBasedParticipantStatus;
    using TurnBasedMatchStatus = TurnBasedMatch.MatchStatus;
    using GKTurnBasedMatchStatus = GKTurnBasedMatch.GKTurnBasedMatchStatus;
    using ParticipantResult = MatchOutcome.ParticipantResult;
    using GKTurnBasedMatchOutcome = GKTurnBasedParticipant.GKTurnBasedMatchOutcome;
    using GKPlayerConnectionState = GKMatch.GKPlayerConnectionState;

    internal static partial class GameKitTypeConverter
    {
        public static GKMatchRequest ToGKMatchRequest(this MatchRequest matchRequest)
        {
            var gkRequest = InteropObjectFactory<GKMatchRequest>.Create(
                                () => new GKMatchRequest(),
                                req => req.ToPointer());
            gkRequest.MinPlayers = matchRequest.MinPlayers;
            gkRequest.MaxPlayers = matchRequest.MaxPlayers;
            /* We're not using DefaultNumberOfPlayers. */
            /* We're not using InviteMessage. */
            gkRequest.PlayerGroup = matchRequest.Variant;   // GameKit playerGroup is equivalent to variant
            gkRequest.PlayerAttributes = ~matchRequest.ExclusiveBitmask;   // cross-platform API uses "1" bits to exclude, while GameKit uses "0" bits.
        
            return gkRequest;
        }

        public static GKMatchType ToGKMatchType(this MatchType matchType)
        {
            switch (matchType)
            {
                case MatchType.RealTime:
                    return GKMatchType.PeerToPeer;
                case MatchType.TurnBased:
                    return GKMatchType.TurnBased;
                default:
                    return GKMatchType.Hosted;
            }
        }

        public static GKTurnBasedParticipantStatus ToGKTurnBasedParticipantStatus(this ParticipantStatus status)
        {
            switch (status)
            {
                case ParticipantStatus.Declined:
                    return GKTurnBasedParticipantStatus.Declined;
                case ParticipantStatus.Done:
                    return GKTurnBasedParticipantStatus.Done;
                case ParticipantStatus.Invited:
                    return GKTurnBasedParticipantStatus.Invited;
                case ParticipantStatus.Joined:
                    return GKTurnBasedParticipantStatus.Active;
                case ParticipantStatus.Matching:
                    return GKTurnBasedParticipantStatus.Matching;
                case ParticipantStatus.Left:
                case ParticipantStatus.NotInvitedYet:
                case ParticipantStatus.Unknown:
                case ParticipantStatus.Unresponsive:
                default:
                    return GKTurnBasedParticipantStatus.Unknown;
            }
        }

        public static ParticipantStatus ToParticipantStatus(this GKTurnBasedParticipantStatus status)
        {
            switch (status)
            {
                case GKTurnBasedParticipantStatus.Active:
                    return ParticipantStatus.Joined;
                case GKTurnBasedParticipantStatus.Declined:
                    return ParticipantStatus.Declined;
                case GKTurnBasedParticipantStatus.Done:
                    return ParticipantStatus.Done;
                case GKTurnBasedParticipantStatus.Invited:
                    return ParticipantStatus.Invited;
                case GKTurnBasedParticipantStatus.Matching:
                    return ParticipantStatus.Matching;
                case GKTurnBasedParticipantStatus.Unknown:
                default:
                    return ParticipantStatus.Unknown;
            }
        }

        public static GKTurnBasedMatchStatus ToGKTurnBasedMatchStatus(this TurnBasedMatchStatus status)
        {
            switch (status)
            {
                case TurnBasedMatchStatus.Active:
                    return GKTurnBasedMatchStatus.Open;
                case TurnBasedMatchStatus.Ended:
                    return GKTurnBasedMatchStatus.Ended;
                case TurnBasedMatchStatus.Matching:
                    return GKTurnBasedMatchStatus.Matching;
                case TurnBasedMatchStatus.Cancelled:
                case TurnBasedMatchStatus.Expired:
                case TurnBasedMatchStatus.Deleted:
                case TurnBasedMatchStatus.Unknown:
                default:
                    return GKTurnBasedMatchStatus.Unknown;
                    
            }
        }

        public static TurnBasedMatchStatus ToTurnBasedMatchStatus(this GKTurnBasedMatchStatus status)
        {
            switch (status)
            {
                case GKTurnBasedMatchStatus.Open:
                    return TurnBasedMatchStatus.Active;
                case GKTurnBasedMatchStatus.Matching:
                    return TurnBasedMatchStatus.Matching;
                case GKTurnBasedMatchStatus.Ended:
                    return TurnBasedMatchStatus.Ended;
                case GKTurnBasedMatchStatus.Unknown:
                default:
                    return TurnBasedMatchStatus.Unknown;
            }
        }

        public static GKTurnBasedMatchOutcome ToGKTurnBasedMatchOutcome(this MatchOutcome matchOutcome, string participantId)
        {
            var gkMatchOutcome = new GKTurnBasedMatchOutcome();
            var result = matchOutcome.GetParticipantResult(participantId);
            var placement = matchOutcome.GetParticipantPlacement(participantId);

            switch (result)
            {
                case ParticipantResult.CustomPlacement:
                    gkMatchOutcome.outcome = GKTurnBasedMatchOutcome.Outcome.Custom;
                    gkMatchOutcome.customOutcome = placement;
                    break;
                case ParticipantResult.Lost:
                    gkMatchOutcome.outcome = GKTurnBasedMatchOutcome.Outcome.Lost;
                    break;
                case ParticipantResult.None:
                    gkMatchOutcome.outcome = GKTurnBasedMatchOutcome.Outcome.None;
                    break;
                case ParticipantResult.Tied:
                    gkMatchOutcome.outcome = GKTurnBasedMatchOutcome.Outcome.Tied;
                    break;
                case ParticipantResult.Won:
                    gkMatchOutcome.outcome = GKTurnBasedMatchOutcome.Outcome.Won;
                    break;
                default:
                    gkMatchOutcome.outcome = GKTurnBasedMatchOutcome.Outcome.None;
                    break;
            }

            return gkMatchOutcome;
        }

        public static ParticipantResult ToParticipantResult(this GKTurnBasedMatchOutcome matchOutcome, out uint placement)
        {
            switch (matchOutcome.outcome)
            {
                case GKTurnBasedMatchOutcome.Outcome.Custom:
                    placement = matchOutcome.customOutcome;
                    return ParticipantResult.CustomPlacement;
                case GKTurnBasedMatchOutcome.Outcome.First:
                    placement = 1;
                    return ParticipantResult.CustomPlacement;
                case GKTurnBasedMatchOutcome.Outcome.Second:
                    placement = 2;
                    return ParticipantResult.CustomPlacement;
                case GKTurnBasedMatchOutcome.Outcome.Third:
                    placement = 3;
                    return ParticipantResult.CustomPlacement;
                case GKTurnBasedMatchOutcome.Outcome.Fourth:
                    placement = 4;
                    return ParticipantResult.CustomPlacement;
                case GKTurnBasedMatchOutcome.Outcome.Won:
                    placement = MatchOutcome.PlacementUnset;
                    return ParticipantResult.Won;
                case GKTurnBasedMatchOutcome.Outcome.Lost:
                    placement = MatchOutcome.PlacementUnset;
                    return ParticipantResult.Lost;
                case GKTurnBasedMatchOutcome.Outcome.Tied:
                    placement = MatchOutcome.PlacementUnset;
                    return ParticipantResult.Tied;
                case GKTurnBasedMatchOutcome.Outcome.None:
                    placement = MatchOutcome.PlacementUnset;
                    return ParticipantResult.None;
                case GKTurnBasedMatchOutcome.Outcome.Quit:
                case GKTurnBasedMatchOutcome.Outcome.TimeExpired:
                default:
                    placement = MatchOutcome.PlacementUnset;
                    return ParticipantResult.CustomPlacement;
            }
        }

        public static ParticipantStatus ToParticipantStatus(this GKPlayerConnectionState connectionState)
        {
            switch (connectionState)
            {
                case GKPlayerConnectionState.Connected:
                    return ParticipantStatus.Joined;
                case GKPlayerConnectionState.Disconnected:
                    return ParticipantStatus.Unknown;
                case GKPlayerConnectionState.Unknown:
                default:
                    return ParticipantStatus.Unknown;
            }
        }
    }
}
#endif