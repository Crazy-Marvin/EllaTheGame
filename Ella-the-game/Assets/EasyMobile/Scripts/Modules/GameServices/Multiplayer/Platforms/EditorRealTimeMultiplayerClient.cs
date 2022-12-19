using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile.Internal.GameServices
{
    internal class EditorRealTimeMultiplayerClient : UnsupportedRealTimeMultiplayerClient
    {
        protected override string mUnavailableMessage
        {
            get { return "Please test Real-Time multiplayer functionalities on an iOS or Android device."; }
        }
    }
}
