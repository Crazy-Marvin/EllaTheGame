using System.Collections;

namespace EasyMobile
{
    /// <summary>
    /// Real-Time multiplayer listener. The methods of this listener 
    /// will be called to notify you of real-time room events.
    /// </summary>
    public interface IRealTimeMultiplayerListener
    {
        /// <summary>
        /// Called during room setup to notify of room setup progress.
        /// </summary>
        /// <param name="percent">The room setup progress in percent (0.0 to 100.0).</param>
        void OnRoomSetupProgress(float percent);

        /// <summary>
        /// Notifies that room setup is finished (all participants have joined).
        /// If <c>success == true</c>, you should
        /// react by starting to play the game; otherwise, you may show an error screen.
        /// </summary>
        /// <param name="success">Whether room setup was successful.</param>
        void OnRoomConnected(bool success);

        /// <summary>
        /// Notifies that the local player has left the room. This may have happened
        /// because you called <see cref="IRealTimeMultiplayerClient.LeaveRoom"/>,
        /// or because an error occurred and the player was dropped from the room.
        /// You should react by stopping your game and possibly showing an error screen
        /// (unless the player requested to leave the room, naturally).
        /// </summary>
        void OnLeftRoom();

        /// <summary>
        /// Notifies that a participant has left during room setup.
        /// This is called during room setup if a player declines an invitation
        /// or leaves. The status of the participant can be inspected to determine
        /// the reason. If all players have left, the room is closed automatically.
        /// </summary>
        /// <param name="participant">The participant that left.</param>
        void OnParticipantLeft(Participant participant);

        /// <summary>
        /// Notifies that peer players have connected to the room.
        /// </summary>
        /// <param name="participantIds">Identifiers of the connected participants.</param>
        void OnPeersConnected(string[] participantIds);

        /// <summary>
        /// Notifies that peer players have disconnected from the room.
        /// </summary>
        /// <param name="participantIds">Identifiers of the disconnected participants.</param>
        void OnPeersDisconnected(string[] participantIds);

        /// <summary>
        /// Notifies that a real-time message has been received.
        /// </summary>
        /// <param name="senderId">Sender identifier.</param>
        /// <param name="data">Data.</param>
        void OnRealTimeMessageReceived(string senderId, byte[] data);

        /// <summary>
        /// [Game Center only] Notifies that a player in a two-player match was disconnected.
        /// </summary>
        /// <returns>Your game should return true if it wants Game Center
        /// to attempt to reconnect the player, otherwise it should return false.</returns>
        /// <param name="participant">The participant that disconnected.</param>
        /// <remarks>
        /// The method is available on Game Center only, it's never called on Google Play Games platform.
        /// </remarks>
        bool ShouldReinviteDisconnectedPlayer(Participant participant);
    }
}
