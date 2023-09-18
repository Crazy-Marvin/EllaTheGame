namespace Yodo1.MAS
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Collections.Generic;
    using UnityEditor.Build;
    using Google;

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
            // Delay initialization until the build target is Android and the editor is not in play
            // mode.
            EditorInitializer.InitializeOnMainThread(condition: () =>
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
            RunOnMainThread.OnUpdate -= PollBundleId;
            RunOnMainThread.OnUpdate += PollBundleId;
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
                Dictionary<string, object> data = GetAppInfo("android", bundleId);
                UpdateData(settings, data);
#endif
#if UNITY_IOS || UNITY_IPHONE
                string bundleId = currentValue;
                Dictionary<string, object> data = GetAppInfo("iOS", bundleId);
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
            bundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            Dictionary<string, object> androidData = GetAppInfo("android", bundleId);
            UpdateData(settings, androidData);

            bundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
            Dictionary<string, object> iosData = GetAppInfo("iOS", bundleId);
            UpdateData(settings, iosData);
        }

        private static void UpdateData(Yodo1AdSettings settings, Dictionary<string, object> dic)
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

        #region Get app-key by bundle-id

        private static Dictionary<string, object> GetAppInfo(string platform, string bundleId)
        {
            string url = "https://sdk.mas.yodo1.com/v1/unity/get-app-info-by-bundle-id";
            string secretKey = "W4OJaaOVAmO2uGaUVCCw24cuHKu4Zc";
            string timestamp = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000) + "";
            string sign = Md5(secretKey + timestamp + bundleId).ToLower();

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("timestamp", timestamp);
            dic.Add("bundle_id", bundleId);
            dic.Add("plugin_version", Yodo1AdUtils.GetPluginVersion());
            dic.Add("platform", platform);
            dic.Add("sign", sign);

            string response = HttpPost(url, Yodo1JSON.Serialize(dic));
            Dictionary<string, object> obj = (Dictionary<string, object>)Yodo1JSON.Deserialize(response);
            return obj;
        }

        #endregion

        private static string HttpPost(string url, string json)
        {
            try
            {
                Debug.Log(Yodo1U3dMas.TAG + "HttpPost request - " + json);

                // Send Request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "POST";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }

                // Get Response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response != null)
                {
                    Debug.Log(Yodo1U3dMas.TAG + "Response.StatusCode: " + response.StatusCode);
                }
                else
                {
                    Debug.Log(Yodo1U3dMas.TAG + "Response is null");
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string result = reader.ReadToEnd();
                Debug.Log(Yodo1U3dMas.TAG + "HttpPost result - " + result);

                reader.Close();
                return result;
            }
            catch (WebException e)
            {
                Debug.LogWarning(Yodo1U3dMas.TAG + "HttpPost Exception.Status - " + e.Status + ", please check your bundle id or package name.");
                return string.Empty;
            }
        }

        private static string Md5(string strToEncryppt)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] bs = UTF8Encoding.UTF8.GetBytes(strToEncryppt);
                byte[] hashBytes = md5.ComputeHash(bs);
                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
