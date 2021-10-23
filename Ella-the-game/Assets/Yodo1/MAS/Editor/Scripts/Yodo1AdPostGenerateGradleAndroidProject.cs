#if UNITY_ANDROID
using UnityEngine;
using UnityEditor.Android;
using System.IO;
using System.Xml;

namespace Yodo1.MAS
{
#if UNITY_2018_1_OR_NEWER
    public class Yodo1PostGenerateGradleAndroidProject : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder
        {
            get { return 100; }
        }

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            Yodo1ValidateGradle(path);
            Yodo1ValidateManifest(path);
            Yodo1ValidateGradleProperties(path);

            // 添加Ad Review
#if UNITY_2019_3_OR_NEWER
            var rootGradlePath = Path.Combine(path, "../build.gradle");
            var buildScriptAdded = Yodo1ProcessGradleBuildFile.AddAdReviewToTootGradle(rootGradlePath);
            var applicationGradlePath = Path.Combine(path, "../launcher/build.gradle");
#else
            var applicationGradlePath = Path.Combine(path, "build.gradle");
#endif
            Yodo1ProcessGradleBuildFile.AddAdReviewToApplicationGradle(applicationGradlePath);
        }

        static void Yodo1ValidateManifest(string path)
        {
            var mainfestPath = Path.Combine(path, "src/main/AndroidManifest.xml");

            if (mainfestPath.Contains("unityLibrary"))
            {
                mainfestPath = mainfestPath.Replace("unityLibrary", "launcher");
            }

            if (File.Exists(mainfestPath))
            {
                Yodo1AdSettings settings = Yodo1AdSettingsSave.Load();
                if (Yodo1PostProcessAndroid.CheckConfiguration_Android(settings))
                {
                    ValidateManifest(mainfestPath, settings);
                }
            }
        }

        static void ValidateManifest(string manifestFile, Yodo1AdSettings settings)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(manifestFile);

            if (doc == null)
            {
                Debug.LogError("[Yodo1 Mas] Couldn't load " + manifestFile);
                return;
            }

            XmlNode manNode = Yodo1PostProcessAndroid.FindChildNode(doc, "manifest");
            string ns = manNode.GetNamespaceOfPrefix("android");

            XmlNode app = Yodo1PostProcessAndroid.FindChildNode(manNode, "application");

            if (app == null)
            {
                Debug.LogError("[Yodo1 Mas] Error parsing " + manifestFile + ", tag for application not found.");
                return;
            }

            ////Enable hardware acceleration for video play
            //XmlElement elem = (XmlElement)app;

            //Add AdMob App ID
            if (settings.androidSettings.GooglePlayStore)
            {
                string admobAppIdValue = settings.androidSettings.AdmobAppID.Trim();
                if (string.IsNullOrEmpty(admobAppIdValue))
                {
                    Debug.LogError("[Yodo1 Mas] MAS Android AdMob App ID is null, please check the configuration.");
                    return;
                }
                string admobAppIdName = "com.google.android.gms.ads.APPLICATION_ID";
                XmlNode metaNode = Yodo1PostProcessAndroid.FindChildNodeWithAttribute(app, "meta-data", "android:name", admobAppIdName);
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
                    Debug.LogError("[Yodo1 Mas] MAS Android Channel is null, please check the configuration.");
                    return;
                }
            }
            if (settings.androidSettings.GooglePlayStore)
            {
                channelValue = "GooglePlay";
            }
            string channelName = "Yodo1ChannelCode";
            XmlNode meta1Node = Yodo1PostProcessAndroid.FindChildNodeWithAttribute(app, "meta-data", "android:name", channelName);
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
        }

        static void Yodo1ValidateGradleProperties(string path) 
        {
            var gradlePropertiesPath = Path.Combine(path.Replace("unityLibrary", ""), "gradle.properties");
            StreamReader streamReader1 = new StreamReader(gradlePropertiesPath);
            string text_all = streamReader1.ReadToEnd();
            streamReader1.Close();

            bool changed = false;
            string oldLineStr = string.Empty;
            string newLineStr = string.Empty;

            string[] array = text_all.Split(System.Environment.NewLine.ToCharArray());
            for (int i = 0; i < array.Length; i++)
            {
                string str = array[i];
                if (str.Contains("android.enableDexingArtifactTransform"))
                {
                    oldLineStr = str;
                    break;
                }
            }

            if(string.IsNullOrEmpty(oldLineStr) || oldLineStr.Contains("false"))
            {
                if (string.IsNullOrEmpty(oldLineStr))
                {
                    newLineStr="android.enableDexingArtifactTransform=false";
                    text_all = text_all + "\n" + newLineStr;
                } else {
                    newLineStr=oldLineStr.Replace("false", "true");
                    text_all = text_all.Replace(oldLineStr, newLineStr);
                }
                StreamWriter streamWriter = new StreamWriter(gradlePropertiesPath);
                streamWriter.Write(text_all);
                streamWriter.Close();
            }
        }

        static void Yodo1ValidateGradle(string path)
        {
            var gradlePath = Path.Combine(path, "build.gradle");
            ValidateGradlePluginVersion(path);
            if (gradlePath.Contains("unityLibrary"))
            {
                gradlePath = gradlePath.Replace("unityLibrary", "launcher");
            }
            Debug.LogFormat("[Yodo1 Mas] Updating gradle for Play Instant: {0}", gradlePath);
            WriteBelow(gradlePath, "defaultConfig {", "\t\tmultiDexEnabled true");
        }

        static void ValidateGradlePluginVersion(string path)
        {
            if(path.Contains("unityLibrary"))
            {
                var projectPath = path.Replace("unityLibrary", "");
                var projectGradlePath = Path.Combine(projectPath, "build.gradle");
                ValidateGradlePluginVersion_(projectGradlePath);
            }
            else
            {
                var projectGradlePath = Path.Combine(path, "build.gradle");
                ValidateGradlePluginVersion_(projectGradlePath);
            }
        }

        private static void ValidateGradlePluginVersion_(string projectGradlePath)
        {

            //Debug.LogError("[ValidateGradlePluginVersion_] projectGradlePath: " + projectGradlePath);
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

            if(!string.IsNullOrEmpty(oldLineStr) && oldLineStr.Contains(":") && oldLineStr.Contains("'"))
            {
                var temp = oldLineStr.Replace("'", "");
                string[] tempArray = temp.Split(":".ToCharArray());
                if(tempArray != null && tempArray.Length > 0)
                {
                    var oldPluginVersion = tempArray[tempArray.Length - 1]; // such as 3.4.0
                    string resultPluginVersion = ValidatePluginVersionStr_(oldPluginVersion);
                    if(resultPluginVersion != null && !oldPluginVersion.Equals(resultPluginVersion))
                    {
                        newLineStr = oldLineStr.Replace(oldPluginVersion, resultPluginVersion);
                        changed = true;
                    }
                }

            }

            if (changed)
            {
                text_all = text_all.Replace(oldLineStr, newLineStr);
                StreamWriter streamWriter = new StreamWriter(projectGradlePath);
                streamWriter.Write(text_all);
                streamWriter.Close();
                Debug.Log("[Yodo1 Mas] changed gradle plugin version from " + oldLineStr + " to " + newLineStr);
                //Debug.LogError("[ValidateGradlePluginVersion_] projectGradlePath:  after changed text_all:  " +  text_all );
            }
        }

        private static string ValidatePluginVersionStr_(string oldPluginVersion)
        {
            if(!oldPluginVersion.Contains("."))
            {
                return null;
            }

            var versionNumStr = oldPluginVersion.Replace(".", "");
            int oldVerionNum = int.Parse(versionNumStr);
            int minVersionNum = 330;

            if(oldVerionNum < minVersionNum)
            {
                Debug.LogError("[Yodo1 Mas] the gradle plugin verison is to low ! you need to update the gradle version: " );
                return null;
            }

            if(oldVerionNum > 410)
            {
                return null;
            }


            string[] resultArray = { "3.3.3", "3.4.3", "3.5.4", "3.6.4", "4.0.1"};
            string subOldPluginVersion = oldPluginVersion.Substring(0, oldPluginVersion.LastIndexOf("."));
            for(int i=0; i<resultArray.Length; i++)
            {
                string result = resultArray[i];
                if(!result.Equals(oldPluginVersion) && result.StartsWith(subOldPluginVersion))
                {
                    return result;
                }
            }

            return null;
        }

        static bool WriteBelow(string filePath, string below, string text)
        {
            StreamReader streamReader = new StreamReader(filePath);
            string text_all = streamReader.ReadToEnd();
            streamReader.Close();

            int beginIndex = text_all.LastIndexOf(below);
            if (beginIndex == -1)
            {
                Debug.LogError("[Yodo1 Mas] Error parsing " + filePath + ", tag for " + below + " not found.");
                return false;
            }

            if (text_all.IndexOf(text) == -1)
            {
                int endIndex = beginIndex + below.Length;

                text_all = text_all.Substring(0, endIndex) + "\n" + text + /*"\n" +*/ text_all.Substring(endIndex);

                StreamWriter streamWriter = new StreamWriter(filePath);
                streamWriter.Write(text_all);
                streamWriter.Close();
                return true;
            }
            return false;
        }
    }
#endif
}
#endif