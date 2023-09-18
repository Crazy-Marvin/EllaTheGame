#if UNITY_IOS
using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.GameKit;
using EasyMobile.iOS.UIKit;
using EasyMobile.Internal.GameServices.iOS;

namespace EasyMobile.Internal.GameServices
{
    using GKTBMVC = GKTurnBasedMatchmakerViewController;
    using GKTBMVC_Delegate = GKTurnBasedMatchmakerViewController.GKTurnBasedMatchmakerViewControllerDelegate;
    using GKTBMOutcome = GKTurnBasedParticipant.GKTurnBasedMatchOutcome;

    /// <summary>
    /// Game Center turn-based multiplayer client.
    /// </summary>
    internal class GCTurnBasedMultiplayerClient : ITurnBasedMultiplayerClient
    {
        private InternalGKLocalPlayerListenerImpl mLocalPlayerListener;
        private GKTBMVC mCurrentMatchmakerVC;

        #region ITurnBasedMultiplayerClient Implementation

        public void AcceptInvitation(Invitation invitation, Action<bool, TurnBasedMatch> callback)
        {
            // This method is a no-op on Game Center.
        }

        public void CreateQuickMatch(MatchRequest request, Action<bool, TurnBasedMatch> callback)
        {
            Util.NullArgumentTest(request);
            Util.NullArgumentTest(callback);

            using (var gkReq = request.ToGKMatchRequest())
            {
                GKTurnBasedMatch.FindMatchForRequest(gkReq, (gkMatch, nsError) =>
                    {
                        TurnBasedMatch match = TurnBasedMatch.FromGKTurnBasedMatch(gkMatch);

                        if (nsError != null)
                            Debug.Log("Failed to create quick match with error " + nsError.LocalizedDescription);

                        if (callback != null)
                            callback(nsError == null, match);
                    });
            }
        }

        public void CreateWithMatchmakerUI(MatchRequest request, Action cancelCallback, Action<string> errorCallback)
        {
            Util.NullArgumentTest(request);

            if (mCurrentMatchmakerVC != null)
            {
                Debug.Log("Ignoring CreateWithMatchmakerUI call because another matchmaker UI is being shown.");
                return;
            }

            // Create a new TBM VC.
            var vc = InteropObjectFactory<GKTBMVC>.Create(
                         () =>
                {
                    using (var gkReq = request.ToGKMatchRequest())
                        return new GKTBMVC(gkReq);
                },
                         viewController =>
                {
                    return viewController.ToPointer();
                });

            // This VC is not showing existing matches.
            vc.ShowExistingMatches = false;

            // Create a delegate for the TBM VC.
            vc.TurnBasedMatchmakerDelegate = new InternalGKTBMVCDelegateImpl()
            { 
                CancelCallback = cancelCallback,
                ErrorCallback = errorCallback,
                ResetMatchmakerVC = () => mCurrentMatchmakerVC = null
            };

            // Store the VC ref.
            mCurrentMatchmakerVC = vc;

            // Now show the VC.
            using (var unityVC = UIViewController.UnityGetGLViewController())
                unityVC.PresentViewController(vc, true, null);
        }

        public void DeclineInvitation(Invitation invitation)
        {
            // This method is a no-op on Game Center.
        }

        public void GetAllMatches(Action<TurnBasedMatch[]> callback)
        {
            Util.NullArgumentTest(callback);

            GKTurnBasedMatch.LoadMatches(
                (gkMatches, nsError) =>
                {
                    TurnBasedMatch[] matches = gkMatches != null ? 
                        gkMatches.Select(gkm => TurnBasedMatch.FromGKTurnBasedMatch(gkm)).ToArray() : null;

                    callback(matches);
                });
        }

