namespace Yodo1.MAS
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Collections.Generic;
    using UnityEngine.Networking;
    using System.Collections;

    public class Yodo1AdWindows : EditorWindow
    {
        private static string app_key = string.Empty;
        private static string app_name = string.Empty;
        private static string app_bundle_id = string.Empty;
        private static string app_platform = string.Empty;
        private static string app_admob_key = string.Empty;
        private static string resultString = string.Empty;
        private bool IsIOSRunTime = false;
        private bool IsAndroidRunTime = false;
        Yodo1AdSettings adSettings;

        delegate void ApiCallback(string response);

        public string App_iOSKey
        {
            get
            {
                return this.adSettings.iOSSettings.AppKey;
            }
            set
            {
                if (this.adSettings.iOSSettings.AppKey == value)
                {
                    return;
                }
                this.adSettings.iOSSettings.AppKey = value;
                resultString = this.RequestAdmobConfig(value);
            }
        }

        public string App_AndroidKey
        {
            get
            {
                return this.adSettings.androidSettings.AppKey;
            }
            set
            {
                if (this.adSettings.androidSettings.AppKey == value)
                {
                    return;
                }
                this.adSettings.androidSettings.AppKey = value;
                resultString = this.RequestAdmobConfig(value);
            }
        }

        public enum PlatfromTab
        {
            Android,
            iOS
        }

        static PlatfromTab selectPlarformTab;
        Vector2 scrollPosition;

        static EditorWindow window;

        public Yodo1AdWindows()
        {
            selectPlarformTab = PlatfromTab.iOS;
        }

        public static void Initialize(PlatfromTab platfromTab)
        {
            if (window != null)
            {
                window.Close();
                window = null;
            }

            window = EditorWindow.GetWindow(typeof(Yodo1AdWindows), false, platfromTab.ToString() + " Setting", true);
            window.Show();
            selectPlarformTab = platfromTab;
        }

        #region cycle

        private void OnDisable()
        {
            this.SaveConfig();
            this.adSettings = null;
        }

        private void OnEnable()
        {
            Yodo1AdAssetsImporter.UpdateAppInfo();
            this.adSettings = Yodo1AdSettingsSave.Load();
        }

        private void OnGUI()
        {
            this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[0]);
            DrawContent();
            GUIStyle gUIStyle = new GUIStyle();
            gUIStyle.padding = new RectOffset(10, 10, 10, 0);
            GUILayout.BeginVertical(gUIStyle, new GUILayoutOption[0]);
            if (GUILayout.Button("Save Configuration"))
            {
                this.SaveConfig();
            }
            GUILayout.EndVertical();

            GUILayout.EndScrollView();
        }

        #endregion

        private void DrawContent()
        {
            GUIStyle gUIStyle = new GUIStyle
            {
                padding = new RectOffset(20, 10, 20, 0)
            };
            GUILayout.BeginVertical(gUIStyle, new GUILayoutOption[0]);


            GUIStyle gUIStyle2 = new GUIStyle();
            gUIStyle2.padding = new RectOffset(0, 10, 10, 0);

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(gUIStyle2, new GUILayoutOption[0]);

            //Set AppKey
            GUILayout.Label("MAS App Key", GUILayout.Width(120));
            if (selectPlarformTab == PlatfromTab.iOS)
            {
                this.App_iOSKey = GUILayout.TextField(this.App_iOSKey.Trim());
                if (!string.IsNullOrEmpty(this.App_iOSKey))
                {
                    if (!IsIOSRunTime && string.IsNullOrEmpty(this.adSettings.iOSSettings.AdmobAppID.Trim()))
                    {
                        resultString = RequestAdmobConfig(this.App_iOSKey);
                        IsIOSRunTime = true;
                    }
                }
            }
            else
            {
                this.App_AndroidKey = GUILayout.TextField(this.App_AndroidKey.Trim());
                if (!string.IsNullOrEmpty(this.App_AndroidKey))
                {
                    if (!IsAndroidRunTime && string.IsNullOrEmpty(this.adSettings.androidSettings.AdmobAppID.Trim()))
                    {
                        resultString = RequestAdmobConfig(this.adSettings.androidSettings.AppKey);
                        IsAndroidRunTime = true;
                    }
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (selectPlarformTab == PlatfromTab.iOS)
            {
                if (string.IsNullOrEmpty(this.adSettings.iOSSettings.AppKey.Trim()))
                {
                    EditorGUILayout.HelpBox("Please fill in the MAS app key correctly, you can find your app key on the MAS dashboard.", MessageType.Error);
                    GUILayout.Space(15);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(this.adSettings.androidSettings.AppKey.Trim()))
                {
                    EditorGUILayout.HelpBox("Please fill in the MAS app key correctly, you can find your app key on the MAS dashboard.", MessageType.Error);
                    GUILayout.Space(15);
                }
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(gUIStyle2, new GUILayoutOption[0]);

            string imagePath = Application.dataPath + "/Yodo1/MAS/Editor/refresh.png";
            FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            byte[] thebytes = new byte[fs.Length];
            fs.Read(thebytes, 0, (int)fs.Length);
            fs.Close();
            fs.Dispose();

            Texture2D image = new Texture2D(30, 30);
            image.LoadImage(thebytes);

            GUILayout.BeginHorizontal(gUIStyle2, new GUILayoutOption[0]);

            //Set AdMob App ID
            GUILayout.Label("AdMob App ID", GUILayout.Width(120));

            if (selectPlarformTab == PlatfromTab.iOS)
            {
                this.adSettings.iOSSettings.AdmobAppID = GUILayout.TextField(this.adSettings.iOSSettings.AdmobAppID);

                GUILayout.Space(20);
                if (GUILayout.Button("Refresh", GUILayout.Width(60)))
                {
                    resultString = RequestAdmobConfig(this.adSettings.iOSSettings.AppKey);
                    this.SaveConfig();
                }
            }
            else
            {
                this.adSettings.androidSettings.AdmobAppID = GUILayout.TextField(this.adSettings.androidSettings.AdmobAppID);

                GUILayout.Space(20);
                if (GUILayout.Button("Refresh", GUILayout.Width(60)))
                {
                    resultString = RequestAdmobConfig(this.adSettings.androidSettings.AppKey);
                    this.SaveConfig();
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (selectPlarformTab == PlatfromTab.iOS)
            {
                if (string.IsNullOrEmpty(this.adSettings.iOSSettings.AdmobAppID.Trim()))
                {
                    if (string.IsNullOrEmpty(resultString))
                    {
                        EditorGUILayout.HelpBox("A null or incorrect value will cause a crash when it builds. Please make sure to copy Admob App ID from MAS dashboard.", MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox(resultString, MessageType.Error);
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(this.adSettings.androidSettings.AdmobAppID.Trim()))
                {
                    if (string.IsNullOrEmpty(resultString))
                    {
                        EditorGUILayout.HelpBox("A null or incorrect value will cause a crash when it builds. Please make sure to copy Admob App ID from MAS dashboard.", MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox(resultString, MessageType.Error);
                    }
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        private void SaveConfig()
        {
            if (selectPlarformTab == PlatfromTab.Android)
            {

                if (Yodo1PostProcessAndroid.CheckConfiguration_Android(this.adSettings))
                {
#if UNITY_2019_1_OR_NEWER
#else
                    Yodo1PostProcessAndroid.ValidateManifest(this.adSettings);
                    Yodo1PostProcessAndroid.GenerateGradle();
#endif
                }
                else
                {
                    return;
                }
            }
            if (selectPlarformTab == PlatfromTab.iOS)
            {
                if (!Yodo1AdSettingsSave.CheckConfiguration_iOS(this.adSettings))
                {
                    return;
                }
            }

            Yodo1AdSettingsSave.Save(this.adSettings);
        }

        #region Get admob-key by app-key

        private string RequestAdmobConfig(string appKey)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return "Please check your network. You can also fill in manually.";
            }

            if (string.IsNullOrEmpty(appKey))
            {
                return "Please enter the correct MAS App Key.";
            }
            string result = string.Empty;
            string api = "https://sdk.mas.yodo1.com/v1/unity/setup/" + appKey;
#if UNITY_2018_1_OR_NEWER
            string response = HttpGet(api);
            Dictionary<string, object> obj = (Dictionary<string, object>)Yodo1JSON.Deserialize(response);
            Debug.Log(Yodo1U3dMas.TAG + "response:" + response);
            if (!this.GetAdMobKey(obj))
            {
                result = "MAS App Key not found. please fill in correctly.";
            }
#else
            ApiCallback callback = delegate (string response)
            {
                Dictionary<string, object> obj = (Dictionary<string, object>)Yodo1JSON.Deserialize(response);
                Debug.Log(Yodo1U3dMas.TAG + "response:" + response);
                if (!this.GetAdMobKey(obj))
                {
                    result = "MAS App Key not found. please fill in correctly.";
                }
            };
            EditorCoroutineRunner.StartEditorCoroutine(SendUrl(api, callback));
#endif
            return result;
        }

        private bool GetAdMobKey(Dictionary<string, object> dic)
        {
            if (dic != null)
            {
                if (dic.ContainsKey("app_key"))
                {
                    app_key = (string)dic["app_key"];
                }
                if (dic.ContainsKey("name"))
                {
                    app_name = (string)dic["name"];
                }
                if (dic.ContainsKey("bundle_id"))
                {
                    app_bundle_id = (string)dic["bundle_id"];
                }
                if (dic.ContainsKey("platform"))
                {
                    app_platform = (string)dic["platform"];
                }
                if (dic.ContainsKey("admob_key"))
                {
                    app_admob_key = (string)dic["admob_key"];
                }

                if (app_platform == "ios" || app_platform == "iOS")
                {
                    this.adSettings.iOSSettings.AdmobAppID = app_admob_key;
                }
                else if (app_platform == "android")
                {
                    this.adSettings.androidSettings.AdmobAppID = app_admob_key;
                }
                return true;
            }
            return false;
        }

        #endregion

        private string HttpGet(string api)
        {
            try
            {
                string serviceAddress = api;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceAddress);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                string returnXml = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                response.Close();
                return returnXml;

            }
            catch (WebException e)
            {
                e.StackTrace.ToString();
                return e.StackTrace.ToString();
            }
        }

        private IEnumerator SendUrl(string url, ApiCallback callback)
        {
            using (UnityWebRequest www = new UnityWebRequest(url))
            {
                yield return www;
                if (www.error != null)
                {
                    Debug.Log(Yodo1U3dMas.TAG + www.error);
                    yield return null;
                }
                else
                {
                    callback(www.downloadHandler.text);
                }
            }
        }
    }
}
