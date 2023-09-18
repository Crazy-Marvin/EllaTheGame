using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace EasyMobile.Internal.Privacy
{
    internal interface IPlatformConsentDialog
    {
        event Action<IPlatformConsentDialog, string, bool> ToggleStateUpdated;
        event Action<IPlatformConsentDialog, string, Dictionary<string, bool>> Completed;
        event Action<IPlatformConsentDialog> Dismissed;

        bool IsShowing();

        void Show(string title, string content, bool isDismissible);

        void SetButtonInteractable(string buttonId, bool interactable);

        void SetToggleInteractable(string toggleId, bool interactable);

        void SetToggleIsOn(string toggleId, bool isOn, bool animated);
    }
}