#if UNITY_EDITOR || !UNITY_ANDROID
using UnityEngine;
using System.Collections;

namespace EasyMobile.Internal
{
    public class DummyAppLifecycleHandler : IAppLifecycleHandler
    {
        public void OnApplicationFocus(bool isFocus)
        {
        }

        public void OnApplicationPause(bool isPaused)
        {
        }

        public void OnApplicationQuit()
        {
        }
    }
}
#endif