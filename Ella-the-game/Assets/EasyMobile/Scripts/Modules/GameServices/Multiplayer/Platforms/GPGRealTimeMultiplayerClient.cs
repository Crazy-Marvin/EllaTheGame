#if UNITY_ANDROID && EM_GPGS && EM_OBSOLETE_GPGS
using System;
using System.Collections.Generic;
using System.Linq;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using GPG_Participant = GooglePlayGames.BasicApi.Multiplayer.Participant;
using EasyMobile.Internal;
using EasyMobile.Internal.GameServices.Android;

namespace EasyMobile.Internal.GameServices
{
    /// <summary>
    /// Google Play Games realtime multiplayer client.
    /// </summary>
    public class GPGRealTimeMultiplayerClient : IRealTimeMultiplayerClient
    {
        public void AcceptInvitation(Invitation invitation, bool showWaitingRoomUI, IRealTimeMultiplayerListener listener)
        {
            Util.NullArgumentTest(invitation);
            Util.NullArgumentTest(listener);

            PlayGamesPlatform.Instance.RealTime.AcceptInvitation(invitation.GPGS_Invitation.InvitationId, new GPGRealTimeMultiplayerListener(listener, !showWaitingRoomUI));
        }

        public void CreateQuickMatch(MatchRequest request, IRealTimeMultiplayerListener listener)
        {
            PlayGamesPlatform.Instance.RealTime.CreateQuickGame(
                request.MinPlayers - 1, 
                request.MaxPlayers - 1, 
                (uint)GPGTypeConverter.ToGPGSVariant(request.Variant, MatchType.RealTime),
                new GPGRealTimeMultiplayerListener(listener, true));
        }

        public void CreateWithMatchmakerUI(MatchRequest request, IRealTimeMultiplayerListener listener)
        {
            PlayGamesPlatform.Instance.RealTime.CreateWithInvitationScreen(
                request.MinPlayers - 1, 
                request.MaxPlayers - 1, 
                (uint)GPGTypeConverter.ToGPGSVariant(request.Variant, MatchType.RealTime),
                new GPGRealTimeMultiplayerListener(listener, false));
        }

        public void DeclineInvitation(Invitation invitation)
        {
            if (invitation != null)
                PlayGamesPlatform.Instance.RealTime.DeclineInvitation(invitation.GPGS_Invitation.InvitationId);
        }

        public List<Participant> GetConnectedParticipants()
        {
            var participants = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
            if (participants == null)
                return null;

            return participants.Select(p => Participant.FromGPGSParticipant(p)).ToList();
        }

        public Participant GetParticipant(string participantId)
        {
            return Participant.FromGPGSParticipant(PlayGamesPlatform.Instance.RealTime.GetParticipant(participantId));
        }

        public Participant GetSelf()
        {
            return Participant.FromGPGSParticipant(PlayGamesPlatform.Instance.RealTime.GetSelf());
        }

        public bool IsRoomConnected()
        {
            return PlayGamesPlatform.Instance.RealTime.IsRoomConnected();
        }

        public void LeaveRoom()
        {
            PlayGamesPlatform.Instance.RealTime.LeaveRoom();
        }

        public void SendMessage(bool reliable, string participantId, byte[] data)
        {
            PlayGamesPlatform.Instance.RealTime.SendMessage(reliable, participantId, data);
        }

        public void SendMessage(bool reliable, string participantId, byte[] data, int offset, int length)
        {
            PlayGamesPlatform.Instance.RealTime.SendMessage(reliable, participantId, data, offset, length);
        }

        public void SendMessageToAll(bool reliable, byte[] data)
        {
            PlayGamesPlatform.Instance.RealTime.SendMessageToAll(reliable, data);
        }

        public void SendMessageToAll(bool reliable, byte[] data, int offset, int length)
        {
            PlayGamesPlatform.Instance.RealTime.SendMessageToAll(reliable, data, offset, length);
        }

        public void ShowInvitationsUI(IRealTimeMultiplayerListener listener)
        {
            PlayGamesPlatform.Instance.RealTime.AcceptFromInbox(new GPGRealTimeMultiplayerListener(listener, false));
        }

#region Internal GPGS RealTimeMultiplayerListener

        private class GPGRealTimeMultiplayerListener : RealTimeMultiplayerListener
        {
            private IRealTimeMultiplayerListener listener;
            private bool isProgrammaticMatchmaking;

            internal GPGRealTimeMultiplayerListener(IRealTimeMultiplayerListener listener, bool programmaticMatchmaking)
            {
                this.listener = listener;
                this.isProgrammaticMatchmaking = programmaticMatchmaking;
            }

            public void OnLeftRoom()
            {
                listener.OnLeftRoom();
            }

            public void OnParticipantLeft(GPG_Participant participant)
            {
                listener.OnParticipantLeft(Participant.FromGPGSParticipant(participant));
            }

            public void OnPeersConnected(string[] participantIds)
            {
                listener.OnPeersConnected(participantIds);
            }

            public void OnPeersDisconnected(string[] participantIds)
            {
                listener.OnPeersDisconnected(participantIds);
            }

            public void OnRealTimeMessageReceived(bool _, string senderId, byte[] data)
            {
                listener.OnRealTimeMessageReceived(senderId, data);
            }

            public void OnRoomConnected(bool success)
            {
                listener.OnRoomConnected(success);
            }

            public void OnRoomSetupProgress(float percent)
            {
                listener.OnRoomSetupProgress(percent);
                if (!isProgrammaticMatchmaking)
                    PlayGamesPlatform.Instance.RealTime.ShowWaitingRoomUI();
            }
        }

#endregion
    }
}
#endif