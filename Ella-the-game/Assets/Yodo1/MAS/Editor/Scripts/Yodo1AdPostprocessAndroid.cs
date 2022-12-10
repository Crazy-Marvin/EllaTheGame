namespace Yodo1.MAS
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using System.IO;
    using System.Xml;

    public class Yodo1PostProcessAndroid
    {
        [PostProcessBuild()]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget == BuildTarget.Android)
            {
#if UNITY_ANDROID
                Yodo1AdSettings settings = Yodo1AdSettingsSave.Load();
                if (CheckConfiguration_Android(settings))
                {
#if UNITY_2019_1_OR_NEWER
#else
                    ValidateManifest(settings);
#endif
#if UNITY_2019_3_OR_NEWER
#else
                    Yodo1ProcessGradleBuildFile.AddAdReviewToApplicationGradle(Path.Combine("Assets/Plugins/Android", "mainTemplate.gradle"));
#endif
                }
#endif
            }
        }

        public static bool CheckConfiguration_Android(Yodo1AdSettings settings)
        {
            if (settings == null)
            {
                string message = "MAS Android settings is null, please check the configuration.";
                Debug.LogError(Yodo1U3dMas.TAG + message);
                Yodo1AdUtils.ShowAlert("Error", message, "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(settings.androidSettings.AppKey.Trim()))
            {
                string message = "MAS Android AppKey is null, please check the configuration.";
                Debug.LogError(Yodo1U3dMas.TAG + message);
                Yodo1AdUtils.ShowAlert("Error", message, "Ok");
                return false;
            }

            if (settings.androidSettings.ChineseAndroidStores && string.IsNullOrEmpty(settings.androidSettings.Channel.Trim()))
            {
                string message = "MAS Android Channel is null, please check the configuration.";
                Debug.LogError(Yodo1U3dMas.TAG + message);
                Yodo1AdUtils.ShowAlert("Error", message, "Ok");
                return false;
            }

            if (settings.androidSettings.GooglePlayStore && string.IsNullOrEmpty(settings.androidSettings.AdmobAppID.Trim()))
            {
                string message = "MAS Android AdMob App ID is null, please check the configuration.";
                Debug.LogError(Yodo1U3dMas.TAG + message);
                Yodo1AdUtils.ShowAlert("Error", message, "Ok");
                return false;
            }
            return true;
        }

        static void GenerateManifest(Yodo1AdSettings settings)
        {
            var outputFile = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
            if (!File.Exists(outputFile))
            {
                var inputFile = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines/androidplayer/AndroidManifest.xml");
                if (!File.Exists(inputFile))
                {
                    inputFile = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines/AndroidPlayer/Apk/AndroidManifest.xml");
                }
                if (!File.Exists(inputFile))
                {
                    string s = EditorApplication.applicationPath;
                    int index = s.LastIndexOf("/");
                    s = s.Substring(0, index + 1);
                    inputFile = Path.Combine(s, "PlaybackEngines/AndroidPlayer/Apk/AndroidManifest.xml");
                }
                if (!File.Exists(inputFile))
                {
                    string s = EditorApplication.applicationPath;
                    int index = s.LastIndexOf("/");
                    s = s.Substring(0, index + 1);
                    inputFile = Path.Combine(s, "PlaybackEngines/AndroidPlayer/Apk/LauncherManifest.xml");
                }
                File.Copy(inputFile, outputFile);
            }
            ValidateManifest(settings);
        }

        static void Yodo1ValidateGradle(string path)
        {
            Debug.Log(Yodo1U3dMas.TAG + "path: " + path);
            var gradlePath = Path.Combine(path, PlayerSettings.productName + "/build.gradle");
            ValidateGradlePluginVersion_(gradlePath);

        }

        public static void GenerateGradle()
        {
            var outputFile = Path.Combine(Application.dataPath, "Plugins/Android/mainTemplate.gradle");
            if (!File.Exists(outputFile))
            {
                var inputFile = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines/androidplayer/Tools/GradleTemplates/mainTemplate.gradle");
                if (!File.Exists(inputFile))
                {
                    inputFile = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines/AndroidPlayer/Tools/GradleTemplates/mainTemplate.gradle");
                }
                if (!File.Exists(inputFile))
                {
                    string s = EditorApplication.applicationPath;
                    int index = s.LastIndexOf("/");
                    s = s.Substring(0, index + 1);
                    inputFile = Path.Combine(s, "PlaybackEngines/AndroidPlayer/Tools/GradleTemplates/mainTemplate.gradle");
                }
                if (!File.Exists(inputFile))
                {
                    string s = EditorApplication.applicationPath;
                    int index = s.LastIndexOf("/");
                    s = s.Substring(0, index + 1);
                    inputFile = Path.Combine(s, "PlaybackEngines/AndroidPlayer//Tools/GradleTemplates/mainTemplate.gradle");
                }
                File.Copy(inputFile, outputFile);
            }

            if (File.Exists(outputFile))
            {
                ValidateGradlePluginVersion_(outputFile);
            }
        }

        private static void ValidateGradlePluginVersion_(string projectGradlePath)
        {

            Debug.Log(Yodo1U3dMas.TAG + "projectGradlePath: " + projectGradlePath);
            StreamReader streamReader1 = new StreamReader(projectGradlePath);
            string text_all = streamReader1.ReadToEnd();
            streamReader1.Close();

            bool changed = false;
            string oldLineStr = string.Empty;
            string newLineStr = string.Empty;

            string[] array = text_all.Split(System.Environment.NewLine.ToCharArray());
            for (int i = 0; i < array.Length; i++)
            {
                string str = array[i];
                if (str.Contains("com.android.tools.build:gradle"))
                {
                    oldLineStr = str;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(oldLineStr) && oldLineStr.Contains(":") && oldLineStr.Contains("'"))
            {
                var temp = oldLineStr.Replace("'", "");
                string[] tempArray = temp.Split(":".ToCharArray());
                if (tempArray != null && tempArray.Length > 0)
                {
                    var oldPluginVersion = tempArray[tempArray.Length - 1]; // such as 3.4.0
                    string resultPluginVersion = ValidatePluginVersionStr_(oldPluginVersion);
                    if (resultPluginVersion != null && !oldPluginVersion.Equals(resultPluginVersion))
                    {
                        newLineStr = oldLineStr.Replace(oldPluginVersion, resultPluginVersion);
                        text_all = text_all.Replace(oldLineStr, newLineStr);
                        changed = true;
                    }
                }

            }

            // deal with multiDexEnabled and sourceCompatibility 1.8
            if (!text_all.Contains("multiDexEnabled true") && text_all.Contains("applicationId"))
            {
                string newText = "multiDexEnabled true " + System.Environment.NewLine +
                    "\t\t" + "applicationId";
                text_all = text_all.Replace("applicationId", newText);
                changed = true;
            }
            if (!text_all.Contains("sourceCompatibility") && text_all.Contains("defaultConfig"))
            {
                string newText = "compileOptions { " + System.Environment.NewLine +
                    "\t\t" + "sourceCompatibility JavaVersion.VERSION_1_8 " + System.Environment.NewLine +
                    "\t\t" + "targetCompatibility JavaVersion.VERSION_1_8 " + System.Environment.NewLine +
                    "\t" + "} " + System.Environment.NewLine +
                    "\t" + "defaultConfig";
                text_all = text_all.Replace("defaultConfig", newText);
                changed = true;
            }


            if (changed)
            {
                StreamWriter streamWriter = new StreamWriter(projectGradlePath);
                streamWriter.Write(text_all);
                streamWriter.Close();
                Debug.Log(Yodo1U3dMas.TAG + "changed gradle plugin version from " + oldLineStr + " to " + newLineStr);
            }


        }

        private static string ValidatePluginVersionStr_(string oldPluginVersion)
        {
            if (!oldPluginVersion.Contains("."))
            {
                return null;
            }

            var versionNumStr = oldPluginVersion.Replace(".", "");
            int oldVerionNum = int.Parse(versionNumStr);
            int minVersionNum = 330;

            if (oldVerionNum < minVersionNum)
            {
                Debug.Log(Yodo1U3dMas.TAG + "need to use the version of Unity as follows:" + System.Environment.NewLine +
                    "Unity 2017 starting from 2017.4.38f1" + System.Environment.NewLine +
                    "Unity 2018 starting from 2018.4.4f1" + System.Environment.NewLine +
                    "Unity 2019 starting from 2019.1.7f1" + System.Environment.NewLine +
                    "Unity 2020 all version");
                return null;
            }

            if (oldVerionNum > 410)
            {
                Debug.Log(Yodo1U3dMas.TAG + "no need do anything");
                return null;
            }


            string[] resultArray = { "3.3.3", "3.4.3", "3.5.4", "3.6.4", "4.0.1" };
            string subOldPluginVersion = oldPluginVersion.Substring(0, oldPluginVersion.LastIndexOf("."));
            for (int i = 0; i < resultArray.Length; i++)
            {
                string result = resultArray[i];
                if (!result.Equals(oldPluginVersion) && result.StartsWith(subOldPluginVersion))
                {
                    return result;
                }
            }

            return null;
        }

        public static bool ValidateManifest(Yodo1AdSettings settings)
        {
            if (settings == null)
            {
                Debug.LogError(Yodo1U3dMas.TAG + "Validate manifest failed. Yodo1 ad settings is not exsit.");
                return false;
            }

            var androidPluginPath = Path.Combine(Application.dataPath, "Plugins/Android/");
            var manifestFile = androidPluginPath + "AndroidManifest.xml";
            if (!File.Exists(manifestFile))
            {
                GenerateManifest(settings);
                return true;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(manifestFile);

            if (doc == null)
            {
                Debug.LogError(Yodo1U3dMas.TAG + "Couldn't load " + manifestFile);
                return false;
            }

            XmlNode manNode = FindChildNode(doc, "manifest");
            string ns = manNode.GetNamespaceOfPrefix("android");

            XmlNode app = FindChildNode(manNode, "application");

            if (app == null)
            {
                Debug.LogError(Yodo1U3dMas.TAG + "Error parsing " + manifestFile + ", tag for application not found.");
                return false;
            }

            ////Enable hardware acceleration for video play
            //XmlElement elem = (XmlElement)app;

            //Add AdMob App ID
            if (settings.androidSettings.GooglePlayStore)
            {
                string admobAppIdValue = settings.androidSettings.AdmobAppID.Trim();
                if (string.IsNullOrEmpty(admobAppIdValue))
                {
                    Debug.LogError(Yodo1U3dMas.TAG + "MAS Android AdMob App ID is null, please check the configuration.");
                    return false;
                }
                string admobAppIdName = "com.google.android.gms.ads.APPLICATION_ID";
                XmlNode metaNode = FindChildNodeWithAttribute(app, "meta-data", "android:name", admobAppIdName);
                if (metaNode == null)
                {
                    metaNode = (XmlElement)doc.CreateNode(XmlNodeType.Element, "meta-data", null);
                    app.AppendChild(metaNode);
                }

                XmlElement metaElement = (XmlElement)metaNode;
                metaElement.SetAttribute("name", ns, admobAppIdName);
                metaElement.SetAttribute("value", ns, admobAppIdValue);
                metaElement.GetNamespaceOfPrefix("android");
            }

            //Add Channel
            string channelValue = string.Empty;
            if (settings.androidSettings.ChineseAndroidStores)
            {
                channelValue = settings.androidSettings.Channel.Trim();
                if (string.IsNullOrEmpty(channelValue))
                {
                    Debug.LogError(Yodo1U3dMas.TAG + "MAS Android Channel is null, please check the configuration.");
                    return false;
                }
            }
            if (settings.androidSettings.GooglePlayStore)
            {
                channelValue = "GooglePlay";
            }
            string channelName = "Yodo1ChannelCode";
            XmlNode meta1Node = FindChildNodeWithAttribute(app, "meta-data", "android:name", channelName);
            if (meta1Node == null)
            {
                meta1Node = (XmlElement)doc.CreateNode(XmlNodeType.Element, "meta-data", null);
                app.AppendChild(meta1Node);
            }

            XmlElement meta1Element = (XmlElement)meta1Node;
            meta1Element.SetAttribute("name", ns, channelName);
            meta1Element.SetAttribute("value", ns, channelValue);
            meta1Element.GetNamespaceOfPrefix("android");

            string ns2 = manNode.GetNamespaceOfPrefix("tools");
            meta1Element.SetAttribute("replace", ns2, "android:value");

            doc.Save(manifestFile);
            return true;
        }

        public static XmlNode FindChildNode(XmlNode parent, string name)
        {
            XmlNode curr = parent.FirstChild;
            while (curr != null)
            {
                if (curr.Name.Equals(name))
                {
                    return curr;
                }
                curr = curr.NextSibling;
            }
            return null;
        }

        public static XmlNode FindChildNodeWithAttribute(XmlNode parent, string name, string attribute, string value)
        {
            XmlNode curr = parent.FirstChild;
            while (curr != null)
            {
                if (curr.Name.Equals(name) && curr.Attributes.GetNamedItem(attribute) != null && curr.Attributes[attribute].Value.Equals(value))
                {
                    return curr;
                }
                curr = curr.NextSibling;
            }
            return null;
        }

        public static XmlNode FindLauncherActivityNode(XmlNode applicaiton)
        {
            XmlNode acNode = applicaiton.FirstChild;
            while (acNode != null)
            {
                if(acNode.Name.Equals("activity"))
                {
                    XmlNode intentFilterNode = acNode.FirstChild;
                    while(intentFilterNode != null)
                    {
                        if(intentFilterNode.Name.Equals("intent-filter"))
                        {
                            XmlNode launcherCategeryNode = FindChildNodeWithAttribute(intentFilterNode, "category", "android:name", "android.intent.category.LAUNCHER");
                            if(launcherCategeryNode != null)
                            {
                                return acNode;
                            }
                        }

                        intentFilterNode = intentFilterNode.NextSibling;
                    }
                    
                }
               
                acNode = acNode.NextSibling;
            }
            return null;
        }
    }
}