        public void ShowMatchesUI()
        {
            if (mCurrentMatchmakerVC != null)
            {
                Debug.Log("Ignoring CreateWithMatchmakerUI call because another matchmaker UI is being shown.");
                return;
            }

            // Create a dummy request with the widest range of players possible.
            var gkRequest = new GKMatchRequest();
            gkRequest.MinPlayers = 2;
            gkRequest.MaxPlayers = GKMatchRequest.MaxPlayersAllowedForMatchType(GKMatchRequest.GKMatchType.TurnBased);

            // Create a new TBM VC with the dummy request.
            var vc = InteropObjectFactory<GKTBMVC>.Create(
                         () => new GKTBMVC(gkRequest),
                         viewController => viewController.ToPointer());

            // This VC should show existing matches.
            vc.ShowExistingMatches = true;

            // Create a delegate for the TBM VC with no callbacks.
            // The delegate is necessary so the VC can be closed.
            vc.TurnBasedMatchmakerDelegate = new InternalGKTBMVCDelegateImpl()
            { 
                CancelCallback = null,
                ErrorCallback = null,
                ResetMatchmakerVC = () => mCurrentMatchmakerVC = null
            };

            // Store the VC ref.
            mCurrentMatchmakerVC = vc;

            // Now show the VC.
            using (var unityVC = UIViewController.UnityGetGLViewController())
                unityVC.PresentViewController(vc, true, null);
        }

        public void RegisterMatchDelegate(MatchDelegate del)
        {
            // Create and register a default local player listener if needed.
            if (mLocalPlayerListener == null)
            {
                mLocalPlayerListener = new InternalGKLocalPlayerListenerImpl()
                {
                    CloseAndResetMatchmakerVC = () =>
                    {
                        if (mCurrentMatchmakerVC != null)
                        {
                            mCurrentMatchmakerVC.DismissViewController(true, null);
                            mCurrentMatchmakerVC = null;
                        }
                    }
                };
                GKLocalPlayer.LocalPlayer.RegisterListener(mLocalPlayerListener);
            }

            mLocalPlayerListener.MatchDelegate = del;
        }

        public void TakeTurn(TurnBasedMatch match, byte[] data, string nextParticipantId, Action<bool> callback)
        {
            Util.NullArgumentTest(match);
            Util.NullArgumentTest(data);

            // Find the next GKTurnBasedParticipant with the given ID.
            var participants = match.GC_TurnBasedMatch.Participants.ToArray(
                                   ptr => InteropObjectFactory<GKTurnBasedParticipant>.FromPointer(ptr, p => new GKTurnBasedParticipant(p)));

            GKTurnBasedParticipant nextGKParticipant;

            if (string.IsNullOrEmpty(nextParticipantId))
            {
                // No specific next player, choosing the the next vacant slot.
                nextGKParticipant = participants.FirstOrDefault(p => p.Player == null);
            }
            else
            {
                // Find the specified next player.
                nextGKParticipant = participants.FirstOrDefault(p =>
                    {
                        var gkPlayer = p.Player;
                        return gkPlayer != null && gkPlayer.PlayerID.Equals(nextParticipantId);
                    });
            }

            if (nextGKParticipant == default(GKTurnBasedParticipant))
            {
                Debug.LogErrorFormat("Not found next participant for ID {0}. Aborting TakeTurn...", string.IsNullOrEmpty(nextParticipantId) ? "[NullOrEmpty]" : nextParticipantId);

                if (callback != null)
                    callback(false);

                return;
            }

            match.GC_TurnBasedMatch.EndTurn(
                new GKTurnBasedParticipant[]{ nextGKParticipant },
                GKTurnBasedMatch.GKTurnTimeoutDefault,
                data,
                error =>
                {
                    if (error != null)
                        Debug.Log("Failed to take turn with error " + error.LocalizedDescription);
                    
                    if (callback != null)
                        callback(error == null);
                }
            );
        }

        public void TakeTurn(TurnBasedMatch match, byte[] data, Participant nextParticipant, Action<bool> callback)
        {
            TakeTurn(match, data, nextParticipant != null ? nextParticipant.ParticipantId : null, callback);
        }

        public int GetMaxMatchDataSize()
        {
            return (int)GKTurnBasedMatch.MatchDataMaximumSize;
        }

