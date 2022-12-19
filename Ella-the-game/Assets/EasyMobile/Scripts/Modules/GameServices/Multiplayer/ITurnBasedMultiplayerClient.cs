using UnityEngine;
using System.Collections;
using System;

namespace EasyMobile
{
    /// <summary>
    /// Turn-Based Multiplayer API.
    /// </summary>
    public interface ITurnBasedMultiplayerClient
    {
        /// <summary>
        /// Creates a turn-based match with randomly selected opponent(s). No UI will be shown.
        /// </summary>
        /// <param name="request">The match request.</param>
        /// <param name="callback">Called when match setup is complete or fails.
        /// If it succeeds, this callback will be called with (true, match); otherwise, the callback will be
        /// called with (false, null). This callback is always invoked on the main thread.</param>
        void CreateQuickMatch(MatchRequest request, Action<bool, TurnBasedMatch> callback);

        /// <summary>
        /// Creates a turn-based match with the standard matchmaker UI where the initiating player
        /// can invite friends, nearby players or pick random opponents to join the match.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="cancelCallback">Cancel callback, called when the UI is closed.</param>
        /// <param name="errorCallback">Error callback, called when the UI encounters some error.</param>
        void CreateWithMatchmakerUI(MatchRequest request, Action cancelCallback, Action<string> errorCallback);

        /// <summary>
        /// Gets all turn-based matches that the local player joins.
        /// </summary>
        /// <param name="callback">Callback.</param>
        void GetAllMatches(Action<TurnBasedMatch[]> callback);

        /// <summary>
        /// Shows the standard UI where the player can pick a 
        /// turn-based match or accept an invitation.
        /// </summary>
        void ShowMatchesUI();

        /// <summary>
        /// Accepts a turn-based match invitation.
        /// </summary>
        /// <param name="invitation">Invitation to accept.</param>
        /// <param name="callback">Called when match setup is complete or fails.
        /// If it succeeds, this callback will be called with (true, match); otherwise, the callback will be
        /// called with (false, null). This callback is always invoked on the main thread.</param>
        void AcceptInvitation(Invitation invitation, Action<bool, TurnBasedMatch> callback);

        /// <summary>
        /// Registers a delegate to be called when a turn-based match arrives.</summary>
        /// <remarks> Matches may arrive
        /// as notifications on the device when it's the player's turn. If the match
        /// arrived via notification (this can be determined from the delegate's parameters),
        /// the recommended implementation is to take the player directly to the game
        /// screen so they can play their turn.
        /// </remarks>
        /// <param name="del">Delegate to notify when a match arrives. Always called on the main thread</param>
        void RegisterMatchDelegate(MatchDelegate del);

        /// <summary>
        /// Takes a turn.</summary>
        /// <remarks>Before you call this method, make sure that it is actually the
        /// player's turn in the match, otherwise this call will fail.
        /// </remarks>
        /// <param name="match">The turn-based match to take turn in.</param>
        /// <param name="data">Data. New match data.</param>
        /// <param name="nextParticipantId">ID of the participant to take the next turn. If
        /// this is null or empty and there are automatch slots open, the turn will be passed
        /// to one of the automatch players. Passing null or empty when there are no open
        /// automatch slots is an error.</param>
        /// <param name="callback">Callback. Will be called with true for success,
        /// false for failure. This callback is always called on the main thread.</param>
        void TakeTurn(TurnBasedMatch match, byte[] data, string nextParticipantId,
                      Action<bool> callback);

        /// <summary>
        /// Takes a turn.</summary>
        /// <remarks>Before you call this method, make sure that it is actually the
        /// player's turn in the match, otherwise this call will fail.
        /// </remarks>
        /// <param name="match">The turn-based match to take turn in.</param>
        /// <param name="data">Data. New match data.</param>
        /// <param name="nextParticipant">The participant to take the next turn. If
        /// this is null and there are automatch slots open, the turn will be passed
        /// to one of the automatch players. Passing null when there are no open
        /// automatch slots is an error.</param>
        /// <param name="callback">Callback. Will be called with true for success,
        /// false for failure. This callback is always called on the main thread.</param>
        void TakeTurn(TurnBasedMatch match, byte[] data, Participant nextParticipant, Action<bool> callback);

        /// <summary>
        /// Gets the maximum size allowed for the match data, in bytes.
        /// </summary>
        /// <returns>The maximum match data size in bytes.</returns>
        int GetMaxMatchDataSize();

