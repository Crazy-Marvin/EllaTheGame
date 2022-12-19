#if UNITY_ANDROID && EM_GPGS && EM_OBSOLETE_GPGS
using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using EasyMobile.Internal.GameServices.Android;

namespace EasyMobile.Internal.GameServices
{
    using GooglePlayGames;
    using GooglePlayGames.BasicApi;
    using GPGS_TurnBasedMatch = GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch;
    using GPGS_PendingTurnBasedMatchDelegate = EasyMobile.GameServices.GPGS_PendingTurnBasedMatchDelegate;

    /// <summary>
    /// Google Play Games turn based multiplayer client.
    /// </summary>
    internal class GPGTurnBasedMultiplayerClient : ITurnBasedMultiplayerClient
    {
        /// <summary>
        /// The pending match delegates that should be invoked as soon as a
        /// valid consumer match delgate is registered.
        /// </summary>
        private Queue<GPGS_PendingTurnBasedMatchDelegate> mPendingMatchDelegates;

        /// <summary>
        /// The delegate registered in <see cref="RegisterMatchDelegate(MatchDelegate)"/>,
        /// used in <see cref="CreateWithMatchmakerUI(MatchRequest, Action, Action{string})"/>
        /// and <see cref="ShowMatchesUI"/>.
        /// </summary>
        private MatchDelegate mRegisteredMatchDelegate = null;

        internal GPGTurnBasedMultiplayerClient(Queue<GPGS_PendingTurnBasedMatchDelegate> pendingMatchDelegates)
        {
            this.mPendingMatchDelegates = pendingMatchDelegates;
        }

#region Public methods

        public void AcceptInvitation(Invitation invitation, Action<bool, TurnBasedMatch> callback)
        {
            if (invitation == null)
                throw new ArgumentNullException();
            
            PlayGamesPlatform.Instance.TurnBased.AcceptInvitation(
                invitation.GPGS_Invitation.InvitationId,
                (flag, match) =>
                {
                    if (callback == null)
                        return;

                    callback(flag, TurnBasedMatch.FromGPGSTurnBasedMatch(match));
                }
            );
        }

        public void CreateQuickMatch(MatchRequest request, Action<bool, TurnBasedMatch> callback)
        {
            if (request == null)
                throw new ArgumentNullException("Request can't be null.");

            PlayGamesPlatform.Instance.TurnBased.CreateQuickMatch(
                request.MinPlayers - 1, 
                request.MaxPlayers - 1, 
                (uint)GPGTypeConverter.ToGPGSVariant(request.Variant, MatchType.TurnBased),
                (flag, match) =>
                {
                    if (callback == null)
                        return;

                    callback(flag, TurnBasedMatch.FromGPGSTurnBasedMatch(match));
                }
            );
        }

        public void CreateWithMatchmakerUI(MatchRequest request, Action cancelCallback, Action<string> errorCallback)
        {
            if (request == null)
                throw new ArgumentNullException("Request can't be null.");
      
            PlayGamesPlatform.Instance.TurnBased.CreateWithInvitationScreen(
                request.MinPlayers - 1, 
                request.MaxPlayers - 1, 
                (uint)GPGTypeConverter.ToGPGSVariant(request.Variant, MatchType.TurnBased),
                delegate(UIStatus uiBased, GPGS_TurnBasedMatch match)
                {
                    if (uiBased == UIStatus.Valid)
                    {
                        if (mRegisteredMatchDelegate == null)
                        {
                            Debug.LogWarning("The MatchDelegate has not been registered.");
                            return;
                        }

                        mRegisteredMatchDelegate(TurnBasedMatch.FromGPGSTurnBasedMatch(match), true, false);    // playerWantsToQuit doesn't occur on GPG.
                        return;
                    }

                    if (uiBased == UIStatus.UserClosedUI)
                    {
                        if (cancelCallback != null)
                            cancelCallback();
                        return;
                    }

                    if (errorCallback != null)
                        errorCallback(GetMessageFromUIStatus(uiBased));
                }
            );
        }

        public void DeclineInvitation(Invitation invitation)
        {
            if (invitation != null)
                PlayGamesPlatform.Instance.TurnBased.DeclineInvitation(invitation.GPGS_Invitation.InvitationId);
        }

        public void GetAllMatches(Action<TurnBasedMatch[]> callback)
        {
            PlayGamesPlatform.Instance.TurnBased.GetAllMatches(matches =>
                {
                    if (callback == null || matches == null)
                        return;

                    /// GPG invokes most of its callbacks on main thread, except for this one,
                    /// so we have to move it to main thread on our own.
                    RuntimeHelper.RunOnMainThread(() =>
                        callback(matches.Select(match => TurnBasedMatch.FromGPGSTurnBasedMatch(match)).ToArray()));
                }
            );
        }

        public void ShowMatchesUI()
        {
            PlayGamesPlatform.Instance.TurnBased.AcceptFromInbox((flag, match) =>
                {
                    if (mRegisteredMatchDelegate == null)
                    {
                        Debug.LogWarning("The MatchDelegate has not been registered.");
                        return;
                    }

                    if (match != null)
                        mRegisteredMatchDelegate(TurnBasedMatch.FromGPGSTurnBasedMatch(match), true, false);   // use has accepted the invitation (wants to play), 'shouldAutoLaunch' should be true; playerWantsToQuit doesn't occur on GPG.
                });
        }

