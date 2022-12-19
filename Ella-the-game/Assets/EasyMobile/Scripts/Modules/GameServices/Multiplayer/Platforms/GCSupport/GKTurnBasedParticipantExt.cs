#if UNITY_IOS
using UnityEngine;
using System.Collections;
using EasyMobile.iOS.GameKit;

namespace EasyMobile.Internal.GameServices.iOS
{
    internal static class GKTurnBasedParticipantExt
    {
        public static bool IsConnectedToRoom(this GKTurnBasedParticipant participant)
        {
            return participant.Status == GKTurnBasedParticipant.GKTurnBasedParticipantStatus.Active;
        }
    }
}
#endif