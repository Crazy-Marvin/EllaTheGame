namespace Yodo1.MAS
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;
    using UnityEditor.Build;

    [InitializeOnLoad]
    public class Yodo1AdAssetsImporter : AssetPostprocessor
    {
        /// <summary>
        /// Polls a value and signals a callback with the change after the specified delay
        /// time.
        /// </summary>
        internal class PropertyPoller<T>
        {
            /// <summary>
            /// Delegate that is called when a value changes.
            /// </summary>
            /// <param name="previousValue">Previous value of the property that
            /// changed.</param>
            /// <param name="currentValue">Current value of the property that
            /// changed.</param>
            public delegate void Changed(T previousValue, T currentValue);

            // Whether the previous value has been initialized.
            private bool previousValueInitialized = false;
            // Previous value of the property.
            private T previousValue = default(T);
            // Previous value of the property when it was last polled.
            private T previousPollValue = default(T);
            // Last time the property was polled.
            private DateTime previousPollTime = DateTime.Now;
            // Time to wait before signalling a change.
            private int delayTimeInSeconds;
            // Name of the property being polled.
            private string propertyName;
            // Previous time we checked the property value for a change.
            private DateTime previousCheckTime = DateTime.Now;
            // Time to wait before checking a property.
            private int checkIntervalInSeconds;

            /// <summary>
            /// Create the poller.
            /// </summary>
            /// <param name="propertyName">Name of the property being polled.</param>
            /// <param name="delayTimeInSeconds">Time to wait before signalling that the value
            /// has changed.</param>
            /// <param name="checkIntervalInSeconds">Time to check the value of the property for
            /// changes.</param>
            public PropertyPoller(string propertyName,
                                  int delayTimeInSeconds = 3,
                                  int checkIntervalInSeconds = 1)
            {
                this.propertyName = propertyName;
                this.delayTimeInSeconds = delayTimeInSeconds;
                this.checkIntervalInSeconds = checkIntervalInSeconds;
            }

            /// <summary>
            /// Poll the specified value for changes.
            /// </summary>
            /// <param name="getCurrentValue">Delegate that returns the value being polled.</param>
            /// <param name="changed">Delegate that is called if the value changes.</param>
            public void Poll(Func<T> getCurrentValue, Changed changed)
            {
                var currentTime = DateTime.Now;
                if (currentTime.Subtract(previousCheckTime).TotalSeconds <
                    checkIntervalInSeconds)
                {
                    return;
                }
                previousCheckTime = currentTime;
                T currentValue = getCurrentValue();
                // If the poller isn't initailized, store the current value before polling for
                // changes.
                if (!previousValueInitialized)
                {
                    previousValueInitialized = true;
                    previousValue = currentValue;
                    return;
                }
                if (!currentValue.Equals(previousValue))
                {
                    if (currentValue.Equals(previousPollValue))
                    {
                        if (currentTime.Subtract(previousPollTime).TotalSeconds >=
                            delayTimeInSeconds)
                        {
                            Debug.Log(String.Format("{0} changed: {1} -> {2}", propertyName, previousValue, currentValue));
                            changed(previousValue, currentValue);
                            previousValue = currentValue;
                        }
                    }
                    else
                    {
                        previousPollValue = currentValue;
                        previousPollTime = currentTime;
                    }
                }
            }
        }
        /// <summary>
        /// Polls for changes in the bundle ID.
        /// </summary>
        private static PropertyPoller<string> bundleIdPoller = new PropertyPoller<string>(Yodo1U3dMas.TAG + "Bundle ID");

        static Yodo1AdAssetsImporter()
        {
            // Delay initialization until the editor is not in play mode.
            Google.EditorInitializer.InitializeOnMainThread(condition: () =>
            {
                return !EditorApplication.isPlayingOrWillChangePlaymode;
            }, initializer: Initialize, name: "Yodo1AdAssetsImporter", logger: null);
        }

        /// <summary>
        /// Initialize the module. This should be called on the main thread only if
        /// current active build target is Android and not in play mode.
        /// </summary>
        private static bool Initialize()
        {
            Google.RunOnMainThread.OnUpdate -= PollBundleId;
            Google.RunOnMainThread.OnUpdate += PollBundleId;
            return false;
        }

        /// <summary>
        /// If the user changes the bundle ID, perform resolution again.
        /// </summary>
        private static void PollBundleId()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return;
            }

            bundleIdPoller.Poll(() => PlayerSettings.applicationIdentifier, (previousValue, currentValue) =>
            {
                Yodo1AdSettings settings = Yodo1AdSettingsSave.Load();
#if UNITY_ANDROID
                string bundleId = currentValue;
                Dictionary<string, object> data = Yodo1Net.GetInstance().GetAppInfoByBundleID("android", bundleId);
                UpdateData(settings, data);
#endif
#if UNITY_IOS || UNITY_IPHONE
                string bundleId = currentValue;
                Dictionary<string, object> data = Yodo1Net.GetInstance().GetAppInfoByBundleID("iOS", bundleId);
                UpdateData(settings, data);
#endif
            });
        }

        // Allow an editor class method to be initialized when Unity loads without action from the user.
        // Will be called when the script file changes
        // Will be called when Unity is opened
        [InitializeOnLoadMethod]
        static void OnProjectLoadedInEditor()
        {
            EditorApplication.update += UpdateAppInfo;
            EditorApplication.update += UpdateAdNetworkAndDependencies;
        }

        static void UpdateAdNetworkAndDependencies()
        {
            EditorApplication.update -= UpdateAdNetworkAndDependencies;
            IntegrationManager.UpdateAdNetworkAndDependencies();
        }

        public static void UpdateAppInfo()
        {
            EditorApplication.update -= UpdateAppInfo;

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return;
            }

            Yodo1AdSettings settings = Yodo1AdSettingsSave.Load();

            string bundleId = string.Empty;
