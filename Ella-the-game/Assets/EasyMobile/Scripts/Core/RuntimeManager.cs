using UnityEngine;
using System.Collections;
using System;
using EasyMobile.Internal;

namespace EasyMobile
{
    public static class RuntimeManager
    {
        private const string APP_INSTALLATION_TIMESTAMP_PPKEY = "EM_APP_INSTALLATION_TIMESTAMP";

        private static bool mIsInitialized = false;

        #region Public API

        /// <summary>
        /// Occurs when the Easy Mobile runtime has been initialized,
        /// when its modules are ready to use.
        /// </summary>
        public static event Action Initialized;

        /// <summary>
        /// Initializes the Easy Mobile runtime. Always do this before
        /// accessing Easy Mobile API. It's recommended to initialize as 
        /// early as possible, ideally as soon as the app launches. This
        /// method is a no-op if the runtime has been initialized before, so it's
        /// safe to be called multiple times. This method must be called on 
        /// the main thread.
        /// </summary>
        public static void Init()
        {
            if (mIsInitialized)
            {
                return;
            }

            if (Application.isPlaying)
            {
                // Initialize runtime Helper.
                RuntimeHelper.Init();

                // Add an 'EasyMobile' game object to the current scene
                // which is the main object responsible for all runtime
                // activities of EM.
                var go = new GameObject("EasyMobile");
                Configure(go);

                // Store the timestamp of the *first* init which can be used 
                // as a rough approximation of the installation time.
                if (StorageUtil.GetTime(APP_INSTALLATION_TIMESTAMP_PPKEY, Util.UnixEpoch) == Util.UnixEpoch)
                    StorageUtil.SetTime(APP_INSTALLATION_TIMESTAMP_PPKEY, DateTime.Now);

                // Done init.
                mIsInitialized = true;

                // Raise the event.
                if (Initialized != null)
                    Initialized();

                Debug.Log("Easy Mobile runtime has been initialized.");
            }
        }

        /// <summary>
        /// Determines whether the Easy Mobile runtime has been initialized or not.
        /// </summary>
        /// <value><c>true</c> if is initialized; otherwise, <c>false</c>.</value>
        public static bool IsInitialized()
        {
            return mIsInitialized;
        }

        /// <summary>
        /// Gets the installation timestamp of this app in local timezone.
        /// This timestamp is recorded when th Easy Mobile runtime is initialized for
        /// the first time so it's not really precise but can serve well as a rough approximation
        /// provided that the initialization is done soon after app launch.
        /// </summary>
        /// <returns>The installation timestamp.</returns>
        public static DateTime GetAppInstallationTimestamp()
        {
            return StorageUtil.GetTime(APP_INSTALLATION_TIMESTAMP_PPKEY, Util.UnixEpoch);
        }

        /// <summary>
        /// Enables or disables Unity debug log.
        /// </summary>
        /// <param name="isEnabled">If set to <c>true</c> is enabled.</param>
        public static void EnableUnityDebugLog(bool isEnabled)
        {
            #if UNITY_2017_1_OR_NEWER
            Debug.unityLogger.logEnabled = isEnabled;
            #else
            Debug.logger.logEnabled = isEnabled;
            #endif
        }

        #endregion

        #region Internal Stuff
        
        //Auto initialization
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoInitialize()
        {
            if(EM_Settings.IsRuntimeAutoInitializationEnabled)
                Init();
        }

        // Adds the required components necessary for the runtime operation of EM modules
        // to the game object this instance is attached to.
        private static void Configure(GameObject go)
        {
            // This game object must prevail.
            GameObject.DontDestroyOnLoad(go);

            // App lifecycle manager.
            go.AddComponent<AppLifecycleManager>();

            // Advertising module.
            if (EM_Settings.IsAdModuleEnable)
                go.AddComponent<Advertising>();

            // IAP module.
            if (EM_Settings.IsIAPModuleEnable)
                go.AddComponent<InAppPurchasing>();

            // Game Services module.
            if (EM_Settings.IsGameServicesModuleEnable)
                go.AddComponent<GameServices>();

            // Notifications module.
            if (EM_Settings.IsNotificationsModuleEnable)
                go.AddComponent<Notifications>();
        }

        #endregion
    }
}

