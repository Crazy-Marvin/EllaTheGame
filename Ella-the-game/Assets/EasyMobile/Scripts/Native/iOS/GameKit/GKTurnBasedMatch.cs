#if UNITY_IOS
using System;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.iOS.GameKit
{
    /// <summary>
    /// An object used to implement turn-based matches between sets of players on Game Center.
    /// </summary>
    internal class GKTurnBasedMatch : iOSObjectProxy
    {
        /// <summary>
        /// The different states that a match can enter.
        /// </summary>
        public enum GKTurnBasedMatchStatus
        {
            /// <summary>
            /// The match is in an unexpected state.
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// The match is currently being played.
            /// </summary>
            Open,
            /// <summary>
            /// The match has been completed.
            /// </summary>
            Ended,
            /// <summary>
            /// Game Center is still searching for other players to join the match.
            /// </summary>
            Matching
        }

        /// <summary>
        /// Indicates that the player has one week to take a turn.
        /// </summary>
        public const long GKTurnTimeoutDefault = -1;

        /// <summary>
        /// Indicates that the player’s turn never times out.
        /// </summary>
        public const long GKTurnTimeoutNone = -2;

        private static uint? mMatchDataMaxSize;
        private DateTime? mCreationDate;
        private string mMatchId;

        internal GKTurnBasedMatch(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// Returns the limit the Game Center servers place on the size of the match data.
        /// </summary>
        /// <value>The size of the match data maximum.</value>
        public static uint MatchDataMaximumSize
        {
            get
            { 
                if (mMatchDataMaxSize == null)
                    mMatchDataMaxSize = C.GKTurnBasedMatch_matchDataMaximumSize(new HandleRef(null, IntPtr.Zero)); 
                return mMatchDataMaxSize.Value;
            }
        }

        /// <summary>
        /// Information about the players participating in the match.
        /// </summary>
        /// <value>The participants.</value>
        public NSArray<GKTurnBasedParticipant> Participants
        {
            get
            {
                var ptr = C.GKTurnBasedMatch_participants(SelfPtr());
                var participants = InteropObjectFactory<NSArray<GKTurnBasedParticipant>>.FromPointer(ptr, p => new NSArray<GKTurnBasedParticipant>(p));
                CoreFoundation.CFFunctions.CFRelease(ptr);  // release pointer returned by the native method.
                return participants;
            }
        }

        /// <summary>
        /// The participant whose turn it is to act next.
        /// </summary>
        /// <value>The current participant.</value>
        public GKTurnBasedParticipant CurrentParticipant
        {
            get
            {
                return InteropObjectFactory<GKTurnBasedParticipant>.FromPointer(C.GKTurnBasedMatch_currentParticipant(SelfPtr()), ptr => new GKTurnBasedParticipant(ptr));
            }
        }

        /// <summary>
        /// Game-specific data that reflects the details of the match.
        /// </summary>
        /// <value>The match data.</value>
        public NSData MatchData
        {
            get
            {
                var ptr = C.GKTurnBasedMatch_matchData(SelfPtr());
                var nsData = InteropObjectFactory<NSData>.FromPointer(ptr, p => new NSData(p));
                CoreFoundation.CFFunctions.CFRelease(ptr);   // release pointer returned by the native method.
                return nsData;
            }
        }

        /// <summary>
        /// A message displayed to all players in the match.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get
            {
                return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                C.GKTurnBasedMatch_message(SelfPtr(), strBuffer, strLen));
            }
            set
            {
                C.GKTurnBasedMatch_setMessage(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The date that the match was created.
        /// </summary>
        /// <value>The creation date.</value>
        public DateTime CreationDate
        {
            get
            { 
                if (mCreationDate == null)
                    mCreationDate = Util.FromMillisSinceUnixEpoch(C.GKTurnBasedMatch_creationDate(SelfPtr()));
                return mCreationDate.Value;
            }
        }

        /// <summary>
        /// A string that uniquely identifies the match.
        /// </summary>
        /// <value>The match I.</value>
        public string MatchID
        {
            get
            {
                if (string.IsNullOrEmpty(mMatchId))
                    mMatchId = PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                        C.GKTurnBasedMatch_matchID(SelfPtr(), strBuffer, strLen));
                return mMatchId;
            }
        }

        /// <summary>
        /// The current state of the match.
        /// </summary>
        /// <value>The status.</value>
        public GKTurnBasedMatchStatus Status
        {
            get { return C.GKTurnBasedMatch_status(SelfPtr()); }
        }

        /// <summary>
        /// Loads the turn-based matches involving the local player and creates a match object for each match.
        /// </summary>
        /// <param name="completionHandler">Completion handler.</param>
        public static void LoadMatches(Action<GKTurnBasedMatch[], NSError> completionHandler)
        {
            C.GKTurnBasedMatch_loadMatches(
                InternalMultipleMatchesCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Loads a specific match.
        /// </summary>
        /// <param name="matchID">Match I.</param>
        /// <param name="completionHandler">Completion handler.</param>
        public static void LoadMatchWithID(string matchID, Action<GKTurnBasedMatch, NSError> completionHandler)
        {
            C.GKTurnBasedMatch_loadMatchWithID(
                matchID,
                InternalSingleMatchCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Programmatically searches for a new match to join.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="completionHandler">Completion handler.</param>
        public static void FindMatchForRequest(GKMatchRequest request, Action<GKTurnBasedMatch, NSError> completionHandler)
        {
            Util.NullArgumentTest(request);

            C.GKTurnBasedMatch_findMatchForRequest(
                request.ToPointer(),
                InternalSingleMatchCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Programmatically accept an invitation to a turn-based match.
        /// </summary>
        /// <param name="completionHandler">Completion handler.</param>
        public void AcceptInvite(Action<GKTurnBasedMatch, NSError> completionHandler)
        {
            C.GKTurnBasedMatch_acceptInvite(
                SelfPtr(), 
                InternalSingleMatchCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Programmatically decline an invitation to a turn-based match.
        /// </summary>
        /// <param name="completionHandler">Completion handler.</param>
        public void DeclineInvite(Action<NSError> completionHandler)
        {
            C.GKTurnBasedMatch_declineInvite(
                SelfPtr(),
                InternalErrorOnlyCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Create a new turn-based match with the same participants as an existing match.
        /// </summary>
        /// <param name="completionHandler">Completion handler.</param>
        public void Rematch(Action<GKTurnBasedMatch, NSError> completionHandler)
        {
            C.GKTurnBasedMatch_rematch(
                SelfPtr(),
                InternalSingleMatchCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Loads the game-specific data associated with a match, including all exchanges.
        /// </summary>
        /// <param name="completionHandler">Completion handler.</param>
        public void LoadMatchData(Action<byte[], NSError> completionHandler)
        {
            C.GKTurnBasedMatch_loadMatchData(
                SelfPtr(),
                InternalLoadMatchDataCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Update the match data without advancing the game to another player.
        /// </summary>
        /// <param name="matchData">Match data.</param>
        /// <param name="completionHandler">Completion handler.</param>
        public void SaveCurrentTurn(byte[] matchData, Action<NSError> completionHandler)
        {
            Util.NullArgumentTest(matchData);

            C.GKTurnBasedMatch_saveCurrentTurn(
                SelfPtr(),
                matchData,
                matchData.Length,
                InternalErrorOnlyCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Updates the data stored on Game Center for the current match.
        /// </summary>
        /// <param name="nextParticipants">Next participants.</param>
        /// <param name="timeout">Timeout.</param>
        /// <param name="matchData">Match data.</param>
        /// <param name="completionHandler">Completion handler.</param>
        public void EndTurn(GKTurnBasedParticipant[] nextParticipants, long timeout, byte[] matchData, Action<NSError> completionHandler)
        {
            Util.NullArgumentTest(nextParticipants);
            Util.NullArgumentTest(matchData);

            C.GKTurnBasedMatch_endTurn(
                SelfPtr(),
                nextParticipants.Select(p => p != null ? p.ToPointer() : IntPtr.Zero).ToArray(),
                nextParticipants.Length,
                timeout,
                matchData,
                matchData.Length,
                InternalErrorOnlyCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Resigns the current player from the match without ending the match.
        /// </summary>
        /// <param name="matchOutcome">Match outcome.</param>
        /// <param name="nextParticipants">Next participants.</param>
        /// <param name="timeout">Timeout.</param>
        /// <param name="matchData">Match data.</param>
        /// <param name="completionHandler">Completion handler.</param>
        public void ParticipantQuitInTurn(GKTurnBasedParticipant.GKTurnBasedMatchOutcome matchOutcome, 
                                          GKTurnBasedParticipant[] nextParticipants, long timeout, byte[] matchData, 
                                          Action<NSError> completionHandler)
        {
            Util.NullArgumentTest(nextParticipants);
            Util.NullArgumentTest(matchData);

            C.GKTurnBasedMatch_participantQuitInTurn(
                SelfPtr(),
                ref matchOutcome,
                nextParticipants.Select(p => p.ToPointer()).ToArray(),
                nextParticipants.Length,
                timeout,
                matchData,
                matchData.Length,
                InternalErrorOnlyCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Resigns the player from the match when that player is not the current player. This action does not end the match.
        /// </summary>
        /// <param name="matchOutcome">Match outcome.</param>
        /// <param name="completionHandler">Completion handler.</param>
        public void ParticipantQuitOutOfTurn(GKTurnBasedParticipant.GKTurnBasedMatchOutcome matchOutcome, Action<NSError> completionHandler)
        {
            C.GKTurnBasedMatch_participantQuitOutOfTurn(
                SelfPtr(),
                ref matchOutcome,
                InternalErrorOnlyCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Ends the match.
        /// </summary>
        /// <param name="matchData">Match data.</param>
        /// <param name="completionHandler">Completion handler.</param>
        public void EndMatchInTurn(byte[] matchData, Action<NSError> completionHandler)
        {
            Util.NullArgumentTest(matchData);

            C.GKTurnBasedMatch_endMatchInTurn(
                SelfPtr(),
                matchData,
                matchData.Length,
                InternalErrorOnlyCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Programmatically removes a match from Game Center.
        /// </summary>
        /// <param name="completionHandler">Completion handler.</param>
        public void Remove(Action<NSError> completionHandler)
        {
            C.GKTurnBasedMatch_remove(
                SelfPtr(),
                InternalErrorOnlyCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        #region Private Stuff

        /// <summary>
        /// Converts the param pointers to corresponding managed types 
        /// and the callback pointer to delegate Action<GKTurnBasedMatch[], NSError> and invoke it.
        /// This callback is to be invoked by native code.
        /// </summary>
        /// <param name="matches">Matches.</param>
        /// <param name="error">Error.</param>
        /// <param name="secondaryCallback">Secondary callback.</param>
        [MonoPInvokeCallback(typeof(C.MultipleMatchesCallback))]
        private static void InternalMultipleMatchesCallback(IntPtr matches, IntPtr error, IntPtr secondaryCallback)
        {
            if (PInvokeUtil.IsNull(secondaryCallback))
                return;

            GKTurnBasedMatch[] gkMatches = null;

            if (PInvokeUtil.IsNotNull(matches))
            {
                // Creating a one-time usage NSArray binder, no need to use the factory.
                using (var nsArray = new NSArray<GKTurnBasedMatch>(matches))
                {
                    gkMatches = nsArray.ToArray(ptr => InteropObjectFactory<GKTurnBasedMatch>.FromPointer(ptr, p => new GKTurnBasedMatch(p)));
                }
            }

            // A new NSError object is always created on native side, so no need
            // to check the binder pool for reusing an existing one.
            NSError nsError = PInvokeUtil.IsNotNull(error) ? new NSError(error) : null;

            // Invoke consumer callback.
            PInvokeCallbackUtil.PerformInternalCallback(
                "GKTurnBasedMatch#InternalMultipleMatchesCallback",
                PInvokeCallbackUtil.Type.Temporary,
                gkMatches, nsError, secondaryCallback);
        }

        /// <summary>
        /// Converts the param pointers to corresponding managed types 
        /// and the callback pointer to delegate Action<GKTurnBasedMatch, NSError> and invoke it.
        /// This callback is to be invoked by native code.
        /// </summary>
        /// <param name="match">Match.</param>
        /// <param name="error">Error.</param>
        /// <param name="secondaryCallback">Secondary callback.</param>
        [MonoPInvokeCallback(typeof(C.SingleMatchCallback))]
        private static void InternalSingleMatchCallback(IntPtr match, IntPtr error, IntPtr secondaryCallback)
        {
            if (PInvokeUtil.IsNull(secondaryCallback))
                return;
            
            GKTurnBasedMatch gkMatch = InteropObjectFactory<GKTurnBasedMatch>.FromPointer(match, ptr => new GKTurnBasedMatch(ptr));
            NSError nsError = PInvokeUtil.IsNotNull(error) ? new NSError(error) : null;

            PInvokeCallbackUtil.PerformInternalCallback(
                "GKTurnBasedMatch#InternalSingleMatchCallback",
                PInvokeCallbackUtil.Type.Temporary,
                gkMatch, nsError, secondaryCallback);
        }

        /// <summary>
        /// Converts the param pointers to corresponding managed types 
        /// and the callback pointer to delegate Action<NSError> and invoke it.
        /// This callback is to be invoked by native code.
        /// </summary>
        /// <param name="error">Error.</param>
        /// <param name="secondaryCallback">Secondary callback.</param>
        [MonoPInvokeCallback(typeof(C.ErrorOnlyCallback))]
        private static void InternalErrorOnlyCallback(IntPtr error, IntPtr secondaryCallback)
        {
            if (PInvokeUtil.IsNull(secondaryCallback))
                return;
            
            NSError nsError = PInvokeUtil.IsNotNull(error) ? new NSError(error) : null;

            PInvokeCallbackUtil.PerformInternalCallback(
                "GKTurnBasedMatch#InternalErrorOnlyCallback",
                PInvokeCallbackUtil.Type.Temporary,
                nsError, secondaryCallback);
        }

        /// <summary>
        /// Converts the param pointers to corresponding managed types 
        /// and the callback pointer to delegate Action<byte[], NSError> and invoke it.
        /// This callback is to be invoked by native code.
        /// </summary>
        /// <param name="matchData">Match data.</param>
        /// <param name="error">Error.</param>
        /// <param name="secondaryCallback">Secondary callback.</param>
        [MonoPInvokeCallback(typeof(C.LoadMatchDataCallback))]
        private static void InternalLoadMatchDataCallback(IntPtr matchData, IntPtr error, IntPtr secondaryCallback)
        {
            if (PInvokeUtil.IsNull(secondaryCallback))
                return;
            
            byte[] data = null;

            if (PInvokeUtil.IsNotNull(matchData))
            {
                // Creating a one-time usage NSData binder, so no need to use the factory.
                using (var nsData = new NSData(matchData))
                {
                    var length = nsData.Length;
                    if (length > 0)
                        data = nsData.GetBytes(length);
                }
            }

            // A new NSError object is always created on native side, so no need
            // to check the binder pool for reusing an existing one.
            NSError nsError = PInvokeUtil.IsNotNull(error) ? new NSError(error) : null;

            PInvokeCallbackUtil.PerformInternalCallback(
                "GKTurnBasedMatch#InternalLoadMatchDataCallback",
                PInvokeCallbackUtil.Type.Temporary,
                data, nsError, secondaryCallback);
        }

        #endregion

        #region C wrapper

        private static class C
        {
            internal delegate void MultipleMatchesCallback(
            /* InteropNSArray<InteropGKTurnBasedMatch> */ IntPtr matches,
            /* InteropNSError */ IntPtr error,
                IntPtr secondaryCallbackPtr);

            internal delegate void SingleMatchCallback(
            /* InteropGKTurnBasedMatch */IntPtr match,
            /* InteropNSError */IntPtr error,
                IntPtr secondaryCallback);

            internal delegate void ErrorOnlyCallback(
            /* InteropNSError */IntPtr error,
                IntPtr secondaryCallback);

            internal delegate void LoadMatchDataCallback(
            /* InteropNSData */IntPtr matchData,
            /* InteropNSError */IntPtr error,
                IntPtr secondaryCallback);

            // Retrieving Existing Matches.
            [DllImport("__Internal")]
            internal static extern void GKTurnBasedMatch_loadMatches(
                MultipleMatchesCallback callback, 
                IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern void GKTurnBasedMatch_loadMatchWithID(
                string matchID, 
                SingleMatchCallback callback, 
                IntPtr secondaryCallback);

            // Creating a New Match.
            [DllImport("__Internal")]
            internal static extern void GKTurnBasedMatch_findMatchForRequest(
                /* InteropGKMatchRequest */ IntPtr requestPointer, 
                                            SingleMatchCallback callback, 
                                            IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern void GKTurnBasedMatch_acceptInvite(
                HandleRef self, 
                SingleMatchCallback callback, 
                IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern void GKTurnBasedMatch_declineInvite(
                HandleRef self, 
                ErrorOnlyCallback callback, 
                IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern void GKTurnBasedMatch_rematch(
                HandleRef self, 
                SingleMatchCallback callback, 
                IntPtr secondaryCallback);

            // Retrieving Information About the Match.

            [DllImport("__Internal")]
            internal static extern /* InteropNSArray<GKTurnBasedParticipant> */ IntPtr GKTurnBasedMatch_participants(HandleRef self);

            [DllImport("__Internal")]
            internal static extern IntPtr /* from(InteropGKTurnBasedParticipant) */ 
            GKTurnBasedMatch_currentParticipant(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* InteropNSData */ IntPtr GKTurnBasedMatch_matchData(HandleRef self);

            [DllImport("__Internal")]
            internal static extern uint /* from(uint32_t) */ GKTurnBasedMatch_matchDataMaximumSize(HandleRef self);

            [DllImport("__Internal")]
            internal static extern int GKTurnBasedMatch_message(
                HandleRef self, 
                [Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern void GKTurnBasedMatch_setMessage(
                HandleRef self, 
                string /* from(const char*) */ message);

            [DllImport("__Internal")]
            internal static extern long /* from(int64_t) */ GKTurnBasedMatch_creationDate(HandleRef self);

            [DllImport("__Internal")]
            internal static extern int GKTurnBasedMatch_matchID(
                HandleRef self, 
                [Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern GKTurnBasedMatchStatus GKTurnBasedMatch_status(HandleRef self);

            // Retrieving the Match’s Custom Data.
            [DllImport("__Internal")]
            internal static extern void GKTurnBasedMatch_loadMatchData(
                HandleRef self, LoadMatchDataCallback callback, IntPtr secondaryCallback);

            // Handling the Current Player’s Turn.
            [DllImport("__Internal")]
            internal static extern void GKTurnBasedMatch_saveCurrentTurn(
                HandleRef self,
                byte[] matchData, int matchDataLength,
                ErrorOnlyCallback callback, IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern void GKTurnBasedMatch_endTurn(
                HandleRef self,
                /* InteropGKTurnBasedParticipant[] */IntPtr[] nextParticipantPointers, int nextParticipantsCount,
                long timeout,
                byte[] matchData, int matchDataLength,
                ErrorOnlyCallback callback, IntPtr secondaryCallback);

            // Leaving a Match.
            [DllImport("__Internal")]
            internal static extern void GKTurnBasedMatch_participantQuitInTurn(
                HandleRef self,
                ref GKTurnBasedParticipant.GKTurnBasedMatchOutcome matchOutcome,
                /* InteropGKTurnBasedParticipant[] */IntPtr[] nextParticipantPointers, int nextParticipantsCount,
                long timeout,
                byte[] matchData, int matchDataLength,
                ErrorOnlyCallback callback, IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern void GKTurnBasedMatch_participantQuitOutOfTurn(
                HandleRef self,
                ref GKTurnBasedParticipant.GKTurnBasedMatchOutcome matchOutcome,
                ErrorOnlyCallback callback, IntPtr secondaryCallback);

            // Ending a Match.
            [DllImport("__Internal")]
            internal static extern void GKTurnBasedMatch_endMatchInTurn(
                HandleRef self,
                byte[] matchData, int matchDataLength,
                ErrorOnlyCallback callback, IntPtr secondaryCallback);

            // Deleting a Match from Game Center.
            [DllImport("__Internal")]
            internal static extern void GKTurnBasedMatch_remove(
                HandleRef self, 
                ErrorOnlyCallback callback, 
                IntPtr secondaryCallback);
        }

        #endregion
    }
}
#endif
