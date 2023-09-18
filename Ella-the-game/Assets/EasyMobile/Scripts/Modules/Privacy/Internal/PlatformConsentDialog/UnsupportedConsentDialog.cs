using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EasyMobile.Internal.Privacy
{
    internal class UnsupportedConsentDialog : IPlatformConsentDialog
    {
        private const string UNSUPPORTED_MSG = "Consent dialog is not supported on this platform.";

        //--------------------------------------------------------
        // IPlatformConsentDialog implementation
        //--------------------------------------------------------

        #pragma warning disable 0067
        public event Action<IPlatformConsentDialog, string, bool> ToggleStateUpdated;
        public event Action<IPlatformConsentDialog, string, Dictionary<string, bool>> Completed;
        public event Action<IPlatformConsentDialog> Dismissed;
        #pragma warning restore 0067

        public bool IsShowing()
        {
            return false;
        }

        public void Show(string title, string content, bool isDimissible)
        {
            Debug.Log(UNSUPPORTED_MSG);
        }

        public void SetButtonInteractable(string buttonId, bool interactble)
        {
            Debug.Log(UNSUPPORTED_MSG);
        }

        public void SetToggleIsOn(string toggleId, bool isOn, bool animated)
        {
            Debug.Log(UNSUPPORTED_MSG);
        }

        public void SetToggleInteractable(string toggleId, bool interactable)
        {
            Debug.Log(UNSUPPORTED_MSG);
        }
    }
}