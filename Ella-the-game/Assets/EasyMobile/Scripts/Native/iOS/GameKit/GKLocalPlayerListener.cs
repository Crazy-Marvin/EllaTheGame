#if UNITY_IOS
using System;

namespace EasyMobile.iOS.GameKit
{
    /// <summary>
    /// A protocol that handles events for Game Center accounts.
    /// </summary>
    internal interface GKLocalPlayerListener : GKInviteEventListener, GKTurnBasedEventListener
    {
    }
}
#endif
