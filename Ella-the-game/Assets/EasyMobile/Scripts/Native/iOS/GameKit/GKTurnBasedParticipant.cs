#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;

namespace EasyMobile.iOS.GameKit
{
    /// <summary>
    /// The information that describes a participant in a turn-based match.
    /// </summary>
    internal class GKTurnBasedParticipant : iOSObjectProxy
    {
        /// <summary>
        /// The state the participant is in during the match.
        /// </summary>
        public enum GKTurnBasedParticipantStatus
        {
            /// <summary>
            /// The participant is in an unexpected state.
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// The participant was invited to the match, but has not responded to the invitation.
            /// </summary>
            Invited,
            /// <summary>
            /// The participant declined the invitation to join the match. 
            /// When a participant declines an invitation to join a match, the match is automatically terminated.
            /// </summary>
            Declined,
            /// <summary>
            /// The participant represents an unfilled position in the match that Game Center 
            /// promises to fill when needed. When you make this participant the next person 
            /// to take a turn in the match, Game Center fills the position and updates the status and playerID properties.
            /// </summary>
            Matching,
            /// <summary>
            /// The participant has joined the match and is an active player in it.
            /// </summary>
            Active,
            /// <summary>
            /// The participant has exited the match. Your game sets the <see cref="MatchOutcome"/> property 
            /// to state why the participant left the match.
            /// </summary>
            Done
        }

        /// <summary>
        /// The state the participant was in when they left the match.
        /// </summary>
        /// <remarks>
        /// Here we have to use struct instead of enum to be able to represent
        /// the possbile custom values of the native GKTurnBasedMatchOutcome.
        /// </remarks>
        public struct GKTurnBasedMatchOutcome
        {
            public enum Outcome
            {
                /// <summary>
                /// The participant’s outcome has not been set yet (typically because the match is still in progress).
                /// </summary>
                None = 0,
                /// <summary>
                /// The participant forfeited the match.
                /// </summary>
                Quit,
                /// <summary>
                /// The participant won the match.
                /// </summary>
                Won,
                /// <summary>
                /// The participant lost the match.
                /// </summary>
                Lost,
                /// <summary>
                /// The participant tied the match.
                /// </summary>
                Tied,
                /// <summary>
                /// The participant was ejected from the match because he or she did not act in a timely fashion.
                /// </summary>
                TimeExpired,
                /// <summary>
                /// The participant finished first.
                /// </summary>
                First,
                /// <summary>
                /// The participant finished second.
                /// </summary>
                Second,
                /// <summary>
                /// The participant finished third.
                /// </summary>
                Third,
                /// <summary>
                /// The participant finished fourth.
                /// </summary>
                Fourth,
                /// <summary>
                /// The participant has a custom outcome.
                /// </summary>
                Custom
            }

            /// <summary>
            /// A mask used to allow your game to provide its own custom outcome. 
            /// Any custom value must fit inside the mask.
            /// </summary>
            private const uint GKTurnBasedMatchOutcomeCustomRange = 0x00FF0000;

            /// <summary>
            /// The participant's outcome. If this equals <see cref="Outcome.Custom"/>,
            /// read <see cref="customOutcome"/> to obtain the custom value.
            /// Likewise, if you need to set a custom outcome, set this to <see cref="Outcome.Custom"/>
            /// and then set <see cref="customOutcome"/> to an appropriate custom value.
            /// </summary>
            public Outcome outcome;

            /// <summary>
            /// The participant's custom outcome value, must be between <c>0x0001</c> and <c>0xFFFF</c>.
            /// </summary>
            public uint customOutcome;
        }

        internal GKTurnBasedParticipant(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// The date and time that this participant last took a turn in the game.
        /// </summary>
        /// <value>The last turn date.</value>
        public DateTime LastTurnDate
        {
            get { return Util.FromMillisSinceUnixEpoch(C.GKTurnBasedParticipant_lastTurnDate(SelfPtr())); }
        }

        /// <summary>
        /// The <see cref="EasyMobile.iOS.GameKit.GKPlayer"/> object that identifies this participant.
        /// </summary>
        /// <value>The player.</value>
        public GKPlayer Player
        {
            get
            { 
                var pointer = C.GKTurnBasedParticipant_player(SelfPtr());
                var player = InteropObjectFactory<GKPlayer>.FromPointer(pointer, ptr => new GKPlayer(ptr));
                CoreFoundation.CFFunctions.CFRelease(pointer);
                return player;
            }
        }

        /// <summary>
        /// The current status of the participant.
        /// </summary>
        /// <value>The status.</value>
        public GKTurnBasedParticipantStatus Status
        {
            get { return C.GKTurnBasedParticipant_status(SelfPtr()); }
        }

        /// <summary>
        /// The date and time that the participant’s turn timed out.
        /// </summary>
        /// <value>The timeout date.</value>
        public DateTime TimeoutDate
        {
            get { return Util.FromMillisSinceUnixEpoch(C.GKTurnBasedParticipant_timeoutDate(SelfPtr())); }
        }

        /// <summary>
        /// The end-state of this participant in the match.
        /// </summary>
        /// <value>The match outcome.</value>
        public GKTurnBasedMatchOutcome MatchOutcome
        {
            get
            { 
                var outcomeBuffer = new GKTurnBasedMatchOutcome();
                C.GKTurnBasedParticipant_matchOutcome(SelfPtr(), ref outcomeBuffer); 
                return outcomeBuffer;
            }
            set
            { 
                C.GKTurnBasedParticipant_setMatchOutcome(SelfPtr(), ref value); 
            }
        }

        #region C wrapper

        private static class C
        {
            [DllImport("__Internal")]
            internal static extern long /* from(int64_t) */ GKTurnBasedParticipant_lastTurnDate(HandleRef self);

            [DllImport("__Internal")]
            internal static extern IntPtr /* from(InteropGKPlayer) */ GKTurnBasedParticipant_player(HandleRef self);

            [DllImport("__Internal")]
            internal static extern GKTurnBasedParticipantStatus GKTurnBasedParticipant_status(HandleRef self);

            [DllImport("__Internal")]
            internal static extern long /* from(int64_t) */ GKTurnBasedParticipant_timeoutDate(HandleRef self);

            [DllImport("__Internal")]
            internal static extern void GKTurnBasedParticipant_matchOutcome(HandleRef self, [In,Out] ref GKTurnBasedMatchOutcome matchOutcome);

            [DllImport("__Internal")]
            internal static extern void GKTurnBasedParticipant_setMatchOutcome(HandleRef self, ref GKTurnBasedMatchOutcome matchOutcome);

        }

        #endregion
    }
}
#endif