        public void Finish(TurnBasedMatch match, byte[] data, MatchOutcome outcome, Action<bool> callback)
        {
            Util.NullArgumentTest(match);
            Util.NullArgumentTest(data);
            Util.NullArgumentTest(outcome);

            // Grab the GKTurnBasedMatch.
            var gkMatch = match.GC_TurnBasedMatch;

            // Set outcome for all participants.
            var participants = gkMatch.Participants.ToArray(
                                   ptr => InteropObjectFactory<GKTurnBasedParticipant>.FromPointer(ptr, p => new GKTurnBasedParticipant(p)));

            foreach (var p in participants)
            {
                var gkPlayer = p.Player;
                if (gkPlayer != null)
                    p.MatchOutcome = outcome.ToGKTurnBasedMatchOutcome(gkPlayer.PlayerID);
                else
                    p.MatchOutcome = new GKTBMOutcome(){ outcome = GKTBMOutcome.Outcome.Quit }; // disconnected player should have outcome as Quit.
            }

            // End match in turn.
            gkMatch.EndMatchInTurn(
                data, 
                error =>
                {
                    if (error != null)
                        Debug.Log("Failed to finish match with error " + error.LocalizedDescription);
                    
                    if (callback != null)
                        callback(error == null);
                });
        }

        public void AcknowledgeFinished(TurnBasedMatch match, Action<bool> callback)
        {
            Util.NullArgumentTest(match);

            // Remove the match if it's finished.
            if (match.GC_TurnBasedMatch.Status == GKTurnBasedMatch.GKTurnBasedMatchStatus.Ended)
            {
                match.GC_TurnBasedMatch.Remove(error =>
                    {
                        if (error != null)
                            Debug.Log("Failed to acknowledge finished match with error " + error.LocalizedDescription);
                        
                        if (callback != null)
                            callback(error == null);
                    });
            }
        }

        public void LeaveMatch(TurnBasedMatch match, Action<bool> callback)
        {
            Util.NullArgumentTest(match);

            // Set the match outcome as Quit.
            var matchOutcome = new GKTBMOutcome() { outcome = GKTBMOutcome.Outcome.Quit };
           
            // Quitting match out of turn.
            match.GC_TurnBasedMatch.ParticipantQuitOutOfTurn(
                matchOutcome,
                error =>
                {
                    if (error != null)
                        Debug.Log("Failed to leave match with error " + error.LocalizedDescription);
                    
                    if (callback != null)
                        callback(error == null);
                });
        }

        public void LeaveMatchInTurn(TurnBasedMatch match, string nextParticipantId, Action<bool> callback)
        {
            Util.NullArgumentTest(match);

            // Find the next GKTurnBasedParticipant with the given ID.
            var participants = match.GC_TurnBasedMatch.Participants.ToArray(
                                   ptr => InteropObjectFactory<GKTurnBasedParticipant>.FromPointer(ptr, p => new GKTurnBasedParticipant(p)));

            GKTurnBasedParticipant nextGKParticipant;

            if (string.IsNullOrEmpty(nextParticipantId))
            {
                // No specific next player, choosing the the next vacant slot.
                nextGKParticipant = participants.FirstOrDefault(p => p.Player == null);
            }
            else
            {
                // Find the specified next player.
                nextGKParticipant = participants.FirstOrDefault(p =>
                    {
                        var gkPlayer = p.Player;
                        return gkPlayer != null && gkPlayer.PlayerID.Equals(nextParticipantId);
                    });
            }

            if (nextGKParticipant == default(GKTurnBasedParticipant))
            {
                Debug.LogErrorFormat("Not found next participant for ID {0}. Aborting LeaveMatchInTurn...", string.IsNullOrEmpty(nextParticipantId) ? "[NullOrEmpty]" : nextParticipantId);

                if (callback != null)
                    callback(false);

                return;
            }

            // Set the match outcome as Quit.
            var matchOutcome = new GKTBMOutcome() { outcome = GKTBMOutcome.Outcome.Quit };

            // Quitting match in turn.
            match.GC_TurnBasedMatch.ParticipantQuitInTurn(
                matchOutcome,
                new GKTurnBasedParticipant[]{ nextGKParticipant },
                GKTurnBasedMatch.GKTurnTimeoutDefault,
                match.Data ?? new byte[0],  // allow leaving a newly created match (whose Data == null)
                error =>
                {
                    if (error != null)
                        Debug.Log("Failed to leave match in turn with error " + error.LocalizedDescription);
                    
                    if (callback != null)
                        callback(error == null);
                });
        }

