using UnityEngine;
using System.Collections;
using EasyMobile.Internal;

namespace EasyMobile
{
    public class AppLifecycleManager : MonoBehaviour
    {
        public static AppLifecycleManager Instance { get; private set; }

        private static IAppLifecycleHandler sAppLifecycleHandler = GetPlatformAppLifecycleHandler();

        #region MonoBehavior Events

        void Awake()
        {
            if (Instance != null)
                Destroy(this);
            else
                Instance = this;
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        void OnApplicationFocus(bool isFocus)
        {
            sAppLifecycleHandler.OnApplicationFocus(isFocus);
        }

        void OnApplicationPause(bool isPaused)
        {
            sAppLifecycleHandler.OnApplicationPause(isPaused);
        }

        void OnApplicationQuit()
        {
            sAppLifecycleHandler.OnApplicationQuit();
        }

        #endregion

        private static IAppLifecycleHandler GetPlatformAppLifecycleHandler()
        {
            #if UNITY_EDITOR
            return new DummyAppLifecycleHandler();
            #elif UNITY_IOS
            return new DummyAppLifecycleHandler();
            #elif UNITY_ANDROID
            return new AndroidAppLifecycleHandler();
            #else
            return new DummyAppLifecycleHandler();
            #endif
        }
    }
}