        /// <summary>
        /// Finishes a turn-based match.
        /// </summary>
        /// <param name="match">The turn-based match to finish.</param>
        /// <param name="data">Final match data.</param>
        /// <param name="outcome">The outcome of the match (who won, who lost, ...)</param>
        /// <param name="callback">Callback. Will be called with true for success,
        /// false for failure. This callback is always called on the main thread.</param>
        void Finish(TurnBasedMatch match, byte[] data, MatchOutcome outcome, Action<bool> callback);

        /// <summary>
        /// Acknowledges that a turn-based match was finished.</summary>
        /// <remarks>
        /// Call this on a finished match that you
        /// have just shown to the user, to acknowledge that the user has seen the results
        /// of the finished match. This will remove the match from the user's matches UI.
        /// </remarks>
        /// <param name="match">The finished turn-based match.</param>
        /// <param name="callback">Callback. Will be called with true for success,
        /// false for failure. This callback is always called on the main thread.</param>
        void AcknowledgeFinished(TurnBasedMatch match, Action<bool> callback);

        /// <summary>
        /// Leaves a turn-based match (not during turn). 
        /// Call this to leave the match when it is not your turn.
        /// </summary>
        /// <param name="match">Match.</param>
        /// <param name="callback">Callback. Will be called with true for success,
        /// false for failure. This callback is always called on the main thread.</param>
        void LeaveMatch(TurnBasedMatch match, Action<bool> callback);

        /// <summary>
        /// Leaves a turn-based match (during turn). 
        /// Call this to leave the match when it's your turn.
        /// </summary>
        /// <param name="match">The turn-based match to leave.</param>
        /// <param name="nextParticipantId">ID of the participant to take the next turn. If
        /// this is null or empty and there are automatch slots open, the turn will be passed
        /// to one of the automatch players. Passing null or empty when there are no open
        /// automatch slots is an error.</param>
        /// <param name="callback">Callback. Will be called with true for success,
        /// false for failure. This callback is always called on the main thread.</param>
        void LeaveMatchInTurn(TurnBasedMatch match, string nextParticipantId, Action<bool> callback);

        /// <summary>
        /// Leaves a turn-based match (during turn). 
        /// Call this to leave the match when it's your turn.
        /// </summary>
        /// <param name="match">The turn-based match to leave.</param>
        /// <param name="nextParticipant">The participant to take the next turn. If
        /// this is null and there are automatch slots open, the turn will be passed
        /// to one of the automatch players. Passing null when there are no open
        /// automatch slots is an error.</param>
        /// <param name="callback">Callback. Will be called with true for success,
        /// false for failure. This callback is always called on the main thread.</param>
        void LeaveMatchInTurn(TurnBasedMatch match, Participant nextParticipant, Action<bool> callback);

        /// <summary>
        /// Requests a rematch. This can be used on a finished match in order to start a new
        /// match with the same opponents.
        /// </summary>
        /// <param name="match">The turn-based match to replay.</param>
        /// <param name="callback">Called when the new match setup is complete or fails.
        /// If it succeeds, this callback will be called with (true, match); otherwise, the callback will be
        /// called with (false, null). This callback is always invoked on the main thread.</param>
        void Rematch(TurnBasedMatch match, Action<bool, TurnBasedMatch> callback);

        /// <summary>
        /// Declines a turn-based match invitation.
        /// </summary>
        /// <param name="invitation">Invitation to decline.</param>
        void DeclineInvitation(Invitation invitation);
    }

    /// <summary>
    /// Match delegate. Called when a match arrives.
    /// <param name="shouldAutoLaunch">If this is true, then the game should immediately
    /// proceed to the game screen to play this match, without prompting the user. If
    /// false, you should prompt the user before going to the match screen. As an example,
    /// when a user taps on the "Play" button on a turn-based multiplayer notification, it is
    /// clear that they want to play that match right away, so the plugin calls this
    /// delegate with shouldAutoLaunch = true. However, if we receive an incoming notification
    /// that the player hasn't specifically indicated they wish to accept (for example,
    /// we received one silently while the app is in foreground), this delegate will be called
    /// with shouldAutoLaunch=false to indicate that you should confirm with the user
    /// before switching to the game.</param>
    /// <param name="playerWantsToQuit">[Game Center only] This is true only if the local player
    /// has removed the match in the matches UI while being the turn holder.
    /// When this happens you should call the <see cref="LeaveMatchInTurn"/> method to end the
    /// local player's turn and pass the match to the next appropriate participant according
    /// to your game logic. Note that this never occur on Google Play Games platform.</param>
    /// </summary>
    public delegate void MatchDelegate(TurnBasedMatch match,bool shouldAutoLaunch,bool playerWantsToQuit);
}
