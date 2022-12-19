#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System;

namespace EasyMobile.Internal.GameServices
{
    internal class EditorTurnBasedMultiplayerClient : UnsupportedTurnBasedMultiplayerClient
    {
        protected override string mUnavailableMessage
        {
            get{ return "Please test Turn-Based multiplayer functionalities on an iOS or Android device."; }
        }
    }
}
#endif