        public void LeaveMatchInTurn(TurnBasedMatch match, Participant nextParticipant, Action<bool> callback)
        {
            LeaveMatchInTurn(match, nextParticipant != null ? nextParticipant.ParticipantId : null, callback);
        }

        public void Rematch(TurnBasedMatch match, Action<bool, TurnBasedMatch> callback)
        {
            Util.NullArgumentTest(match);

            match.GC_TurnBasedMatch.Rematch((gkMatch, error) =>
                {
                    if (error != null)
                        Debug.Log("Failed to rematch with error " + error.LocalizedDescription);
                    
                    if (callback != null)
                        callback(error == null, TurnBasedMatch.FromGKTurnBasedMatch(gkMatch));
                });
        }

        #endregion

        #region Private Stuff

        // Matchmaker VC delegate.
        private class InternalGKTBMVCDelegateImpl : GKTurnBasedMatchmakerViewControllerDelegateImpl
        {
            public Action CancelCallback { get; set; }

            public Action<string> ErrorCallback { get; set; }

            public Action ResetMatchmakerVC { get; set; }

            public override void TurnBasedMatchmakerViewControllerWasCancelled(GKTurnBasedMatchmakerViewController viewController)
            {
                // Automatically close the VC.
                if (viewController != null)
                    viewController.DismissViewController(true, null);

                if (ResetMatchmakerVC != null)
                    ResetMatchmakerVC();

                // Invoke consumer callback.
                if (CancelCallback != null)
                    CancelCallback();
            }

            public override void TurnBasedMatchmakerViewControllerDidFailWithError(GKTurnBasedMatchmakerViewController viewController, NSError error)
            {
                // Automatically close the VC.
                if (viewController != null)
                    viewController.DismissViewController(true, null);

                if (ResetMatchmakerVC != null)
                    ResetMatchmakerVC();

                // Invoke consumer callback.
                if (ErrorCallback != null)
                    ErrorCallback(error != null ? error.LocalizedDescription : string.Empty);
            }
        }

        // Local user listener for turnbased events.
        private class InternalGKLocalPlayerListenerImpl : GKLocalPlayerListenerImpl
        {
            public MatchDelegate MatchDelegate { get; set; }

            public Action CloseAndResetMatchmakerVC { get; set; }

            public override void PlayerMatchEnded(GKPlayer player, GKTurnBasedMatch match)
            {
                if (CloseAndResetMatchmakerVC != null)
                    CloseAndResetMatchmakerVC();
                
                if (MatchDelegate != null)
                    MatchDelegate(TurnBasedMatch.FromGKTurnBasedMatch(match), false, false);
            }

            public override void PlayerReceivedTurnEventForMatch(GKPlayer player, GKTurnBasedMatch match, bool didBecomeActive)
            {
                if (CloseAndResetMatchmakerVC != null)
                    CloseAndResetMatchmakerVC();

                if (MatchDelegate != null)
                    MatchDelegate(TurnBasedMatch.FromGKTurnBasedMatch(match), didBecomeActive, false);
            }

            public override void PlayerWantsToQuitMatch(GKPlayer player, GKTurnBasedMatch match)
            {
                if (MatchDelegate != null)
                    MatchDelegate(TurnBasedMatch.FromGKTurnBasedMatch(match), false, true);
            }
        }

        #endregion
    }
}
#endif