#if UNITY_2023_2_OR_NEWER
            bundleId = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android);
#else
            bundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
#endif
            Dictionary<string, object> androidData = Yodo1Net.GetInstance().GetAppInfoByBundleID("android", bundleId);
            UpdateData(settings, androidData);

#if UNITY_2023_2_OR_NEWER
            bundleId = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android);
#else
            bundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
#endif
            Dictionary<string, object> iosData = Yodo1Net.GetInstance().GetAppInfoByBundleID("iOS", bundleId);
            UpdateData(settings, iosData);
        }

        public static void UpdateData(Yodo1AdSettings settings, Dictionary<string, object> dic)
        {
            if (settings == null || dic == null)
            {
                return;
            }

            string appKey = string.Empty;
            string admobKey = string.Empty;
            string platform = string.Empty;
            string bundleId = string.Empty;
            if (dic.ContainsKey("platform"))
            {
                platform = (string)dic["platform"];
            }
            if (dic.ContainsKey("app_key"))
            {
                appKey = (string)dic["app_key"];
            }

            if (dic.ContainsKey("admob_key"))
            {
                admobKey = (string)dic["admob_key"];
            }

            if (dic.ContainsKey("bundle_id"))
            {
                bundleId = (string)dic["bundle_id"];
            }

            if (string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(admobKey) || string.IsNullOrEmpty(bundleId))
            {
                return;
            }
            if (platform == "ios" || platform == "iOS")
            {
                settings.iOSSettings.AppKey = appKey;
                settings.iOSSettings.AdmobAppID = admobKey;
                settings.iOSSettings.BundleID = bundleId;
            }
            else if (platform == "android")
            {
                settings.androidSettings.AppKey = appKey;
                settings.androidSettings.AdmobAppID = admobKey;
                settings.androidSettings.BundleID = bundleId;
            }
            Yodo1AdSettingsSave.Save(settings);
        }
    }
}
