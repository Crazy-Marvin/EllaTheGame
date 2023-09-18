using UnityEngine;
using System.Collections;
using System;

namespace EasyMobile.Internal.GameServices
{
    internal class UnsupportedTurnBasedMultiplayerClient : ITurnBasedMultiplayerClient
    {
        protected virtual string mUnavailableMessage
        {
            get { return "Turn-based multiplayer is not available on this platform."; }
        }

        public void AcceptInvitation(Invitation invitation, Action<bool, TurnBasedMatch> callback)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void CreateQuickMatch(MatchRequest request, Action<bool, TurnBasedMatch> callback)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void CreateWithMatchmakerUI(MatchRequest request, Action cancelCallback, Action<string> errorCallback)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void DeclineInvitation(Invitation invitation)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void GetAllMatches(Action<TurnBasedMatch[]> callback)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void ShowMatchesUI()
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void RegisterMatchDelegate(MatchDelegate del)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void TakeTurn(TurnBasedMatch match, byte[] data, string nextParticipantId, Action<bool> callback)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void TakeTurn(TurnBasedMatch match, byte[] data, Participant nextParticipant, Action<bool> callback)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public int GetMaxMatchDataSize()
        {
            return 0;
        }

        public void Finish(TurnBasedMatch match, byte[] data, MatchOutcome outcome, Action<bool> callback)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void AcknowledgeFinished(TurnBasedMatch match, Action<bool> callback)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void LeaveMatch(TurnBasedMatch match, Action<bool> callback)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void LeaveMatchInTurn(TurnBasedMatch match, string pendingParticipantId, Action<bool> callback)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void LeaveMatchInTurn(TurnBasedMatch match, Participant pendingParticipant, Action<bool> callback)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void Rematch(TurnBasedMatch match, Action<bool, TurnBasedMatch> callback)
        {
            Debug.LogWarning(mUnavailableMessage);
        }
    }
}
