#if UNITY_IOS
using UnityEngine;
using System;

namespace EasyMobile.iOS.GameKit
{
    /// <summary>
    /// Error codes for the GameKit error domain.
    /// </summary>
    internal enum GKErrorCode
    {
        GKErrorUnknown = 1,
        GKErrorCancelled,
        GKErrorCommunicationsFailure,
        GKErrorUserDenied,
        GKErrorInvalidCredentials,
        GKErrorNotAuthenticated,
        GKErrorAuthenticationInProgress,
        GKErrorInvalidPlayer,
        GKErrorScoreNotSet,
        GKErrorParentalControlsBlocked,
        GKErrorPlayerStatusExceedsMaximumLength,
        GKErrorPlayerStatusInvalid,
        GKErrorMatchRequestInvalid,
        GKErrorUnderage,
        GKErrorGameUnrecognized,
        GKErrorNotSupported,
        GKErrorInvalidParameter,
        GKErrorUnexpectedConnection,
        GKErrorChallengeInvalid,
        GKErrorTurnBasedMatchDataTooLarge,
        GKErrorTurnBasedTooManySessions,
        GKErrorTurnBasedInvalidParticipant,
        GKErrorTurnBasedInvalidTurn,
        GKErrorTurnBasedInvalidState,
        GKErrorOffline,
        GKErrorInvitationsDisabled,
        GKErrorPlayerPhotoFailure,
        GKErrorUbiquityContainerUnavailable,
        GKErrorMatchNotConnected,
        GKErrorGameSessionRequestInvalid
    }
}
#endif
