using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

#if UNITY_ANDROID && EM_GPGS && EM_OBSOLETE_GPGS
using GPGS_MatchOutcome = GooglePlayGames.BasicApi.Multiplayer.MatchOutcome;
#endif

namespace EasyMobile
{
    /// <summary>
    /// Represents the outcome of a multiplayer match (who won, who lost, participants' placements, etc.).
    /// </summary>
    [Serializable]
    public class MatchOutcome
    {
        public enum ParticipantResult
        {
            /// <summary>
            /// The participant has a custom placement. 
            /// Do not set this result directly, instead use <see cref="SetParticipantPlacement"/>.
            /// If a participant has this result and a placement equal <see cref="PlacementUnset"/>
            /// at the same time, it's an indicator that he or she has quit the match.
            /// </summary>
            CustomPlacement = -1,

            /// <summary>
            /// The participant’s outcome has not been set yet 
            /// (typically because the match is still in progress).
            /// </summary>
            None = 0,

            /// <summary>
            /// The participant won the match.
            /// </summary>
            Won = 1,

            /// <summary>
            /// The participant lost the match.
            /// </summary>
            Lost = 2,

            /// <summary>
            /// The participant tied the match.
            /// </summary>
            Tied = 3
        }

        public const uint PlacementUnset = 0;

        private List<string> mParticipantIds = new List<string>();
        private Dictionary<string, uint> mPlacements = new Dictionary<string, uint>();
        private Dictionary<string, ParticipantResult> mResults = new Dictionary<string, ParticipantResult>();

        public MatchOutcome()
        {
        }

        /// <summary>
        /// Sets a result for the given participant. Do not call this method with a
        /// <see cref="ParticipantResult.CustomPlacement"/> result, instead use 
        /// <see cref="SetParticipantPlacement"/> if you want to give the participant
        /// a result with a custom placement.
        /// </summary>
        public void SetParticipantResult(string participantId, ParticipantResult result)
        {
            if (result == ParticipantResult.CustomPlacement)
                Debug.Log("Do not set ParticipantResult.CustomPlacement directly. Use SetParticipantPlacement method instead.");

            SetParticipantResultAndPlacement(participantId, result, PlacementUnset);
        }

        /// <summary>
        /// Sets a placement for the given participant, whose result will be set to
        /// <see cref="ParticipantResult.CustomPlacement"/> automatically.
        /// </summary>
        /// <param name="participantId">Participant identifier.</param>
        /// <param name="placement">Participant's placement, must be a value from 1 to 65535 (inclusive).</param>
        public void SetParticipantPlacement(string participantId, uint placement)
        {
            SetParticipantResultAndPlacement(participantId, ParticipantResult.CustomPlacement, placement);
        }

        // Sets a result and placement for the given participant.
        private void SetParticipantResultAndPlacement(string participantId, ParticipantResult result, uint placement)
        {
            if (!mParticipantIds.Contains(participantId))
            {
                mParticipantIds.Add(participantId);
            }

            mPlacements[participantId] = placement;
            mResults[participantId] = result;
        }

        /// <summary>
        /// The list of the participant IDs added to this match outcome.
        /// </summary>
        /// <value>The participant ids.</value>
        public List<string> ParticipantIds
        {
            get
            {
                return mParticipantIds;
            }
        }

        /// <summary>
        /// Returns the result for the given participant ID.
        /// </summary>
        /// <returns>The result for.</returns>
        /// <param name="participantId">Participant identifier.</param>
        public ParticipantResult GetParticipantResult(string participantId)
        {
            return mResults.ContainsKey(participantId) ? mResults[participantId] :
                                ParticipantResult.None;
        }

        /// <summary>
        /// Returns the placement for the given participant ID.
        /// </summary>
        /// <returns>The placement for.</returns>
        /// <param name="participantId">Participant identifier.</param>
        public uint GetParticipantPlacement(string participantId)
        {
            return mPlacements.ContainsKey(participantId) ? mPlacements[participantId] :
                                PlacementUnset;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[MatchOutcome");
            foreach (string pid in mParticipantIds)
            {
                sb.Append(string.Format(" {0}->({1},{2})", pid,
                        GetParticipantResult(pid), GetParticipantPlacement(pid)));
            }

            return sb.Append("]").ToString();
        }

#if UNITY_ANDROID && EM_GPGS && EM_OBSOLETE_GPGS

        /// <summary>
        /// Construct new <see cref="GooglePlayGames.BasicApi.Multiplayer.MatchOutcome"/> based on this object.
        /// </summary>
        public GPGS_MatchOutcome ToGPGSMatchOutcome()
        {
            var outcome = new GPGS_MatchOutcome();
            foreach (string id in ParticipantIds)
            {
                outcome.SetParticipantResult(id, mResults[id].ToGPGSParticipantResult(), mPlacements[id]);
            }
            return outcome;
        }

#endif
    }

    public static class ParticipantResultExtension
    {
#if UNITY_ANDROID && EM_GPGS && EM_OBSOLETE_GPGS

        /// <summary>
        /// Convert from <see cref="MatchOutcome.ParticipantResult"/>
        /// to <see cref="GooglePlayGames.BasicApi.Multiplayer.MatchOutcome.ParticipantResult"/>.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static GPGS_MatchOutcome.ParticipantResult ToGPGSParticipantResult(this MatchOutcome.ParticipantResult result)
        {
            switch (result)
            {
                case MatchOutcome.ParticipantResult.Won:
                    return GPGS_MatchOutcome.ParticipantResult.Win;

                case MatchOutcome.ParticipantResult.Lost:
                    return GPGS_MatchOutcome.ParticipantResult.Loss;

                case MatchOutcome.ParticipantResult.Tied:
                    return GPGS_MatchOutcome.ParticipantResult.Tie;

                case MatchOutcome.ParticipantResult.None:
                case MatchOutcome.ParticipantResult.CustomPlacement: // CustomPlacement is for iOS only. So this won't happen anw.
                default:
                    return GPGS_MatchOutcome.ParticipantResult.None;
            }
        }

#endif
    }
}
