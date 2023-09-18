using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyMobile.Internal;

#if UNITY_IOS
using EasyMobile.iOS.GameKit;
using EasyMobile.Internal.GameServices.iOS;
#endif

#if UNITY_ANDROID && EM_GPGS && EM_OBSOLETE_GPGS
using GPGSTurnBasedMatch = GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch;
using EasyMobile.Internal.GameServices.Android;
#endif

namespace EasyMobile
{
    /// <summary>
    /// Represents a turn-based match. 
    /// </summary>
    public class TurnBasedMatch
    {
        /// <summary>
        /// Represents a turn-based match status.
        /// </summary>
        /// /// <remarks>
        /// The table below describes how this enum translates to the turn-based match status 
        /// of the underlaying Game Center and Google Play Games platforms.
        /// 
        /// Cross-Platform          GameKit                                         GPGS
        /// -----------------------------------------------------------------------------------------------------
        /// Unknown                 GKTurnBasedMatchStatusUnknown                   MatchStatus.Unknown
        /// Active                  GKTurnBasedMatchStatusOpen                      MatchStatus.Active
        /// Ended                   GKTurnBasedMatchStatusEnded                     MatchStatus.Complete
        /// Matching                GKTurnBasedMatchStatusMatching                  MatchStatus.AutoMatching 
        /// Cancelled               N/A                                             MatchStatus.Cancelled
        /// Expired                 N/A                                             MatchStatus.Expired
        /// Deleted                 N/A                                             MatchStatus.Deleted                          
        /// </remarks>
        public enum MatchStatus
        {
            /// <summary>
            /// The match status is unknown.
            /// </summary>
            Unknown,

            /// <summary>
            /// The match is currently being played.
            /// </summary>
            Active,

            /// <summary>
            /// The match has been completed.
            /// </summary>
            Ended,

            /// <summary>
            /// The match is searching for more auto-matched players to join.
            /// </summary>
            Matching,

            /// <summary>
            /// [Google Play Games only] The match was cancelled by one of the participants.
            /// </summary>
            /// <remarks>
            /// This status only exists on Google Play Games platform. A turn-based match
            /// never has this status on Game Center platform.
            /// </remarks>
            Cancelled,

            /// <summary>
            /// [Google Play Games only] The match expired.
            /// </summary>
            /// <remarks>
            /// This status only exists on Google Play Games platform. A turn-based match
            /// never has this status on Game Center platform.
            /// </remarks>
            Expired,

            /// <summary>
            /// [Google Play Games only] The match was deleted.
            /// </summary>
            /// <remarks>
            /// This status only exists on Google Play Games platform. A turn-based match
            /// never has this status on Game Center platform.
            /// </remarks>
            Deleted
        }

        private string mMatchId;
        private int mPlayerCount;
        private byte[] mData;
        private string mSelfParticipantId;
        private Participant[] mParticipants;
        private string mCurrentParticipantId;
        private MatchStatus mMatchStatus;

        /// <summary>
        /// The match identifier.
        /// </summary>
        /// <value>The match identifier.</value>
        public string MatchId
        {
            get
            {
                return mMatchId;
            }
        }

