using UnityEngine;
using System.Collections;
using System;

namespace EasyMobile
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Real-Time Multiplayer API.
    /// </summary>
    public interface IRealTimeMultiplayerClient
    {
        /// <summary>
        /// Creates a real-time match with randomly selected opponent(s). No UI will be shown.
        /// </summary>
        /// <param name="request">The match request.</param>
        /// <param name="listener">The listener for this match.</param>
        void CreateQuickMatch(MatchRequest request, IRealTimeMultiplayerListener listener);

        /// <summary>
        /// Creates a real-time match with the standard matchmaker UI where the initiating player
        /// can invite friends, nearby players or pick random opponents to join the match.
        /// </summary>
        /// <param name="request">The match request.</param>
        /// <param name="listener">The listener for this match.</param>
        void CreateWithMatchmakerUI(MatchRequest request, IRealTimeMultiplayerListener listener);

        /// <summary>
        /// [Google Play Games only] Shows the standard UI where the user can
        /// accept or decline real-time match invitations.
        /// This UI is only available on Google Play Games platform as invitations
        /// are communicated via the iMessage app on iOS (Game Center platform).
        /// </summary>
        /// <remarks>On the invitations screen,
        /// the player can select an invitation to accept, in which case the room setup
        /// process will start. The listener's
        /// <see cref="IRealTimeMultiplayerListener.OnRoomSetupProgress"/> will be called
        /// to report room setup progress, and eventually
        /// <see cref="IRealTimeMultiplayerListener.OnRoomConnected"/> will be called to
        /// indicate that the room setup is either complete or has failed (check the
        /// <b>success</b> parameter of the callback).
        /// </remarks>
        /// <param name="listener">The listener to handle relevant events of the accepted match.</param>
        void ShowInvitationsUI(IRealTimeMultiplayerListener listener);

        /// <summary>
        /// Accepts a real-time match invitation.
        /// </summary>
        /// <remarks>The listener's
        /// <see cref="IRealTimeMultiplayerListener.OnRoomSetupProgress"/> will be called
        /// to report room setup progress, and eventually
        /// <see cref="IRealTimeMultiplayerListener.OnRoomConnected"/> will be called to
        /// indicate that the room setup is either complete or has failed (check the
        /// <b>success</b> parameter of the callback).
        /// You can optionally show the standard waiting room UI during the room setup process
        /// using the "showWaitingRoomUI" parameter.
        /// </remarks>
        /// <param name="invitation">Invitation to accept.</param>
        /// <param name="showWaitingRoomUI">Whether to show the waiting room UI.</param>
        /// <param name="listener">The listener to handle relevant events of the accepted match.</param>
        void AcceptInvitation(Invitation invitation, bool showWaitingRoomUI, IRealTimeMultiplayerListener listener);

        /// <summary>Sends a message to all other participants.</summary>
        /// <param name="reliable">If set to true, reliable messaging is used; otherwise,
        /// it is unreliable. Unreliable messages are faster, but are not guaranteed to arrive
        /// and may arrive out of order.</param>
        /// <param name="data">The data to send.</param>
        void SendMessageToAll(bool reliable, byte[] data);

        /// <summary>
        /// Same as <see cref="SendMessageToAll(bool,byte[])" />, but allows you to specify
        /// offset and length of the data buffer. A typical use of this method is to broadcast
        /// a large amount of data as smaller chunks of reliable messages.
        /// </summary>
        /// <param name="reliable">If set to true, reliable messaging is used; otherwise,
        /// it is unreliable. Unreliable messages are faster, but are not guaranteed to arrive
        /// and may arrive out of order.</param>
        /// <param name="data">The data to send.</param>
        /// <param name="offset">Offset of the data buffer where data starts.</param>
        /// <param name="length">Length of data (from offset).</param>
        void SendMessageToAll(bool reliable, byte[] data, int offset, int length);

        /// <summary>
        /// Sends a message to a particular participant.
        /// </summary>
        /// <param name="reliable">If set to true, reliable messaging is used; otherwise,
        /// it is unreliable. Unreliable messages are faster, but are not guaranteed to arrive
        /// and may arrive out of order.</param>
        /// <param name="participantId">Identifier of the participant to whom the message
        /// will be sent.</param>
        /// <param name="data">The data to send.</param>
        void SendMessage(bool reliable, string participantId, byte[] data);

        /// <summary>
        /// Same as <see cref="SendMessage(bool,string,byte[])" />, but allows you to specify
        /// the offset and length of the data buffer. A typical use of this method is to send
        /// a large amount of data as smaller chunks of reliable messages.
        /// </summary>
        /// <param name="reliable">If set to true, reliable messaging is used; otherwise,
        /// it is unreliable. Unreliable messages are faster, but are not guaranteed to arrive
        /// and may arrive out of order.</param>
        /// <param name="participantId">Identifier of the participant to whom the message
        /// will be sent.</param>
        /// <param name="data">The data to send.</param>
        /// <param name="offset">Offset of the data buffer where data starts.</param>
        /// <param name="length">Length of data (from offset).</param>
        void SendMessage(bool reliable, string participantId, byte[] data, int offset, int length);

        /// <summary>
        /// Gets the connected participants in the room, including self (the local player).
        /// </summary>
        /// <returns>The connected participants, including self. This list is guaranteed
        /// to be ordered lexicographically by Participant ID, which means the ordering will be
        /// the same to all participants.</returns>
        List<Participant> GetConnectedParticipants();

        /// <summary>
        /// Gets the participant that represents the local player in the room.
        /// </summary>
        /// <returns>The participant representing the local player.</returns>
        Participant GetSelf();

        /// <summary>
        /// Gets a connected participant by ID.
        /// </summary>
        /// <returns>The participant, or <c>null</c> if not found.</returns>
        /// <param name="participantId">Participant identifier.</param>
        Participant GetParticipant(string participantId);

        /// <summary>
        /// Leaves the room.
        /// </summary>
        /// <remarks>Call this method to leave the room after you have
        /// started room setup. Leaving the room is not an immediate operation - you
        /// must wait for <see cref="IRealTimeMultplayerListener.OnLeftRoom"/>
        /// to be called. If you leave a room before setup is complete, you will get
        /// a call to
        /// <see cref="IRealTimeMultiplayerListener.OnRoomConnected"/> with <b>false</b>
        /// parameter instead. If you attempt to leave a room that is shutting down or
        /// has shutdown already, you will immediately receive the
        /// <see cref="RealTimeMultiplayerListener.OnLeftRoom"/> callback.
        /// </remarks>
        void LeaveRoom();

        /// <summary>
        /// Returns whether or not the room is connected, which means all participants have
        /// joined and it's ready to play.
        /// </summary>
        /// <returns><c>true</c> if the room is connected; otherwise, <c>false</c>.</returns>
        bool IsRoomConnected();

        /// <summary>
        /// Declines a real-time match invitation.
        /// </summary>
        /// <param name="invitation">Invitation to decline.</param>
        void DeclineInvitation(Invitation invitation);
    }
}