using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile.Internal.GameServices
{
    internal class UnsupportedRealTimeMultiplayerClient : IRealTimeMultiplayerClient
    {
        protected virtual string mUnavailableMessage
        {
            get { return "Real-time multiplayer API is not available on this platform."; }
        }

        public void AcceptInvitation(Invitation invitation, bool showWaitingRoomUI, IRealTimeMultiplayerListener listener)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void CreateQuickMatch(MatchRequest request, IRealTimeMultiplayerListener listener)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void CreateWithMatchmakerUI(MatchRequest request, IRealTimeMultiplayerListener listener)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public List<Participant> GetConnectedParticipants()
        {
            Debug.LogWarning(mUnavailableMessage);
            return null;
        }

        public void DeclineInvitation(Invitation invitation)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public Participant GetParticipant(string participantId)
        {
            Debug.LogWarning(mUnavailableMessage);
            return null;
        }

        public Participant GetSelf()
        {
            Debug.LogWarning(mUnavailableMessage);
            return null;
        }

        public bool IsRoomConnected()
        {
            Debug.LogWarning(mUnavailableMessage);
            return false;
        }

        public void LeaveRoom()
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void SendMessage(bool reliable, string participantId, byte[] data)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void SendMessage(bool reliable, string participantId, byte[] data, int offset, int length)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void SendMessageToAll(bool reliable, byte[] data)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void SendMessageToAll(bool reliable, byte[] data, int offset, int length)
        {
            Debug.LogWarning(mUnavailableMessage);
        }

        public void ShowInvitationsUI(IRealTimeMultiplayerListener listener)
        {
            Debug.LogWarning(mUnavailableMessage);
        }
    }
}