        /// <summary>
        /// The total number of players required for this match.
        /// </summary>
        /// <value>The player count.</value>
        public int PlayerCount
        {
            get
            {
                return mPlayerCount;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this match has at least one slot to be filled by a participant.
        /// </summary>
        /// <value><c>true</c> if this match has at least one vacant slot; otherwise, <c>false</c>.</value>
        public bool HasVacantSlot
        {
            get
            {
                return Participants.Length < PlayerCount;
            }
        }

        /// <summary>
        /// Returns true if the current turn belongs to the local player.
        /// </summary>
        /// <value><c>true</c> if this instance is my turn; otherwise, <c>false</c>.</value>
        public bool IsMyTurn
        {
            get
            {
                return SelfParticipantId.Equals(CurrentParticipantId);
            }
        }

        /// <summary>
        /// The data associated with the match. The meaning of this data is defined by the game.
        /// When the match has just been created, this value is <c>null</c>.
        /// </summary>
        /// <value>The data.</value>
        public byte[] Data
        {
            get
            {
                return mData;
            }
        }

        /// <summary>
        /// The ID of the participant that represents the local player.
        /// </summary>
        /// <value>The self participant identifier.</value>
        public string SelfParticipantId
        {
            get
            {
                return mSelfParticipantId;
            }
        }

        /// <summary>
        /// The participant that represents the local player in the match.
        /// </summary>
        /// <value>The self.</value>
        public Participant Self
        {
            get
            {
                return GetParticipant(mSelfParticipantId);
            }
        }

        /// <summary>
        /// The participants of the match, i.e. the players who actually joined the match
        /// (on Google Play Games platform, this also includes the players who are invited
        /// but haven't joined the match yet). It doesn't include participants who
        /// are being auto-matched (whom will be added when they actually joined the match).
        /// </summary>
        /// <value>The participants.</value>
        public Participant[] Participants
        {
            get
            {
                return mParticipants;
            }
        }

        /// <summary>
        /// The ID of the participant who holds the current turn.
        /// </summary>
        /// <value>The current participant identifier.</value>
        public string CurrentParticipantId
        {
            get
            {
                return mCurrentParticipantId;
            }
        }

        /// <summary>
        /// The participant who holds the current turn.
        /// </summary>
        /// <value>The current participant.</value>
        public Participant CurrentParticipant
        {
            get
            {
                return mCurrentParticipantId == null ? null :
                        GetParticipant(mCurrentParticipantId);
            }
        }

        /// <summary>
        /// The match status.
        /// </summary>
        /// <value>The status.</value>
        public MatchStatus Status
        {
            get
            {
                return mMatchStatus;
            }
        }

        /// <summary>
        /// Gets a participant by ID. Returns null if not found.
        /// </summary>
        /// <returns>The participant.</returns>
        /// <param name="participantId">Participant identifier.</param>
        public Participant GetParticipant(string participantId)
        {
            foreach (Participant p in mParticipants)
            {
                if (p.ParticipantId.Equals(participantId))
                {
                    return p;
                }
            }

            Debug.LogWarning("Participant not found in turn-based match: " + participantId);
            return null;
        }

        public override string ToString()
        {
            return string.Format("[TurnBasedMatch: mMatchId={0}, mData={1}, " +
                "mSelfParticipantId={2}, mParticipants={3}, mCurrentParticipantId={4}, " +
                "mMatchStatus={5}, mPlayerCount={6}]",
                mMatchId,
                mData,
                mSelfParticipantId,
                string.Join(",", mParticipants.Select(p => p.ToString()).ToArray()),
                mCurrentParticipantId,
                mMatchStatus,
                mPlayerCount);
        }

        // Private constructor: cross-platform matches should only created from GKTurnBasedMatch
        // or GPG TurnBasedMatch using the factory methods.
        protected TurnBasedMatch(string matchId, int playerCount, byte[] data,
                                 string selfParticipantId, Participant[] participants,
                                 string currentParticipantId, MatchStatus matchStatus)
        {
            mMatchId = matchId;
            mPlayerCount = playerCount;
            mData = data;
            mSelfParticipantId = selfParticipantId;
            mParticipants = participants;
            mCurrentParticipantId = currentParticipantId;
            mMatchStatus = matchStatus;
        }

#if UNITY_IOS
        
        /// <summary>
        /// The <see cref="EasyMobile.iOS.GameKit.GKTurnBasedMatch"/> used to create this object.
        /// </summary>
        /// <value>The G c turn based match.</value>
        internal GKTurnBasedMatch GC_TurnBasedMatch { get; private set; }

        /// <summary>
        /// Construct a new instance from <see cref="EasyMobile.iOS.GameKit.GKTurnBasedMatch"/> object.
        /// </summary>
        /// <returns>The GK turn based match.</returns>
        /// <param name="turnBasedMatch">Turn based match.</param>
        internal static TurnBasedMatch FromGKTurnBasedMatch(GKTurnBasedMatch turnBasedMatch)
        {
            if (turnBasedMatch == null)
                return null;

            // Cache values to save on calls to native side.
            var currentGKParticipant = turnBasedMatch.CurrentParticipant;
            var nsParticipantsArray = turnBasedMatch.Participants;
            var nsMatchData = turnBasedMatch.MatchData;

            var currentGKPlayer = currentGKParticipant != null ? currentGKParticipant.Player : null;
            var allParticipants = nsParticipantsArray == null ? null : nsParticipantsArray.ToArray(
                                      ptr => InteropObjectFactory<GKTurnBasedParticipant>.FromPointer(ptr, p => new GKTurnBasedParticipant(p)));
            var matchData = nsMatchData != null && nsMatchData.Length > 0 ? nsMatchData.ToBytes() : null;   // if data length == 0 we also return null to have a consistent behaviour between GC & GPG.

            return new TurnBasedMatch(
                turnBasedMatch.MatchID,
                allParticipants == null ? 0 : allParticipants.Length,
                matchData,
                GKLocalPlayer.LocalPlayer == null ? null : GKLocalPlayer.LocalPlayer.PlayerID,  // GKTurnBasedMatch doesn't have 'SelfParticipantId', using the equivalent local player's ID
                allParticipants == null ? null : allParticipants.Where(p => p.Player != null).Select(p => Participant.FromGKTurnBasedParticipant(p)).ToArray(), // only returns participants who have joined
                currentGKPlayer == null ? null : currentGKPlayer.PlayerID,
                turnBasedMatch.Status.ToTurnBasedMatchStatus())
            {
                GC_TurnBasedMatch = turnBasedMatch
            };
        }

#endif

#if UNITY_ANDROID && EM_GPGS && EM_OBSOLETE_GPGS

        /// <summary>
        /// The <see cref="GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch"/> used to create this object.
        /// </summary>
        internal GPGSTurnBasedMatch GPGS_TurnBasedMatch { get; private set; }

        /// <summary>
        /// Construct new instance from <see cref="GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch"/> object.
        /// </summary>
        internal static TurnBasedMatch FromGPGSTurnBasedMatch(GPGSTurnBasedMatch turnBasedMatch)
        {
            if (turnBasedMatch == null)
                return null;

            return new TurnBasedMatch(
                turnBasedMatch.MatchId,
                turnBasedMatch.Participants.Count + (int)turnBasedMatch.AvailableAutomatchSlots, // total players = joined/invited players + automatch players
                turnBasedMatch.Data,
                turnBasedMatch.SelfParticipantId,
                turnBasedMatch.Participants.Select(p => Participant.FromGPGSParticipant(p)).ToArray(),
                turnBasedMatch.PendingParticipantId,
                turnBasedMatch.Status.ToEMMatchStatus())
            {
                GPGS_TurnBasedMatch = turnBasedMatch
            };
        }

#endif
    }
}