        public void RegisterMatchDelegate(MatchDelegate del)
        {
            mRegisteredMatchDelegate = del;

            // Report pending matches if any.
            if (mRegisteredMatchDelegate != null && mPendingMatchDelegates != null && mPendingMatchDelegates.Count > 0)
            {
                RuntimeHelper.RunOnMainThread(() =>
                    {
                        while (mPendingMatchDelegates.Count > 0)
                        {
                            var pendingDel = mPendingMatchDelegates.Dequeue();
                            mRegisteredMatchDelegate(TurnBasedMatch.FromGPGSTurnBasedMatch(pendingDel.Match), pendingDel.ShouldAutoLaunch, false);  // playerWantsToQuit doesn't occur on GPG.
                        }

                        mPendingMatchDelegates = null;
                    });
            }

            // Register the delegate with GPGS.
            PlayGamesPlatform.Instance.TurnBased.RegisterMatchDelegate((match, flag) =>
                {
                    if (del == null)
                        return;

                    del(TurnBasedMatch.FromGPGSTurnBasedMatch(match), flag, false); // playerWantsToQuit doesn't occur on GPG.
                }
            );
        }

        public void TakeTurn(TurnBasedMatch match, byte[] data, Participant nextParticipant, Action<bool> callback)
        {
            TakeTurn(match, data, nextParticipant != null ? nextParticipant.ParticipantId : null, callback);
        }

        public void TakeTurn(TurnBasedMatch match, byte[] data, string pendingParticipantId,
                             Action<bool> callback)
        {
            if (match == null)
                throw new ArgumentNullException();

            PlayGamesPlatform.Instance.TurnBased.TakeTurn(match.GPGS_TurnBasedMatch, data, pendingParticipantId, callback);         
        }

        public int GetMaxMatchDataSize()
        {
            //return PlayGamesPlatform.Instance.TurnBased.GetMaxMatchDataSize(); // hasn't been implemented yet...
            return 128000; // 128KB.
        }

        public void Finish(TurnBasedMatch match, byte[] data, MatchOutcome outcome, Action<bool> callback)
        {
            if (match == null || outcome == null)
                throw new ArgumentNullException();

            PlayGamesPlatform.Instance.TurnBased.Finish(match.GPGS_TurnBasedMatch, data, outcome.ToGPGSMatchOutcome(), callback);
        }

        public void AcknowledgeFinished(TurnBasedMatch match, Action<bool> callback)
        {
            if (match == null)
                throw new ArgumentNullException();

            PlayGamesPlatform.Instance.TurnBased.AcknowledgeFinished(match.GPGS_TurnBasedMatch, callback);
        }

        public void LeaveMatch(TurnBasedMatch match, Action<bool> callback)
        {
            if (match == null)
                throw new ArgumentNullException();

            PlayGamesPlatform.Instance.TurnBased.Leave(match.GPGS_TurnBasedMatch, callback);
        }

        public void LeaveMatchInTurn(TurnBasedMatch match, string nextParticipantId, Action<bool> callback)
        {
            if (match == null)
                throw new ArgumentNullException();

            PlayGamesPlatform.Instance.TurnBased.LeaveDuringTurn(match.GPGS_TurnBasedMatch, nextParticipantId, callback);
        }

        public void LeaveMatchInTurn(TurnBasedMatch match, Participant nextParticipant, Action<bool> callback)
        {
            LeaveMatchInTurn(match, nextParticipant != null ? nextParticipant.ParticipantId : null, callback);
        }

        public void Rematch(TurnBasedMatch match, Action<bool, TurnBasedMatch> callback)
        {
            if (match == null)
                throw new ArgumentNullException();

            PlayGamesPlatform.Instance.TurnBased.Rematch(match.GPGS_TurnBasedMatch, (flag, param) =>
                {
                    if (callback == null)
                        return;

                    callback(flag, TurnBasedMatch.FromGPGSTurnBasedMatch(param));
                }
            );
        }

#endregion

#region Private methods

        /// <summary>
        /// Get the right message to use in <see cref="CreateWithMatchmakerUI(MatchRequest, Action, Action{string})"/>.
        /// </summary>
        private string GetMessageFromUIStatus(UIStatus status)
        {
            switch (status)
            {
                case UIStatus.Valid:
                    return "Valid.";

                case UIStatus.InternalError:
                    return "Error: InternalError. An internal error occurred.";

                case UIStatus.NotAuthorized:
                    return "Error: NotAuthorized. The player is not authorized to perform the operation.";

                case UIStatus.VersionUpdateRequired:
                    return "Error: VersionUpdateRequired. The installed version of Google Play services is out of date.";

                case UIStatus.Timeout:
                    return "Error: Timeout. Timed out while awaiting the result.";

                case UIStatus.UserClosedUI:
                    return "Error: UserClosedUI. UI closed by user.";

                case UIStatus.UiBusy:
                    return "Error: UiBusy. UI was busy.";

                case UIStatus.LeftRoom:
                    return "Error: LeftRoom. The player left the multiplayer room.";

                default:
                    return string.Empty;
            }
        }

#endregion
    }
}
#endif
